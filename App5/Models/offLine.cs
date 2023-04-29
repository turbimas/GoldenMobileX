using GoldenMobileX.Models;
using System.Collections.Generic;

namespace GoldenMobileX.OfflineData
{
    public class offLine
    {
        public offLine()
        {

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