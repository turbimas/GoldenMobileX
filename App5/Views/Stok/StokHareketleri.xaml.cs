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
    public partial class StokHareketleri : ContentPage
    {
        public int ProductID = 0;
        public StokHareketleri()
        {
            InitializeComponent();
            Appearing += StokHareketleri_Appearing;
        }

        private void StokHareketleri_Appearing(object sender, EventArgs e)
        {
            if (!DataLayer.IsOnline)
            {
                appSettings.UyariGoster("Bu işlem için online olmalısınız.");
                return;
            }
            ListViewHareketler.ItemsSource = DataLayer.TRN_StockTransLinesByProductID(ProductID);
        }

        private void IlgiliKayit_Clicked(object sender, EventArgs e)
        {

        }
    }
}