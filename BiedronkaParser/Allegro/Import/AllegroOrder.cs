using System;
using System.Collections.Generic;
using System.Text;

namespace Integrations.Allegro.Import
{
    public class AllegroOrder
    {
        public long OfferId { get; set; }
        public string Title { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal OriginalPrice { get; set; }
        public string SellerLogin { get; set; }
    }
}
