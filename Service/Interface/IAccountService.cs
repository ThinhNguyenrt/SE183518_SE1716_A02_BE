using Repository.Requests;
using Repository.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountResponse>> GetAllAccountsAsync();
        Task<AccountDetailResponseDto> GetAccountByIdAsync(Guid accountId);
        Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request);
        Task<AccountResponse> UpdateAccountAsync(UpdateAccountRequest request);
        Task<bool> DeleteAccountAsync(Guid accountId);

        Task<IEnumerable<AccountResponse>> GetAccountsByRoleAsync(int roleId);
        Task<bool> CanDeleteAccountAsync(Guid accountId);
    }
}
