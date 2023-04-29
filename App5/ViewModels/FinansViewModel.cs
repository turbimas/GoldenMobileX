using GoldenMobileX.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
namespace GoldenMobileX.ViewModels
{

    public class FinansViewModel : BaseViewModel
    {
        public Command AddItemCommand
        {
            get;
        }
        public ObservableCollection<V_CariHareketler> hareketler
        {
            get; set;
        }
        public FinansViewModel()
        {
            AddItemCommand = new Command(OnAddItem);
            this.PropertyChanged += (_, __) => AddItemCommand.ChangeCanExecute();

        }

        private async void OnAddItem()
        {

        }
        public List<X_Types> cariTypes
        {
            get
            {
                return DataLayer.x_types_carihesap;
            }
        }
        public List<X_Types> bankaIslemTypes
        {
            get
            {
                return DataLayer.x_types_BankaIslem;
            }
        }
        public CRD_Cari _item;
        public CRD_Cari item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }
        public ObservableCollection<CRD_Cari> items
        {
            get; set;
        }
        public List<CRD_BankaHesaplari> CRD_BankaHesaplari { get; set; }
        public TRN_BankaHareketleri hareket { get; set; }
        public List<CRD_Bankalar> CRD_Bankalar { get; set; }
    }
}
