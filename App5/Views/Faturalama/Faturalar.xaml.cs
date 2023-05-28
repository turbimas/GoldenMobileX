using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Faturalar : ContentPage
    {
        GoldenContext c = new GoldenContext();
     public   StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
        }
        public X_Types InvoiceType
        {
            get; set;
        }
        public Faturalar()
        {
            InitializeComponent();
            this.BindingContext = new StokFisleriViewModel();
            Appearing += StokFisleri_Appearing;
        }

        private void StokFisleri_Appearing(object sender, EventArgs e)
        {
            Rebind();
        }

        DataTable items = new DataTable();
        void Rebind()
        {
            if (DataLayer.IsOfflineAlert) { return; }
            try
            {
                IsBusy = true;
                int warehouse = appSettings.User.WareHouseID.convInt();
                var t = c.TRN_Invoice.Where(s => s.Date > DateTime.Now.AddYears(-1) && s.Type==InvoiceType.Code).Select(s => s).OrderByDescending(s => s.ID);


                this.BindingContext = new StokFisleriViewModel() { InvoiceList = new List<TRN_Invoice>(t) };
                IsBusy = false;
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster(ex.Message);
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void YeniFis_Clicked(object sender, EventArgs e)
        {
            Fatura fm = new Fatura();
            fm.newAdd = true;
            viewModel.Invoice = new TRN_Invoice()
            {
                InvoiceNo = appSettings.GetCounter("Fatura", "TRN_Invoice", "InvoiceNo", "TRN" + DateTime.Now.Year),
                Date = DateTime.Now,
                Lines = new List<TRN_StockTransLines>(),
                Status = 0,
                Branch = appSettings.UserDefaultBranch
            };
            fm.viewModel = viewModel;
            fm.Disappearing += Fm_Disappearing;

            Navigation.PushAsync(fm);
        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            Rebind();
        }



        private void Refresh_Clicked(object sender, EventArgs e)
        {
            Rebind();
        }



        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) { return; }
            var mi = sender as SwipeItem;
            TRN_Invoice t = (TRN_Invoice)mi.CommandParameter;
            Fatura fm = new Fatura();
            fm.Disappearing += Fm_Disappearing;
            t.Lines = c.TRN_StockTransLines.Where(s => s.InvoiceID == t.ID).ToList();
            viewModel.Invoice = t;
            fm.viewModel = viewModel;
            Navigation.PushAsync(fm);
        }


        private async void Sil_Clicked(object sender, EventArgs e)
        {
            if (!await appSettings.Onay("Fiş Silinecektir..")) return;
            if(DataLayer.IsOfflineAlert) { return; }
            var mi = sender as SwipeItem;
            TRN_Invoice t = (TRN_Invoice)mi.CommandParameter;
            c.TRN_Invoice.Remove(t);
            Rebind();
        }

        private void Onayla_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            var mi = sender as SwipeItem;
            TRN_StockTrans t = (TRN_StockTrans)mi.CommandParameter;
            if (t.Type_.Direction != 0)
            {
                appSettings.UyariGoster("Bu kaydı onaylayamazsınız..");
                return;
            }
            if ((t.DestStockWareHouseID_?.ID).convInt() == appSettings.User.WareHouseID)
            {
                t.Status = 1;
                c.TRN_StockTrans.Update(t);
                c.SaveContextWithException();
                Rebind();
            }
            else
                appSettings.UyariGoster("Bu kaydı onaylayamazsınız..");
        }

        private void Goruntule_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) { return; }
            var mi = sender as SwipeItem;
            TRN_Invoice t = (TRN_Invoice)mi.CommandParameter;
            Fatura fm = new Fatura();
            viewModel.Invoice = t;
            if (t.ID > 0)
                if (t.Lines == null || t.Lines?.Count() == 0)
                {
                    try
                    {
                        using (GoldenContext c = new GoldenContext())
                        {
                            t.Lines = c.TRN_StockTransLines.Where(s => s.InvoiceID == t.ID).Select(s => s).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.UyariGoster();
                    }
                }
            fm.viewModel = viewModel;
            fm.ReadOnly = true;
            Navigation.PushAsync(fm);
        }
    }
}


