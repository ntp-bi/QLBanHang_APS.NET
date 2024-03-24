using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020508.BusinessLayers;
using SV20T1020508.DomainModels;
using SV20T1020508.Web.Models;

namespace SV20T1020508.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 20; // sluong dòng trong 1 bảng 
        private const string CATEGORY_SEARCH = "category_search";// Tên biến dùng để lưu trong session
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            // Lấy đầu vào tìm kiếm đang lưu lại trong session
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);

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

            var data = CommonDataService.ListOfCategorys(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new Models.CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm trong session 
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            Category model = new Category()
            {
                CategoryID = 0 // gán id = 0 => bổ sung, id !=0 => cập nhật
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            Category? model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost] // chỉ nhận dữ liệu khi dùng method: POST =>không ghi gì thì method GET
        public IActionResult Save(Category data)
        {
            try
            {
                // kiểm tra đầu vào và đưa các thông báo lỗi vào trong ModelSate (nếu có)
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                    ModelState.AddModelError("CategoryName", "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError("Description", "Mô tả không được để trống");               

                // Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không 
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";
                    return View("Edit", data);
                }

                if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
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
            // NẾU POST => XÓA CATEGORY 
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }

            // nếu GET => LẤY THÔNG TIN NGƯỜI DÙNG 
            var model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");

            // nếu khách hàng không có đơn hàng thì cho phép xóa 
           // ViewBag.AllowDelete = !CommonDataService.IsUsedCategory(id);

            return View(model);
        }
    }
}
