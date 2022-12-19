using GoldenMobileX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Filtre : ContentPage
    {
        public Filtre()
        {
            InitializeComponent();
           
        }

        public event EventHandler FilterOk;


        public void filterOk(object sender, EventArgs e)
        {
            if (this.FilterOk != null)
                this.FilterOk(this, e);
        }
        public X_Types type { get; set; }
        public string aranacakKelime { get; set; }
        public DateTime tarih1 { get; set; }
        public DateTime tarih2 { get; set; }
        public bool filteractive { get; set; }
        private void FiltreButton_Clicked(object sender, EventArgs e)
        {
            aranacakKelime = AranacakKelimeler.Text;
            tarih1 = Tarih1.Date;
            tarih2 = Tarih2.Date;
            filteractive = true;
            filterOk(this, e);
        }

        private void FiltreTemizle_Clicked(object sender, EventArgs e)
        {
            type = null;
            aranacakKelime = "";
            filteractive = false;
            filterOk(this, e);
        }
    }
}