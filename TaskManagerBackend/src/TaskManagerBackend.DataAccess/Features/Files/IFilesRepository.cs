namespace TaskManagerBackend.DataAccess.Features.Files;

public interface IFilesRepository
{
    IReadOnlyCollection<string> GetAllFiles();
    string GetFilePath(string fileName);
}