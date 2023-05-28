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
    public partial class Siparisler : ContentPage
    {
        GoldenContext c = new GoldenContext();
        public X_Types OrderType
        {
            get; set;
        }
        public OrdersViewModel viewModel
        {
            get { return (OrdersViewModel)BindingContext; }
        }
        public Siparisler()
        {
            InitializeComponent();
            this.BindingContext = new OrdersViewModel();
            OrderType = viewModel.types.Where(s => s.Code == 5).FirstOrDefault();
            Appearing += Siparisler_Appearing;

        }

        private void Siparisler_Appearing(object sender, EventArgs e)
        {
            this.Title = OrderType.Name + " Listesi";
            Rebind();
        }

        void Rebind()
        {
            if (DataLayer.IsOfflineAlert) return;

            try
            {
                IsBusy = true;
                viewModel.OrderList =c.TRN_Orders.Where(s=>s.OrderType==OrderType.Code).Select(s=>s).OrderByDescending(s => s.ID).ToList();
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
                    Branch = appSettings.UserDefaultBranch,
                    Lines = new List<TRN_OrderLines>(),
                    Status = 0
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
            if(DataLayer.IsOfflineAlert) return;
            var mi = sender as SwipeItem;
            TRN_Orders t = (TRN_Orders)mi.CommandParameter;
            Siparis fm = new Siparis();

            fm.Disappearing += Fm_Disappearing;



            if (t.Status>0)
            {
                appSettings.UyariGoster("Bu fiş onaydan geçmiş. Değiştiremezsiniz.");
                return;
            }
            viewModel.Order = t;
            t.Lines = c.TRN_OrderLines.Where(s => s.OrderID == t.ID).ToList();
            fm.viewModel = viewModel;

            Navigation.PushAsync(fm);
        }


        private void Sil_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            var mi = sender as SwipeItem;
            TRN_Orders t = (TRN_Orders)mi.CommandParameter;
            c.TRN_OrderLines.RemoveRange(c.TRN_OrderLines.Where(s => s.OrderID == t.ID).ToList());
            c.TRN_Orders.Remove(t);
            c.SaveContextWithException();
            Rebind();
        }

 

        private void Onayla_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            var mi = sender as SwipeItem;
            TRN_Orders t = (TRN_Orders)mi.CommandParameter;

            t.Status = 1;
            c.TRN_Orders.Update(t);
            c.SaveContextWithException();
            Rebind();

        }

        private void Goruntule_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            var mi = sender as SwipeItem;
            TRN_Orders t = (TRN_Orders)mi.CommandParameter;
            Siparis fm = new Siparis();
            viewModel.Order = t;
            t.Lines = c.TRN_OrderLines.Where(s => s.OrderID == t.ID).ToList();
            fm.viewModel = viewModel;
            fm.ReadOnly = true;
            Navigation.PushAsync(fm);
        }

        private void Orders_Tapped(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            var mi = sender as StackLayout;
            TRN_Orders t = (TRN_Orders)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
 
            Siparis fm = new Siparis();

            fm.Disappearing += Fm_Disappearing;



            if (t.Status > 0)
            {
                appSettings.UyariGoster("Bu fiş onaydan geçmiş. Değiştiremezsiniz.");
                fm.ReadOnly = true;
            }
            viewModel.Order = t;
            t.Lines = c.TRN_OrderLines.Where(s => s.OrderID == t.ID).ToList();
            fm.viewModel = viewModel;

            Navigation.PushAsync(fm);
        }
    }
}