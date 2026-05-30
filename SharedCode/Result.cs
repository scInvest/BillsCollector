using System.Diagnostics.CodeAnalysis;

namespace SharedCode
{
    public class Result
    {
        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; }
        public string? Error { get; }

        protected Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(string error) => new Result(false, error);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        [MemberNotNullWhen(true, nameof(Value))]
        public new bool IsSuccess => base.IsSuccess;

        public T? Value
        {
            get
            {
                return IsSuccess ? _value : default;
            }
        }

        public Result(T value)
            : base(true, null)
        {
            _value = value;
        }

        public Result(string error)
            : base(false, error)
        {
            _value = default;
        }

        public static Result<T> Success(T value) => new Result<T>(value);
        public static new Result<T> Failure(string error) => new Result<T>(error);
    }
}
