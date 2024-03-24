using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV20T1020508.DomainModels;
using System.Net;
using System.Numerics;

// ctrl M O: thu gọn 
// ctrl M L: mở ra 
namespace SV20T1020508.DataLayers.SQLServer
{
    /// <summary>
    /// : => kế thừa  ( internal interface ICommonDAL<T> where T : class )
    /// </summary>
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer>
    {
        /// <summary>
        /// Hàm không có giá trị trả về
        /// Chuyển cho th cha xử lý (không làm gì cả)
        /// </summary>
        /// <param name="connectionString"></param>
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                // kqua cuối cùng của câu lệnh trả về 1 giá trị(select -1, select 0) => Scalar
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1 
                            else
                                begin
                                    insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,IsLocked)
                                    values(@CustomerName,@ContactName,@Province,@Address,@Phone,@Email,@IsLocked);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
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
                var sql = @"select count(*) from Customers 
                            where (@searchValue = N'') or (CustomerName like @searchValue)";

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
                var sql = @"delete from Customers where CustomerId = @customerId";
                var parameters = new
                {
                    customerId = id
                };
                // thực thi câu lệnh => có số dòng bị tác động (bị xóa) > 0
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id,
                };
                // thực thi câu lệnh => QueryFirstOrDefault: trả về dòng đầu tiên không có trả về null
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return data;
        }

        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // 1: true, 0: false
                var sql = @"if exists(select * from Orders where CustomerId = @CustomerId)
                                select 1
                            else 
                                select 0";

                var parameters = new
                {
                    CustomerId = id,
                };

                // thực thi câu lệnh
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối 

            using (var connection = OpenConnection())
            {
                var sql = @"select  *
                            from
                            (
                                select  *, row_number() over (order by CustomerName) as RowNumber
                                from    Customers
                                where   (@searchValue = N'') or (CustomerName like @searchValue)
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
                data = connection.Query<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // @: viết chuỗi trên nhiều dòng 
                // kiểm tra email(khóa phụ) => không được trùng  
                // => nonquery
                var sql = @"if not exists(select * from Customers where CustomerId <> @customerId and Email = @email)
                                begin
                                    update Customers 
                                    set CustomerName = @customerName,
                                        ContactName = @contactName,
                                        Province = @province,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email,
                                        IsLocked = @isLocked
                                    where CustomerId = @customerId
                                end";

                var parameters = new
                {
                    customerId = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked                    
                };

                // thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
