using System;
using System.Collections.Generic;

#nullable disable

namespace EfTestApp.Model.Tables
{
    public partial class Musteri
    {
        public Musteri()
        {
            Siparis = new HashSet<Siparis>();
        }

        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string Adres { get; set; }

        public virtual ICollection<Siparis> Siparis { get; set; }
    }
}
