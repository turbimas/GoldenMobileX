using System.Data;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SatinAlmaTalepleri : Siparisler
    {
        public SatinAlmaTalepleri()
        {
            OrderType = viewModel.types.Where(s => s.Code == 1).FirstOrDefault();

        }


    }
}