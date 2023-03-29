using System.Data;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SatisTeklifleri : Siparisler
    {
        public SatisTeklifleri()
        {
            OrderType = viewModel.types.Where(s => s.Code == 4).FirstOrDefault();

        }


    }
}