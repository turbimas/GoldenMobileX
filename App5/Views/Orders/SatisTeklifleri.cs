using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using GoldenMobileX.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

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