namespace ContactsManager.Presentation.Models;

public class ImportViewModel
{
    public bool IsSuccess { get; set; }
    public string UploadError { get; set; } = string.Empty;
    public string SuccessUpload { get; set; } = string.Empty;
    public int ContactsAdded { get; set; } 
    public int ContactsFailedToAdd { get; set; } 
}