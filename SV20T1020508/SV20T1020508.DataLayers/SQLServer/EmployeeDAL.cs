using Dapper;
using SV20T1020508.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.DataLayers.SQLServer
{
    public class EmployeeDAL : _BaseDAL, ICommonDAL<Employee>
    {
        /// <summary>
        /// Hàm không có giá trị trả về
        /// Chuyển cho th cha xử lý (không làm gì cả)
        /// </summary>
        /// <param name="connectionString"></param>
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                // kqua cuối cùng của câu lệnh trả về 1 giá trị(select -1, select 0) => Scalar
                var sql = @"if exists(select * from Employees where Email = @Email)
                                select -1 
                            else
                                begin
                                    insert into Employees(FullName,BirthDate,Address,Phone,Email,Photo,IsWorking)
                                    values(@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking
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
                var sql = @"select count(*) from Employees 
                            where (@searchValue = N'') or (FullName like @searchValue)";

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
                var sql = @"delete from Employees where EmployeeId = @employeeId";
                var parameters = new
                {
                    employeeId = id
                };
                // thực thi câu lệnh => có số dòng bị tác động (bị xóa) > 0
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Employee? Get(int id)
        {
            Employee? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Employees where EmployeeId = @EmployeeId";
                var parameters = new
                {
                    EmployeeId = id,
                };
                // thực thi câu lệnh => QueryFirstOrDefault: trả về dòng đầu tiên không có trả về null
                data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
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
                var sql = @"if exists(select * from Orders where EmployeeId = @EmployeeId)
                                select 1
                            else 
                                select 0";

                var parameters = new
                {
                    EmployeeId = id,
                };

                // thực thi câu lệnh
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public IList<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> data = new List<Employee>();

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối 

            using (var connection = OpenConnection())
            {
                var sql = @"select  *
                            from
                            (
                                select  *, row_number() over (order by FullName) as RowNumber
                                from    Employees
                                where   (@searchValue = N'') or (FullName like @searchValue)
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
                data = connection.Query<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // @: viết chuỗi trên nhiều dòng 
                // kiểm tra email(khóa phụ) => không được trùng  => ktra xem có tồn tại email trùng nhân viên khác hay không 
                // => nonquery
                var sql = @"if not exists(select * from Employees where EmployeeId <> @employeeId and Email = @email)
                                begin
                                    update Employees 
                                    set FullName = @FullName,
                                        BirthDate = @BirthDate,                                     
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        Photo = @Photo,
                                        IsWorking = @IsWorking
                                    where EmployeeId = @EmployeeId
                                end";

                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking,
                    EmployeeId = data.EmployeeID
                };

                // thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
