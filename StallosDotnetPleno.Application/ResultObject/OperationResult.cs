using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StallosDotnetPleno.Application.ResultObject;

public class OperationResult
{
    public bool Success { get; }
    public string ErrorMessage { get; }

    protected OperationResult(bool success, string errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    public static OperationResult SuccessResult() => new OperationResult(true, null);

    public static OperationResult FailureResult(string errorMessage) => new OperationResult(false, errorMessage);
}

public class OperationResult<T> : OperationResult
{
    public T Data { get; }

    private OperationResult(bool success, T data, string errorMessage)
        : base(success, errorMessage)
    {
        Data = data;
    }

    public static OperationResult<T> SuccessResult(T data) => new OperationResult<T>(true, data, null);

    public static OperationResult<T> FailureResult(string errorMessage) => new OperationResult<T>(false, default(T), errorMessage);
}