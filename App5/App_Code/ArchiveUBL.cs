using UblInvoiceObject;

/// <summary>
/// Copyright © 2018 Foriba Teknoloji
/// Bu proje örnek bir web servis test projesidir. Yalnızca test sisteminde çalışmaktadır.
/// Version Info    : Version.txt
/// Readme          : Readme.txt
/// </summary>

namespace Turbim.OE.UBL.UBLCreate
{
    public class ArchiveUBL : BaseInvoiceUBL
    {


        public ArchiveUBL(string profileId, string documentCurrencyCode, string note, System.DateTime date, System.Guid guid)
            : base(profileId,  documentCurrencyCode, note, date, guid)
        {
            BaseUBL.IssueTime = new IssueTimeType { Value = date };
            ArsivUBLOlusturma();
        }

        /// <summary>
        /// e-Arşiv UBL de fatura ye ek olarak eklenecek alanların eklenmesi
        /// </summary>
        private void ArsivUBLOlusturma()
        {
            DocRefList.Clear();
            DocRefList.Add(new DocumentReferenceType
            {
                ID = new IDType { Value = "0100" },//Email Gonder
                IssueDate = BaseUBL.IssueDate,
                DocumentTypeCode = new DocumentTypeCodeType { Value = "OUTPUT_TYPE" }   //OUTPUT_TYPE   -  KAGIT veya ELEKTRONIK olmalıdır.
            });
            DocRefList.Add(new DocumentReferenceType  //efatura dan farklı olarak sadece bu alan eklenmiştir. 
            {
                ID = new IDType { Value = "KAGIT" },
                IssueDate = BaseUBL.IssueDate,
                DocumentTypeCode = new DocumentTypeCodeType { Value = "EREPSENDT" }
            });

            DocRefList.Add(new DocumentReferenceType
            {
                ID = new IDType { Value = "99" },
                IssueDate = BaseUBL.IssueDate,
                DocumentTypeCode = new DocumentTypeCodeType { Value = "TRANSPORT_TYPE" }
            });
            BaseUBL.AdditionalDocumentReference = DocRefList.ToArray();



        }
        /// <summary>
        /// Fatura AdditionalDocumentReference alanlarını ekleme
        /// </summary>
        public void PutAdditionalDocumentReference(DocumentReferenceType documentRef)
        {
            DocRefList.Clear();
            if (BaseUBL.AdditionalDocumentReference == null)
                BaseUBL.AdditionalDocumentReference = new DocumentReferenceType[0];

            DocRefList.AddRange(BaseUBL.AdditionalDocumentReference);

            documentRef.IssueDate = BaseUBL.IssueDate;

            DocRefList.Add(documentRef);

            BaseUBL.AdditionalDocumentReference = DocRefList.ToArray();
        }
    }
}
