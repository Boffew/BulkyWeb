using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
	{
		/*Unit of work is not required but this opproach this cleaner for sure but if the unit of work class has more
	   Controllers inside it it will create all of their repositories and it can make the program runs slower*/
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();


			return View(products);
		}
		public IActionResult Upsert(int? id)
		{
			//This is call projection, means that use the select() method to convert an object to another object and select the fields you want
			//i'm retreiving the list of categories to display in the drop downlist
			IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});
			ProductVM productVM = new ProductVM()
			{
				CategoryList = categoryList,
				Product = new Product()

			};
			if (id == null || id == 0)
			{
				//Create
				return View(productVM);
			}
			else
			{
				//Update
				//Update
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}

		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			//when creating a product the modelstate of Categorylist will be false because we chose an item in the list not the whole list to submit so....
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");
					//this is for the update
					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						//Delete the old image
						var oldImagePath =
							Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}
					using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}
				if (productVM.Product.Id == 0)
				{
					_unitOfWork.Product.Add(productVM.Product);
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);
				}
				_unitOfWork.Save();
				TempData["success"] = "Product created successfully";
				return RedirectToAction("Index", "Product");
			}
			else
			{
				//when redirect the create view still need the dropdown items retrieved from the database so....
				IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});

				productVM.CategoryList = categoryList;

				return View(productVM);
			}

		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Product? productFromDB = _unitOfWork.Product.Get(c => c.Id == id);
			if (productFromDB == null)
			{
				return NotFound();
			}
			return View(productFromDB);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			Product productFromDB = _unitOfWork.Product.Get(c => c.Id == id);
			if (productFromDB == null)
			{
				return NotFound();
			}
			_unitOfWork.Product.Remove(productFromDB);
			_unitOfWork.Save();
			//Temp data will pass data to the next redirected page and it will only available 1 time
			TempData["success"] = $"Product with id {id} deleted successfully";
			return RedirectToAction("Index");
		}
	}
}
