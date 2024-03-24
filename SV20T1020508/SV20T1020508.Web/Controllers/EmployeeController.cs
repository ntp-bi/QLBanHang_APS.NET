using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020508.BusinessLayers;
using SV20T1020508.DomainModels;
using SV20T1020508.Web.Models;

namespace SV20T1020508.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 20; // sluong dòng trong 1 bảng 
        private const string EMPLOYEE_SEARCH = "employee_search";// Tên biến dùng để lưu trong session
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            // Lấy đầu vào tìm kiếm đang lưu lại trong session
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);

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

            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new Models.EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm trong session 
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee model = new Employee()
            {
                EmployeeID = 0, // gán id = 0 => bổ sung, id !=0 => cập nhật
                BirthDate = new DateTime(1990, 1, 1),
                Photo = "nophoto.png"
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            Employee? model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "nophoto.jpg";

            return View(model);
        }

        [HttpPost] // chỉ nhận dữ liệu khi dùng method: POST => không ghi gì thì method GET
                   // Customer tương đương = mình sẽ viết liệt kê các name trong form (VD: CustomerName, ContactName...)
        public IActionResult Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {
            try
            {
                // kiểm tra đầu vào và đưa các thông báo lỗi vào trong ModelSate (nếu có)
                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError("FullName", "Tên không được để trống");              
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError("Email", "Vui lòng nhập Email của nhân viên");               

                // Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không 
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";
                    return View("Edit", data);
                }

                // xử lý ngày sinh
                DateTime? birthDate = birthDateInput.ToDateTime();
                if (birthDate.HasValue)
                    data.BirthDate = birthDate.Value;

                // Xử lý ảnh được upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho nhân viên)
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu 
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\employees"); // Đường dẫn đến thư mục lưu file 
                    string filePath = Path.Combine(folder, fileName); // Đường dẫn đến file cần lưu 

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    };
                    data.Photo = fileName;
                }

                if (data.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng với nhân viên khác");
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
            // NẾU POST => XÓA EMPLOYEE
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            // nếu GET => LẤY THÔNG TIN NGƯỜI DÙNG 
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            // nếu khách hàng không có đơn hàng thì cho phép xóa 
            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);

            return View(model);
        }
    }
}
