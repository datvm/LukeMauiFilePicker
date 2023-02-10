using Android.App;
using AndroidX.Activity;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using Uri = Android.Net.Uri;

namespace LukeMauiFilePicker;
partial class FilePickerService : Java.Lang.Object, IActivityResultCallback
{
    TaskCompletionSource<Uri?>? tcs;

    public void OnActivityResult(Java.Lang.Object? p0)
    {
        tcs?.TrySetResult(p0 as Uri);
    }

    public partial async Task<IEnumerable<IPickFile>?> PickFilesAsync(string title, Dictionary<DevicePlatform, IEnumerable<string>>? types, bool multiple)
        => await DefaultPickFilesAsync(title, types, multiple);

    public partial async Task<bool> SaveFileAsync(SaveFileOptions options)
    {
        using var activity = await Platform.WaitForActivityAsync();

        var uri = await LaunchCreateDocumentAsync(options, activity);
        if (uri is null) { return false; }
        
        ArgumentNullException.ThrowIfNull(activity?.ContentResolver);

        using var outStream = activity.ContentResolver.OpenOutputStream(uri);
        ArgumentNullException.ThrowIfNull(outStream);

        await options.Content.CopyToAsync(outStream);

        return true;
    }

    async Task<Uri?> LaunchCreateDocumentAsync(SaveFileOptions o, Activity activity)
    {
        if (activity is not ComponentActivity comp)
        {
            throw new InvalidOperationException("MainActivity is not a " + nameof(ComponentActivity));
        }

        tcs = new();

        var launcher = comp.RegisterForActivityResult(new ActivityResultContracts.CreateDocument(o.AndroidMimeType), this);
        launcher.Launch(o.SuggestedFileName);

        return await tcs.Task;
    }

}
