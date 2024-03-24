using SV20T1020508.DataLayers;
using SV20T1020508.DataLayers.SQLServer;
using SV20T1020508.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(1, 0, searchValue, 0, 0, 0, 0).ToList();
        }

        /// <summary>
        /// Tìm kiếm lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang (0 nếu không phân trang)</param>
        /// <param name="searchValue">Tên mặt hàng cần tìm (chuỗi rỗng nếu không tìm thấy)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu không tìm theo loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm (0 nếu không tìm theo nhà cung cấp )</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng giá cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không hạn chế mức giá lớn nhất)</param>
        /// <returns></returns>
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0,
            string searchValue = "", int categoryID = 0, int supplierID = 0,
            decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue);
            return productDB.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice).ToList();
        }

        /// <summary>
        /// Lấy thông tin mặt hàng theo mã mặt hàng 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        /// <summary>
        /// Bổ sung mặt hàng mới (hàm trả về mã của mặt hàng được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin mặt hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeteleProduct(int productID)
        {
            if (productDB.InUsed(productID))
                return false;
            return productDB.Detele(productID);
        }
        /// <summary>
        /// Kiểm tra xem mặt hàng hiện có đơn hàng liên quan hay không 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }
        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (sắp xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return (List<ProductPhoto>)productDB.ListPhotos(productID);
        }
        /// <summary>
        /// Lấy thông tin 1 ảnh dựa trên ID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static ProductPhoto? GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }
        /// <summary>
        /// Bổ sung 1 ảnh mặt hàng (hàm trả về mã của ảnh được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        /// <summary>
        /// Cập nhật ảnh của mặt hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        /// <summary>
        /// Xóa ảnh của mặt hàng 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }
        /// <summary>
        /// Lấy danh sách các thuộc tính của mặt hàng (sắp xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return (List<ProductAttribute>)productDB.ListAttributes(productID);
        }
        /// <summary>
        /// Lấy thông tin của thuộc tính dựa trên ID
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }
        /// <summary>
        /// Bổ sung thuộc tính mặt hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddAtrribute(ProductAttribute data)
        {
            return productDB.AddAtrribute(data);
        }
        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        /// <summary>
        /// Xóa thuôc tính của mặt hàng 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }

    }
}
