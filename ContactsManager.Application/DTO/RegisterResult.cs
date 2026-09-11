namespace ContactsManager.Application.DTO;

public class RegisterResult
{
    public bool Succeeded { get; set; }
    public Guid UserId { get; set; }
    public bool IsEmailDuplicate { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
}