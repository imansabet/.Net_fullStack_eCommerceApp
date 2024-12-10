namespace eCommerceApp.Application.Services.Interfaces.Logging;

public interface IAppLogger<T>
{
    void LogInformation(string message);
    void LogWarnning(string message);
    void LogError(Exception ex,string message);
}
