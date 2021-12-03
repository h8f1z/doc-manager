using Microsoft.AspNetCore.Mvc;
using doc_manager.ViewModels;
using System.IO;

namespace doc_manager.Controllers;

public class DocController : Controller
{
  private readonly ILogger<DocController> _logger;
  private readonly IConfiguration _config;

  public DocController(ILogger<DocController> logger, IConfiguration config)
  {
    _logger = logger;
    _config = config;
  }

  public IActionResult Create()
  {
    _logger.LogInformation("Page Loading...");
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  // Set the limit to 5 MB
  [RequestFormLimits(MultipartBodyLengthLimit = 5242880)]
  public async Task<IActionResult> CreateAsync(DocumentCreateViewModel createVm)
  {
    _logger.LogInformation("Post data received.");
    if (!ModelState.IsValid)
    {
      _logger.LogError("Model Not Valid...");
      return View(createVm);
    }

    if (createVm.FormFile.Length > 0)
    {
      var uploadPath = _config["UploadPath"];
      _logger.LogInformation("Upload Path from Config: " + uploadPath);
      
      var fileName = Path.GetFileName(createVm.FileName) + Path.GetExtension(createVm.FormFile.FileName).ToLowerInvariant();
      _logger.LogInformation("File Name: " + fileName);
      
      var filePath  = Path.Combine(uploadPath, fileName);
      _logger.LogInformation("File Path: " + filePath);

      using (var stream = System.IO.File.Create(filePath))
      {
        // ! bug: uploading files with same name replaces the existing file
        // todo: generate random file name
        await createVm.FormFile.CopyToAsync(stream);
        _logger.LogInformation("File Copied");
      }

      // todo: save to database

    }
    else
    {
      _logger.LogError("File Length is Zero...");
    }

    ViewBag.UploadSuccessfull = true;
    return View();
  }
}