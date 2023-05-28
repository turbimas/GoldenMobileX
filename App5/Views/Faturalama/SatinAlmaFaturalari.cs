using System.Data;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SatinAlmaFaturalari : Faturalar
    {
        public SatinAlmaFaturalari()
        {
            InvoiceType = viewModel.types.Where(s => s.Code == 0).FirstOrDefault();
        }


    }
}