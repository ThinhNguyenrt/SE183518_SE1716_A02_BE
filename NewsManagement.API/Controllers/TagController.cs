using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Requests;
using Repository.Responses;
using Service.Interface;

namespace NewsManagement.API.Controllers
{
    [Route("api/tags")]
    [ApiController]
    [Authorize(Policy = "StaffOnly")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tags = await _tagService.GetAllTagsAsync();
                return Ok(ApiResponse<IEnumerable<TagResponseDto>>.Success(tags, "Fetched all tags successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        // GET: api/tags/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var tag = await _tagService.GetTagByIdAsync(id);
                if (tag == null)
                    return NotFound(ApiResponse<string>.Fail("Tag not found", 404));

                return Ok(ApiResponse<TagResponseDto>.Success(tag, "Fetched tag successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        // POST: api/tags
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.Fail("Invalid input data", 400));

                var createdTag = await _tagService.CreateTagAsync(request);
                return Ok(ApiResponse<TagResponseDto>.Success(createdTag, "Tag created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        // PUT: api/tags
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTagRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.Fail("Invalid input data", 400));

                var updatedTag = await _tagService.UpdateTagAsync(request);
                return Ok(ApiResponse<TagResponseDto>.Success(updatedTag, "Tag updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }

        // DELETE: api/tags/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _tagService.DeleteTagAsync(id);
                return Ok(ApiResponse<string>.Success("Tag deleted successfully", "Success", 200));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.Fail(ex.Message, 404));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail(ex.Message, 500));
            }
        }
    }
}
