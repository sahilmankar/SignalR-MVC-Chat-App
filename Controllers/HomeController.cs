using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRMVCApp.Models;
using SignalRMVCApp.SignalRHub;

namespace SignalRMVCApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHubContext<ChatHub,IChatClient> _hub;  
    // not required but thats how you inject signalR Hub .

    public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub, IChatClient> hub)
    {
        _logger = logger;
        _hub = hub;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
