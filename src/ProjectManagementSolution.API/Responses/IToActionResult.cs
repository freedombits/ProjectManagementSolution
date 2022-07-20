using Microsoft.AspNetCore.Mvc;

namespace ProjectManagementSolution.API.Responses;

public interface IToActionResult
{
    IActionResult ToActionResult();
}
