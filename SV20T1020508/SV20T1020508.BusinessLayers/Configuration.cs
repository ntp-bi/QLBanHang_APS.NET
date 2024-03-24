using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020508.BusinessLayers
{
    /// <summary>
    /// Cấu hình 
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Chuỗi kết thông số kết nối đến CSDL
        /// </summary>
        public static string ConnectionString { get; private set; } = "";

        /// <summary>
        /// Khởi tạo cấu hình cho BussinessLayer 
        /// Hàm này phải gọi trước khi ứng dụng chạy 
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;  
        }
    }
}
