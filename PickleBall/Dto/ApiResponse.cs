namespace PickleBall.Dto
{
    public class ApiResponse<T>
    {
        public string Error { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        private ApiResponse(string message,bool Success, T data, int statusCode) { 
           Error = message;
           this.Success = Success;
           Data = data;
           StatusCode = statusCode;
        }

        public static ApiResponse<T> Ok(T data, int code)
        {
            return new ApiResponse<T>(null, true, data, code);
        }

        public static ApiResponse<T> Fail(string error, int code)
        {
            return new ApiResponse<T>(error, false, default(T), code);
        }
             
    }
}
