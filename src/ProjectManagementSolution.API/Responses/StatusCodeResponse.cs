using Microsoft.AspNetCore.Mvc;

namespace ProjectManagementSolution.API.Responses;

public record StatusCodeResponse(int StatusCode, string? Message = null) : IToActionResult
{
    public IActionResult ToActionResult()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            return new StatusCodeResult(StatusCode);
        }
        else
        {
            var result = new
            {
                statusCode = StatusCode,
                message = Message
            };

            return new ObjectResult(result) { StatusCode = StatusCode };
        }
    }
}

public record StatusCodeResponse<T>(int StatusCode, T? Data = default) : IToActionResult
{
    public IActionResult ToActionResult()
    {
        if (Data is null)
        {
            return new StatusCodeResult(StatusCode);
        }
        else
        {
            return new ObjectResult(Data) { StatusCode = StatusCode };
        }
    }
}
