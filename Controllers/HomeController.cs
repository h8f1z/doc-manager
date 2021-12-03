using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using doc_manager.Models;
using doc_manager.Data;
using doc_manager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace doc_manager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    private readonly DocumentContext _context;

    public HomeController(ILogger<HomeController> logger, DocumentContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> IndexAsync()
    {
        var docs = await _context.Document
        .Where(doc => doc.IsHidden.Equals(false))
        .Select(doc => new DocumentListViewModel{
            FileName = doc.FileName,
            Id = doc.Id,
            UploadedAt = doc.UploadedAt
        }).ToListAsync();
        _logger.LogInformation("Found " + docs.Count() + " documents.");
        return View(docs);
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
