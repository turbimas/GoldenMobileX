using GoldenMobileX.Models;
using System.Collections.Generic;

namespace GoldenMobileX.OfflineData
{
    public class offLine
    {
        public offLine()
        {

        }
        public class Products
        {
            public Products()
            {
                v_AllItems = new List<V_AllItems>();
                v_AllItemsWaitingSend = new List<V_AllItems>();
            }
            public List<V_AllItems> v_AllItems { get; set; }
            public List<V_AllItems> v_AllItemsWaitingSend { get; set; }
        }
        public class Cari
        {
            public Cari()
            {
                cRD_Cari = new List<CRD_Cari>();
                cRD_CariWaitingSend = new List<CRD_Cari>();
            }
            public List<CRD_Cari> cRD_Cari { get; set; }
            public List<CRD_Cari> cRD_CariWaitingSend { get; set; }
        }

        public class Types
        {
            public Types()
            {
                X_Types = new List<X_Types>();
                X_Settings = new List<x_Settings>();
                L_Units = new List<L_Units>();
                X_Currency = new List<X_Currency>();

            }
            public List<X_Types> X_Types { get; set; }
            public List<x_Settings> X_Settings { get; set; }
            public List<L_Units> L_Units { get; set; }
            public List<X_Currency> X_Currency { get; set; }
        }
        public class CRD
        {
            public CRD()
            {
                CRD_StockWareHouse = new List<CRD_StockWareHouse>();
                CRD_Bankalar = new List<CRD_Bankalar>();
                CRD_Kasalar = new List<CRD_Kasalar>();
                CRD_BankaHesaplari = new List<CRD_BankaHesaplari>();
            }
            public List<CRD_StockWareHouse> CRD_StockWareHouse { get; set; }

            public List<CRD_BankaHesaplari> CRD_BankaHesaplari { get; set; }
            public List<CRD_Bankalar> CRD_Bankalar { get; set; }
            public List<CRD_Kasalar> CRD_Kasalar { get; set; }
        }

        public class WaitingSent
        {
            public WaitingSent()
            {
                tRN_StockTrans = new List<TRN_StockTrans>();
                tRN_Orders = new List<TRN_Orders>();
                TRN_EtiketBasim = new List<TRN_EtiketBasim>();
            }
            public List<TRN_StockTrans> tRN_StockTrans { get; set; }
            public List<TRN_Orders> tRN_Orders { get; set; }
            public List<TRN_EtiketBasim> TRN_EtiketBasim { get; set; }
        }

        public class Sent
        {
            public Sent()
            {
                tRN_StockTrans = new List<TRN_StockTrans>();
                tRN_Orders = new List<TRN_Orders>();
                TRN_EtiketBasim = new List<TRN_EtiketBasim>();
                DepoTransferleri = new List<TRN_StockTrans>();
            }
            public List<TRN_StockTrans> tRN_StockTrans { get; set; }
            public List<TRN_Orders> tRN_Orders { get; set; }
            public List<TRN_EtiketBasim> TRN_EtiketBasim { get; set; }
            public List<TRN_StockTrans> DepoTransferleri { get; set; }
        }
        class DepoTransfer
        {
            public TRN_StockTrans GelenFis { get; set; }
            public TRN_StockTrans FazlaGelen { get; set; }
            public TRN_StockTrans EksikGelen { get; set; }
        }
    }
}