namespace Backend.Persistence.Logging;

public class FileLogger : IFileLogger {
    private readonly string _path = "Database/logs.txt";

    public void LogError(Exception ex, string mensaje) {
        var linea = $"[{DateTime.Now}] ERROR: {mensaje} | Ex: {ex.Message}{Environment.NewLine}";
        File.AppendAllText(_path, linea);
    }

    public void LogInfo(string mensaje) {
        File.AppendAllText(_path, $"[{DateTime.Now}] INFO: {mensaje}{Environment.NewLine}");
    }
}