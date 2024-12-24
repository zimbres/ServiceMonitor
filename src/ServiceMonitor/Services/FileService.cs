namespace ServiceMonitor.Services;

public class FileService
{
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    public bool CheckFile(string filePath, int minutes, bool fileMissingRestart)
    {
        if (!File.Exists(filePath) && fileMissingRestart)
        {
            _logger.LogError("The file at path {FilePath} is missing.", filePath);
            return false;
        }

        DateTime lastModified = File.GetLastWriteTime(filePath);
        double minutesDifference = (DateTime.Now - lastModified).TotalMinutes;
        bool isModifiedWithinMinutes = minutesDifference <= minutes;

        if(!isModifiedWithinMinutes)
        {
            _logger.LogError("The file at path {FilePath} modification is greater than the configured value {minutes} mintes.", filePath, minutes);
        }

        return isModifiedWithinMinutes;
    }
}
