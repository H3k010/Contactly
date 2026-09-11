namespace ContactsManager.Application.DTO;

public class UploadResultDto
{
    public int ContactsAdded { get; set; }
    public int ContactsFailedToAdd { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}