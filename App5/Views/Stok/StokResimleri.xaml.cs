using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using Microsoft.EntityFrameworkCore;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StokResimleri : ContentPage
    {
        public StoklarViewModel viewModel
        {
            get { return (StoklarViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public StokResimleri()
        {
            InitializeComponent();
            Appearing += StokResimleri_Appearing;
        }

        private void StokResimleri_Appearing(object sender, EventArgs e)
        {
            filesListview.ItemsSource = new List<TRN_Files>(DataLayer.TRN_Files(viewModel.item.ID));
        }

        private void  ResimSil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            viewModel.files.Remove((TRN_Files)mi.CommandParameter);
            if (DataLayer.IsOfflineAlert) return;
            using(GoldenContext c = new GoldenContext())
            {
                var file = new TRN_Files { ID = ((TRN_Files)mi.CommandParameter).ID };
                c.Entry(file).State = EntityState.Deleted;
                if (!c.SaveContextWithException()) return;
            }
 
            Rebind();

        }
        public void Rebind()
        {

            filesListview.ItemsSource = new List<TRN_Files>(viewModel.files);
        }
        private async void BtnResimCek_Clicked(object sender, EventArgs e)
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
 


            DateTime currDate = DateTime.Now;
            var PhotoFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "GoldenERPPictures",
                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear,
                Name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small

            });
            if (PhotoFile != null)
            {

                viewModel.files.Add(new TRN_Files()
                {
                    File = PhotoFile.GetStream().convStreamToByteArray(),
                    FileName = currDate.ToString("yyyyMMddHHmm") + ".jpg",
                    Name = currDate.ToString("yyyyMMddHHmm")
                ,
                    RecordID = viewModel.item.ID,
                    ID=0
                });
           
                Rebind();
            }

        }

    }
}