using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;
using GoldenMobileX.Models;
using System.Linq;
using GoldenMobileX;

public static class db
{
    public static string SQLSelect(string sql)
    {
        return TurbimSQLHelper.defaultconn.SQLSelect(sql);
    }
    public static string SQLExecuteWithParameter(string sql, SqlParameter[] parameters)
    {
        return TurbimSQLHelper.defaultconn.SQLExecuteWithParameter(sql, parameters);
    }
    public static string SQLExecuteWithParameter(string sql, string[] parameters, object[] values, System.Data.SqlDbType[] types)
    {
        return TurbimSQLHelper.defaultconn.SQLExecuteWithParameter(sql, parameters, values, types);
    }
    public static DataTable SQLSelectToDataTable(string sql)
    {
        return TurbimSQLHelper.defaultconn.SQLSelectToDataTable(sql, TurbimTools.getinnertext(sql.Replace("  ", " "), " FROM ", " ", true).Replace(" ", ""));
 
    }
    public static int connTimeOut
    {
        get { return TurbimSQLHelper.defaultconn.connTimeOut; }
        set { TurbimSQLHelper.defaultconn.connTimeOut = value; }
    }
    public static DataRow SQLSelectDataRow(string sql)
    {
        return TurbimSQLHelper.defaultconn.SQLSelectDataRow(sql);
    }
    public static string SQLExecuteNonQuery(string sql)
    {
        return TurbimSQLHelper.defaultconn.SQLExecuteNonQuery(sql);
    }
    public static int UpdateOrInsert<T>(T obj)
    {
        string sql = "";
        int ID = 0;
        List<SqlParameter> param = new List<SqlParameter>();
        List<string> ColumnsName = new List<string>();

        List<string> update = new List<string>();

        string TableName = obj.GetType().Name + "";

        foreach (System.Reflection.PropertyInfo prp in obj.GetType().GetProperties())
        {
            if (prp.Name != "ID")
            {
                if (prp.PropertyType.FullName.Contains("GoldenMobileX") && prp.PropertyType.FullName.Contains("Collection")) continue;
                if (prp.Name.EndsWith("_")) continue;
                if (prp.PropertyType.FullName.Contains("GoldenMobileX"))
                {
                    /*if (prp.PropertyType.FullName.Contains("GoldenMobileX.Models.X_Currency"))
                    {
                        param.Add(new SqlParameter("@" + prp.Name, (((X_Currency)prp.GetValue(obj))?.CurrencyNumber).convInt()));
                    }*/
                }
                else
                {
                    
                    SqlParameter pp = new SqlParameter();
                   
                    pp.ParameterName = "@" + prp.Name;
                    pp.SqlDbType = ConverSqlDbType(prp.PropertyType);
                    object val = prp.GetValue(obj) ?? null;
                    try
                    {
                        if (prp.PropertyType == typeof(Int32) || prp.PropertyType == typeof(int) || prp.PropertyType == typeof(Nullable<int>))
                            pp.Value = val ?? DBNull.Value;
                        else if (prp.PropertyType == typeof(DateTime))
                            pp.Value = (val.convDateTime() < DateTime.Now.AddYears(-200)) ? DateTime.Now : val;
                        else
                            pp.Value = val == null ? DBNull.Value : val;
                    }
                    catch(Exception ex)
                    {
                        throw (ex);
                    }
                    param.Add(pp);
                }
                update.Add(prp.Name + "=@" + prp.Name + "");
                ColumnsName.Add(prp.Name);
            }
            else
                ID = prp.GetValue(obj).convInt();
        }


        if (ID > 0)
        {
            sql = string.Format("UPDATE {0} SET {1} WHERE ID={2}; SELECT @ID={2}", TableName, string.Join(",", update), ID);
        }
        else
        {
            sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2}); SELECT @ID=scope_identity();", TableName, string.Join(",", ColumnsName), string.Join(",@", ColumnsName));
        }
        SqlParameter p = new SqlParameter("@ID", SqlDbType.Int, 4);
        p.Direction = ParameterDirection.Output;
        param.Add(p);
        
        db.SQLExecuteWithParameter(sql, param.ToArray());
        return p.Value.convInt();
    }
    public static SqlCommand UpdateOrInsertCommand<T>(T obj)
    {
        SqlCommand cmd = new SqlCommand();
        string sql = "";
        int ID = 0;
 
        List<string> ColumnsName = new List<string>();

        List<string> update = new List<string>();

        string TableName = obj.GetType().Name + "";

        foreach (System.Reflection.PropertyInfo prp in obj.GetType().GetProperties())
        {
            if (prp.Name != "ID")
            {
                if (prp.PropertyType.FullName.Contains("GoldenMobileX") && prp.PropertyType.FullName.Contains("Collection")) continue;
                if (prp.PropertyType.FullName.Contains("GoldenMobileX"))
                {
                   /*if (prp.PropertyType.FullName.Contains("GoldenMobileX.Models.X_Currency"))
                    {
                        cmd.Parameters.Add(new SqlParameter("@" + prp.Name, ((X_Currency)prp.GetValue(obj)).CurrencyNumber.convInt()));
                    }
                   */

                }
                else
                {
                    SqlParameter pp = new SqlParameter();
                    pp.ParameterName = "@" + prp.Name;
            


                    if (prp.PropertyType == typeof(Int32))
                        pp.Value = prp.GetValue(obj).convInt();
                    else if (prp.PropertyType == typeof(DateTime))
                        pp.Value = (prp.GetValue(obj).convDateTime() < DateTime.Now.AddYears(-200)) ? DateTime.Now : prp.GetValue(obj);
                    else
                        pp.Value = prp.GetValue(obj) == null ? DBNull.Value : prp.GetValue(obj);
                    pp.Value = prp.GetValue(obj);
                    pp.SqlDbType = ConverSqlDbType(prp.PropertyType);
                    cmd.Parameters.Add(pp);
                }
                update.Add(prp.Name + "=@" + prp.Name + "");
                ColumnsName.Add(prp.Name);
            }
            else
                ID = prp.GetValue(obj).convInt();
        }


        if (ID > 0)
        {
            sql = string.Format("UPDATE {0} SET {1} WHERE ID={2}; SELECT @ID={2}", TableName, string.Join(",", update), ID);
        }
        else
        {
            sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2}); SELECT @ID=scope_identity();", TableName, string.Join(",", ColumnsName), string.Join(",@", ColumnsName));
        }
        SqlParameter p = new SqlParameter("@ID", SqlDbType.Int, 4);
        p.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(p);
  
        cmd.CommandText = sql;
     
        return cmd;
    }
    public static SqlDbType ConverSqlDbType(Type giveType)
    {
        var typeMap = new Dictionary<Type, SqlDbType>();

        typeMap[typeof(string)] = SqlDbType.NVarChar;
        typeMap[typeof(char[])] = SqlDbType.NVarChar;
        typeMap[typeof(int)] = SqlDbType.Int;
        typeMap[typeof(int?)] = SqlDbType.Int;
        typeMap[typeof(Int32)] = SqlDbType.Int;
        typeMap[typeof(Int32?)] = SqlDbType.Int;
        typeMap[typeof(Int16)] = SqlDbType.SmallInt;
        typeMap[typeof(Int16?)] = SqlDbType.SmallInt;
        typeMap[typeof(Int64)] = SqlDbType.BigInt;
        typeMap[typeof(Int64?)] = SqlDbType.BigInt;
        typeMap[typeof(Byte[])] = SqlDbType.VarBinary;
        typeMap[typeof(Image)] = SqlDbType.VarBinary;
        typeMap[typeof(Boolean)] = SqlDbType.Bit;
        typeMap[typeof(Boolean?)] = SqlDbType.Bit;
        typeMap[typeof(DateTime)] = SqlDbType.DateTime; //SqlDbType.DateTime2;
        typeMap[typeof(DateTime?)] = SqlDbType.DateTime; //SqlDbType.DateTime2;
        typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
        typeMap[typeof(Decimal)] = SqlDbType.Decimal;
        typeMap[typeof(Double)] = SqlDbType.Float;
        typeMap[typeof(Nullable<double>)] = SqlDbType.Float;
        typeMap[typeof(Nullable<decimal>)] = SqlDbType.Money;
        //typeMap[typeof(Decimal)] = SqlDbType.Money;
        typeMap[typeof(Byte)] = SqlDbType.TinyInt;
        typeMap[typeof(TimeSpan)] = SqlDbType.Time;
        typeMap[typeof(Guid)] = SqlDbType.UniqueIdentifier;
        return typeMap[(giveType)];
    }
 
 
}

/// <summary>
/// Version 1.4.3.1
/// 
/// 1.4.2.1 Select List added, Select DataRow Added
/// 1.4.3.1 SelecToDaataTable Added
/// LObject DLL için yazıldı
/// </summary>
public class TurbimSQLHelper
{

    SqlConnection conn = new SqlConnection();


    public static string server { get; set; }
    public static string database { get; set; }
    public static string User  { get; set; }
    public static string Pass    { get; set; }
    public static SqlConnectionStringBuilder connBuilder { get; set; }
    public static TurbimSQLHelper defaultconn { get; set; }
    public int connTimeOut
    {
        get { return connBuilder.ConnectTimeout; }
        set { connBuilder.ConnectTimeout = value;
            if (conn.State == ConnectionState.Open)
                this.Close();
            conn.ConnectionString = connBuilder.ToString();
            this.isOpen();
        }
    }

    public TurbimSQLHelper(string connectionString)
    {
        if (connectionString + "" == "") return;
        connBuilder = new SqlConnectionStringBuilder(connectionString);
        conn.ConnectionString = connBuilder.ToString();
        if (conn.State == ConnectionState.Closed)
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            { 
                SqlHatasi("Cannot Connect Database", ex, false);
            }
    }

    public bool isOpen()
    {
        try
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }
        catch { return false; }
        return (conn.State == ConnectionState.Open);
    }
    public void Close()
    {
        if (conn.State == ConnectionState.Open)
        {
            conn.Close();

        }
    }

    public async void SqlHatasi(string sql, Exception ex, bool saveQuery= false)
    {
        return;
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        if (await appSettings.Onay("HATA : Sunucuda işlem yapılamıyor. Bu işlem sunucuya bağlanıldığında otomatik olarak yapılacaktır. Hata Detayını görmek istermisiniz?"))
        {
            appSettings.UyariGoster(ex.Message + " " + sql + " " + stackTrace.GetFrame(2).GetMethod().ReflectedType.FullName + "." + stackTrace.GetFrame(2).GetMethod().Name + " - Line: " + stackTrace.GetFrame(2).GetFileLineNumber() + "<br>" + stackTrace.GetFrame(1).GetMethod().ReflectedType.FullName + "." + stackTrace.GetFrame(1).GetMethod().Name + " - Line: " + stackTrace.GetFrame(2).GetFileLineNumber());
        }


        if (saveQuery)
            if (sql.ToUpper().StartsWith("INSERT") || sql.ToUpper().StartsWith("UPDATE"))
            {
                appSettings.OfflineData.SQLListToRun.Add(sql);
                appSettings.OfflineData.SaveXML();
            }
    }

    #region SQLSelect

    /// <summary>
    /// This function selects only one string value from database
    /// </summary>
    /// <remarks>e.g: "SELECT [ID] FROM [TABLE]" returns first row ID of TABLE</remarks>
    /// <param name="sql">String: Sql query</param>
    /// <returns>Returns only one column value of database as a string.</returns>
    public string SQLSelect(string sql, int commandTimeOut = 3)
    {
        string _return = null;
        try
        {

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandTimeout = commandTimeOut;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        _return = dr[0].ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SqlHatasi(sql, ex);
            _return = "ERROR: " + sql + " - " + ex;
        }
        return _return;

    }
    public string SQLSelectWithSeperator(string sql, string Seperator = ",")
    {
        DataTable _return = new DataTable();
        List<string> str = new List<string>();
        try
        {

            using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                da.Fill(_return);

                foreach (DataRow dr in _return.Rows)
                {
                    str.Add(dr[0] + "");
                }
            }

        }
        catch (Exception ex)
        {
            SqlHatasi(sql, ex);
            return "ERROR: " + sql + " - " + ex.Message;
        }
        return string.Join(Seperator, str);

    }
    /// <summary>
    /// This function selects a dataset from database
    /// </summary>
    /// <remarks>e.g: "SELECT * FROM [TABLE]" returns DataTable</remarks>
    /// <param name="sql">String: Sql query</param>
    /// <param name="datasetname">Name of dataset whcih returns</param>
    /// <returns>Returns a dataset.</returns>
    public DataTable SQLSelectToDataTable(string sql, string tablename = "dt")
    {
        using (DataTable _return = new DataTable(tablename))
        {
            try
            {

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    da.Fill(_return);
                }


            }
            catch (Exception ex)
            {
                SqlHatasi(sql, ex);
                return null;
            }
            return _return;
        }
    }

    public DataRow SQLSelectDataRow(string sql)
    {
        DataTable dt = new DataTable();
        try
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
   
            da.Fill(dt);
            da.Dispose();
        }
        catch (Exception ex)
        {
            SqlHatasi(sql, ex);
        }
        if (dt.Rows.Count > 0)
            return dt.Rows[0];
        else
            return null;
    }

    #endregion SQLSelect
    #region Execute
    public string SQLExecuteNonQuery(string sql, int commandTimeOut=3)
    {
        string _return = "";
        try
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
          
                cmd.CommandTimeout = commandTimeOut;
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            SqlHatasi(sql, ex, true);
            _return = ex.ToString();

        }
        return _return;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters">("@param", value)</param>
    /// <returns></returns>
    public string SQLExecuteWithParameter(string sql, SqlParameter[] parameters)
    {

        string _return = "";
        try
        {

            SqlCommand cmd = new SqlCommand(sql, conn);
             
            int i = 0;
            foreach (SqlParameter param in parameters)
            {
                cmd.Parameters.Add(param);
                i++;
            }


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            cmd.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            SqlHatasi(sql, ex);
            _return = ex.ToString();

        }
        return _return;
    }

    public string SQLExecuteWithTransaction(string transactionName, List<SqlCommand> commands)
    {
        SqlTransaction transaction;
        transaction = conn.BeginTransaction(transactionName);
        string _return = "";


            if (conn.State == ConnectionState.Closed)
                conn.Open();
        try
        {
            foreach (SqlCommand cmd in commands)
            {
                cmd.Connection = conn;
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
  
            _return = ex.ToString();
            transaction.Rollback();
        }
        finally
        {
            //transaction.Commit();
        }
        return _return;
    }
    public string SQLExecuteWithParameter(string sql, string[] parameters, object[] values, System.Data.SqlDbType[] types)
    {

        string _return = "";
        try
        {

            SqlCommand cmd = new SqlCommand(sql, conn);
            int i = 0;
            foreach (string param in parameters)
            {
                cmd.Parameters.Add(param, types[i]).Value = values[i];
                i++;
            }


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            cmd.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            SqlHatasi(sql, ex);
            _return = ex.ToString();
        }
        return _return;
    }
    public string SQLExecuteNonQuery(SqlCommand cmd, int commandTimeOut = 3)
    {
        string _return = "";
        try
        {
            cmd.Connection = conn;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            cmd.CommandTimeout = commandTimeOut;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            SqlHatasi(cmd.CommandText, ex, true);
            _return = ex.ToString();
        }
        return _return;
    }
 
    #endregion

    // Getting Query for Table Creation
    public static string CreateTableFromDataTable(DataTable table)
    {
        string sqlsc = "CREATE TABLE " + table.TableName + "(";
        for (int i = 0; i < table.Columns.Count; i++)
        {
            sqlsc += "[" + table.Columns[i].ColumnName + "]";
            string columnType = table.Columns[i].DataType.ToString();
            switch (columnType)
            {
                case "System.Int32":
                    sqlsc += " int ";
                    break;
                case "System.Int64":
                    sqlsc += " bigint ";
                    break;
                case "System.Int16":
                    sqlsc += " smallint";
                    break;
                case "System.Byte":
                    sqlsc += " tinyint";
                    break;
                case "System.Decimal":
                    sqlsc += " decimal ";
                    break;
                case "System.DateTime":
                    sqlsc += " datetime ";
                    break;
                case "System.String":
                default:
                    sqlsc += string.Format(" nvarchar({0}) ",
                    table.Columns[i].MaxLength == -1 ? "max" :
                    table.Columns[i].MaxLength.ToString());
                    break;
            }
            if (table.Columns[i].AutoIncrement)
                sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() +
                "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
            if (!table.Columns[i].AllowDBNull)
                sqlsc += " NOT NULL ";
            sqlsc += ",";


        }
        return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
    }
}



