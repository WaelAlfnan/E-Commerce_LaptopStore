using LapStore.BLL.DTOs;
using LapStore.BLL.DTOs.ProductDTO;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IProductService productService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDTOs = products.Select(ProductReadDTO.FromProduct).ToList();
            return Ok(productDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productDTO = ProductReadDTO.FromProduct(product);
            return Ok(productDTO);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] ProductWriteDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if product name already exists
            if (await _productService.IsProductNameExistAsync(productDTO.Name))
            {
                ModelState.AddModelError("Name", "Product name already exists");
                return BadRequest(ModelState);
            }

            var product = ProductWriteDTO.FromProductDTO(productDTO);
            await _productService.AddAsync(product);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, ProductReadDTO.FromProduct(product));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductUpdateDTO productDTO)
        {
            if (id != productDTO.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Check if the updated name conflicts with another product
            if (productDTO.Name != existingProduct.Name &&
                await _productService.IsProductNameExistAsync(productDTO.Name))
            {
                ModelState.AddModelError("Name", "Product name already exists");
                return BadRequest(ModelState);
            }

            var product = ProductUpdateDTO.FromProductDTO(productDTO);

            // Preserve existing images
            product.productImages = existingProduct.productImages;

            await _productService.UpdateAsync(product);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteAsync(product);
            return NoContent();
        }

        // Image related endpoints
        [Authorize]
        [HttpPost("{productId}/images")]
        public async Task<IActionResult> AddImage(int productId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            // Add the image
            var productImage = await _productService.AddProductImageAsync(productId, file);

            return CreatedAtAction(
                nameof(GetById),
                new { id = productId },
                new
                {
                    id = productImage.Id,
                    url = productImage.URL,
                    isMain = productImage.IsMain,
                    productId = productImage.ProductId
                });
        }

        [HttpGet("{productId}/images")]
        public async Task<IActionResult> GetProductImages(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var images = await _productService.GetProductImagesAsync(productId);
            var imagesDto = images.Select(i => new {
                id = i.Id,
                url = i.URL,
                isMain = i.IsMain,
                productId = i.ProductId
            });

            return Ok(imagesDto);
        }

        [Authorize]
        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _productService.GetImageByIdAsync(imageId);
            if (image == null)
            {
                return NotFound("Image not found");
            }

            await _productService.RemoveProductImageAsync(imageId);
            return NoContent();
        }

        [Authorize]
        [HttpPut("{productId}/images/{imageId}/main")]
        public async Task<IActionResult> SetMainImage(int productId, int imageId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            var image = await _productService.GetImageByIdAsync(imageId);
            if (image == null || image.ProductId != productId)
            {
                return NotFound("Image not found for this product");
            }

            await _productService.SetMainProductImageAsync(productId, imageId);
            return NoContent();
        }
    }
}