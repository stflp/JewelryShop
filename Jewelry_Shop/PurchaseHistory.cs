using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewelry_Shop
{
    public class PurchaseHistory
    {
        public int purchaseId { get; set; }
        public string username { get; set; }
        public string jewelryName { get; set; }
        public decimal jewelryPrice { get; set; }
        public byte[] jewelryImage { get; set; }
        public DateTime purchaseDate { get; set; }
    }
}
