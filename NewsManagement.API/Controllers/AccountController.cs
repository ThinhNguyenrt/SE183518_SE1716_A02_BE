using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Requests;
using Service.Interface;

namespace NewsManagement.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Get all accounts - Admin only
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return Ok(new
                {
                    success = true,
                    data = accounts,
                    message = "Accounts retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get account by ID - Admin only
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);

                if (account == null)
                    return NotFound(new { success = false, message = "Account not found" });

                return Ok(new { success = true, data = account });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create new account - Admin only
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState });

                var account = await _accountService.CreateAccountAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = account.AccountId },
                    new { success = true, data = account, message = "Account created successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update account - Admin only
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountRequest request)
        {
            try
            {
                if (id != request.AccountId)
                    return BadRequest(new { success = false, message = "ID mismatch" });

                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState });

                var account = await _accountService.UpdateAccountAsync(request);

                return Ok(new
                {
                    success = true,
                    data = account,
                    message = "Account updated successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete account - Admin only
        /// Note: Cannot delete if account has created any news articles
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _accountService.DeleteAccountAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Account deleted successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    hint = "This account has created news articles and cannot be deleted"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Check if account can be deleted - Admin only
        /// </summary>
        [HttpGet("{id}/can-delete")]
        public async Task<IActionResult> CanDelete(Guid id)
        {
            try
            {
                var canDelete = await _accountService.CanDeleteAccountAsync(id);

                return Ok(new
                {
                    success = true,
                    canDelete = canDelete,
                    message = canDelete
                        ? "Account can be deleted"
                        : "Account cannot be deleted - has created news articles"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get accounts by role - Admin only
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetByRole(int roleId)
        {
            try
            {
                if (roleId < 1 || roleId > 3)
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid role. Role must be 1 (Staff), 2 (Lecturer), or 3 (Admin)"
                    });

                var accounts = await _accountService.GetAccountsByRoleAsync(roleId);

                return Ok(new
                {
                    success = true,
                    data = accounts,
                  
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
