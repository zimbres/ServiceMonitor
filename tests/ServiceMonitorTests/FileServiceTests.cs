namespace ServiceMonitor.Services.Tests;

public class FileServiceTests
{
    private readonly Mock<ILogger<FileService>> _loggerMock;
    private readonly FileService _fileService;

    public FileServiceTests()
    {
        _loggerMock = new Mock<ILogger<FileService>>();
        _fileService = new FileService(_loggerMock.Object);
    }

    [Fact]
    public void CheckFile_FileDoesNotExist_FileMissingRestartTrue_ReturnsFalse()
    {
        string filePath = "nonexistentfile.txt";
        int minutes = 10;
        bool fileMissingRestart = true;

        bool result = _fileService.CheckFile(filePath, minutes, fileMissingRestart);

        Assert.False(result);
    }

    [Fact]
    public void CheckFile_FileDoesNotExist_FileMissingRestartFalse_ReturnsTrue()
    {
        string filePath = "nonexistentfile.txt";
        int minutes = 10;
        bool fileMissingRestart = false;

        bool result = _fileService.CheckFile(filePath, minutes, fileMissingRestart);

        Assert.False(result);
    }

    [Fact]
    public void CheckFile_FileExists_ModifiedWithinMinutes_ReturnsTrue()
    {
        string filePath = "existingfile.txt";
        int minutes = 10;
        bool fileMissingRestart = true;

        File.WriteAllText(filePath, "Test content");
        File.SetLastWriteTime(filePath, DateTime.Now.AddMinutes(-5));

        try
        {
            bool result = _fileService.CheckFile(filePath, minutes, fileMissingRestart);

            Assert.True(result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void CheckFile_FileExists_ModifiedOutsideMinutes_ReturnsFalse()
    {
        string filePath = "existingfile.txt";
        int minutes = 10;
        bool fileMissingRestart = true;

        File.WriteAllText(filePath, "Test content");
        File.SetLastWriteTime(filePath, DateTime.Now.AddMinutes(-15));

        try
        {
            bool result = _fileService.CheckFile(filePath, minutes, fileMissingRestart);

            Assert.False(result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}
