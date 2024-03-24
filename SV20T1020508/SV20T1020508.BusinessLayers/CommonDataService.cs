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
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu chung
    /// (Tỉnh/thành, khách hàng, nhà cung cấp, loại hàng, người giao hàng, nhân viên)
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;


        /// <summary>
        /// Ctor (Câu hỏi : static constructor hoạt động như thế nào?)
        /// => Static constructor chỉ được gọi một lần duy nhất, khi lớp đó được tải lần đầu tiên vào bộ nhớ.
        /// trước khi bất kỳ phương thức static nào của lớp đó được gọi hoặc bất kỳ instance nào của lớp đó được tạo.
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;

            provinceDB = new ProvinceDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
        }

        // start tỉnh thành
        /// <summary>
        /// Danh sách tỉnh thành 
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }
        // end tỉnh thành 


        // start nhà cung cấp
        /// <summary>
        /// Tìm kiếm lấy danh sách nhà cung cấp 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 nhà cung cấp qua id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }

        /// <summary>
        /// Bổ sung nhà cung cấp
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier supplier)
        {
            return supplierDB.Add(supplier);
        }

        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier supplier)
        {
            return supplierDB.Update(supplier);
        }

        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            // khi dữ liệu đang được sử dụng thì không thể xóa 
            //if (supplierDB.IsUsed(id))
               // return false;
            return supplierDB.Delete(id);
        }

        /// <summary>
        /// Kiểm tra nhà cung cấp có dữ liệu liên quan hay không 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedSupplier(int id)
        {
            return supplierDB.IsUsed(id);
        }
        // end nhà cung cấp

        // start khách hàng 
        /// <summary>
        /// Tìm kiếm lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 khách hàng qua id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }

        /// <summary>
        /// Bổ sung khách hàng 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer customer)
        {
            return customerDB.Add(customer);
        }

        /// <summary>
        /// Cập nhật khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer customer)
        {
            return customerDB.Update(customer);
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            // khi dữ liệu đang được sử dụng thì không thể xóa 
            if (customerDB.IsUsed(id))
                return false;
            return customerDB.Delete(id);
        }

        /// <summary>
        /// Kiểm tra khách hàng có dữ liệu liên quan hay không 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.IsUsed(id);
        }
        // end khách hàng 

        // start người giao hàng  
        /// <summary>
        /// Tìm kiếm lấy danh sách người giao hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 người giao hàng qua id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }

        /// <summary>
        /// Bổ sung người giao hàng
        /// </summary>
        /// <param name="shipper"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper shipper)
        {
            return shipperDB.Add(shipper);
        }

        /// <summary>
        /// Cập nhật người giao hàng
        /// </summary>
        /// <param name="shipper"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper shipper)
        {
            return shipperDB.Update(shipper);
        }

        /// <summary>
        /// Xóa người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            // khi dữ liệu đang được sử dụng thì không thể xóa 
            if (shipperDB.IsUsed(id))
                return false;
            return shipperDB.Delete(id);
        }

        /// <summary>
        /// Kiểm tra người giao hàng có dữ liệu liên quan hay không 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.IsUsed(id);
        }
        // end người giao hàng

        // start nhân viên 
        /// <summary>
        /// Tìm kiếm lấy danh sách nhân viên
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 nhân viên qua id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }

        /// <summary>
        /// Bổ sung nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee employee)
        {
            return employeeDB.Add(employee);
        }

        /// <summary>
        /// Cập nhật nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee employee)
        {
            return employeeDB.Update(employee);
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            // khi dữ liệu đang được sử dụng thì không thể xóa 
            if (employeeDB.IsUsed(id))
                return false;
            return employeeDB.Delete(id);
        }

        /// <summary>
        /// Kiểm tra nhân viên có dữ liệu liên quan hay không 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.IsUsed(id);
        }
        // end nhân viên

        // start loại hàng 
        /// <summary>
        /// Tìm kiếm lấy danh sách loại hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategorys(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 loại hàng qua id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }

        /// <summary>
        /// Bổ sung loại hàng
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static int AddCategory(Category category)
        {
            return categoryDB.Add(category);
        }

        /// <summary>
        /// Cập nhật loại hàng
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category category)
        {
            return categoryDB.Update(category);
        }

        /// <summary>
        /// Xóa loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            // khi dữ liệu đang được sử dụng thì không thể xóa 
            //if (categoryDB.IsUsed(id))
                //return false;
            return categoryDB.Delete(id);
        }

        /// <summary>
        /// Kiểm tra loại hàng có dữ liệu liên quan hay không 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.IsUsed(id);
        }
        // end loại hàng
    }
}