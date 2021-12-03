using System.ComponentModel.DataAnnotations;

namespace doc_manager.ViewModels;

public class DocumentCreateViewModel
{
  [Required]
  [Display(Name = "File Name")]
  public string FileName { get; set; }
  [Display(Name = "Choose File")]

  [Required]
  public IFormFile FormFile { get; set; }
  public bool IsHidden { get; set; }
}