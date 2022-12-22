using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldenMobileX.ViewModels;
using GoldenMobileX.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cihazlar : ContentPage
    {
        public BakimOnarimViewModel viewModel
        {
            get; set;
        }
        public Cihazlar()
        {
            InitializeComponent();
        }
    }
}