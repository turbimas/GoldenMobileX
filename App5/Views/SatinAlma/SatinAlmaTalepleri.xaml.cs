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
    public partial class SatinAlmaTalepleri : ContentPage
    {
       public X_Types OrderType
        {
            get { return viewModel.types.Where(s => s.Code == 1).First(); }
        }
        public OrdersViewModel viewModel
        {
            get { return (OrdersViewModel)BindingContext; }
        }
        public SatinAlmaTalepleri()
        {
            InitializeComponent();
            this.BindingContext = new OrdersViewModel();
            Rebind();

        }



        void Rebind()
        {
            try
            {
                viewModel.ListEditable = true;
                IsBusy = true;
                viewModel.OrderList = DataLayer.TRN_Orders(OrderType.Code.convInt());
                this.BindingContext = new OrdersViewModel() { OrderList = new List<TRN_Orders>(viewModel.OrderList) };
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

        private void YeniFis_Clicked(object sender, EventArgs e)
        {
            Siparis fm = new Siparis();
            fm.newAdd = true;
            fm.viewModel = new OrdersViewModel()
            {
                Order = new TRN_Orders()
                {
                    FicheNo = appSettings.GetCounter("Orders", "TRN_Orders", "FicheNo", "TRN" + DateTime.Now.Year),
                    OrderDate = DateTime.Now,
                    OrderType_ = OrderType,
                    CurrencyID_ = DataLayer.X_Currency.Where(s => s.CurrencyNumber == 0).First(),
                    Branch = appSettings.UserDefaultBranch
                }
            };
            fm.Disappearing += Fm_Disappearing;

            Navigation.PushAsync(fm);

        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            Rebind();
        }



        private void Filtrele_Clicked(object sender, EventArgs e)
        {
            Rebind();
        }



        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            TRN_Orders t = (TRN_Orders)mi.CommandParameter;
            Siparis fm = new Siparis();

            fm.Disappearing += Fm_Disappearing;



            if ((t.OrderDate).convDateTime().Date < DateTime.Now.Date)
            {
                appSettings.UyariGoster("Geçmiş tarihli fişleri değiştiremezsiniz.");
                return;
            }

            viewModel.Order = t;
            fm.viewModel = viewModel;

            Navigation.PushAsync(fm);
        }


        private void Sil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            TRN_Orders t = (TRN_Orders)mi.CommandParameter;

            DataLayer.WaitingSent.tRN_Orders.Remove(t);
            DataLayer.WaitingSent.SaveJSON();
        }

        private void SunucuyaGonder_Invoked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_Orders t = (TRN_Orders)mi.CommandParameter;
            DataLayer.TRN_OrdersInsert(t);
            Rebind();
        }

        private void Gonderilmis_Clicked(object sender, EventArgs e)
        {
            viewModel.OrderList = DataLayer.Sent.tRN_Orders.Where(s => s.OrderType_ == OrderType).ToList();
            viewModel.ListEditable = false;
            this.BindingContext = new OrdersViewModel() { OrderList = new List<TRN_Orders>(viewModel.OrderList) };
 
        }
    }
}