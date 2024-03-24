using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020508.BusinessLayers;
using SV20T1020508.DomainModels;
using SV20T1020508.Web.Models;
using System.Reflection;

namespace SV20T1020508.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = " nhập mặt hàng mới";
        const string PRODUCT_SEARCH = "product_search";//session dùng để lưu lại điều kiện tìm kiếm
        public IActionResult Index()
        {
            Models.ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);

            if (input == null)
            {
                input = new ProductSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                };
            }
            return View(input);
        }
        public IActionResult Search(ProductSearchInput input)
        {

            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", input.CategoryID, input.SupplierID);
            var model = new Models.ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                CategoryID = input.CategoryID,
                SupplierID = input.SupplierID,
                RowCount = rowCount,
                Data = data
            };
            // Lưu lại điều kiện tìm kiếm trong session 
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);

        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            ViewBag.IsEdit = false;
            var model = new Product()
            {
                ProductID = 0, // gán id = 0 => bổ sung, id !=0 => cập nhật
                Photo = "nophoto.jpg",
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật mặt hàng";
            ViewBag.IsEdit = true;
            Product? model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(model.Photo))
            {
                model.Photo = "nophoto.jpg";
            }

            return View(model);
        }
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Photo))
                {
                    data.Photo = "nophoto.jpg";
                }

                if (string.IsNullOrWhiteSpace(data.ProductName))
                {
                    ModelState.AddModelError(nameof(data.ProductName), "Tên không được để trống");
                }

                if (string.IsNullOrWhiteSpace(data.Unit))
                {
                    ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
                }

                if (data.Price == 0)
                {
                    ModelState.AddModelError(nameof(data.Price), "Giá của mặt hàng không được để trống");
                }

                if (data.CategoryID.ToString() == "0")
                {
                    ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được để trống");
                }

                if (data.SupplierID.ToString() == "0")
                {
                    ModelState.AddModelError(nameof(data.SupplierID), "Nhà cung cấp không được để trống");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.IsEdit = data.ProductID == 0 ? false : true;
                    ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
                    return View("Edit", data);
                }

                // xử lý ảnh upload:
                // Xử lý ảnh được upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho mặt hàng)
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu 
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\products");// Đường dẫn đến thư mục lưu file 
                    string filePath = Path.Combine(folder, fileName); // Đường dẫn đến file cần lưu 

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    };
                    data.Photo = fileName;
                }

                if (data.ProductID == 0)
                {
                    int id = ProductDataService.AddProduct(data);
                }
                else
                {
                    bool result = ProductDataService.UpdateProduct(data);
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

            if (Request.Method == "POST")
            {
                ProductDataService.DeteleProduct(id);
                return RedirectToAction("Index");
            }

            // nếu GET => LẤY THÔNG TIN NGƯỜI DÙNG 
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            ProductPhoto model = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh mặt hàng";
                    model = new ProductPhoto()
                    {
                        PhotoID = 0,
                        ProductID = id,
                        Photo = "nophoto.jpg",
                    };
                    return View(model);

                case "edit":
                    ViewBag.Title = "Thay đổi ảnh mặt hàng";
                    if (photoId < 0)
                    {
                        return RedirectToAction("Edit");
                    }
                    model = ProductDataService.GetPhoto(photoId);

                    if (model == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model);
                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public ActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.Description))
                {
                    ModelState.AddModelError(nameof(data.Description), "Mô tả ảnh mặt hàng không được để trống");
                }
                if (data.DisplayOrder == 0)
                {
                    ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị hình ảnh không được để trống");
                }

                else if (data.DisplayOrder < 1)
                {
                    ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị hình ảnh phải là một số tự nhiên dương");
                }

                List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(data.ProductID);
                bool isUsedDisplayOrder = false;

                foreach (ProductPhoto item in productPhotos)
                {
                    if (item.DisplayOrder == data.DisplayOrder && data.PhotoID != item.PhotoID)
                    {
                        isUsedDisplayOrder = true;
                        break;
                    }
                }
                if (isUsedDisplayOrder)
                {
                    ModelState.AddModelError("DisplayOrder",
                        $"Thứ tự hiển thị {data.DisplayOrder} của hình ảnh đã được sử dụng trước đó");
                }


                // data.IsHidden = Convert.ToBoolean(data.IsHidden.ToString());
                // xử lý nghiệp vụ upload file
                if (uploadPhoto != null)
                {
                    //Tên file sẽ lưu trên server
                    /* string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; /*//*/Tên file sẽ lưu trên server*/

                    string fileName = $"{data.ProductID}_{uploadPhoto.FileName}";//Đường dẫn đến file sẽ lưu trên server 
                    string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                    //Lưu file lên server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    //Gán tên file ảnh cho model.Photo
                    data.Photo = fileName;
                }
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh mặt hàng" : "Thay đổi ảnh mặt hàng";
                    return View("Photo", data);
                }

                // thực hiện thêm hoặc cập nhật
                if (data.PhotoID == 0)
                {
                    ProductDataService.AddPhoto(data);
                }
                else
                {
                    ProductDataService.UpdatePhoto(data);
                }

                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút");//ex.Message);
                return View("Edit", data);
            }
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            ProductAttribute model = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính mặt hàng";
                    model = new ProductAttribute()
                    {
                        AttributeID = 0,
                        ProductID = id,
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính mặt hàng";
                    if (attributeId < 0)
                    {
                        return RedirectToAction("Index");
                    }
                    model = ProductDataService.GetAttribute(attributeId);
                    if (model == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public ActionResult SaveAttribute(ProductAttribute data)
        {
            // kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.AttributeName))
            {
                ModelState.AddModelError("AttributeName", "Tên thuộc tính không được để trống");
            }

            if (string.IsNullOrWhiteSpace(data.AttributeValue))
            {
                ModelState.AddModelError("AttributeValue", "Giá trị thuộc tính không được để trống");
            }

            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị thuộc tính không được để trống");
            }
            else if (data.DisplayOrder < 1)
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị thuộc tính phải là một số tự nhiên dương");
            }

            List<ProductAttribute> productAttributes = ProductDataService.ListAttributes(data.ProductID);
            bool isUsedDisplayOrder = false;
            foreach (ProductAttribute item in productAttributes)
            {
                if (item.DisplayOrder == data.DisplayOrder && data.AttributeID != item.AttributeID)
                {
                    isUsedDisplayOrder = true;
                    break;
                }
            }
            if (isUsedDisplayOrder)
            {
                ModelState.AddModelError("DisplayOrder",
                        $"Thứ tự hiển thị {data.DisplayOrder} của thuộc tính đã được sử dụng trước đó");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                return View("Attribute", data);
            }

            // thực hiện thêm hoặc cập nhật
            if (data.AttributeID == 0)
            {
                ProductDataService.AddAtrribute(data);
            }
            else
            {
                ProductDataService.UpdateAttribute(data);
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
    }
}
