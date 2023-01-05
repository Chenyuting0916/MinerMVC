using Microsoft.AspNetCore.Mvc;
using MinerMVC.Services;

namespace MinerMVC.Controllers;

public class JobController : Controller
{
    private readonly IJobCrawlerService _jobCrawlerService;
    public JobController(IJobCrawlerService jobCrawlerService)
    {
        _jobCrawlerService = jobCrawlerService;
    }

    public IActionResult Index()
    {
        var companies = _jobCrawlerService.GetHsinChuScienceParkCompany();
        return View(companies);
    }
}