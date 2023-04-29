using GoldenMobileX.Models;
using GoldenMobileX.OfflineData;
using GoldenMobileX.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
class DataLayer
{
    public DataLayer()
    {

    }

    public async static void LoadStaticData()
    {
        SqlLiteInitDataBase();
        await Device.InvokeOnMainThreadAsync(() => appSettings.activity.viewModel = new BaseViewModel() { activityText = "Sqlite database initialized.." });

        offLineMod = true;

        WaitingSent = new offLine.WaitingSent();
        Sent = new offLine.Sent();

        v_DepodakiLotlar = new List<V_DepodakiLotlar>();
        WaitingSent.tRN_StockTrans = new List<TRN_StockTrans>();
        WaitingSent.TRN_EtiketBasim = new List<TRN_EtiketBasim>();
        WaitingSent.tRN_Orders = new List<TRN_Orders>();
        LoadDataFromSQL(true);
    }
    public static offLine.WaitingSent WaitingSent { get; set; }
    public static offLine.Sent Sent { get; set; }
    public static bool offLineMod
    {
        get; set;
    }
    public static bool IsOnline
    {
        get
        {
            db.connTimeOut = minConnTimeOut;
            return db.SQLSelect("SELECT 1").convBool();
        }
    }
    public static bool IsOfflineAlert
    {
        get
        {
            bool offline = !IsOnline;
            if (offline) appSettings.UyariGoster("Bu işlem için online olmanız gerekmektedir.");
            return offline;
        }
    }
    static int maxConnTimeOut = 100, minConnTimeOut = 1;
    public static List<CRD_StockWareHouse> Depolar
    {
        get;set;
    }
    public static List<V_DepodakiLotlar> DepolakiLotlar
    {
        get; set;
    }
    public static List<CRD_BankaHesaplari> CRD_BankaHesaplari
    {
        get;set;
    }
    public static List<CRD_Bankalar> CRD_Bankalar
    {
        get;set;
    }
    public static List<CRD_Kasalar> CRD_Kasalar
    {
        get;set;
    }
    public static List<X_Reports> X_Reports
    {
        get; set;
    }

    public static List<x_Settings> X_Settings
    {
        get;set;
    }

    public static List<X_Currency> X_Currency
    {
        get;set;
    }
    static List<CRD_Cari> _Cariler;
    public static List<CRD_Cari> Cariler
    {
        get
        {
            try
            {

                if (_Cariler == null)
                    using (GoldenContext c = new GoldenContext())
                    {
                        _Cariler = c.CRD_Cari.Where(s => s.Active == true).ToList().Select(s => s).ToList();
                    }
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("CRD_Cari : " + ex.Message + ex.InnerException?.Message);
            }
            return _Cariler;
        }
        set { _Cariler = value; }
    }
    static List<V_AllItems> _V_AllItems;
    public static List<V_AllItems> V_AllItems
    {
        get
        {
            try
            {

                if (_V_AllItems == null)
                {
                    using (GoldenContext c = new GoldenContext())
                    {
                        _V_AllItems = c.V_AllItems.Where(s => s.Active == true).Select(s => s).ToList();
                    }

                }
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("V_AllItems : " + ex.Message + ex.InnerException?.Message);
            }
            return _V_AllItems;
        }
        set { _V_AllItems = value; }
    }
    static List<V_DepodakiLotlar> _v_DepodakiLotlar;
    public static List<V_DepodakiLotlar> v_DepodakiLotlar
    {
        get {
            try
            {

                if (_v_DepodakiLotlar == null)
                {
                    using (GoldenContext c = new GoldenContext())
                    {

                        v_DepodakiLotlar = c.V_DepodakiLotlar.Select(s => s).ToList();

                    }

                }
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("V_DepodakiLotlar : " + ex.Message + ex.InnerException?.Message);
            }
            return _v_DepodakiLotlar;
        } set { _v_DepodakiLotlar = value; }
    }
    public static List<X_Types> X_Types { get; set; }
    public static List<L_Units> L_Units { get; set; }
    public static Color statusbackColor(int Status)
    {
        switch (Status)
        {
            case -1:
                return Color.FromRgb(255, 204, 204);
            case 1:
                return Color.FromRgb(204, 255, 204);
            case 2:
                return Color.FromRgb(255, 255, 204);
            case 3:
                return Color.FromRgb(204, 255, 255);
            case 4:
                return Color.FromRgb(204, 204, 255);
            case 5:
                return Color.FromRgb(255, 204, 229);
            case 6:
                return Color.FromRgb(204, 255, 229);
            case 7:
                return Color.FromRgb(255, 229, 204);
            case 11:
                return Color.FromRgb(204, 229, 255);
        }
        return Color.FromRgb(204, 229, 255);
    }
    public static Color moduleColor(string module)
    {
        switch (module)
        {
            case "FATURA":
                return Color.FromRgb(102, 0, 0);
            case "BANKA":
                return Color.FromRgb(51, 102, 0);
            case "KASA":
                return Color.FromRgb(0, 51, 102);
            case "CARI":
                return Color.FromRgb(102, 102, 0);
            case "CEKSENET":
                return Color.FromRgb(51, 0, 102);

        }
        return Color.FromRgb(51, 0, 102);
    }
    public static List<X_Types> x_types_bankahesaplari
    {
        get { return new List<X_Types>(X_Types.Where(x => x.TableName == "CRD_BankaHesaplari" && x.ColumnsName == "Turu").ToList()); }
    }

    public static List<X_Types> x_types_carihesap { get; set; }
    public static List<X_Types> x_types_stokkarti { get; set; }
    public static List<X_Types> x_types_stokfisi { get; set; }
    public static List<X_Types> x_types_Invoice { get; set; }
    public static List<X_Types> x_types_BankaIslem { get; set; }
    public static List<X_Types> x_types_siparisTuru { get; set; }
    public static List<X_Types> x_types_SatisSiparisleriDurum { get; set; }
    public static List<X_Types> x_types_SatinAlmaSiparisleriDurum { get; set; }


    static void LoadTypes()
    {
        x_types_carihesap = X_Types.Where(x => x.TableName == "Cari" && x.ColumnsName == "Turu").ToList();
        x_types_stokkarti = X_Types.Where(x => x.TableName == "TRN_StockTransLines" && x.ColumnsName == "ProductType").ToList();
        x_types_stokfisi = X_Types.Where(x => x.TableName == "TRN_StockTrans" && x.ColumnsName == "Type").ToList();
        x_types_Invoice = X_Types.Where(x => x.TableName == "TRN_Invoice" && x.ColumnsName == "Type").ToList();
        x_types_siparisTuru = X_Types.Where(x => x.TableName == "Orders" && x.ColumnsName == "Type").ToList();
        x_types_SatisSiparisleriDurum = X_Types.Where(x => x.TableName == "OrderStatus" && x.ColumnsName == "5").ToList();
        x_types_SatinAlmaSiparisleriDurum = X_Types.Where(x => x.TableName == "OrderStatus" && x.ColumnsName == "3").ToList();
        x_types_BankaIslem=X_Types.Where(x => x.TableName == "TRN_BankaHareketleri" && x.ColumnsName == "TurKodu").ToList();
    }
 
    public static void LoadDataFromSQL(bool FromServer = false)
    {

        if (IsOnline)
        {
            db.connTimeOut = maxConnTimeOut;
            GoldenContext c = new GoldenContext();

            try
            {
                DataLayer.Depolar = c.CRD_StockWareHouse.Select(s => s).ToList().Where(r => r.AuthCode + "" == "" || appSettings.UserAuthCode.Split(',').Any(x => x == "*") || (r.AuthCode + "").Split(',').Any(x => (appSettings.UserAuthCode + "").Split(',').Contains(x))).Select(s => s).ToList();
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("CRD_StockWareHouse : " + ex.Message + ex.InnerException?.Message);
            }


            try
            {
                DataLayer.X_Types = c.X_Types.Select(s => s).ToList();
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("X_Types : " + ex.Message + ex.InnerException?.Message);
            }
            LoadTypes();
            try
            {
       
                    DataLayer.X_Settings = c.x_Settings.Select(s => s).ToList();
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("X_Settings : " + ex.Message + ex.InnerException?.Message);
            }
            try
            {
     
                    DataLayer.L_Units = c.L_Units.Select(s => s).ToList();
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("L_Units : " + ex.Message + ex.InnerException?.Message);
            }
            try
            {
      
                    DataLayer.X_Currency = c.X_Currency.Select(s => s).ToList();
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("X_Currency : " + ex.Message + ex.InnerException?.Message);
            }

 
            try
            {
                CRD_BankaHesaplari = c.CRD_BankaHesaplari.Select(s => s).ToList();
                CRD_Bankalar = c.CRD_Bankalar.Select(s => s).ToList();
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster("CRD_Bankalar_CRD_BankaHesaplari : " + ex.Message + ex.InnerException?.Message);
            }
 

            // Start a new task (this launches a new thread)
            Task.Factory.StartNew(() =>
            {
                //Evet Here
            }).ContinueWith(task =>
            {

            }, TaskScheduler.FromCurrentSynchronizationContext());
            db.connTimeOut = minConnTimeOut;
            c.Dispose();
        }
    }

    public class Online
    {

    }

    #region Offline
    public static List<TRN_Orders> TRN_Orders(int OrderType)
    {

        return WaitingSent.tRN_Orders.Where(s => s.OrderType_.Code == OrderType).ToList();
    }

    #endregion

    #region Online
    public static List<TRN_StockTrans> TRN_StockTransGelenTransfer()
    {
        try
        {
            using (GoldenContext c = new GoldenContext())
            {
                if (DataLayer.Depolar.Count() == 0)
                {
                    appSettings.UyariGoster("İşlem yapabileceğiniz bir depo bulunmamaktadır.");
                    return new List<TRN_StockTrans>();
                }
                if (IsOfflineAlert)
                {
                    return new List<TRN_StockTrans>();
                }

                int warehouse = (appSettings.User.WareHouseID + "").convInt();
                List<TRN_StockTrans> trn_StockTrans = c.TRN_StockTrans.Where(s => s.Status == 4 && s.Type == 2 && s.DestStockWareHouseID == warehouse).Select(s => s).OrderByDescending(s => s.ID).ToList();

                foreach (var t in trn_StockTrans)
                {
                    if (DataLayer.WaitingSent.tRN_StockTrans.Where(s => s.ID == t.ID).Count() == 0)
                    {
                        t.Lines = TRN_StockTransLines(t.ID);
                        DataLayer.WaitingSent.tRN_StockTrans.Add(t);
                    }
                }

                return DataLayer.WaitingSent.tRN_StockTrans;
            }
        }
        catch (Exception ex)
        {
            Task.Run(() => ex.UyariGoster());
            return new List<TRN_StockTrans>();
        }
    }
    public static List<TRN_StockTransLines> TRN_StockTransLines(int TransID)
    {
        try
        {
            using (GoldenContext c = new GoldenContext())
            {
                return c.TRN_StockTransLines.Where(s => s.StockTransID == TransID).OrderByDescending(s => s.ID).ToList();
            }
        }
        catch (Exception ex)
        {
            Task.Run(() => ex.UyariGoster());
            return new List<TRN_StockTransLines>();
        }
    }
    public static List<TRN_StockTransLines> TRN_StockTransLinesByProductID(int ProductID)
    {
        try
        {
            using (GoldenContext c = new GoldenContext())
            {
                return c.TRN_StockTransLines.Where(s => s.ProductID == ProductID).OrderByDescending(s => s.ID).ToList();
            }
        }
        catch (Exception ex)
        {
            Task.Run(() => ex.UyariGoster());
            return new List<TRN_StockTransLines>();
        }
    }
    public static void TRN_StockTransInsert(TRN_StockTrans t, bool showError = true)
    {
        if (t.ID > 0 && t.Status != 6 && t.Status != 1)
        {
            if (showError)
                appSettings.UyariGoster("Bu işlem sunucuya gönderilmiş. Tekrar gönderemezsiniz.");
            DataLayer.MoveToAnotherList(DataLayer.WaitingSent.tRN_StockTrans, DataLayer.Sent.tRN_StockTrans, t);
            DataLayer.WaitingSent.SaveJSON();
            DataLayer.Sent.SaveJSON();
            return;
        }
        if (t.ID > 0 && t.Status == 4 && t.DestStockWareHouseID == appSettings.User.WareHouseID)
        {
            return;
        }
        if (IsOfflineAlert) return;
        using (GoldenContext c = new GoldenContext())
        {
            c.Database.BeginTransaction();
            if (t.ID > 0)
                c.TRN_StockTrans.Update(t);
            else
                c.TRN_StockTrans.Add(t);

            if (!c.SaveContextWithException()) return;

            if (t.ID > 0)
            {
                foreach (var l in t.Lines)
                {
                    l.StockTransID = t.ID;
                    l.CurrencyID = t.CurrencyID;
                    l.StockWareHouseID = t.StockWareHouseID;
                    l.DestStockWareHouseID = t.DestStockWareHouseID;
                    l.Date = t.TransDate;
                    l.Type = t.Type_.Code;
                    l.Status = t.Status_?.Code;
                    if (DataLayer.DepolakiLotlar?.Count() > 0)
                        DataLayer.DepolakiLotlar.Add(new V_DepodakiLotlar() { ProductID = l.ProductID, SeriLot = l.SeriLot, SeriNo = l.SeriNo, LotID = l.LotID, BalyaNo = l.BalyaNo, Miktar = (l.Amount * l.Direction).convDouble(), Direction = l.Direction, Date = l.Date, Depo = (l.Direction == 1 ? l.DestStockWareHouseID : l.StockWareHouseID), PaketNo = l.PaketNo, WorkOrderID = l.WorkOrderID });
                    if (l.ID > 0)
                        c.TRN_StockTransLines.Update(l);
                    else
                        c.TRN_StockTransLines.Add(l);
                }
                if (c.SaveContextWithException())
                {
                    c.Database.CommitTransaction();
                    DataLayer.MoveToAnotherList(DataLayer.WaitingSent.tRN_StockTrans, DataLayer.Sent.tRN_StockTrans, t);
                    DataLayer.WaitingSent.SaveJSON();
                    DataLayer.Sent.SaveJSON();
                }
                else
                {
                    c.Database.RollbackTransaction();
                    t.ID = 0;
                    foreach (var l in t.Lines)
                        l.ID = 0;
                }
            }


        }
    }
    public static List<CRD_ItemBarcodes> CRD_ItemBarcodes(int UrunID)
    {
        using (GoldenContext c = new GoldenContext())
        {
            return c.CRD_ItemBarcodes.Where(s => s.UrunID == UrunID).OrderByDescending(s => s.ID).ToList();
        }
    }
    public static void TRN_EtiketBasimInsert(TRN_EtiketBasim t, bool showError = true)
    {
        if (t.ID > 0)
        {
            if (showError)
                appSettings.UyariGoster("Bu işlem sunucuya gönderilmiş. Tekrar gönderemezsiniz.");
            DataLayer.MoveToAnotherList(DataLayer.WaitingSent.TRN_EtiketBasim, DataLayer.Sent.TRN_EtiketBasim, t);
            DataLayer.WaitingSent.SaveJSON();
            DataLayer.Sent.SaveJSON();
            return;
        }
        if (IsOfflineAlert) return;

        if (!DataLayer.IsOfflineAlert)
        {
            try
            {
                using (GoldenContext c = new GoldenContext())
                {
                    c.Database.BeginTransaction();
                    c.TRN_EtiketBasim.Add(t);
                    if (!c.SaveContextWithException()) return;
                    if (t.ID > 0)
                    {
                        foreach (var l in t.TRN_EtiketBasimEmirleri)
                        {
                            l.FisID = t.ID;

                            c.TRN_EtiketBasimEmirleri.Add(l);
                        }
                        if (c.SaveContextWithException())
                        {
                            c.Database.CommitTransaction();
                            DataLayer.MoveToAnotherList(DataLayer.WaitingSent.TRN_EtiketBasim, DataLayer.Sent.TRN_EtiketBasim, t);
                            DataLayer.WaitingSent.SaveJSON();
                            DataLayer.Sent.SaveJSON();
                        }
                        else
                            c.Database.RollbackTransaction();
                    }
                }

            }
            catch (Exception ex)
            {
                Task.Run(() => ex.UyariGoster());
            }
        }
    }


    public static string GetBarkod(string Module, string startmask = "", int digit = 8)
    {
        if (IsOfflineAlert) return "";
        string counter = TurbimSQLHelper.defaultconn.SQLSelect(string.Format("SELECT TOP 1 ISNULL(StartMask,'') + REPLACE(STR([Current], [Digit]), SPACE(1), '0')  FROM X_Counters (NOLOCK)  WHERE ModuleName='{0}'  ORDER BY [Current] DESC", Module));
        if (counter + "" == "")
        {
            TurbimSQLHelper.defaultconn.SQLExecuteNonQuery(string.Format("INSERT INTO X_Counters([ModuleName],[StartMask],[Start],[End],[Current],[Digit]) VALUES('{0}','{1}',0,{2},1,{3})", Module, startmask, "".PadLeft(digit, '9'), digit));
            return "00000001";
        }
        return counter;
    }
    #endregion
    #region TRN_Orders
    public static void TRN_OrdersInsert(TRN_Orders t, bool showError = true)
    {
        if (t.ID > 0)
        {
            if (showError)
                appSettings.UyariGoster("Bu işlem sunucuya gönderilmiş. Tekrar gönderemezsiniz.");
            DataLayer.MoveToAnotherList(DataLayer.WaitingSent.tRN_Orders, DataLayer.Sent.tRN_Orders, t);
            DataLayer.WaitingSent.SaveJSON();
            DataLayer.Sent.SaveJSON();
            return;
        }
        if (IsOfflineAlert) return;
        using (GoldenContext c = new GoldenContext())
        {
            c.Database.BeginTransaction();
            c.TRN_Orders.Add(t);
            if (!c.SaveContextWithException()) return;
            if (t.ID > 0)
            {
                foreach (var l in t.Lines)
                {
                    l.OrderID = t.ID;
                    l.CurrencyID = t.CurrencyID;
                    c.TRN_OrderLines.Add(l);
                }
                if (c.SaveContextWithException())
                {
                    c.Database.CommitTransaction();
                    DataLayer.MoveToAnotherList(DataLayer.WaitingSent.tRN_Orders, DataLayer.Sent.tRN_Orders, t);
                    DataLayer.WaitingSent.SaveJSON();
                    DataLayer.Sent.SaveJSON();
                }
                else
                    c.Database.RollbackTransaction();


            }

        }
    }
    public static List<TRN_OrderLines> TRN_OrderLines(int OrderID)
    {
        using (GoldenContext c = new GoldenContext())
        {
            return c.TRN_OrderLines.Where(s => s.OrderID == OrderID).OrderByDescending(s => s.ID).ToList();
        }

    }
    #endregion
    public static List<V_CariHareketler> V_CariHareketler(int CariID)
    {
        if (IsOfflineAlert) return new List<V_CariHareketler>();
        try
        {
            using (GoldenContext c = new GoldenContext())
            {
                return c.V_CariHareketler.Where(s => s.CariID == CariID).OrderByDescending(s => s.ID).ToList();
            }
        }
        catch (Exception ex)
        {
            Task.Run(() => ex.UyariGoster());
            return new List<V_CariHareketler>();
        }
    }
    public static List<TRN_Files> TRN_Files(int RecordID)
    {
        if (IsOfflineAlert) return new List<TRN_Files>();
        using (GoldenContext c = new GoldenContext())
        {
            return c.TRN_Files.Where(s => s.TableName == "Items" && s.RecordID == RecordID).OrderByDescending(s => s.ID).ToList();
        }
    }
    public static IEnumerable<T> ExecuteObject<T>(string sql)
    {
        List<T> items = new List<T>();
        foreach (var row in db.SQLSelectToDataTable(sql).Rows)
        {
            T item = (T)Activator.CreateInstance(typeof(T), row);
            items.Add(item);
        }
        return items;
    }
    public static List<T> DataTableToObject<T>(DataTable tbl) where T : new()
    {
        List<T> lst = new List<T>();
        foreach (DataRow r in tbl.Rows)
        {
            T item = new T();
            foreach (DataColumn c in r.Table.Columns)
            {

                PropertyInfo prp = item.GetType().GetProperty(c.ColumnName);
                if (prp != null && r[c] != DBNull.Value)
                {
                    if (prp.PropertyType.FullName.Contains("GoldenMobileX"))
                    {
                        /*
                          if (prp.PropertyType.FullName.Contains("GoldenMobileX.Models.X_Currency"))
                           {
                               prp.SetValue(item, DataLayer.X_Currency.Where(x => x.CurrencyNumber == r[c].convInt()).FirstOrDefault(), null);

                           }
                        */

                    }
                    else
                    {
                        try
                        {
                            if (prp.PropertyType == typeof(decimal) || prp.PropertyType == typeof(decimal?))
                                prp.SetValue(item, r[c].convDecimal());
                            else if (prp.PropertyType == typeof(double) || prp.PropertyType == typeof(double?))
                                prp.SetValue(item, r[c].convDouble());
                            else
                                prp.SetValue(item, r[c]);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }

                }

            }
            lst.Add(item);
        }
        return lst;
    }
    public static void MoveToAnotherList<T>(List<T> source, List<T> dest, T o)
    {
        try
        {
            dest.Add(o);
            dest = new List<T>(dest);
            source.Remove(o);
        }
        catch (Exception ex)
        {
            Task.Run(() => appSettings.UyariGoster(ex.Message));
        }
    }

    public static SQLite.SQLiteAsyncConnection LocalDataBase { get; set; }
    public static void SqlLiteInitDataBase()
    {
        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        LocalDataBase = new SQLite.SQLiteAsyncConnection(System.IO.Path.Combine(basePath, appSettings.CurrentFirm + "LocalDataBase.sqlite"));

    }

}

