using System.Data;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SatinAlmaTeklifleri : Siparisler
    {
        public SatinAlmaTeklifleri()
        {
            OrderType = viewModel.types.Where(s => s.Code == 2).FirstOrDefault();

        }


    }
}