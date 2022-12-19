﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoldenMobileX.Models
{
    class GoldenContext : DbContext
    {
        public GoldenContext()
        {

            this.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(TurbimSQLHelper.connBuilder.ToString());
            sqlConnection.Open();
            optionsBuilder.UseSqlServer(sqlConnection);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<V_AllItems>().HasNoKey();
            modelBuilder.Entity<X_Currency>().HasNoKey();
            // modelBuilder.Entity<TRN_StockTrans>().HasMany(s => s.Lines);
        }
        public virtual DbSet<X_Currency> X_Currency { get; set; }
        public virtual DbSet<X_Types> X_Types { get; set; }
        public virtual DbSet<x_Settings> x_Settings { get; set; }
        public virtual DbSet<L_Units> L_Units { get; set; }
        public virtual DbSet<CRD_StockWareHouse> CRD_StockWareHouse { get; set; }
        public virtual DbSet<CRD_ItemBarcodes> CRD_ItemBarcodes { get; set; }
        public virtual DbSet<CRD_Items> CRD_Items { get; set; }
        public virtual DbSet<TRN_Files> TRN_Files { get; set; }
        public virtual DbSet<V_DepodakiLotlar> V_DepodakiLotlar { get; set; }

        public virtual DbSet<CRD_Cari> CRD_Cari { get; set; }
        public virtual DbSet<V_CariHareketler> V_CariHareketler { get; set; }
        public virtual DbSet<V_AllItems> V_AllItems { get; set; }

        public virtual DbSet<CRD_BankaHesaplari> CRD_BankaHesaplari { get; set; }
        public virtual DbSet<CRD_Bankalar> CRD_Bankalar { get; set; }
        public virtual DbSet<TRN_StockTrans> TRN_StockTrans { get; set; }
        public virtual DbSet<TRN_StockTransLines> TRN_StockTransLines { get; set; }
        public virtual DbSet<TRN_Orders> TRN_Orders { get; set; }
        public virtual DbSet<TRN_OrderLines> TRN_OrderLines { get; set; }
        public virtual DbSet<TRN_EtiketBasim> TRN_EtiketBasim { get; set; }
        public virtual DbSet<TRN_EtiketBasimEmirleri> TRN_EtiketBasimEmirleri { get; set; }
        public virtual DbSet<X_Users> X_Users { get; set; }
    }
    public static class contextExtensions
    {
        public static  bool SaveContextWithException(this DbContext o)
        {
            try
            {
                o.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster(ex.InnerException.Message);

                return false;
            }
        }
    }
}