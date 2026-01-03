namespace Application.Interfaces;

public interface IFileStorageService
{
    Task SaveAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
    Task RemoveAsync(string relativePath, CancellationToken cancellationToken = default);
    string GetRelativePath(string fileName);
}
