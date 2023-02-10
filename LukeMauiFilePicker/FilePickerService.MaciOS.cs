using Foundation;
using UIKit;
using UniformTypeIdentifiers;

namespace LukeMauiFilePicker;
partial class FilePickerService
{
    public partial async Task<IEnumerable<IPickFile>?> PickFilesAsync(
        string title,
        Dictionary<DevicePlatform, IEnumerable<string>>? types,
        bool multiple)
    {
#if IOS
        var platform = DevicePlatform.iOS;
#else
        var platform = DevicePlatform.MacCatalyst;
#endif

        if (types is null || !types.TryGetValue(platform, out var openingTypes))
        {
            openingTypes = new[] { "public.item", };
        }

        var utTypes = openingTypes
            .Select(q =>
                UTType.CreateFromIdentifier(q) ??
                UTType.CreateFromMimeType(q) ??
                UTType.CreateFromExtension(q) ??
                UTTypes.Item)
            .ToArray();

        var picker = new UIDocumentPickerViewController(
            utTypes,
            asCopy: true)
        {
            AllowsMultipleSelection = multiple
        };

        TaskCompletionSource<IEnumerable<IosFile>> filesTcs = new();
        picker.DidPickDocumentAtUrls += (_, e) =>
        {
            filesTcs.TrySetResult(e.Urls
            .Select(q => new IosFile(q))
                .ToList());
        };

        var controller = Platform.GetCurrentUIViewController();
        ArgumentNullException.ThrowIfNull(controller);

        await controller.PresentViewControllerAsync(picker, true);

        return await filesTcs.Task;
    }

    public partial async Task<bool> SaveFileAsync(SaveFileOptions options)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, options.SuggestedFileName);
        using (var file = File.Create(path))
        {
            await options.Content.CopyToAsync(file);
        }
        var url = new NSUrl(path, false);
        
        var picker = new UIDocumentPickerViewController(new[] { url });
        TaskCompletionSource<bool> filesTcs = new();
        picker.DidPickDocumentAtUrls += (_, e) =>
        {
            filesTcs.TrySetResult(e.Urls.Length > 0);
        };
        
        var controller = Platform.GetCurrentUIViewController();
        ArgumentNullException.ThrowIfNull(controller);
        await controller.PresentViewControllerAsync(picker, true);
        
        return await filesTcs.Task;
    }

    class IosFile : IPickFile
    {

        readonly NSUrl url;
        public IosFile(NSUrl url)
        {
            this.url = url;
        }

        public string FileName
            => Path.GetFileName(url.Path!);

        public FileResult? FileResult => null;

        public Task<Stream> OpenReadAsync()
            => Task.FromResult(new FileStream(url.Path!, FileMode.Open) as Stream);
    }

}
