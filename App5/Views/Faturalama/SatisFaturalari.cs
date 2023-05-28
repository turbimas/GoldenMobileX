using System.Data;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SatisFaturalari : Faturalar
    {
        public SatisFaturalari()
        {
            InvoiceType = viewModel.types.Where(s => s.Code == 1).FirstOrDefault();
        }


    }
}