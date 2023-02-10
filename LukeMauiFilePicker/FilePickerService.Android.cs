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

    public partial async Task<bool> SaveFileAsync(SaveFileOptions options)
    {
        var activity = await Platform.WaitForActivityAsync();
        var uri = await CreateDocumentService.Instance
            .RequestSaveFileAsync(options, activity);
        if (uri is null) { return false; }
                
        ArgumentNullException.ThrowIfNull(activity?.ContentResolver);

        using var outStream = activity.ContentResolver.OpenOutputStream(uri);
        ArgumentNullException.ThrowIfNull(outStream);

        await options.Content.CopyToAsync(outStream);

        return true;
    }

}

