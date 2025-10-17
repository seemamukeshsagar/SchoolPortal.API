namespace SchoolPortal.Web.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResult(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResult(string message, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }

    // Non-generic version for responses without data
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static ApiResponse SuccessResult(string? message = null)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        public static ApiResponse ErrorResult(string message)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}