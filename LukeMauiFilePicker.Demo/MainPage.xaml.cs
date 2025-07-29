using System.Text;

namespace LukeMauiFilePicker.Demo;

public partial class MainPage : ContentPage
{
    static readonly Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
    {
        {  DevicePlatform.Android, new[] { "text/*" } } ,
        { DevicePlatform.iOS, new[] { "public.json", "public.plain-text" } },
        { DevicePlatform.MacCatalyst, new[] { "public.json", "public.plain-text" } },
        { DevicePlatform.WinUI, new[] { ".txt", ".json" } }
    };

    readonly IFilePickerService picker;
    public MainPage(IFilePickerService picker)
    {
        this.picker = picker;

        InitializeComponent();
    }

    async Task OnFilesPickedAsync(IEnumerable<IPickFile> files)
    {
        var str = new StringBuilder();

        foreach (var f in files)
        {
            using var s = await f.OpenReadAsync();
            using var reader = new StreamReader(s);

            str.AppendLine(await reader.ReadToEndAsync());
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            TextEditor.Text = str.ToString();
        });
    }

    private async void PickOne(object sender, EventArgs e)
    {
        var file = await picker.PickFileAsync("Select a file", FileType);
        if (file is null)
        {
            await DisplayAlert("Cancelled", "No file was selected.", "OK");
            return;
        }

        await OnFilesPickedAsync(new[] { file });
    }

    private async void PickMany(object sender, EventArgs e)
    {
        var files = await picker.PickFilesAsync("Select a file", FileType, true);
        if (files is null || !files.Any())
        {
            await DisplayAlert("Cancelled", "No files were selected.", "OK");
            return;
        }

        await OnFilesPickedAsync(files);
    }

    MemoryStream GetContentStream()
    {
        var bytes = Encoding.UTF8.GetBytes(TextEditor.Text ?? "");
        return new MemoryStream(bytes);
    }

    private async void Save(object sender, EventArgs e)
    {
        using var memory = GetContentStream();

        var result = await picker.SaveFileAsync(new SaveFileOptions("text.txt", memory)
        {
            AndroidMimeType = "text/plain",
            WindowsFileTypes = ("Text files", [".txt",])
        });

        await DisplayAlert("File Saved", result ? "File saved successfully." : "File save cancelled.", "OK");
    }

    private async void DeferredSave(object sender, EventArgs e)
    {
        // Simulate generating content
        async Task<Stream> streamFn()
        {
            await Task.Delay(2000);
            return GetContentStream();
        }

        var result = await picker.SaveFileAsync(new DeferredSaveFileOptions("deferred.txt", streamFn)
        {
            AndroidMimeType = "text/plain",
            WindowsFileTypes = ("Text files", [".txt",])
        });

        await DisplayAlert("File Saved", result ? "File saved successfully." : "File save cancelled.", "OK");
    }
}

