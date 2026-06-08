using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Collections.Concurrent;

namespace SharedCode
{
    public class Result
    {
        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; }

        [MemberNotNullWhen(true, nameof(Error))]
        public bool IsFailed => !IsSuccess;

        public string? Error { get; }

        protected Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Composite non-generic result that can aggregate multiple <see cref="Result"/> instances.
        /// - IsSuccess is the logical OR of contained results' IsSuccess values (true if any result is success).
        /// - Error is a joined string of all error messages. The first line contains a summary like "Errors: N".
        /// </summary>
        public static Result Success() => new Result(true, null);
        public static Result Failure(string error) => new Result(false, error);
    }

    public class CompositeResult : Result
    {
        private readonly ConcurrentBag<Result> _results = new ConcurrentBag<Result>();
        private readonly ConcurrentBag<string> _errors = new ConcurrentBag<string>();

        public CompositeResult()
            : base(true, null)
        {
        }

        public void Add(Result result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            _results.Add(result);
            if (!string.IsNullOrEmpty(result.Error))
            {
                var e = result.Error!.Trim();
                if (!string.IsNullOrEmpty(e)) _errors.Add(e);
            }
        }

        public void AddRange(IEnumerable<Result> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            foreach (var r in results) Add(r);
        }

        /// <summary>
        /// Snapshot of current results. This creates an array snapshot of the underlying bag.
        /// </summary>
        public IReadOnlyList<Result> Results => _results.ToArray();

        [MemberNotNullWhen(false, nameof(Error))]
        public new bool IsSuccess
        {
            get
            {
                var arr = _results.ToArray();
                if (arr.Length == 0) return true;
                return arr.Any(r => r.IsSuccess);
            }
        }

        public new string? Error
        {
            get
            {
                var errors = _errors.ToArray();
                if (errors.Length == 0) return null;
                var header = $"Errors: {errors.Length}";
                return string.Join(Environment.NewLine, new[] { header }.Concat(errors));
            }
        }
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        [MemberNotNullWhen(true, nameof(Value))]
        public new bool IsSuccess => base.IsSuccess;

        [MemberNotNullWhen(false, nameof(Value))]
        public new bool IsFailed => !base.IsSuccess;


        public T? Value
        {
            get
            {
                return IsSuccess ? _value : default;
            }
        }

        public Result<T2> ToOtherError<T2>()
        {
            if (this.IsSuccess)
            {
                throw new InvalidOperationException("Result must be an error");
            }
            return Result<T2>.Failure(Error ?? "Unknown error");
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
