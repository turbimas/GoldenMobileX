using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateAPK : ContentPage
    {
        public UpdateAPK()
        {
            InitializeComponent();
            Appearing += UpdateAPK_Appearing;
        }

        private async void UpdateAPK_Appearing(object sender, EventArgs e)
        {
           await DownloadApkAsync();
        }

        private async Task DownloadApkAsync()
        {

           string downloadedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "GoldenMobileX.apk");

            var success = await DownloadFileAsync("https://turbim.com/GoldenMobileX.apk", downloadedFilePath);

            if (success)
            {
               appSettings.UyariGoster($"File downloaded to: {downloadedFilePath}");
          /*

             await   Launcher.OpenAsync
                               (new OpenFileRequest()
                               {
                                   File = new ReadOnlyFile(downloadedFilePath)
                               }
                           );
          */
            }
            else
            {
                Console.WriteLine("Download failed");
            }
        }

        private async Task<bool> DownloadFileAsync(string fileUrl, string downloadedFilePath)
        {
            try
            {
                var client = new HttpClient();

                var downloadStream = await client.GetStreamAsync(fileUrl);

                var fileStream = File.Create(downloadedFilePath);

                await downloadStream.CopyToAsync(fileStream);

                return true;
            }
            catch (Exception ex)
            {
                //TODO handle exception
                return false;
            }
        }




    }

}

 