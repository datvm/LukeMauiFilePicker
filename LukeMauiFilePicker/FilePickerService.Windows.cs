using Windows.Storage;
using Windows.Storage.Pickers;

namespace LukeMauiFilePicker;

partial class FilePickerService
{

    public partial async Task<IEnumerable<IPickFile>?> PickFilesAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types, bool multiple)
        => await DefaultPickFilesAsync(title, types, multiple);

    static async Task<StorageFile?> GetSaveStorageFileAsync(BaseSaveFileOptions options)
    {
        var picker = CreatePicker();
        if (picker is null) { return null; }

        picker.FileTypeChoices.Add(options.WindowsFileTypes.FileTypeName, options.WindowsFileTypes.FileTypeExts);
        picker.SuggestedFileName = options.SuggestedFileName;

        var file = await picker.PickSaveFileAsync();
        return file;
    }

    public partial async Task<bool> SaveFileAsync(SaveFileOptions options)
    {
        var file = await GetSaveStorageFileAsync(options);
        if (file is null) { return false; }

        return await WriteToFileAsync(file, options.Content);
    }

    public partial async Task<bool> SaveFileAsync(DeferredSaveFileOptions options)
    {
        var file = await GetSaveStorageFileAsync(options);
        if (file is null) { return false; }

        await using var content = await options.GetContentAsync();
        return await WriteToFileAsync(file, content);
    }

    static async Task<bool> WriteToFileAsync(IStorageFile file, Stream content)
    {
        CachedFileManager.DeferUpdates(file);

        await using var stream = await file.OpenStreamForWriteAsync();
        stream.SetLength(0);

        await content.CopyToAsync(stream);

        var status = await CachedFileManager.CompleteUpdatesAsync(file);
        return status == Windows.Storage.Provider.FileUpdateStatus.Complete;
    }

    static FileSavePicker? CreatePicker()
    {
        var diag = new FileSavePicker();

        // Need this for HWND fix
        // https://github.com/dotnet/maui/issues/11527

        var currWin = Application.Current?.Windows.FirstOrDefault();
        var hwnd = (currWin?.Handler.PlatformView as MauiWinUIWindow)?.WindowHandle;
        if (hwnd is null) { return null; }

        WinRT.Interop.InitializeWithWindow.Initialize(diag, hwnd.Value);

        return diag;
    }


}
