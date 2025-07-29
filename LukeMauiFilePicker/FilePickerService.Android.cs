using Android.App;
using Android.Content;
using AndroidX.Activity;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using Uri = Android.Net.Uri;

namespace LukeMauiFilePicker;
partial class FilePickerService
{
    
    public partial async Task<IEnumerable<IPickFile>?> PickFilesAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types, bool multiple)
        => await DefaultPickFilesAsync(title, types, multiple);

    static async Task<Stream?> GetSaveStreamAsync(BaseSaveFileOptions o)
    {
        var activity = await Platform.WaitForActivityAsync();
        var uri = await CreateDocumentService.Instance
            .RequestSaveFileAsync(o, activity);
        if (uri is null) { return null; }

        ArgumentNullException.ThrowIfNull(activity?.ContentResolver);

        var outStream = activity.ContentResolver.OpenOutputStream(uri);
        ArgumentNullException.ThrowIfNull(outStream);

        return outStream;
    }

    public partial async Task<bool> SaveFileAsync(SaveFileOptions options)
    {
        await using var outStream = await GetSaveStreamAsync(options);
        if (outStream is null) { return false; }

        await options.Content.CopyToAsync(outStream);
        return true;
    }

    public partial async Task<bool> SaveFileAsync(DeferredSaveFileOptions options)
    {
        using var outStream = await GetSaveStreamAsync(options);
        if (outStream is null) { return false; }

        var content = await options.GetContentAsync();        
        await content.CopyToAsync(outStream);
        return true;
    }

}

