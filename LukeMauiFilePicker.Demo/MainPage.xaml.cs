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
        if (file is null) { return; }

        await OnFilesPickedAsync(new[] { file });
    }

    private async void PickMany(object sender, EventArgs e)
    {
        var files = await picker.PickFilesAsync("Select a file", FileType, true);
        if (files is null || !files.Any()) { return; }

        await OnFilesPickedAsync(files);
    }

    private async void Save(object sender, EventArgs e)
    {
        var bytes = Encoding.UTF8.GetBytes(TextEditor.Text ?? "");
        using var memory = new MemoryStream(bytes);

        await picker.SaveFileAsync(new("text.txt", memory)
        {
            AndroidMimeType = "text/plain",
            WindowsFileTypes = ("Text files", new() { ".txt", })
        });
    }

}

