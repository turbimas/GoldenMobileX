using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using Microsoft.EntityFrameworkCore;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Files : ContentPage
    {
        public string TableName = "";
        public string fileTypes = "Tümü|*.*|PDF|*.pdf|Resimler|*.jpg;*.jpeg;*.tiff;*.png|Dosyalar|*.xsl;*.xlsx;*.doc;*.docx;*.pps";
        public bool ReadOnly = false;
        public bool AciklamaGoster = true, AutoSave = false, SilmeAcik = true;

        public int RecordID = 0;
        /// <summary>
        public BaseViewModel viewModel
        {
            get { return (BaseViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public Files()
        {
            InitializeComponent();
            Appearing += Files_Appearing;
        }

        private void Files_Appearing(object sender, EventArgs e)
        {
            Rebind();
        }

        private void ResimSil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            viewModel.files.Remove((TRN_Files)mi.CommandParameter);
            if (DataLayer.IsOfflineAlert) return;
            using (GoldenContext c = new GoldenContext())
            {
                var file = new TRN_Files { ID = ((TRN_Files)mi.CommandParameter).ID };
                c.Entry(file).State = EntityState.Deleted;
                if (!c.SaveContextWithException()) return;
            }
            Rebind();
        }

        private async void ResimEkle_Clicked(object sender, EventArgs e)
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                if (stream != null)
                {
                    DateTime currDate = DateTime.Now;
                    viewModel.files.Add(new TRN_Files()
                    {
                        File = stream.convStreamToByteArray(),
                        FileName = currDate.ToString("yyyyMMddHHmm") + ".jpg",
                        Name = currDate.ToString("yyyyMMddHHmm"),
                        TableName=TableName,
                        RecordID = viewModel.SelectedFile.ID,
                        ID = 0
                    });
                    Rebind();
                }
            }
        }

        public void Rebind()
        {
            if (viewModel.files != null)
                filesListview.ItemsSource = new List<TRN_Files>(viewModel.files);
            else
            {
                if (DataLayer.IsOfflineAlert) return;
                using (GoldenContext c = new GoldenContext())
                {
                    filesListview.ItemsSource = c.TRN_Files.Where(s => s.TableName == TableName && s.RecordID == RecordID).OrderByDescending(s => s.ID).ToList();
                }
            }
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
                    Name = currDate.ToString("yyyyMMddHHmm"),
                    RecordID = viewModel.SelectedFile.ID,
                    ID = 0
                });

                Rebind();
            }
        }
    }
}