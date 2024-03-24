using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020508.BusinessLayers;
using SV20T1020508.DomainModels;
using SV20T1020508.Web.Models;

namespace SV20T1020508.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    //[Authorize(Roles = $"{WebUserRoles.Customer}")] // ko truy cập dc
    //[Authorize(Roles = $"{WebUserRoles.Customer}, {WebUserRoles.Employee}")] // nếu có employee mới cho phép truy cập 
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20; // sluong dòng trong 1 bảng 
        private const string CUSTOMER_SEARCH = "customer_search";// Tên biến dùng để lưu trong session
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            // Lấy đầu vào tìm kiếm đang lưu lại trong session
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);

            // Trường hợp trong session chưa có điều kiện thì tạo điều kiện mới 
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(input);
        }

        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0; // kqua hienthi ra bảng 

            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new Models.CustomerSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm trong session 
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            Customer model = new Customer()
            {
                CustomerID = 0 // gán id = 0 => bổ sung, id !=0 => cập nhật
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            Customer? model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost] // chỉ nhận dữ liệu khi dùng method: POST =>không ghi gì thì method GET
        // Customer tương đương = mình sẽ viết liệt kê các name trong form (VD: CustomerName, ContactName...)
        public IActionResult Save(Customer data)
        {
            try
            {
                // kiểm tra đầu vào và đưa các thông báo lỗi vào trong ModelSate (nếu có)
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError("CustomerName", "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError("Email", "Vui lòng nhập Email của khách hàng");
                if (string.IsNullOrWhiteSpace(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");

                // Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không 
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                    return View("Edit", data);
                }

                if (data.CustomerID == 0)
                {
                    int id = CommonDataService.AddCustomer(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCustomer(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng với khách hàng khác");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút");//ex.Message);
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            // NẾU POST => XÓA CUSTOMER 
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            // nếu GET => LẤY THÔNG TIN NGƯỜI DÙNG 
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");

            // nếu khách hàng không có đơn hàng thì cho phép xóa 
            ViewBag.AllowDelete = !CommonDataService.IsUsedCustomer(id);

            return View(model);
        }
    }
}
