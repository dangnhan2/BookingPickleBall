namespace PickleBall.Dto
{
    public class Result<T>
    {
        public string Error { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }

        private Result(string message,bool Success, T data) { 
           Error = message;
           this.Success = Success;
           Data = data;
        }

        public static Result<T> Ok(T data)
        {
            return new Result<T>(null, true, data);
        }

        public static Result<T> Fail(string error)
        {
            return new Result<T>(error, false, default(T));
        }
             
    }
}
