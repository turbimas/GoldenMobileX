using GoldenMobileX.Models;
using GoldenMobileX.OfflineData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

class DataLayer
{
    public DataLayer()
    {

    }
    public static void LoadStaticData()
    {
        SqlLiteInitDataBase();
        offLineMod = true;
        CRD = new offLine.CRD();
        Products = new offLine.Products();
        Cari = new offLine.Cari();
        WaitingSent = new offLine.WaitingSent();
        Sent = new offLine.Sent();
        Types = new offLine.Types();
        v_DepodakiLotlar = new List<V_DepodakiLotlar>();
        LoadDataFromJSON();
    }
    public static offLine.Types Types { get; set; }
    public static offLine.CRD CRD { get; set; }
    public static offLine.Products Products { get; set; }
    public static offLine.Cari Cari { get; set; }
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
        get { return CRD.CRD_StockWareHouse; }
        set
        {
            CRD.CRD_StockWareHouse = value;
        }
    }
    public static List<V_DepodakiLotlar> DepolakiLotlar
    {
        get; set;
    }
    public static List<CRD_BankaHesaplari> CRD_BankaHesaplari
    {
        get { return CRD.CRD_BankaHesaplari; }
        set
        {
            CRD.CRD_BankaHesaplari = value;
        }
    }
    public static List<CRD_Bankalar> CRD_Bankalar
    {
        get { return CRD.CRD_Bankalar; }
        set
        {
            CRD.CRD_Bankalar = value;
        }
    }
    public static List<CRD_Kasalar> CRD_Kasalar
    {
        get { return CRD.CRD_Kasalar; }
        set
        {
            CRD.CRD_Kasalar = value;
        }
    }
    public static List<X_Reports> X_Reports
    {
        get; set;
    }

    public static List<x_Settings> X_Settings
    {
        get
        {
            return Types.X_Settings;
        }
        set { Types.X_Settings = value; }
    }

    public static List<X_Currency> X_Currency
    {
        get { return Types.X_Currency; } set { Types.X_Currency = value; }
    }

    public static List<CRD_Cari> Cariler
    {
        get { return Cari.cRD_Cari; } set { Cari.cRD_Cari = value; }
    }

    public static List<V_AllItems> V_AllItems
    {
        get { return Products.v_AllItems; }
        set
        {
            Products.v_AllItems = value;
        }
    }
    public static List<V_DepodakiLotlar> v_DepodakiLotlar
    {
        get; set;
    }
    public static List<X_Types> X_Types { get { return Types.X_Types; } set { Types.X_Types = value; } }
    public static List<L_Units> L_Units { get { return Types.L_Units; } set { Types.L_Units = value; } }
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
    public static List<X_Types> x_types_siparisTuru { get; set; }
    public static List<X_Types> x_types_satisdurum { get; set; }
    public static List<X_Types> x_types_satinalmadurum { get; set; }


    static void LoadTypes()
    {
        x_types_carihesap = X_Types.Where(x => x.TableName == "Cari" && x.ColumnsName == "Turu").ToList();
        x_types_stokkarti = X_Types.Where(x => x.TableName == "TRN_StockTransLines" && x.ColumnsName == "ProductType").ToList();
        x_types_stokfisi = X_Types.Where(x => x.TableName == "TRN_StockTrans" && x.ColumnsName == "Type").ToList();
        x_types_Invoice = X_Types.Where(x => x.TableName == "TRN_Invoice" && x.ColumnsName == "Type").ToList();
        x_types_siparisTuru = X_Types.Where(x => x.TableName == "Orders" && x.ColumnsName == "Type").ToList();
        x_types_satisdurum = X_Types.Where(x => x.TableName == "Orders" && x.ColumnsName == "Status").ToList();
        x_types_satinalmadurum = X_Types.Where(x => x.TableName == "SatinAlma" && x.ColumnsName == "Durum").ToList();
    }
    static async void LoadDataFromJSON()
    {

        LocalDataBase.CreateTableAsync<X_Currency>().Wait();
        LocalDataBase.CreateTableAsync<L_Units>().Wait();
        LocalDataBase.CreateTableAsync<X_Types>().Wait();
        LocalDataBase.CreateTableAsync<x_Settings>().Wait();

        LocalDataBase.CreateTableAsync<V_DepodakiLotlar>().Wait();
        LocalDataBase.CreateTableAsync<CRD_ItemBarcodes>().Wait();
        //LocalDataBase.CreateTableAsync<V_AllItems>().Wait();
        //LocalDataBase.CreateTableAsync<CRD_Cari>().Wait();

        //LocalDataBase.CreateTableAsync<TRN_StockTrans>().Wait();
        //ProductDataBase.CreateTableAsync<TRN_StockTransLines>().Wait();


        DataLayer.Types = TurbimJSON.Read<offLine.Types>(new offLine.Types());  //0
        LoadTypes();
        DataLayer.CRD = TurbimJSON.Read<offLine.CRD>(new offLine.CRD());  //1
       
        DataLayer.Products = TurbimJSON.Read<offLine.Products>(new offLine.Products());  //2
        DataLayer.Cari = TurbimJSON.Read<offLine.Cari>(new offLine.Cari());  //3 
        DataLayer.WaitingSent = TurbimJSON.Read<offLine.WaitingSent>(new offLine.WaitingSent());  //4
        if (DataLayer.Products.v_AllItems.Count == 0) LoadDataFromSQL(true);
    }
    public static void LoadDataFromSQL(bool FromServer = false)
    {

        if (IsOnline)
        {
            db.connTimeOut = maxConnTimeOut;



            GoldenContext c = new GoldenContext();

            DataLayer.Depolar = c.CRD_StockWareHouse.Select(s => s).ToList().Where(r => r.AuthCode + "" == "" || appSettings.UserAuthCode.Split(',').Any(x => x == "*") || (r.AuthCode + "").Split(',').Any(x => (appSettings.UserAuthCode + "").Split(',').Contains(x))).Select(s => s).ToList();


            if (DataLayer.Depolar.Count == 0 || FromServer)
                DataLayer.Depolar = c.CRD_StockWareHouse.Select(s => s).ToList().Where(r => r.AuthCode + "" == "" || appSettings.UserAuthCode.Split(',').Any(x => x == "*") || (r.AuthCode + "").Split(',').Any(x => (appSettings.UserAuthCode + "").Split(',').Contains(x))).Select(s => s).ToList();



            if (X_Types.Count == 0 || FromServer)
                DataLayer.X_Types = c.X_Types.Select(s => s).ToList(); ;
            LoadTypes();
            if (X_Settings.Count == 0 || FromServer)
                DataLayer.X_Settings = c.x_Settings.Select(s => s).ToList();
            if (L_Units.Count == 0 || FromServer)
                DataLayer.L_Units = c.L_Units.Select(s => s).ToList();
            if (X_Currency.Count == 0 || FromServer)
                DataLayer.X_Currency = c.X_Currency.Select(s => s).ToList();

            if (Cariler.Count == 0)
                DataLayer.Cariler = c.CRD_Cari.Where(s => s.Active == true).ToList().Select(s => s).ToList();
            else if (FromServer)
            {
                var maxModifiedDate = (DataLayer.Cariler.Max(s => s.ModifiedDate)).convDateTime();
                DataLayer.Cariler = c.CRD_Cari.Where(s => s.Active == true && s.ModifiedDate > maxModifiedDate).Select(s => s).ToList();
            }
            if (V_AllItems.Count == 0)
            {
                DataLayer.V_AllItems = c.V_AllItems.Where(s => s.Active == true).Select(s => s).ToList();
            }
            else if (FromServer)
            {
                var maxModifiedDate = (DataLayer.V_AllItems.Max(s => s.ModifiedDate)).convDateTime();
                List<V_AllItems> items = c.V_AllItems.Where(s => s.Active == true && s.ModifiedDate > maxModifiedDate).Select(s => s).ToList();
                DataLayer.V_AllItems.AddRange(items.Where(s => DataLayer.V_AllItems.Where(t => t.ID == s.ID && t.Barcode == s.Barcode).Count() == 0));
            }

            if (DataLayer.v_DepodakiLotlar.Count == 0)
            {
                LocalDataBase.ExecuteAsync("DELETE FROM V_DepodakiLotlar");
                List<V_DepodakiLotlar> items = c.V_DepodakiLotlar.Select(s => s).ToList();
                LocalDataBase.InsertAllAsync(DataLayer.v_DepodakiLotlar);
            }
            else if (FromServer)
            {
                var maxModifiedDate = (DataLayer.v_DepodakiLotlar.Max(s => s.ModifiedDate)).convDateTime();
                List<V_DepodakiLotlar> items = c.V_DepodakiLotlar.Where(s => s.ModifiedDate > maxModifiedDate).Select(s => s).ToList();
                LocalDataBase.InsertAllAsync(items.Where(s => DataLayer.v_DepodakiLotlar.Where(t => t.HareketID == s.HareketID).Count() == 0));
                DataLayer.v_DepodakiLotlar.AddRange(items.Where(s => DataLayer.v_DepodakiLotlar.Where(t => t.HareketID == s.HareketID).Count() == 0));
            }
            if (CRD_BankaHesaplari.Count == 0 || FromServer)
                DataLayer.CRD_BankaHesaplari = c.CRD_BankaHesaplari.Select(s => s).ToList();
            if (CRD_Bankalar.Count == 0 || FromServer)
                DataLayer.CRD_Bankalar = c.CRD_Bankalar.Select(s => s).ToList();

            DataLayer.CRD.SaveJSON();
            DataLayer.Products.SaveJSON();
            DataLayer.Cari.SaveJSON();
            DataLayer.Types.SaveJSON();

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
            List<TRN_StockTrans> trn_StockTrans = c.TRN_StockTrans.Where(s => s.Status == 4 && s.Type == 2 && s.DestStockWareHouseID == appSettings.User.WareHouseID).Select(s => s).OrderByDescending(s => s.ID).ToList();

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
    public static List<TRN_StockTransLines> TRN_StockTransLines(int TransID)
    {
        using (GoldenContext c = new GoldenContext())
        {
            return c.TRN_StockTransLines.Where(s => s.StockTransID == TransID).OrderByDescending(s => s.ID).ToList();
        }
    }
    public static List<TRN_StockTransLines> TRN_StockTransLinesByProductID(int ProductID)
    {
        using (GoldenContext c = new GoldenContext())
        {
            return c.TRN_StockTransLines.Where(s => s.ProductID == ProductID).OrderByDescending(s => s.ID).ToList();
        }
    }
    public static void TRN_StockTransInsert(TRN_StockTrans t)
    {
        if (t.ID > 0)
        {
            appSettings.UyariGoster("Bu işlem sunucuya gönderilmiş. Tekrar gönderemezsiniz.");
            return;
        }
        if (IsOfflineAlert) return;
        using (GoldenContext c = new GoldenContext())
        {
            c.Database.BeginTransaction();

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
                    l.Status = t.Status_.Code;
                    if (DataLayer.DepolakiLotlar?.Count() > 0)
                        DataLayer.DepolakiLotlar.Add(new V_DepodakiLotlar() { ProductID = l.ProductID, SeriLot = l.SeriLot, SeriNo = l.SeriNo, LotID = l.LotID, BalyaNo = l.BalyaNo, Miktar = (l.Amount * l.Direction).convDouble(), Direction = l.Direction, Date = l.Date, Depo = (l.Direction == 1 ? l.DestStockWareHouseID : l.StockWareHouseID), PaketNo = l.PaketNo, WorkOrderID = l.WorkOrderID });
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
    public static void TRN_EtiketBasimInsert(TRN_EtiketBasim t)
    {
        if (t.ID > 0)
        {
            appSettings.UyariGoster("Bu işlem sunucuya gönderilmiş. Tekrar gönderemezsiniz.");
            return;
        }
        if (IsOfflineAlert) return;

        if (!DataLayer.IsOfflineAlert)
        {
            using (GoldenContext c = new GoldenContext())
            {
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
    public static void TRN_OrdersInsert(TRN_Orders t)
    {
        if (t.ID > 0)
        {
            appSettings.UyariGoster("Bu işlem sunucuya gönderilmiş. Tekrar gönderemezsiniz.");
            return;
        }
        if (IsOfflineAlert) return;
        using (GoldenContext c = new GoldenContext())
        {
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
        using (GoldenContext c = new GoldenContext())
        {
            return c.V_CariHareketler.Where(s => s.CariID == CariID).OrderByDescending(s => s.ID).ToList();
        }
 
    }
    public static List<TRN_Files> TRN_Files(int RecordID)
    {
        if (IsOfflineAlert) return new List<TRN_Files>();
        using (GoldenContext c = new GoldenContext())
        {
            return c.TRN_Files.Where(s => s.TableName == "Items" && s.RecordID==RecordID).OrderByDescending(s => s.ID).ToList();
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
        dest.Add(o);
        dest = new List<T>(dest);
        source.Remove(o);
    }

    public  static SQLite.SQLiteAsyncConnection LocalDataBase { get; set; }
    public static void SqlLiteInitDataBase()
    {
        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        LocalDataBase = new SQLite.SQLiteAsyncConnection(System.IO.Path.Combine(basePath, appSettings.CurrentFirm  + "LocalDataBase.sqlite"));

    }

}

