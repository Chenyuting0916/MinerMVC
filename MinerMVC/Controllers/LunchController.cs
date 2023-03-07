using Microsoft.AspNetCore.Mvc;
using MinerMVC.Models.CustomExcelDb;
using MinerMVC.Services;

namespace MinerMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LunchController : ControllerBase
{
    private readonly ICustomExcelService _customExcelService;

    public LunchController(ICustomExcelService customExcelService)
    {
        _customExcelService = customExcelService;
    }

    [HttpGet]
    [Route("Test")]
    public string Test()
    {
        return "Test";
    }
}