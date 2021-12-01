using Microsoft.AspNetCore.Mvc;

namespace doc_manager.Controllers;

public class DocController : Controller
{
  private readonly ILogger<DocController> _logger;

  public DocController(ILogger<DocController> logger)
  {
    _logger = logger;
  }

  public IActionResult Create()
  {
    return View();
  }
}