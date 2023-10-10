using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Entities
{
    public class Purchase_t
    {
        public int PurchaseId { get; set; }
        public int VoucherNo { get; set; }
        public DateTime PruchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ImgPath { get; set; }
        public int SupplierId { get; set; }
        public int EmployeeId { get; set; }
        public int ProductId { get; set; }
    }
}
