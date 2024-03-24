using Dapper;
using SV20T1020508.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        /// <summary>
        /// Hàm không có giá trị trả về
        /// Chuyển cho th cha xử lý (không làm gì cả)
        /// </summary>
        /// <param name="connectionString"></param>
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                // kqua cuối cùng của câu lệnh trả về 1 giá trị(select -1, select 0) => Scalar
                var sql = @"insert into Suppliers(SupplierName,ContactName,Province,Address,Phone,Email)
                            values(@SupplierName,@ContactName,@Province,@Address,@Phone,@Email);
                            select @@identity;";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    SupplierId = data.SupplierID
                };
                // thực thi câu lệnh 
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối 
            }

            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Suppliers 
                            where (@searchValue = N'') or (SupplierName like @searchValue)";

                var parameters = new
                {
                    // tên tham số của câu lệnh sql = giá trị chúng ta truyền vào 
                    searchValue = searchValue ?? "", // ?? => nếu searchValue = null thì thay thế = chuỗi rỗng
                };

                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // => nonquyery
                var sql = @"delete from Suppliers where SupplierId = @supplierId";
                var parameters = new
                {
                    supplierId = id
                };
                // thực thi câu lệnh => có số dòng bị tác động (bị xóa) > 0
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Suppliers where SupplierId = @SupplierId";
                var parameters = new
                {
                    SupplierId = id,
                };
                // thực thi câu lệnh => QueryFirstOrDefault: trả về dòng đầu tiên không có trả về null
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return data;
        }

        public bool IsUsed(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối 

            using (var connection = OpenConnection())
            {
                var sql = @"select  *
                            from
                            (
                                select  *, row_number() over (order by SupplierName) as RowNumber
                                from    Suppliers
                                where   (@searchValue = N'') or (SupplierName like @searchValue)
                            ) as t                            
                            where  (@pageSize = 0)
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    // tên tham số của câu lệnh sql = giá trị chúng ta truyền vào 
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // @: viết chuỗi trên nhiều dòng 
                // kiểm tra email(khóa phụ) => không được trùng  
                // => nonquery
                var sql = @"update Suppliers 
                             set SupplierName = @supplierName,
                                 ContactName = @contactName,
                                 Province = @province,
                                 Address = @address,
                                 Phone = @phone,
                                 Email = @email
                             where SupplierId = @supplierId";

                var parameters = new
                {
                    supplierId = data.SupplierID,
                    supplierName = data.SupplierName ?? "",
                    contactName = data.ContactName ?? "",
                    province = data.Province ?? "",
                    address = data.Address ?? "",
                    phone = data.Phone ?? "",
                    email = data.Email ?? ""
                };

                // thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
