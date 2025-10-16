using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Services
{
    public interface ILoginAttemptService
    {
        Task<bool> IsAccountLocked(string username);
        Task RecordFailedAttempt(string username);
        Task ResetAttempts(string username);
        Task<DateTime?> GetLockoutEndTime(string username);
    }

    public class LoginAttemptService : ILoginAttemptService
    {
        private static readonly ConcurrentDictionary<string, (int Attempts, DateTime? LockoutEnd)> _loginAttempts = new();
        private const int MaxFailedAttempts = 3;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(2);

        public Task<bool> IsAccountLocked(string username)
        {
            if (_loginAttempts.TryGetValue(username, out var attempt))
            {
                if (attempt.LockoutEnd.HasValue && DateTime.UtcNow < attempt.LockoutEnd.Value)
                {
                    return Task.FromResult(true);
                }
                
                // If lockout has expired, clear the lockout
                if (attempt.LockoutEnd.HasValue && DateTime.UtcNow >= attempt.LockoutEnd.Value)
                {
                    _loginAttempts.TryRemove(username, out _);
                }
            }
            return Task.FromResult(false);
        }

        public Task RecordFailedAttempt(string username)
        {
            var attempts = _loginAttempts.AddOrUpdate(
                username,
                _ => (1, null),
                (_, current) =>
                {
                    var newAttempts = current.Attempts + 1;
                    DateTime? lockoutEnd = newAttempts >= MaxFailedAttempts 
                        ? DateTime.UtcNow.Add(LockoutDuration) 
                        : null;
                    return (newAttempts, lockoutEnd);
                });

            return Task.CompletedTask;
        }

        public Task ResetAttempts(string username)
        {
            _loginAttempts.TryRemove(username, out _);
            return Task.CompletedTask;
        }

        public Task<DateTime?> GetLockoutEndTime(string username)
        {
            if (_loginAttempts.TryGetValue(username, out var attempt))
            {
                return Task.FromResult(attempt.LockoutEnd);
            }
            return Task.FromResult<DateTime?>(null);
        }
    }
}
