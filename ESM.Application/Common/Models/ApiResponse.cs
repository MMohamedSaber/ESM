namespace ESM.Application.Common.Models;

public class ApiResponse
{
    public bool Success { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResponse SuccessResponse()
    {
        return new ApiResponse { Success = true };
    }

    public static ApiResponse ErrorResponse(string code, string message)
    {
        return new ApiResponse 
        { 
            Success = false, 
            Error = new ApiError { Code = code, Message = message } 
        };
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T> { Success = true, Data = data };
    }

    public new static ApiResponse<T> ErrorResponse(string code, string message)
    {
        return new ApiResponse<T> 
        { 
            Success = false, 
            Error = new ApiError { Code = code, Message = message } 
        };
    }
}
