namespace LukeMauiFilePicker;

public interface IPickFile
{

    string FileName { get; }
    FileResult? FileResult { get; }

    Task<Stream> OpenReadAsync();

}

public class SaveFileOptions
{

    public string SuggestedFileName { get; init; }
    public Stream Content { get; init; }
    public (string FileTypeName, List<string> FileTypeExts) WindowsFileTypes { get; init; } = ("All Files", new() { "*.*" });
    public string AndroidMimeType { get; init; } = "application/octet-stream";

    public SaveFileOptions(string fileName, Stream content)
    {
        SuggestedFileName = fileName;
        Content = content;
    }
}