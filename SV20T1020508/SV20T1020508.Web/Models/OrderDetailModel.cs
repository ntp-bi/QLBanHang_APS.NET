using SV20T1020508.DomainModels;

namespace SV20T1020508.Web.Models
{
    /// <summary>
    /// biểu diễn dữ liệu sử dụng cho chức năng hiển thị chi tiết của đơn hàng  (Order/Details): 
    /// </summary>
    public class OrderDetailModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }

    }
}
