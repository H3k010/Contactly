namespace ContactsManager.Application.Interfaces;

public interface IStepUpAuthentication
{
    bool IsAuthenticated(string action);

    void Authenticate(string action);

    void Clear(string action);
}