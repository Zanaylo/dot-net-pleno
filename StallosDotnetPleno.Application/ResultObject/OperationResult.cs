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
    public object Data { get; }

    protected OperationResult(bool success, string errorMessage, object data = null)
    {
        Success = success;
        ErrorMessage = errorMessage;
        Data = data;
    }

    public static OperationResult SuccessResult() => new OperationResult(true, null);

    public static OperationResult FailureResult(string errorMessage) => new OperationResult(false, errorMessage);

    public static OperationResult SuccessResult(object data) => new OperationResult(true, null, data);

    public static OperationResult FailureResult(string errorMessage, object data) => new OperationResult(false, errorMessage, data);
}