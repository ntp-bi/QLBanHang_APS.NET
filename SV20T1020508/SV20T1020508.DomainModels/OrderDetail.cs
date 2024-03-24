using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.DomainModels
{
    /// <summary>
    /// biểu diễn thông tin các mặt hàng được bán trong đơn hàng (chi tiết đơn hàng) 
    /// </summary>
    public class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string Unit { get; set; } = "";
        public int Quantity { get; set; } = 0;
        public decimal SalePrice { get; set; } = 0;
        public decimal TotalPrice
        {
            get
            {
                return Quantity * SalePrice;
            }
        }
    }

}
