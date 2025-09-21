namespace PickleBall.Dto
{
    public class Result<T>
    {
        public string Error { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        private Result(string message,bool Success, T data, int statusCode) { 
           Error = message;
           this.Success = Success;
           Data = data;
           StatusCode = statusCode;
        }

        public static Result<T> Ok(T data, int code)
        {
            return new Result<T>(null, true, data, code);
        }

        public static Result<T> Fail(string error, int code)
        {
            return new Result<T>(error, false, default(T), code);
        }
             
    }
}
