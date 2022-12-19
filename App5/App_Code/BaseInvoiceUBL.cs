using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using UblInvoiceObject;
using System.Linq;

/// <summary>
/// Copyright © 2018 Foriba Teknoloji
/// Bu proje örnek bir web servis test projesidir. Yalnızca test sisteminde çalışmaktadır.
/// Version Info    : Version.txt
/// Readme          : Readme.txt
/// </summary>

namespace Turbim.OE.UBL.UBLCreate
{
    public abstract class BaseInvoiceUBL
    {
        string KDVIstisnaKodu = "";
        string KDVIstisnaSebebi = "";
        DataRow FaturaDR;
        string _documentCurrencyCode = "";
        public InvoiceType BaseUBL { get; protected set; }
        public List<DocumentReferenceType> DocRefList { get; set; }
        protected BaseInvoiceUBL(string profileId, string documentCurrencyCode, string note, DateTime date, Guid guid)
        {
            _documentCurrencyCode = documentCurrencyCode;
            DocRefList = new List<DocumentReferenceType>();
            FaturaDR = TurbimSQLHelper.defaultconn.SQLSelectDataRow(string.Format("SELECT * FROM TRN_Invoice WHERE  [Guid]='{0}'", guid));

            int FaturaTuru = FaturaDR["Type"].convInt();
            BaseUBL = new InvoiceType { IssueDate = new IssueDateType { Value = date } };
            string invoiceTypeCode = "SATIS";
            if (FaturaTuru == 3 )
            {
                invoiceTypeCode = "IADE";
                if (profileId != "EARSIVFATURA")
                    profileId = appParams.Fatura.IadeFaturaTuru;
            }

            DataRow KDVIstisna = TurbimSQLHelper.defaultconn.SQLSelectDataRow(string.Format("SELECT Code, Name FROM X_Types WHERE TableName='KDV' AND ColumnsName='TamIstisna' AND Code='{0}'", FaturaDR["KDVIstisnaKodu"]));

            if (FaturaTuru == 8)
            {
                profileId = "IHRACAT";
            }
            if (KDVIstisna != null)
            {
                KDVIstisnaKodu = KDVIstisna["Code"] + "";
                KDVIstisnaSebebi = KDVIstisna["Name"] + "";
                invoiceTypeCode = "ISTISNA";
                note += @"
" + KDVIstisnaKodu + " " + KDVIstisnaSebebi;
            }


            var accountingSupplierParty = new SupplierPartyType //göndericinin fatura üzerindeki bilgileri
            {
                Party = FirmaBilgisi()
            };
            if (appSettings.GetSetting("IBAN6", "TR", "", "IBAN").Length > 10)
            {
                List<PaymentMeansType> paymentMeansTypes = new List<PaymentMeansType>();
                foreach (DataRow dr in TurbimSQLHelper.defaultconn.SQLSelectToDataTable("SELECT TOP 1 SettingDesc, SettingValue FROM x_Settings WHERE SettingCategory='IBAN' AND LEN(SettingValue)>20").Rows)
                {
                    paymentMeansTypes.Add(new PaymentMeansType
                    {
                        PayeeFinancialAccount = new FinancialAccountType
                        {
                            CurrencyCode = new CurrencyCodeType { Value = documentCurrencyCode },
                            ID = new IDType { Value = appSettings.GetSetting("IBAN1", "TR", "", "IBAN") }
                        },
                        PaymentMeansCode = new PaymentMeansCodeType { Value = "1" },
                        PaymentDueDate = new PaymentDueDateType { Value = DateTime.Now }
                    });
                }
                BaseUBL.PaymentMeans = paymentMeansTypes.ToArray();
            }

            var accountingCustomerParty = new CustomerPartyType //Alıcının fatura üzerindeki bilgileri
            {
                Party = MusteriBilgisi(FaturaDR["CariID"].convInt())
            };
            if (FaturaTuru == 8)
            {
                accountingCustomerParty = new CustomerPartyType //Alıcının fatura üzerindeki bilgileri
                {
                    Party = GIB()
                };
                var buyerCustomerParty = new CustomerPartyType //Alıcının fatura üzerindeki bilgileri
                {
                    Party = MusteriBilgisi(FaturaDR["CariID"].convInt())
                };
                buyerCustomerParty.Party.PartyIdentification = new[]
                {
                   new PartyIdentificationType { ID = new IDType { schemeID = "PARTYTYPE", Value = "EXPORT" } }
                };
                buyerCustomerParty.Party.PartyLegalEntity = new PartyLegalEntityType[] {
                    new PartyLegalEntityType {  RegistrationName= new RegistrationNameType{
                    Value = buyerCustomerParty.Party.PartyName.Name.Value  } }};

                BaseUBL.BuyerCustomerParty = buyerCustomerParty;
            }
            else if (TurbimExtensions.x_types_invoice.Where(x => x.Code == FaturaTuru).First().Direction == 1)
            {
                accountingSupplierParty = new SupplierPartyType //göndericinin fatura üzerindeki bilgileri
                {
                    Party = MusteriBilgisi(FaturaDR["CariID"].convInt())
                };
                accountingCustomerParty = new CustomerPartyType //Alıcının fatura üzerindeki bilgileri
                {
                    Party = FirmaBilgisi()
                };
            }



            BaseUBL.AccountingSupplierParty = accountingSupplierParty;
            BaseUBL.AccountingCustomerParty = accountingCustomerParty;

            if (FaturaDR["TevkifatKodu"] + "" != "" && FaturaDR["TevkifatKodu"] + "" != "0")
            {
                DataRow tevkifatdr = db.SQLSelectDataRow("SELECT Name, Code, Code2 FROM X_Types WHERE TableName='Tevkifat' AND Code=" + FaturaDR["TevkifatKodu"].convInt());
                note += @"
" + tevkifatdr.GetValue("Code") + " " + tevkifatdr.GetValue("Name");
                invoiceTypeCode = "TEVKIFAT";
                BaseUBL.WithholdingTaxTotal = new TaxTotalType[]
                {

                new TaxTotalType()
                {
                    TaxAmount = new TaxAmountType
                    {
                        currencyID = documentCurrencyCode,
                        Value = (FaturaDR["TotalTax"].convDecimal(2)*((new  DataTable()).Compute(tevkifatdr.GetValue("Code2")+"", "").convDouble()).convDecimal(2))

                    },
                    TaxSubtotal = new[]
                    {
                        new TaxSubtotalType
                        {
                            TaxableAmount = new TaxableAmountType
                            {
                                currencyID = documentCurrencyCode,
                                Value =FaturaDR["NetTotal"].convDecimal(2)
                            },

                            TaxAmount = new TaxAmountType
                            {
                                currencyID = documentCurrencyCode,
                                Value =(FaturaDR["TotalTax"].convDecimal(2)*((new  DataTable()).Compute(tevkifatdr.GetValue("Code2")+"", "").convDecimal(2)).convDecimal(2))
                            },
                            CalculationSequenceNumeric=new CalculationSequenceNumericType
                            {
                                Value=1
                            },

                            Percent = new PercentType1 { Value =Convert.ToDecimal(((new  DataTable()).Compute(tevkifatdr.GetValue("Code2")+"", "").convDouble()*100).convInt())},

                            TaxCategory = new TaxCategoryType
                            {
                                TaxScheme = new TaxSchemeType
                                {
                                    Name = new NameType1 { Value = tevkifatdr.GetValue("Name") + "" },
                                    TaxTypeCode = new TaxTypeCodeType { Value =  tevkifatdr.GetValue("Code")+"" }
                                }
                            }

                        }

                    }
                }
                };
            }


            var doc = new XmlDocument();
            doc.LoadXml("<xml />");
            XmlElement element = doc.DocumentElement;

            BaseUBL.UBLExtensions = new[]
                 {
                    new UBLExtensionType
                    {
                        ExtensionContent = element

                    }
                };

            BaseUBL.UBLVersionID = new UBLVersionIDType { Value = "2.1" }; //uluslararası fatura standardı 2.1
            BaseUBL.CustomizationID = new CustomizationIDType { Value = "TR1.2" }; //fakat GİB UBLTR olarak isimlendirdiği Türkiye'ye özgü 1.2 efatura formatını kullanıyor.
            BaseUBL.InvoiceTypeCode = new InvoiceTypeCodeType { Value = invoiceTypeCode };
            BaseUBL.DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = documentCurrencyCode };
            BaseUBL.ProfileID = new ProfileIDType { Value = profileId };
            BaseUBL.CopyIndicator = new CopyIndicatorType { Value = false };
            BaseUBL.UUID = new UUIDType { Value = guid.ToString() };
            BaseUBL.Note = new NoteType[] { new NoteType { Value = note } };

        }

        /// <summary>
        ///  Fatura ID otomatik oluşacak ise bu alanı göndermelisiniz.
        /// </summary>
        public void SetCustInvIdDocumentReference(string FNo = "")
        {

            var idRef = new DocumentReferenceType()
            {
                ID = new IDType { Value = FNo },

                IssueDate = BaseUBL.IssueDate,

                DocumentTypeCode = new DocumentTypeCodeType { Value = "CUST_INV_ID" }
            };


            DocRefList.Add(idRef);
            BaseUBL.AdditionalDocumentReference = DocRefList.ToArray();
        }

 

        /// <summary>
        /// Fatura imza düğümü
        /// </summary>
        public void SetSignature()
        {
            var signature = new[]
            {
               new SignatureType
               {
                ID = new IDType { schemeID = "VKN_TCKN", Value = appParams.Firma.VergiNumarasi },
                SignatoryParty = new PartyType
                {
                    WebsiteURI = new WebsiteURIType { Value = appParams.Firma.WebSiteURL },
                    PartyIdentification = new[]
                    {
                            new PartyIdentificationType
                           {
                              ID = new IDType { schemeID = "VKN", Value = appParams.Firma.VergiNumarasi }
                           }
                    },

                    PartyName = new PartyNameType
                    {
                        Name = new NameType1 { Value = appParams.Firma.FirmaUnvani }
                    },

                    PostalAddress = new AddressType
                    {
                        Room = new RoomType { Value = appParams.Firma.IlKodu+"" },
                        StreetName = new StreetNameType { Value = appParams.Firma.Sokak},
                        BuildingName = new BuildingNameType { Value = appParams.Firma.BinaAdi },
                        BuildingNumber = new BuildingNumberType { Value = appParams.Firma.BinaNo  },
                        CitySubdivisionName = new CitySubdivisionNameType { Value = appParams.Firma.Semt },
                        CityName = new CityNameType { Value = appParams.Firma.Sehir },
                        PostalZone = new PostalZoneType { Value = appParams.Firma.PostaKodu },
                        Region = new RegionType { Value = appParams.Firma.Bolge },
                        Country = new CountryType { Name = new NameType1 { Value = appParams.Firma.Ulke } }
                    },
                    Contact = new ContactType
                    {
                        ElectronicMail = new ElectronicMailType { Value = appParams.Firma.Mail },
                        Telefax = new TelefaxType { Value =  appParams.Firma.Faks },
                        Telephone = new TelephoneType { Value = appParams.Firma.Telefon }
                    }
                },

                DigitalSignatureAttachment = new AttachmentType
                {
                    ExternalReference = new ExternalReferenceType
                    {
                        URI = new URIType { Value = "#Signature" }
                    }
                }
               }
              };
            BaseUBL.Signature = signature;
        }



        AddressType MusteriAdresi(DataRow Cari)
        {
            string Sehir = TurbimSQLHelper.defaultconn.SQLSelect("SELECT Name FROM L_City WHERE Code=" + Cari["CityID"].convInt());
            string Ilce = TurbimSQLHelper.defaultconn.SQLSelect("SELECT Name FROM L_Town WHERE ID=" + Cari["TownID"].convInt());
            string Ulke = TurbimSQLHelper.defaultconn.SQLSelect("SELECT CountryName FROM L_Countries WHERE ID=" + Cari["CountryID"].convInt());

            return new AddressType
            {
                Room = new RoomType { Value = Cari["BuildingNo"] + "" },
                StreetName = new StreetNameType { Value = Cari["Adress"] + "" },
                BuildingName = new BuildingNameType { Value = Cari["BuildingName"] + "" },
                BuildingNumber = new BuildingNumberType { Value = Cari["BuildingNo"] + "" },
                CitySubdivisionName = new CitySubdivisionNameType { Value = Ilce + "" },
                CityName = new CityNameType { Value = Sehir },
                PostalZone = new PostalZoneType { Value = Cari["PostCode"] + "" },
                Region = new RegionType { Value = Cari["StreetName"] + "" },
                Country = new CountryType { Name = new NameType1 { Value = Ulke } }
            };
        }

        /// <summary>
        /// fatura gönderici ve alıcı bilgilerinin doldurulması
        /// </summary>
        public PartyType MusteriBilgisi(int CariID)
        {
            string parametre = "VKN";
            System.Data.DataRow Cari = TurbimSQLHelper.defaultconn.SQLSelectDataRow("SELECT * FROM CRD_Cari WHERE ID=" + CariID);
            string vknTckn = Cari["TCKNo"] + "";
            if (vknTckn == "")
                vknTckn = Cari["TaxNumber"] + "";
            if (vknTckn.Length == 11)
                parametre = "TCKN";
            if (vknTckn.Length < 10)
            {
                appSettings.UyariGoster("Firmanın vergi bilgileri doğru girilmemiş.");
                GoldenNameSpace.Finans.CariHesapKarti fm = new GoldenNameSpace.Finans.CariHesapKarti();
                fm.ID = CariID;
                appSettings.showDialog(fm);
            }
            string firstname = (Cari["Name"] + "").Split(' ')[0];
            string lastname = ((Cari["Name"] + "").Split(' ').Length > 1) ? (Cari["Name"] + "").Split(' ')[1] : (Cari["Name"] + "").Split(' ')[0];
            /*
            if ((Cari["Name"] + "").Split(' ').Length > 2)
            {
                firstname = (Cari["Name"] + "").Split(' ')[0];
                lastname = (Cari["Name"] + "").Substring((Cari["Name"] + "").Length);
            }*/

            return new PartyType
            {
                WebsiteURI = new WebsiteURIType { Value = Cari["WebUrl"] + "" },


                PartyIdentification = new[]
                {
                   new PartyIdentificationType { ID = new IDType { schemeID = parametre, Value = vknTckn } }
                },

                PartyName = new PartyNameType { Name = new NameType1 { Value = Cari["Name"] + "" } },

                PostalAddress = MusteriAdresi(Cari),

                PartyTaxScheme = new PartyTaxSchemeType
                {
                    TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = Cari["TaxOffice"] + "" } }
                },

                Contact = new ContactType
                {
                    Telephone = new TelephoneType { Value = Cari["Phone"] + "" },
                    Telefax = new TelefaxType { Value = Cari["Fax"] + "" },
                    ElectronicMail = new ElectronicMailType { Value = Cari["Email"] + "" }
                },
                Person = (parametre == "VKN") ? null : new PersonType
                {
                    FirstName = new FirstNameType { Value = firstname },
                    FamilyName = new FamilyNameType { Value = lastname },
                }
            };
        }

        /// <summary>
        /// fatura gönderici ve alıcı bilgilerinin doldurulması
        /// </summary>
        public PartyType GIB()
        {
            string parametre = "VKN";

            return new PartyType
            {
                WebsiteURI = new WebsiteURIType { Value =  "http://gib.gov.tr" },


                PartyIdentification = new[]
                {
                   new PartyIdentificationType { ID = new IDType { schemeID = parametre, Value = "1460415308" } }
                },

                PartyName = new PartyNameType { Name = new NameType1 { Value = "GÜMRÜK VE TİCARET BAKANLIĞI BİLGİ İŞLEM DAİRESİ BAŞKANLIĞI" } },

                PostalAddress = new AddressType
                {
                    Room = new RoomType { Value = "151" },
                    StreetName = new StreetNameType { Value = "Üniversiteler Mahallesi Dumlupınar Bulvarı" },
                    BuildingName = new BuildingNameType { Value = "151" },
                    BuildingNumber = new BuildingNumberType { Value = "151" },
                    CitySubdivisionName = new CitySubdivisionNameType { Value = "Çankaya" },
                    CityName = new CityNameType { Value = "Ankara" },
                    PostalZone = new PostalZoneType { Value ="0600" },
                    Region = new RegionType { Value = "Çankaya" },
                    Country = new CountryType { Name = new NameType1 { Value = "Türkiye" } }
                },

                PartyTaxScheme = new PartyTaxSchemeType
                {
                    TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = "Ulus" } }
                }
            };
        }




        public PartyType FirmaBilgisi()
        {
            if (appSettings.UserFullName.Split(' ').Length < 2)
            {
                appSettings.UserFullName = appSettings.showInputBox("Lütfen ad soyadınızı arada boşluk bırakarak yazınız.", "Adınızı yazınız", appSettings.UserFullName);
                TurbimSQLHelper.defaultconn.SQLExecuteNonQuery(string.Format("UPDATE X_Users SET NameSirName='{0}' WHERE ID={1}", appSettings.UserFullName, appSettings.UserID));
            }
            string vknTckn = appParams.Firma.VergiNumarasi;
            string parametre = "VKN";
            if (vknTckn.Length == 11)
                parametre = "TCKN";
            return new PartyType
            {
                WebsiteURI = new WebsiteURIType { Value = appParams.Firma.WebSiteURL },


                PartyIdentification = new[]
                {
                   new PartyIdentificationType { ID = new IDType { schemeID = parametre, Value = vknTckn } },
                           new PartyIdentificationType
                           {
                              ID = new IDType { schemeID = "TICARETSICILNO", Value = appParams.Firma.TicaretSicilNo}
                           }
                           ,
                           new PartyIdentificationType
                           {
                              ID = new IDType { schemeID = "MERSISNO", Value = appParams.Firma.MersisNo}
                           }
                           ,
                           new PartyIdentificationType
                           {
                              ID = new IDType { schemeID = "TICARETSICILNO", Value = appParams.Firma.TicaretSicilNo}
                           }
                           /*
                           ,
                           new PartyIdentificationType
                           {
                              ID = new IDType { schemeID = "KEPADRESI", Value = appParams.Firma.KepAdresi}
                           }*/
                },

                PartyName = new PartyNameType { Name = new NameType1 { Value = appParams.Firma.FirmaUnvani } },

                PostalAddress = new AddressType
                {
                    Room = new RoomType { Value = appParams.Firma.BinaNo + "" },
                    StreetName = new StreetNameType { Value = (appParams.Firma.Sokak != "") ? appParams.Firma.Sokak : appParams.Firma.Adres },
                    BuildingName = new BuildingNameType { Value = appParams.Firma.BinaNo },
                    BuildingNumber = new BuildingNumberType { Value = appParams.Firma.BinaNo + "" },
                    CitySubdivisionName = new CitySubdivisionNameType { Value = appParams.Firma.Semt },
                    CityName = new CityNameType { Value = appParams.Firma.Sehir },
                    PostalZone = new PostalZoneType { Value = appParams.Firma.PostaKodu },
                    Region = new RegionType { Value = appParams.Firma.Semt },
                    Country = new CountryType { Name = new NameType1 { Value = appParams.Firma.Ulke } }
                },

                PartyTaxScheme = new PartyTaxSchemeType
                {
                    TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = appParams.Firma.VergiDairesi } }
                },

                Contact = new ContactType
                {
                    Telephone = new TelephoneType { Value = appParams.Firma.Telefon },
                    Telefax = new TelefaxType { Value = appParams.Firma.Faks },
                    ElectronicMail = new ElectronicMailType { Value = appParams.Firma.Mail }
                },
                Person = (parametre == "VKN") ? null : new PersonType
                {
                    FirstName = new FirstNameType { Value = appParams.Firma.FirmaYetkilisi.Split(' ')[0] },
                    FamilyName = new FamilyNameType { Value = (appParams.Firma.FirmaYetkilisi.Split(' ').Length > 1) ? appParams.Firma.FirmaYetkilisi.Split(' ')[1] : appParams.Firma.FirmaYetkilisi.Split(' ')[0] },
                }
            };
        }
        /// <summary>
        /// Ödenecek fiyat
        /// </summary>
        public virtual void SetAllowanceCharge(AllowanceChargeType[] allowenceCharges)
        {
            BaseUBL.AllowanceCharge = allowenceCharges;
        }

 

        /// <summary>
        /// Ödenecek fiyatın hesaplanması
        /// </summary>
        public virtual AllowanceChargeType[] CalculateAllowanceCharges()
        {
            AllowanceChargeType allowanceCharge = new AllowanceChargeType
            {
                Amount = new AmountType2 { Value = 0 },
                BaseAmount = new BaseAmountType { Value = 0 },
                ChargeIndicator = new ChargeIndicatorType { Value = false },

            };
            foreach (var item in BaseUBL.InvoiceLine)
            {
                foreach (var iskonto in item.AllowanceCharge)
                {
                    allowanceCharge.BaseAmount.currencyID = iskonto.Amount.currencyID;
                    allowanceCharge.Amount.currencyID = iskonto.Amount.currencyID;
                    allowanceCharge.Amount.Value += iskonto.Amount.Value;
                    allowanceCharge.BaseAmount.Value += iskonto.BaseAmount.Value;
                }
            }

            return new[] { allowanceCharge };
        }



        /// <summary>
        /// Toplam verginin hesaplanması
        /// </summary>
        public virtual TaxTotalType[] CalculateTaxTotal()
        {
            List<TaxTotalType> taxTotalList = new List<TaxTotalType>();
            List<TaxSubtotalType> taxSubTotalList = new List<TaxSubtotalType>();

            TaxTotalType taxTotal = new TaxTotalType { TaxAmount = new TaxAmountType { Value = 0 } };

            var taxSubtotal18 = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = 0 },
                TaxAmount = new TaxAmountType { Value = 0 },
                Percent = new PercentType1 { Value = 18 },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        Name = new NameType1 { Value = "KDV" },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = "0015"

                        }
                    }
                }
            };
            var taxSubtotal8 = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = 0 },
                TaxAmount = new TaxAmountType { Value = 0 },
                Percent = new PercentType1 { Value = 8 },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        Name = new NameType1 { Value = "KDV" },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = "0015"

                        }
                    }
                }
            };
            var taxSubtotal2 = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = 0 },
                TaxAmount = new TaxAmountType { Value = 0 },
                Percent = new PercentType1 { Value = 2 },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        Name = new NameType1 { Value = "KDV" },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = "0015"

                        }
                    }
                }
            };
            var taxSubtotal1 = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = 0 },
                TaxAmount = new TaxAmountType { Value = 0 },
                Percent = new PercentType1 { Value = 1 },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        Name = new NameType1 { Value = "KDV" },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = "0015"

                        }
                    }
                }
            };
            var taxSubtotal = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = 0 },
                TaxAmount = new TaxAmountType { Value = 0 },
                Percent = new PercentType1 { Value = 0 },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        Name = new NameType1 { Value = "KDV" },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = "0015"

                        }
                    }
                }
            };

            var taxSubtotalstopaj = new TaxSubtotalType
            {
                TaxableAmount = new TaxableAmountType { Value = FaturaDR["NetTotal"].convDecimal(), currencyID= _documentCurrencyCode },
                TaxAmount = new TaxAmountType { Value = 0 },
                Percent = new PercentType1 { Value = FaturaDR["StopajOrani"].convInt() },
                TaxCategory = new TaxCategoryType
                {
                    TaxScheme = new TaxSchemeType
                    {
                        Name = new NameType1 { Value = "STOPAJ" },
                        TaxTypeCode = new TaxTypeCodeType
                        {
                            Value = "003"

                        }
                    }
                }
            };


 






            if (KDVIstisnaKodu.convInt() > 0)
            {
                taxSubtotal18.TaxCategory.TaxExemptionReason = new TaxExemptionReasonType() { Value = KDVIstisnaSebebi };
                taxSubtotal18.TaxCategory.TaxExemptionReasonCode = new TaxExemptionReasonCodeType() { Value = KDVIstisnaKodu };
                taxSubtotal8.TaxCategory.TaxExemptionReason = new TaxExemptionReasonType() { Value = KDVIstisnaSebebi };
                taxSubtotal8.TaxCategory.TaxExemptionReasonCode = new TaxExemptionReasonCodeType() { Value = KDVIstisnaKodu };
                taxSubtotal2.TaxCategory.TaxExemptionReason = new TaxExemptionReasonType() { Value = KDVIstisnaSebebi };
                taxSubtotal2.TaxCategory.TaxExemptionReasonCode = new TaxExemptionReasonCodeType() { Value = KDVIstisnaKodu };
                taxSubtotal1.TaxCategory.TaxExemptionReason = new TaxExemptionReasonType() { Value = KDVIstisnaSebebi };
                taxSubtotal1.TaxCategory.TaxExemptionReasonCode = new TaxExemptionReasonCodeType() { Value = KDVIstisnaKodu };
                taxSubtotal.TaxCategory.TaxExemptionReason = new TaxExemptionReasonType() { Value = KDVIstisnaSebebi };
                taxSubtotal.TaxCategory.TaxExemptionReasonCode = new TaxExemptionReasonCodeType() { Value = KDVIstisnaKodu };
            }
            foreach (var item in BaseUBL.InvoiceLine)
            {
                taxTotal.TaxAmount.Value += item.TaxTotal.TaxAmount.Value;
                taxTotal.TaxAmount.currencyID = item.TaxTotal.TaxAmount.currencyID;

                foreach (var tax in item.TaxTotal.TaxSubtotal)
                {
                    if (tax.Percent.Value == 18)
                    {
                        taxSubtotal18.TaxableAmount.Value += tax.TaxableAmount.Value;
                        taxSubtotal18.TaxableAmount.currencyID = tax.TaxableAmount.currencyID;
                        taxSubtotal18.TaxAmount.Value += item.TaxTotal.TaxAmount.Value;
                        taxSubtotal18.TaxAmount.currencyID = tax.TaxAmount.currencyID;
                    }
                    else if (tax.Percent.Value == 8)
                    {
                        taxSubtotal8.TaxableAmount.Value += tax.TaxableAmount.Value;
                        taxSubtotal8.TaxableAmount.currencyID = tax.TaxableAmount.currencyID;
                        taxSubtotal8.TaxAmount.Value += item.TaxTotal.TaxAmount.Value;
                        taxSubtotal8.TaxAmount.currencyID = tax.TaxAmount.currencyID;
                    }
                    else if (tax.Percent.Value == 2)
                    {
                        taxSubtotal2.TaxableAmount.Value += tax.TaxableAmount.Value;
                        taxSubtotal2.TaxableAmount.currencyID = tax.TaxableAmount.currencyID;
                        taxSubtotal2.TaxAmount.Value += item.TaxTotal.TaxAmount.Value;
                        taxSubtotal2.TaxAmount.currencyID = tax.TaxAmount.currencyID;
                    }
                    else if (tax.Percent.Value == 1)
                    {
                        taxSubtotal1.TaxableAmount.Value += tax.TaxableAmount.Value;
                        taxSubtotal1.TaxableAmount.currencyID = tax.TaxableAmount.currencyID;
                        taxSubtotal1.TaxAmount.Value += item.TaxTotal.TaxAmount.Value;
                        taxSubtotal1.TaxAmount.currencyID = tax.TaxAmount.currencyID;
                    }
                    else
                    {
                        taxSubtotal.TaxableAmount.Value += tax.TaxableAmount.Value;
                        taxSubtotal.TaxableAmount.currencyID = tax.TaxableAmount.currencyID;
                        taxSubtotal.TaxAmount.Value += item.TaxTotal.TaxAmount.Value;
                        taxSubtotal.TaxAmount.currencyID = tax.TaxAmount.currencyID;
                        taxSubtotal.Percent.Value = tax.Percent.Value.convInt();
                    }

                }
            }
            if (taxSubtotal18.TaxAmount.Value > 0)
                taxSubTotalList.Add(taxSubtotal18);
            if (taxSubtotal8.TaxAmount.Value > 0)
                taxSubTotalList.Add(taxSubtotal8);
            if (taxSubtotal2.TaxAmount.Value > 0)
                taxSubTotalList.Add(taxSubtotal2);
            if (taxSubtotal1.TaxAmount.Value > 0)
                taxSubTotalList.Add(taxSubtotal1);
            if (taxSubtotal.TaxAmount.Value > 0)
                taxSubTotalList.Add(taxSubtotal);
            if (taxSubTotalList.Count == 0)
            {

                taxSubTotalList.Add(taxSubtotal);

            }
            if (FaturaDR["StopajOrani"].convInt() > 0)
                taxSubTotalList.Add(taxSubtotalstopaj);
            taxTotal.TaxSubtotal = taxSubTotalList.ToArray();
            taxTotalList.Add(taxTotal);

            return taxTotalList.ToArray();
        }



        /// <summary>
        /// Toplam vergi değeri
        /// </summary>
        public virtual void SetTaxTotal(TaxTotalType[] taxTotal)
        {
            BaseUBL.TaxTotal = taxTotal;
        }



        /// <summary>
        /// Toplam parasal değerlerin hesaplanması
        /// </summary>
        public virtual MonetaryTotalType CalculateLegalMonetaryTotal()
        {
            MonetaryTotalType legalMonetaryTotal = new MonetaryTotalType
            {
                LineExtensionAmount = new LineExtensionAmountType { Value = 0 },

                TaxExclusiveAmount = new TaxExclusiveAmountType { Value = 0 },

                TaxInclusiveAmount = new TaxInclusiveAmountType { Value = 0 },

                AllowanceTotalAmount = new AllowanceTotalAmountType { Value = 0 },

                PayableAmount = new PayableAmountType { Value = 0 }
            };

            foreach (var line in BaseUBL.InvoiceLine)
            {


                foreach (var allowance in line.AllowanceCharge)
                {
                    legalMonetaryTotal.AllowanceTotalAmount.currencyID = allowance.Amount.currencyID;
                    legalMonetaryTotal.AllowanceTotalAmount.Value += allowance.Amount.Value;
                    legalMonetaryTotal.TaxInclusiveAmount.currencyID = line.LineExtensionAmount.currencyID;

                    legalMonetaryTotal.TaxInclusiveAmount.Value += line.LineExtensionAmount.Value -
                        allowance.Amount.Value + line.TaxTotal.TaxAmount.Value;
                }

                legalMonetaryTotal.LineExtensionAmount.currencyID = line.LineExtensionAmount.currencyID;
                legalMonetaryTotal.LineExtensionAmount.Value += line.LineExtensionAmount.Value;
                legalMonetaryTotal.PayableAmount.currencyID = line.LineExtensionAmount.currencyID;
                legalMonetaryTotal.PayableAmount.Value = FaturaDR["Total"].convDecimal(2);

                foreach (var tax in line.TaxTotal.TaxSubtotal)
                {
                    legalMonetaryTotal.TaxExclusiveAmount.currencyID = tax.TaxableAmount.currencyID;
                    legalMonetaryTotal.TaxExclusiveAmount.Value += tax.TaxableAmount.Value;
                }

            }
            return legalMonetaryTotal;
        }


        /// <summary>
        /// Toplam parasal değerler
        /// </summary>
        public virtual void SetLegalMonetaryTotal(MonetaryTotalType legalMonetoryTotal)
        {
            BaseUBL.LegalMonetaryTotal = legalMonetoryTotal;
        }



        /// <summary>
        /// fatura kalem bilgileri
        /// </summary>
        public virtual void SetInvoiceLines(InvoiceLineType[] invoiceLines)
        {
            BaseUBL.InvoiceLine = invoiceLines;
            BaseUBL.LineCountNumeric = new LineCountNumericType { Value = invoiceLines.Length };
        }

        List<ItemIdentificationType> SatirEkAlanlari(DataRow satir)
        {
            List<ItemIdentificationType> t = new List<ItemIdentificationType>();
            foreach(DataRow dr in db.SQLSelectToDataTable("SELECT SettingValue FROM x_Settings WHERE SettingName LIKE 'EFaturaEkAlanlari%' ORDER BY SettingName").Rows)
            {
                if (dr["SettingValue"].convString().ToLower().StartsWith("select "))
                {

                }
                else if (dr["SettingValue"].convString().Split('|').Length > 1)
                {
                    t.Add(new ItemIdentificationType() { ID = new IDType { schemeID = dr["SettingValue"].convString().Split('|')[1], Value = dr["Value"] + "" } });
                }
            }
            return t;
            
        }


        DeliveryType[] SatirIhracatbilgileri(DataRow satir, DataRow Cari)
        {
            if (satir["Type"].convInt() != 8) return null;
            if (satir["GTIP"] + ""=="")
            {
                throw new Exception("İhracat faturalarında GTIP kodu girilmesi zorunludur.Ürünün GTIP Kodunu kontrol ediniz.");
            }
            if (satir["SevkiyatTeslimSekli"] + "" == "")
            {
                throw new Exception("İhracat faturalarında Sevkiyat teslim şeklinin girilmesi zorunludur.");
            }
            if (satir["SevkiyatPaketMiktari"] + "" == "")
            {
                throw new Exception("İhracat faturalarında Sevkiyat paket miktarı girilmesi zorunludur.");
            }
            if (satir["SevkiyatPaketTuru"] + "" == "")
            {
                throw new Exception("İhracat faturalarında Sevkiyat paket türü girilmesi zorunludur.");
            }
            if (satir["SevkiyatTasimaYolu"] + "" == "")
            {
                throw new Exception("İhracat faturalarında Sevkiyat taşıma yolu girilmesi zorunludur.");
            }
            return new DeliveryType[]
            {
                        new DeliveryType
                        {
                          DeliveryAddress = MusteriAdresi(Cari)
                        , DeliveryTerms = new DeliveryTermsType[] {new DeliveryTermsType{  ID = new IDType { schemeID = "INCOTERMS", Value = satir["SevkiyatTeslimSekli"] +"" }  } }
                        , Shipment= new ShipmentType
                            { ID= new IDType{ Value = satir["Code"]+"" }
                              , ShipmentStage= new ShipmentStageType[]{ new ShipmentStageType{ TransportModeCode= new TransportModeCodeType{ Value= satir["SevkiyatTasimaYolu"] + "" } } }
                              , GoodsItem= new GoodsItemType[] { new GoodsItemType { RequiredCustomsID = new RequiredCustomsIDType { Value = satir["GTIP"] + "" } } }
                              , TransportHandlingUnit = new TransportHandlingUnitType[] 
                                        { new TransportHandlingUnitType 
                                                {  
                                                    ActualPackage= new PackageType[] { new PackageType{ ID= new IDType { Value = "" }, Quantity=new QuantityType2{ Value=satir["SevkiyatPaketMiktari"].convInt()}, PackagingTypeCode=new PackagingTypeCodeType{ Value= (satir["SevkiyatPaketTuru"]+""==""?"CN": satir["SevkiyatPaketTuru"] + "") } } }  
                                                } 
                                         }
                            }

                        }

             };

        }



        /// <summary>
        /// fatura kalem bilgilerinin doldurulması
        /// </summary>
        public InvoiceLineType[] GetInvoiceLines(DataRow Invoice, string Currency)
        {          
            DataTable Lines = TurbimSQLHelper.defaultconn.SQLSelectToDataTable(
    string.Format(@"SELECT  L.UnitPrice, SUM(L.Amount) AS Amount, L.Renk, L.LineExp, L.TaxRate, SUM(L.TotalTax) AS TotalTax, SUM(L.LineTotal) LineTotal, 
SUM(L.Total) AS Total, L.InvoiceID,SUM(L.SevkiyatPaketMiktari) AS SevkiyatPaketMiktari,L.SevkiyatPaketTuru,L.SevkiyatTeslimSekli,L.SevkiyatTasimaYolu, 
 Min(SatirNo), SeriNo, ProductID, I.Code,I.Barcode,I.EnCm, I.BoyCm,I.Derinlik, I.AgirlikGr, I.Agirlik,L.Type,
AVG(DiscountRate) DiscountRate, SUM(L.Discount) AS Discount, L.UnitID, U.GlobalCode, U.UnitCode, I.GTIP, I.PakettekiMiktar, I.BalyadakiPaket
    FROM TRN_StockTransLines L, CRD_Items I, L_Units U WHERE L.UnitID=U.ID AND I.ID=L.ProductID AND L.InvoiceID={0}
GROUP BY L.ProductID, L.SeriNo, L.UnitPrice, L.Renk, L.LineExp, L.TaxRate, L.InvoiceID, L.UnitID,  ProductID, I.Code,I.Barcode,I.EnCm, 
I.BoyCm,I.Derinlik, I.AgirlikGr, I.Agirlik, U.GlobalCode, U.UnitCode, I.GTIP, I.PakettekiMiktar, I.BalyadakiPaket,L.SevkiyatPaketTuru,L.SevkiyatTeslimSekli,L.SevkiyatTasimaYolu, L.Type
ORDER BY Min(L.SatirNo)", Invoice["ID"]));
            DataRow Cari = db.SQLSelectDataRow("SELECT * FROM CRD_Cari  WHERE ID=" + Invoice["CariID"].convInt());

            Random rnd = new Random();
            int lineCount = rnd.Next(1, 10);

            List<InvoiceLineType> list = new List<InvoiceLineType>();
            int i = 0;
            foreach (DataRow dr in Lines.Rows)
            {

                string _unitCode =  dr["GlobalCode"]+"";
                if (_unitCode + "" == "")
                    _unitCode = "EA";
                i++;
                #region invoiceLine
                InvoiceLineType invoiceLine = new InvoiceLineType
                {

                    ID = new IDType { Value = i + "" },
                    InvoicedQuantity = new InvoicedQuantityType { unitCode = _unitCode, Value = dr["Amount"].convDecimal(2) },
                    LineExtensionAmount = new LineExtensionAmountType { currencyID = Currency, Value = dr["LineTotal"].convDecimal(2) },
                    Delivery = SatirIhracatbilgileri(dr, Cari),
                    AllowanceCharge = new[]
                        {
                        new AllowanceChargeType
                        {
                            ChargeIndicator = new ChargeIndicatorType { Value = false },
                            MultiplierFactorNumeric = new MultiplierFactorNumericType { Value = (dr["DiscountRate"].convDecimal(2) / 100) },

                            Amount = new AmountType2
                            {
                                currencyID = Currency,
                                Value = dr["Discount"].convDecimal(2)
                            },

                            BaseAmount = new BaseAmountType
                            {
                                currencyID = Currency,
                                Value = dr["LineTotal"].convDecimal(2)
                            }
                        }
                    },

                    TaxTotal = new TaxTotalType
                    {
                        TaxAmount = new TaxAmountType
                        {
                            currencyID = Currency,
                            Value = dr["TotalTax"].convDecimal(2)
                        },

                        TaxSubtotal = new[]
                            {
                            new TaxSubtotalType
                            {
                                TaxableAmount = new TaxableAmountType
                                {
                                    currencyID = Currency,
                                    Value = (dr["Total"].convDecimal(2) - dr["TotalTax"].convDecimal(2)).convDecimal(2)
                                },

                                TaxAmount = new TaxAmountType
                                {
                                    currencyID = Currency,
                                    Value = dr["TotalTax"].convDecimal(2)
                                },
                                CalculationSequenceNumeric = new CalculationSequenceNumericType
                                {
                                    Value = 1
                                },

                                Percent = new PercentType1 { Value = dr["TaxRate"].convInt() },

                                TaxCategory = new TaxCategoryType
                                {
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Name = new NameType1 { Value = "KDV" },
                                        TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                                    }
                                }

                            }

                        }
                    },
                    WithholdingTaxTotal = new TaxTotalType[]
                    {
                        (FaturaDR["TevkifatKodu"].convInt() > 0) ?
                new TaxTotalType() {
                    TaxAmount = new TaxAmountType
                    {
                        currencyID = Currency,
                        Value = (dr["TotalTax"].convDecimal(2) * (Lines.Compute(db.SQLSelect("SELECT Code2 FROM X_Types WHERE TableName='Tevkifat' AND Code=" + FaturaDR["TevkifatKodu"].convInt()), "").convDouble(2)).convDecimal(2))

                    },
                    TaxSubtotal = new[]
                            {
                        new TaxSubtotalType
                        {
                            TaxableAmount = new TaxableAmountType
                            {
                                currencyID = Currency,
                                Value = (dr["Total"].convDecimal(2) - dr["TotalTax"].convDecimal(2)).convDecimal(2)
                            },

                            TaxAmount = new TaxAmountType
                            {
                                currencyID = Currency,
                                Value = (dr["TotalTax"].convDecimal(2) * (Lines.Compute(db.SQLSelect("SELECT Code2 FROM X_Types WHERE TableName='Tevkifat' AND Code=" + FaturaDR["TevkifatKodu"].convInt()), "").convDecimal(2)).convDecimal(2))
                            },
                            CalculationSequenceNumeric = new CalculationSequenceNumericType
                            {
                                Value = 1
                            },

                            Percent = new PercentType1 { Value = Convert.ToDecimal((Lines.Compute(db.SQLSelect("SELECT Code2 FROM X_Types WHERE TableName='Tevkifat' AND Code=" + FaturaDR["TevkifatKodu"].convInt()), "").convDouble() * 100).convInt()) },

                            TaxCategory = new TaxCategoryType
                            {
                                TaxScheme = new TaxSchemeType
                                {
                                    Name = new NameType1 { Value = db.SQLSelect("SELECT Name FROM X_Types WHERE TableName='Tevkifat' AND Code=" + FaturaDR["TevkifatKodu"].convInt()) },
                                    TaxTypeCode = new TaxTypeCodeType { Value = db.SQLSelect("SELECT Code FROM X_Types WHERE TableName='Tevkifat' AND Code=" + FaturaDR["TevkifatKodu"].convInt()) }
                                }
                            }

                        }

                    }


                } : null
                    },
                    Item = new ItemType
                    {

                        Name = new NameType1
                        {
                            Value = (((dr["LineExp"] + "").Replace(" ", "").StartsWith("/") || (dr["LineExp"] + "").Replace(" ", "").StartsWith("*")) ? (dr["LineExp"] + "").Substring(1) : TurbimSQLHelper.defaultconn.SQLSelect("SELECT Name FROM CRD_Items WHERE ID=" + dr["ProductID"].convInt()) + " " + dr["LineExp"]) + " " + dr["SeriNo"] +
                            (appParams.Fatura.EFaturaSatırlarındaGosterSQL.Replace(" ", "") != "" ? db.SQLSelect(string.Format(appParams.Fatura.EFaturaSatırlarındaGosterSQL, dr["ProductID"].convInt(), dr["InvoiceID"].convInt())) : "")
                        }
                        ,
                        SellersItemIdentification = new ItemIdentificationType { ID = new IDType { Value = dr["Code"] + "" } }
                        ,
                        BuyersItemIdentification = new ItemIdentificationType { ID = new IDType { Value = dr["Barcode"] + "" } }
                        ,
                        AdditionalItemIdentification = SatirEkAlanlari(dr).ToArray()
                        
                    },

                    Price = new PriceType
                    {
                        PriceAmount = new PriceAmountType
                        {
                            currencyID = Currency,
                            Value = dr["UnitPrice"].convDecimal(appParams.Fatura.BirimFiyatKurusHanesi)
                        }
                    }
                };
                /*
                if (KDVIstisnaKodu.convInt() > 0)
                {
                    invoiceLine.TaxTotal.TaxSubtotal.
                    taxSubtotal.TaxCategory.TaxExemptionReason = new TaxExemptionReasonType() { Value = KDVIstisnaSebebi };
                    taxSubtotal.TaxCategory.TaxExemptionReasonCode = new TaxExemptionReasonCodeType() { Value = KDVIstisnaKodu };
                }
                 * */
                #endregion

                list.Add(invoiceLine);
            }

            return list.ToArray();

        }

    }


   
}
