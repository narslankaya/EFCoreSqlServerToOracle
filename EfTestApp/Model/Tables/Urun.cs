using System;
using System.Collections.Generic;

#nullable disable

namespace EfTestApp.Model.Tables
{
    public partial class Urun
    {
        public Urun()
        {
            Siparis = new HashSet<Siparis>();
        }

        public int Id { get; set; }
        public string UrunAdi { get; set; }

        public virtual ICollection<Siparis> Siparis { get; set; }
    }
}
