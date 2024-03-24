using Dapper;
using SV20T1020508.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.DataLayers.SQLServer
{
    public class ShipperDAL : _BaseDAL, ICommonDAL<Shipper>
    {
        /// <summary>
        /// Hàm không có giá trị trả về
        /// Chuyển cho th cha xử lý (không làm gì cả)
        /// </summary>
        /// <param name="connectionString"></param>
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Shippers(ShipperName, Phone)
                            values(@ShipperName, @Phone);
                            select @@identity;";
                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",   
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
                var sql = @"select count(*) from Shippers 
                            where (@searchValue = N'') or (ShipperName like @searchValue)";

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
                var sql = @"delete from Shippers where ShipperId = @shipperId";
                var parameters = new
                {
                    shipperId = id
                };
                // thực thi câu lệnh => có số dòng bị tác động (bị xóa) > 0
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Shippers where ShipperId = @shipperId";
                var parameters = new
                {
                    shipperId = id,
                };
                // thực thi câu lệnh => QueryFirstOrDefault: trả về dòng đầu tiên không có trả về null
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
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
                var sql = @"if exists(select * from Orders where ShipperId = @shipperId)
                                select 1
                            else 
                                select 0";

                var parameters = new
                {
                    shipperId = id,
                };

                // thực thi câu lệnh
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> data = new List<Shipper>();

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối 

            using (var connection = OpenConnection())
            {
                var sql = @"select  *
                            from
                            (
                                select  *, row_number() over (order by ShipperName) as RowNumber
                                from    Shippers
                                where   (@searchValue = N'') or (ShipperName like @searchValue)
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
                data = connection.Query<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // @: viết chuỗi trên nhiều dòng                
                // => nonquery
                var sql = @"update Shippers 
                            set ShipperName = @ShipperName,
                                Phone = @Phone
                            where ShipperId = @ShipperId";

                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
                    ShipperId = data.ShipperID
                };

                // thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
