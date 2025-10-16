using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SchoolPortal.API.Models;
using SchoolPortal.Web.DTOs;

namespace SchoolPortal.Web.Services
{
	public class UserProfileService : IUserProfileService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ICachingService _cachingService;
		private readonly ILogger<UserProfileService> _logger;
		private readonly IConfiguration _configuration;
		private const string UserDetailsCachePrefix = "UserProfile_";
		private const string RolesCachePrefix = "Role_";
		private const string DesignationsCachePrefix = "Designation_";

		public UserProfileService(
			IHttpClientFactory httpClientFactory,
			ICachingService cachingService,
			ILogger<UserProfileService> logger,
			IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_cachingService = cachingService;
			_logger = logger;
			_configuration = configuration;
		}

		public async Task<UserProfileDto> GetUserProfileAsync(string username)
		{
			try
			{
				if (string.IsNullOrEmpty(username))
				{
					return new UserProfileDto { Success = false, Message = "Username is required" };
				}

				// Try to get user details from cache first
				var cacheKey = $"{UserDetailsCachePrefix}{username}";
				var cachedUser = _cachingService.Get<UserDetail>(cacheKey);

				if (cachedUser != null)
				{
					return await CreateUserProfileDto(cachedUser);
				}

				// If not in cache, fetch from API
				using var client = _httpClientFactory.CreateClient("AuthApi");
				var response = await client.GetAsync($"User/GetUserDetails?username={Uri.EscapeDataString(username)}");
				
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning("Failed to fetch user details for {Username}. Status: {StatusCode}", 
						username, response.StatusCode);
					return new UserProfileDto { Success = false, Message = "Failed to fetch user details" };
				}

				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};
				
				var userDetails = await response.Content.ReadFromJsonAsync<UserDetail>(options);
				
				if (userDetails == null)
				{
					_logger.LogWarning("Received null user details for {Username}", username);
					return new UserProfileDto { Success = false, Message = "User details not found" };
				}

				// Cache the user details for future use
				var cacheDuration = TimeSpan.FromMinutes(_configuration.GetValue("CacheSettings:UserDetailsCacheDurationMinutes", 30));
				_cachingService.Set(cacheKey, userDetails, cacheDuration);

				return await CreateUserProfileDto(userDetails);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching user profile for {Username}", username);
				return new UserProfileDto { Success = false, Message = "An error occurred while fetching user profile" };
			}
		}

		public async Task<RoleDto?> GetRoleByIdAsync(Guid roleId)
		{
			if (roleId == Guid.Empty)
				return null;

			var cacheKey = $"{RolesCachePrefix}{roleId}";
			var cachedRole = _cachingService.Get<RoleDto>(cacheKey);
			if (cachedRole != null)
				return cachedRole;

			try
			{
				using var client = _httpClientFactory.CreateClient("AuthApi");
				var response = await client.GetAsync($"Roles/{roleId}");
				
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning("Failed to fetch role with ID {RoleId}. Status: {StatusCode}", 
						roleId, response.StatusCode);
					return null;
				}

				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};

				var role = await response.Content.ReadFromJsonAsync<RoleDto>(options);
				if (role != null)
				{
					var cacheDuration = TimeSpan.FromHours(1); // Cache roles for 1 hour
					_cachingService.Set(cacheKey, role, cacheDuration);
				}

				return role;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching role with ID {RoleId}", roleId);
				return null;
			}
		}

		public async Task<DesignationDto?> GetDesignationByIdAsync(Guid designationId)
		{
			if (designationId == Guid.Empty)
				return null;

			var cacheKey = $"{DesignationsCachePrefix}{designationId}";
			var cachedDesignation = _cachingService.Get<DesignationDto>(cacheKey);
			if (cachedDesignation != null)
				return cachedDesignation;

			try
			{
				using var client = _httpClientFactory.CreateClient("AuthApi");
				var response = await client.GetAsync($"Designations/{designationId}");
				
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning("Failed to fetch designation with ID {DesignationId}. Status: {StatusCode}", 
						designationId, response.StatusCode);
					return null;
				}

				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};

				var designation = await response.Content.ReadFromJsonAsync<DesignationDto>(options);
				if (designation != null)
				{
					var cacheDuration = TimeSpan.FromHours(1); // Cache designations for 1 hour
					_cachingService.Set(cacheKey, designation, cacheDuration);
				}

				return designation;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching designation with ID {DesignationId}", designationId);
				return null;
			}
		}

		private async Task<UserProfileDto> CreateUserProfileDto(UserDetail user)
		{
			var role = user.UserRoleId.HasValue 
				? await GetRoleByIdAsync(user.UserRoleId.Value)
				: null;

			var designation = user.DesignationId != Guid.Empty
				? await GetDesignationByIdAsync(user.DesignationId)
				: null;

			return new UserProfileDto
			{
				Success = true,
				FullName = $"{user.FirstName} {user.LastName}".Trim(),
				Email = user.EmailAddress ?? "Not specified",
				Role = role?.Name ?? "Not specified",
				Department = designation?.Name ?? "Not specified",
				Designation = designation?.Name ?? "Not specified"
			};
		}
	}
}
