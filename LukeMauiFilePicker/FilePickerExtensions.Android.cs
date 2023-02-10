using LukeMauiFilePicker;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui.Hosting;

partial class FilePickerHostingExtensions
{

    static void RegisterCreateDocument(MauiAppBuilder builder, int reqCode)
    {
        builder.ConfigureLifecycleEvents(config =>
        {
            config.AddAndroid(and =>
            {
                var srv = CreateDocumentService.Instance;
                srv.ReqCode = reqCode;
                srv.RegisteredForActivityResult = true;

                and.OnActivityResult(srv.OnActivityResult);
            });
        });
    }

}
