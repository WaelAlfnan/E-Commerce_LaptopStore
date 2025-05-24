using LapStore.BLL.DTOs;
using LapStore.BLL.DTOs.ProductDTO;
using LapStore.BLL.Interfaces;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Add Product with Images
        [Authorize(Roles = "Admin,Vendor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductWriteDTO productDTO, [FromForm] List<IFormFile> images)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _productService.IsProductNameExistAsync(productDTO.Name))
            {
                ModelState.AddModelError("Name", "Product name already exists");
                return BadRequest(ModelState);
            }

            var product = ProductWriteDTO.FromProductDTO(productDTO);
            await _productService.AddProductWithImagesAsync(product, images);
            var productReadDto = ProductReadDTO.FromProduct(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, productReadDto);
        }

        // Update Product with Images
        [Authorize(Roles = "Admin,Vendor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDTO productDTO, [FromForm] List<IFormFile> images)
        {
            if (id != productDTO.Id || !ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            if (productDTO.Name != existingProduct.Name &&
                await _productService.IsProductNameExistAsync(productDTO.Name))
            {
                ModelState.AddModelError("Name", "Product name already exists");
                return BadRequest(ModelState);
            }

            var product = ProductUpdateDTO.FromProductDTO(productDTO);
            await _productService.UpdateProductWithImagesAsync(product, images);
            return NoContent();
        }

        // Delete Product and Its Images
        [Authorize(Roles = "Admin,Vendor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            await _productService.DeleteProductWithImagesAsync(id);
            return NoContent();
        }

        // Get Product by ID with All Images
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var productDTO = ProductReadDTO.FromProduct(product);
            return Ok(productDTO);
        }

        // Get All Products with Main Image Only
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDTOs = products.Select(p =>
            {
                var dto = ProductReadDTO.FromProduct(p);
                if (dto.Images != null && dto.Images.Any())
                {
                    var main = dto.Images.FirstOrDefault(i => i.IsMain) ?? dto.Images.First();
                    dto.Images = new List<GetProductImageDTO> { main };
                    dto.MainImageUrl = main.URL;
                }
                return dto;
            }).ToList();

            return Ok(productDTOs);
        }
    }
}