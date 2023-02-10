namespace LukeMauiFilePicker;

public interface IFilePickerService
{
    Task<IPickFile?> PickFileAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types);
    Task<IEnumerable<IPickFile>?> PickFilesAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types, bool multiple);
    Task<bool> SaveFileAsync(SaveFileOptions options);
}


public partial class FilePickerService : IFilePickerService
{

    public async Task<IPickFile?> PickFileAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types)
    {
        var files = await PickFilesAsync(title, types, false);
        return files?.FirstOrDefault();
    }
    public partial Task<IEnumerable<IPickFile>?> PickFilesAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types, bool multiple);
    public partial Task<bool> SaveFileAsync(SaveFileOptions options);

    static async Task<IEnumerable<IPickFile>?> DefaultPickFilesAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types, bool multiple)
    {
        // Use the MAUI default Picker for Android because it's still working well

        var options = new PickOptions()
        {
            PickerTitle = title,
        };

        if (types is not null)
        {
            options.FileTypes = new(types);
        }

        IEnumerable<FileResult>? files;
        if (multiple)
        {
            files = await FilePicker.Default.PickMultipleAsync(options);
        }
        else
        {
            var file = await FilePicker.Default.PickAsync(options);
            files = file is null ? null : new[] { file, };
        }

        return files?
            .Select(q => new DefaultPickFile(q))
            .ToList();
    }
    
    class DefaultPickFile : IPickFile
    {

        readonly FileResult file;
        public DefaultPickFile(FileResult file)
        {
            this.file = file;
        }

        public string FileName => file.FileName;

        public FileResult? FileResult => file;

        public Task<Stream> OpenReadAsync()
            => file.OpenReadAsync();
    }
}
