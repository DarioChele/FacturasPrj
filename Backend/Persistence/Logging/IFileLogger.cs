namespace Backend.Persistence.Logging;

public interface IFileLogger {
    void LogError(Exception ex, string mensaje);
    void LogInfo(string mensaje);
}