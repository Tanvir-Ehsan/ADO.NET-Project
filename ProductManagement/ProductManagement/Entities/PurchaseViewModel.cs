using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Entities
{
    public class PurchaseViewModel
    {
        public int PurchaseId { get; set; }
        public int EmployeeId { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public int VoucherNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public string EmployeeName { get; set; }
        public string ImgPath { get; set; }
    }
}
