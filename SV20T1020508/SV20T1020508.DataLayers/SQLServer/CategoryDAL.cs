using Dapper;
using SV20T1020508.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.DataLayers.SQLServer
{
    public class CategoryDAL : _BaseDAL, ICommonDAL<Category>
    {
        /// <summary>
        /// Hàm không có giá trị trả về
        /// Chuyển cho th cha xử lý (không làm gì cả)
        /// </summary>
        /// <param name="connectionString"></param>
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Category data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Categories where CategoryName = @CategoryName)
                                select -1 
                            else
                                begin
                                    insert into Categories(CategoryName, Description)
                                    values(@CategoryName, @Description);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
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
                var sql = @"select count(*) from Categories 
                            where (@searchValue = N'') or (CategoryName like @searchValue)";

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
                var sql = @"delete from Categories where CategoryId = @categoryId";
                var parameters = new
                {
                    categoryId = id
                };
                // thực thi câu lệnh => có số dòng bị tác động (bị xóa) > 0
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Category? Get(int id)
        {
            Category? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Categories where CategoryId = @CategoryId";
                var parameters = new
                {
                    CategoryId = id,
                };
                // thực thi câu lệnh => QueryFirstOrDefault: trả về dòng đầu tiên không có trả về null
                data = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return data;
        }

        public bool IsUsed(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> data = new List<Category>();

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối 

            using (var connection = OpenConnection())
            {
                var sql = @"select  *
                            from
                            (
                                select  *, row_number() over (order by CategoryName) as RowNumber
                                from    Categories
                                where   (@searchValue = N'') or (CategoryName like @searchValue)
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
                data = connection.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Category data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // @: viết chuỗi trên nhiều dòng                
                // => nonquery
                var sql = @"if not exists(select * from Categories where CategoryId <> @CategoryId and CategoryName = @CategoryName)
                            begin
                                update Categories 
                                set CategoryName = @CategoryName,
                                    Description = @Description
                                where CategoryId = @CategoryId
                            end";

                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
                    CategoryId = data.CategoryID
                };

                // thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
