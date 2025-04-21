using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;



        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region GetAll
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _unitOfWork.GenericRepository<Product>().GetAllAsync();
                var productVMs = products.Select(ProductVM.FromProduct);
                return View(productVMs);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading products: " + ex.Message;
                return View(new List<ProductVM>());
            }
        }
        #endregion

        #region Add
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                var categories = await _unitOfWork.GenericRepository<Category>().GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading categories: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductVM productVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var product = ProductVM.FromProductVM(productVM);
                    await _unitOfWork.GenericRepository<Product>().AddAsync(product);
                    await _unitOfWork.CompleteAsync();
                    TempData["SuccessMessage"] = "Product added successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error adding product: " + ex.Message);
            }

            // If we got this far, something failed; redisplay form
            var categories = await _unitOfWork.GenericRepository<Category>().GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", productVM.CategoryId);
            return View(productVM);
        }
        #endregion

        #region GetById
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);

                if (product == null)
                {
                    return NotFound();
                }
                var productVM = ProductVM.FromProduct(product);
                return View(productVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading product details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var categories = await _unitOfWork.GenericRepository<Category>().GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                
                var productVM = ProductVM.FromProduct(product);
                return View(productVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading product for editing: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVM productVM)
        {
            if (id != productVM.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var product = ProductVM.FromProductVM(productVM);
                    _unitOfWork.GenericRepository<Product>().Update(product);
                    await _unitOfWork.CompleteAsync();
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating product: " + ex.Message);
            }

            // If we got this far, something failed; redisplay form
            var categories = await _unitOfWork.GenericRepository<Category>().GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", productVM.CategoryId);
            return View(productVM);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var productVM = ProductVM.FromProduct(product);
                return View(productVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading product for deletion: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);
                if (product != null)
                {
                    _unitOfWork.GenericRepository<Product>().Delete(product);
                    await _unitOfWork.CompleteAsync();
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting product: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

    }
}