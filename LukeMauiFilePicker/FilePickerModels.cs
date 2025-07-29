namespace LukeMauiFilePicker;

public interface IPickFile
{

    string FileName { get; }
    FileResult? FileResult { get; }

    Task<Stream> OpenReadAsync();

}

public class BaseSaveFileOptions(string suggestedFileName)
{
    public string SuggestedFileName { get; init; } = suggestedFileName;
    public (string FileTypeName, List<string> FileTypeExts) WindowsFileTypes { get; init; } = ("All Files", ["*.*"]);
    public string AndroidMimeType { get; init; } = "application/octet-stream";
}

public class SaveFileOptions(string suggestedFileName, Stream content) : BaseSaveFileOptions(suggestedFileName)
{

    public Stream Content { get; init; } = content;
}

public class DeferredSaveFileOptions : BaseSaveFileOptions
{
    private readonly Func<Task<Stream>> _getStreamAsync;

    public DeferredSaveFileOptions(string suggestedFileName, Func<Stream> getStream)
        : base(suggestedFileName)
    {
        _getStreamAsync = () => Task.FromResult(getStream());
    }

    public DeferredSaveFileOptions(string suggestedFileName, Func<Task<Stream>> getStreamAsync)
        : base(suggestedFileName)
    {
        _getStreamAsync = getStreamAsync;
    }

    public Task<Stream> GetContentAsync() => _getStreamAsync();
}