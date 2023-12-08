
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        /*Unit of work is not required but this opproach this cleaner for sure but if the unit of work class has more
		Controllers inside it it will create all of their repositories and it can make the program runs slower*/

        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //Custom validation
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                //the "Name" indicates the Name property of Category model, that where the error message displayed
                ModelState.AddModelError("Name", "Name and Display order can not be the same");
            }
            if (obj.Name.ToLower() == "test")
            {
                //the "" indicates the Name property of Category model, the error message only displayed in the error summary
                ModelState.AddModelError("", "Name can't be test");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _unitOfWork.Category.Get(c => c.Id == id);
            //this also can be used
            //Category? categoryFromDB = _db.Categories.Find(id)
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = $"Category with id {obj.Id} edited successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDB = _unitOfWork.Category.Get(c => c.Id == id);
            //this also can be used
            //Category? categoryFromDB = _db.Categories.Find(id)
            if (categoryFromDB == null)
            {
                return NotFound();
            }
            return View(categoryFromDB);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category categoryDB = _unitOfWork.Category.Get(c => c.Id == id);
            if (categoryDB == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(categoryDB);
            _unitOfWork.Save();
            //Temp data will pass data to the next redirected page and it will only available 1 time
            TempData["success"] = $"Category with id {id} deleted successfully";
            return RedirectToAction("Index");
        }
    }

}
