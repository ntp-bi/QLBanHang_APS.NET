using SV20T1020508.DomainModels;
namespace SV20T1020508.Web.Models
{
    /// <summary>
    /// Lớp cha cho các lớp biểu diễn dữ liệu kết quả tìm kiếm, phân trang 
    /// </summary>
    public abstract class BasePaginationResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; }
        public int RowCount { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;

                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)  // rowCount = 20 => 1 page, 21 => 2 page...
                    c += 1;
                return c;
            }
        }
    }

    /// <summary>
    /// Kết quả tìm kiếm và lấy danh sách khách hàng
    /// </summary>
    public class CustomerSearchResult : BasePaginationResult
    {
        public List<Customer> Data { get; set; }
    }

    /// <summary>
    /// Kết quả tìm kiếm và lấy danh sách loại hàng 
    /// </summary>
    public class CategorySearchResult : BasePaginationResult
    {
        public List<Category> Data { get; set; }
    }

    /// <summary>
    /// Kết quả tìm kiếm và lấy danh sách nhân viên 
    /// </summary>
    public class EmployeeSearchResult : BasePaginationResult
    {
        public List<Employee> Data { get; set; }
    }

    /// <summary>
    /// Kết quả tìm kiếm và lấy danh sách người giao hàng 
    /// </summary>
    public class ShipperSearchResult : BasePaginationResult
    {
        public List<Shipper> Data { get; set; }
    }

    /// <summary>
    /// Kết quả tìm kiếm và lấy danh sách nhà cung cấp 
    /// </summary>
    public class SupplierSearchResult : BasePaginationResult
    {
        public List<Supplier> Data { get; set; }
    }
    /// <summary>
    /// Kết quả tìm kiếm và lấy danh sách mặt hàng
    /// </summary>
    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> Data { get; set; }
    }

    /// <summary>
    /// biểu diễn dữ liệu kết quả tìm kiếm đơn hàng: 
    /// </summary>
    public class OrderSearchResult : BasePaginationResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }
}
