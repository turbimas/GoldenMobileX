using GoldenMobileX.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Diagnostics;
using System.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GoldenMobileX.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        public OrdersViewModel()
        {
            Items = DataLayer.V_AllItems;
            types = DataLayer.x_types_siparisTuru;
            durum = DataLayer.x_types_satisdurum;
            depolar = DataLayer.Depolar;
            cariList = DataLayer.Cariler;
            x_Currencies = DataLayer.X_Currency;
            ListEditable = true;
        }
        public Nullable<bool> ListEditable
        {
            get; set;
        }
        public List<CRD_StockWareHouse> depolar
        {
            get;set;
        }
        public List<X_Currency> x_Currencies
        {
            get; set;
        }
        public List<CRD_Cari> cariList
        {
            get; set;
        }
        public List<X_Types> types
        {
            get;set;
        }
        public List<X_Types> durum
        {
            get; set;
        }
        public List<V_AllItems> Items
        {
            get;
            set;
        }
        public List<TRN_Orders> OrderList
        {
            get; set;
        }
        public TRN_Invoice Invoice
        {
            get; set;
        }
        public TRN_Orders Order
        {
            get; set;
        }

        public TRN_OrderLines Line
        {
            get; set;
        }
   
    }
}