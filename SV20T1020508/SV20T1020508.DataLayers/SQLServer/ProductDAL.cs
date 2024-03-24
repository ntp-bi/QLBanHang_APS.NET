using Azure;
using Dapper;
using SV20T1020508.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                                    VALUES(@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                                    select @@identity;";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAtrribute(ProductAttribute data)
        {
            throw new NotImplementedException();
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                                    VALUES(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                                    select @@identity;";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                                    VALUES(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                                    select @@identity;";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Products 
                     where (@searchValue = N'') or (ProductName like @searchValue) or (categoryID = @categoryID) or (supplierID = @supplierID) or (Price between @minPrice and @maxPrice)";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductAttributes
                            WHERE ProductID = @ProductID
                            DELETE FROM ProductPhotos
                            WHERE ProductID = @ProductID
                            DELETE FROM Products 
                            WHERE ProductID = @ProductID AND NOT EXISTS(SELECT * FROM OrderDetails WHERE ProductID = @ProductID)";
                var parameters = new
                {
                    ProductID = productID,
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductAttributes 
                                    WHERE AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID,
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM ProductPhotos 
                                    WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID,
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool Detele(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // => nonquyery
                var sql = @"delete from ProductAttributes
                            where ProductID = @ProductID
                            delete from ProductPhotos
                            where ProductID = @ProductID
                            delete from Products 
                            where ProductID = @ProductID and not exists (SELECT * FROM OrderDetails WHERE ProductID = @ProductID)";
                var parameters = new
                {
                    ProductID = productID
                };
                // thực thi câu lệnh => có số dòng bị tác động (bị xóa) > 0
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Product Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Products WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                //Thực thi
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductAttributes WHERE AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                //Thực thi
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM ProductPhotos WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                //Thực thi
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ProductID = productID,
                };
                //Thực thi
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> list = new List<Product>();

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"with cte as(
                    select  *,
                            row_number() over(order by ProductName) as RowNumber
                    from    Products
                    where   (@searchValue = N'' or ProductName like @searchValue)
                        and (@categoryID = 0 or CategoryID = @categoryID)
                        and (@supplierID = 0 or SupplierId = @supplierID)
                        and (Price >= @minPrice)
                        and (@maxPrice <= 0 or Price <= @maxPrice)
                    )
                    select * from cte
                    where   (@pageSize = 0)
                        or (RowNumber between (@page - 1)*@pageSize + 1 and @page * @pageSize)";

                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                list = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }

            return list;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> list = new List<ProductAttribute>();

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	*
	                                FROM	ProductAttributes 
	                                WHERE   ProductID = @ProductID
                                    ORDER BY DisplayOrder;";
                //Có dấu @ là tham số 
                var parameters = new
                {
                    ProductID = productID,
                };
                list = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> list = new List<ProductPhoto>();

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	*
	                                FROM	ProductPhotos 
	                                WHERE   ProductID = @ProductID
                                    ORDER BY DisplayOrder;";
                //Có dấu @ là tham số 
                var parameters = new
                {
                    ProductID = productID,
                };
                list = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Products
                                    SET ProductName = @ProductName, ProductDescription = @ProductDescription, SupplierID = @SupplierID, CategoryID = @CategoryID, Unit = @Unit, Price = @Price, Photo = @Photo, IsSelling = @IsSelling
                                    WHERE ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductAttributes
                                    SET ProductID = @ProductID, AttributeName = @AttributeName, AttributeValue = @AttributeValue, DisplayOrder = @DisplayOrder
                                    WHERE AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductPhotos
                                    SET ProductID = @ProductID, Photo = @Photo, Description = @Description, DisplayOrder = @DisplayOrder, IsHidden = @IsHidden
                                    WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}

