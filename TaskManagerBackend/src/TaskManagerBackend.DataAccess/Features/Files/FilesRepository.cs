using TaskManagerBackend.Domain.Shared.Data;

namespace TaskManagerBackend.DataAccess.Features.Files;

public class FilesRepository : IFilesRepository
{
    public IReadOnlyCollection<string> GetAllFiles()
    {
        string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        
        if (!Directory.Exists(dirPath))
        {
            return [];
        }

        List<string> files = Directory.GetFiles(dirPath)
                                      .Select(Path.GetFileName)
                                      .Where(o => !string.IsNullOrWhiteSpace(o))
                                      .Cast<string>()
                                      .ToList();

        return files;
    }

    public string GetFilePath(string fileName)
    {
        IReadOnlyCollection<string> files = GetAllFiles();

        if (!files.Contains(fileName))
        {
            throw new NotFoundException();
        }
        
        string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        string filePath = Path.Combine(dirPath, fileName);

        return filePath;
    }
}