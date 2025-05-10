using LapStore.BLL.DTOs;
using LapStore.BLL.Interfaces;
using LapStore.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICategoryFileHandler _fileHandler;

        public CategoriesController(
            ICategoryService categoryService,
            IFileStorageService fileStorageService,
            ICategoryFileHandler categoryFileHandler)
        {
            _categoryService = categoryService;
            _fileStorageService = fileStorageService;
            _fileHandler = categoryFileHandler;
        }

        // GET: api/Categories
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetCategoryDTO>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoryDTOs = new List<GetCategoryDTO>();

            foreach (var category in categories)
            {
                categoryDTOs.Add(GetCategoryDTO.FromCategory(category));
            }

            return Ok(categoryDTOs);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetCategoryDTO>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(GetCategoryDTO.FromCategory(category));
        }

        // POST: api/Categories
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GetCategoryDTO>> CreateCategory([FromForm] AddCategoryDTO categoryDto, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Handle file upload
                if (file != null)
                {
                    await _fileHandler.HandleAddCategoryFileUpload(categoryDto, file);
                }

                var addedCategory = await _categoryService.AddCategoryAsync(categoryDto);
                return CreatedAtAction(
                    nameof(GetCategory),
                    new { id = addedCategory.Id },
                    GetCategoryDTO.FromCategory(addedCategory));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Categories/5
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryDTO categoryDto, IFormFile file)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest("ID mismatch between route parameter and request body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Handle file upload
                if (file != null)
                {
                    await _fileHandler.HandleUpdateCategoryFileUpload(categoryDto, file);
                }

                await _categoryService.UpdateCategoryAsync(categoryDto);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Categories/5
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // GET: api/Categories/exists?name=Electronics
        [HttpGet("exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckCategoryNameExists([FromQuery] string name, [FromQuery] int? id = null)
        {
            bool exists = await _categoryService.IsCategoryNameExistAsync(name, id);
            return Ok(exists);
        }
    }
}