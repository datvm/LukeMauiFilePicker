using LukeMauiFilePicker;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilePickerDiExtensions
    {
        public static IServiceCollection AddFilePicker(this IServiceCollection services) =>
            services.AddSingleton<IFilePickerService>(FilePickerService.Instance);
    }
}

namespace Microsoft.Maui.Hosting
{

    public static partial class FilePickerHostingExtensions
    {

        public static MauiAppBuilder ConfigureFilePicker(this MauiAppBuilder builder)
            => ConfigureFilePicker(builder, 2112);

        public static MauiAppBuilder ConfigureFilePicker(this MauiAppBuilder builder, int reqCode)
        {
#if ANDROID
            RegisterCreateDocument(builder, reqCode);
#endif
            return builder;
        }
    }

}