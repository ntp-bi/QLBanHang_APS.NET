using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020508.BusinessLayers;
using System.Drawing.Printing;

namespace SV20T1020508.Web
{
    public static class SelectListHelper
    {
        /// <summary>
        /// Danh sách tỉnh thành 
        /// </summary>
        /// <returns></returns>
        /// 
        // SelectListItem => để tạo ra thẻ select option 
        public static List<SelectListItem> Provinces()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn tỉnh thành --"
            });
            foreach (var item in CommonDataService.ListOfProvinces())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }
            return list;
        }

        public static List<SelectListItem> Categorys()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn loại hàng --"
            });

            int rowCount = 0;

            foreach (var item in CommonDataService.ListOfCategorys(out rowCount, 1, 0, ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        public static List<SelectListItem> Customers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn khách hàng --"
            });

            int rowCount = 0;

            foreach (var item in CommonDataService.ListOfCustomers(out rowCount, 1, 0, ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CustomerID.ToString(),
                    Text = item.CustomerName
                });
            }
            return list;
        }

        public static List<SelectListItem> Suppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn nhà cung cấp --"
            });

            int rowCount = 0;

            foreach (var item in CommonDataService.ListOfSuppliers(out rowCount, 1, 0, ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }

        public static List<SelectListItem> Shippers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn người giao hàng --"
            });

            int rowCount = 0;

            foreach (var item in CommonDataService.ListOfShippers(out rowCount, 1, 0, ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ShipperID.ToString(),
                    Text = item.ShipperName
                });
            }
            return list;
        }
    }
}
