namespace EmployeeManagement.Domain.Common
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string>? Errors { get; set; }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static ServiceResult<T> Failure(List<string> errors)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Errors = errors
            };
        }
    }
}