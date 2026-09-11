namespace ContactsManager.Application.DTO;

public class AuthenticationResultDto
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
}