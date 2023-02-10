using Android.App;
using Android.Content;

using Uri = Android.Net.Uri;
using Android.Runtime;

namespace LukeMauiFilePicker;

internal class CreateDocumentService
{
    public static readonly CreateDocumentService Instance = new();

    public int ReqCode { get; set; }
    public bool RegisteredForActivityResult { get; set; }

    TaskCompletionSource<Uri?>? tcs;

    public async Task<Uri?> RequestSaveFileAsync(SaveFileOptions o, Activity? activity)
    {
        if (!RegisteredForActivityResult)
        {
            throw new InvalidOperationException("Service is not initialized. Please call builder.ConfigureFilePicker() in your MauiProgram.cs.");
        }

        tcs = new();

        var i = GetIntent(o);

        activity ??= await Platform.WaitForActivityAsync();
        activity.StartActivityForResult(i, ReqCode);

        return await tcs.Task;
    }

    public void OnActivityResult(Activity _, int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode != ReqCode || tcs is null) { return; }

        if (resultCode == Result.Canceled)
        {
            tcs.TrySetResult(null);
            return;
        }
        else if (resultCode != Result.Ok)
        {
            tcs.TrySetException(new ArgumentException("The result wasn't OK: " + resultCode));
        }

        tcs.TrySetResult(data?.Data);
        tcs = null;
    }

    Intent GetIntent(SaveFileOptions o)
    {
        var i = new Intent(Intent.ActionCreateDocument);
        i.AddCategory(Intent.CategoryOpenable);
        i.SetType(o.AndroidMimeType);
        i.PutExtra(Intent.ExtraTitle, o.SuggestedFileName);

        return i;
    }

}

