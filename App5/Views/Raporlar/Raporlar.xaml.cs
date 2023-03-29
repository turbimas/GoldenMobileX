using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Raporlar : ContentPage
    {
        public RaporlarViewModel viewModel
        {
            get { return (RaporlarViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public int KayitID = 0;
        public string ReportModule = "Rapor";
        public Raporlar()
        {
            InitializeComponent();
            BindingContext = new RaporlarViewModel();
            this.Appearing += Raporlar_Appearing;
        }

        private void Raporlar_Appearing(object sender, EventArgs e)
        {
            List<X_Reports> _reports = DataLayer.X_Reports;

            BindingContext = new RaporlarViewModel() { reports = new List<X_Reports>(_reports) };
        }

        private void Raporlar_Tapped(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;



            MemoryStream mem = new MemoryStream();

            string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "1.pdf");
            File.WriteAllBytes(filepath, mem.ToArray());
            System.Diagnostics.Process.Start(filepath);
        }




    }
}

namespace GoldenMobileX.ViewModels
{
    public partial class RaporlarViewModel : BaseViewModel
    {
        public List<X_Reports> reports { get; set; }
        public RaporlarViewModel()
        {
            Title = "Raporlar";
        }
    }
}