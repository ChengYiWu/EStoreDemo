namespace Application.Common.Models
{
    public class Result
    {
        public string Message { get; set; }

        public bool IsSucceed { get; set; }

        private static Result GetResult(bool isSucceed, string message = "")
        {
            return new Result
            {
                IsSucceed = isSucceed,
                Message = message,
            };
        }

        public static Result Success() => GetResult(true);

        public static Result Success(string message) => GetResult(true, message);

        public static Result Failure(string message) => GetResult(false, message);

    }
}
