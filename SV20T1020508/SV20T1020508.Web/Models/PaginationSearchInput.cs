namespace SV20T1020508.Web.Models
{
    /// <summary>
    /// Đầu vào tìm kiếm dữ liệu để nhận dữ liệu dưới dạng phân trang 
    /// </summary>
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";
    }

    /// <summary>
    /// Đầu vào sử dụng cho tìm kiếm mặt hàng 
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
    }

    /// <summary>
    /// biểu diễn dữ liệu đầu vào sử dụng để tìm kiếm đơn hàng:
    /// </summary>
    public class OrderSearchInput : PaginationSearchInput
    {
        /// <summary> 
        /// Trạng thái của đơn hàng cần tìm         
        /// </summary> 
        public int Status { get; set; } = 0;

        /// <summary> 
        /// Khoảng thời gian cần tìm (chuỗi 2 giá trị ngày có dạng dd/MM/yyyy - dd/MM/yyyy)  
        /// </summary> 
        public string DateRange { get; set; } = "";

        /// <summary> 
        /// Lấy thời điểm bắt đầu dựa vào DateRange 
        /// </summary> 
        public DateTime? FromTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DateRange))
                    return null;

                string[] times = DateRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = Converter.ToDateTime(times[0].Trim()); 
                    return value;
                }
                return null;
            }
        }

        /// <summary> 
        /// Lấy thời điểm kết thúc dựa vào DateRange 
        /// (thời điểm kết thúc phải là cuối ngày) 
        /// </summary> 
        public DateTime? ToTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DateRange))
                    return null;

                string[] times = DateRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = Converter.ToDateTime(times[1].Trim()); 
                    if (value.HasValue)
                        value = value.Value.AddMilliseconds(86399998); //86399999  return value; 
                    return value;
                }

                return null;
            }
        }
    }
}
