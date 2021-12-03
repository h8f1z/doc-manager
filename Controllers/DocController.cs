using Microsoft.AspNetCore.Mvc;
using doc_manager.ViewModels;
using System.IO;
using doc_manager.Data;

namespace doc_manager.Controllers;

public class DocController : Controller
{
  private readonly ILogger<DocController> _logger;
  private readonly IConfiguration _config;
  private readonly DocumentContext _context;

  public DocController(ILogger<DocController> logger,
                       IConfiguration config,
                       DocumentContext context)
  {
    _logger = logger;
    _config = config;
    _context = context;
  }

  public IActionResult Upload()
  {
    _logger.LogInformation("Page Loading...");
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  // Set the limit to 5 MB
  [RequestFormLimits(MultipartBodyLengthLimit = 5242880)]
  public async Task<IActionResult> UploadAsync(DocumentCreateViewModel createVm)
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

      // save to database
      try{
        await _context.Document.AddAsync(new Models.Document{
          FileName = fileName,
          FilePath = filePath,
          IsHidden = createVm.IsHidden,
          UploadedAt = DateTime.Now,          
        });
        await _context.SaveChangesAsync();
        _logger.LogInformation("Document added to db");
      } catch (Exception ex) {
        _logger.LogError(ex.Message);
        ViewBag.ErrorMessage = "Could not upload document. Please contact admin";
        return View();
      }

    }
    else
    {
      _logger.LogError("File Length is Zero...");
    }

    ViewBag.UploadSuccessfull = true;
    return View();
  }
}