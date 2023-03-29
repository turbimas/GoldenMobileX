using Plugin.Media;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarkodOku : ContentPage
    {
        public BarkodOku()
        {
            InitializeComponent();
            Oku();
        }
        private async void Oku()
        {
            ZXingScannerPage scanPage = new ZXingScannerPage();

            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();


                });
            };
            await Navigation.PushAsync(scanPage);
        }
        async void TakePhoto(string photoname = "")
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    appSettings.UyariGoster("Telefonda kamera kullanımı etkinleştirilmemiş.");
                    return;
                }
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("Kamera etkinleştirilemedi. " + ex.Message);
            }
            try
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "GoldenERPPictures",
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear,
                    Name = ((photoname == "") ? DateTime.Now.ToString("yyyyMMddHHmmss") + "" : photoname) + ".jpg"

                });
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("Fotoğraf çekilirken hata oluştu.. " + ex.Message);
            }
        }
    }
}