using System;
using System.Collections.Generic;

#nullable disable

namespace EfTestApp.Model.Tables
{
    public partial class Siparis
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public int MusteriId { get; set; }
        public Guid SiparisGrubu { get; set; }
        public decimal Miktar { get; set; }
        public DateTime? SiparisTarihi { get; set; }

        public virtual Musteri Musteri { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
