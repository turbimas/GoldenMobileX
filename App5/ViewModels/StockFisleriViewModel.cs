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
    [QueryProperty(nameof(TransType), nameof(TransType))]
    public class StokFisleriViewModel : BaseViewModel
    {
        private string _transType;
        public StokFisleriViewModel()
        {
            Items = DataLayer.V_AllItems;
            types = DataLayer.x_types_stokfisi;
            depolar = DataLayer.Depolar;
            cariList = DataLayer.Cariler;
            CheckListLines = new List<TRN_StockTransLines>();
        }
        public List<CRD_StockWareHouse> depolar
        {
            get;set;
        }
        public List<CRD_Cari> cariList
        {
            get; set;
        }
        public List<X_Types> types
        {
            get;set;
        }
        public string TransType
        {
            get { return _transType; }
            set { _transType = value;
                TransList = TransList.Where(s => s.Type == value.convInt()).ToList(); }
        }
        public List<V_AllItems> Items
        {
            get;
            set;
        }
        public List<TRN_StockTrans> TransList
        {
            get; set;
        }
        public TRN_Invoice Invoice
        {
            get; set;
        }
        public TRN_StockTrans Trans
        {
            get; set;
        }

        public TRN_StockTransLines Line
        {
            get; set;
        }
        public List<TRN_StockTransLines> CheckListLines { get; set; }
        public TRN_EtiketBasim EtiketBasim
        {
            get; set;
        }
        public List<TRN_EtiketBasim> EtiketBasim_List
        {
            get; set;
        }
        public TRN_EtiketBasimEmirleri EtiketBasimEmirleri
        {
            get; set;
        }
        public List<TRN_EtiketBasimEmirleri> EtiketBasimEmirleri_List
        {
            get; set;
        }

    }
}