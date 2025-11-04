using Microsoft.AspNetCore.Identity;
using Repository.Models;
using Repository.Repository.Interface;
using Repository.Requests;
using Repository.Responses;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordHasher<SystemAccount> _passwordHasher;
        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = new PasswordHasher<SystemAccount>();
        }
        public async Task<IEnumerable<AccountResponse>> GetAllAccountsAsync()
        {
            var accounts = await _unitOfWork.Repository<SystemAccount>().GetAllAsync();
            var newsArticles = await _unitOfWork.Repository<NewsArticle>().GetAllAsync();

            return accounts.Select(a => MapToResponseDto(a, newsArticles)).ToList();
        }

        public async Task<AccountDetailResponseDto> GetAccountByIdAsync(Guid accountId)
        {
            var account = await _unitOfWork.Repository<SystemAccount>().GetByIdAsync(accountId);

            if (account == null)
                return null;

            var newsArticles = await _unitOfWork.Repository<NewsArticle>().GetAllAsync();
            var createdCount = newsArticles.Count(n => n.CreatedById == accountId);
            var updatedCount = newsArticles.Count(n => n.UpdatedById == accountId);

            return new AccountDetailResponseDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                RoleName = GetRoleName(account.AccountRole),
                CreatedNewsArticlesCount = createdCount,
                UpdatedNewsArticlesCount = updatedCount,
                CanDelete = createdCount == 0,
                CreatedDate = DateTime.Now, // Add these fields to SystemAccount if needed
                LastModifiedDate = null,
                LastLoginDate = null
            };
        }

        public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request)
        {
            // Check if email already exists
            var existingAccount = await _unitOfWork.Repository<SystemAccount>()
                .FindAsync(a => a.AccountEmail == request.AccountEmail);

            if (existingAccount.Any())
                throw new ArgumentException("An account with this email already exists");

            var account = new SystemAccount
            {
                AccountId = Guid.NewGuid(),
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountRole = request.AccountRole
            };

            // Hash password
            account.AccountPassword = _passwordHasher.HashPassword(account, request.AccountPassword);

            await _unitOfWork.Repository<SystemAccount>().AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return new AccountResponse
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                RoleName = GetRoleName(account.AccountRole),
                CreatedNewsArticlesCount = 0,
                UpdatedNewsArticlesCount = 0,
                CanDelete = true
            };
        }

        public async Task<AccountResponse> UpdateAccountAsync(UpdateAccountRequest request)
        {
            var account = await _unitOfWork.Repository<SystemAccount>().GetByIdAsync(request.AccountId);

            if (account == null)
                throw new KeyNotFoundException("Account not found");

            // Check if email is being changed and if new email already exists
            if (account.AccountEmail != request.AccountEmail)
            {
                var existingAccount = await _unitOfWork.Repository<SystemAccount>()
                    .FindAsync(a => a.AccountEmail == request.AccountEmail && a.AccountId != request.AccountId);

                if (existingAccount.Any())
                    throw new ArgumentException("An account with this email already exists");
            }

            // Update account information
            account.AccountName = request.AccountName;
            account.AccountEmail = request.AccountEmail;
            account.AccountRole = request.AccountRole;

            _unitOfWork.Repository<SystemAccount>().Update(account);
            await _unitOfWork.SaveChangesAsync();

            // Get news articles count
            var newsArticles = await _unitOfWork.Repository<NewsArticle>().GetAllAsync();
            var createdCount = newsArticles.Count(n => n.CreatedById == account.AccountId);
            var updatedCount = newsArticles.Count(n => n.UpdatedById == account.AccountId);

            return new AccountResponse
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                RoleName = GetRoleName(account.AccountRole),
                CreatedNewsArticlesCount = createdCount,
                UpdatedNewsArticlesCount = updatedCount,
                CanDelete = createdCount == 0
            };
        }

        public async Task<bool> DeleteAccountAsync(Guid accountId)
        {
            var account = await _unitOfWork.Repository<SystemAccount>().GetByIdAsync(accountId);

            if (account == null)
                throw new KeyNotFoundException("Account not found");

            // Check if account has created any news articles
            var hasCreatedArticles = await _unitOfWork.Repository<NewsArticle>()
                .ExistsAsync(n => n.CreatedById == accountId);

            if (hasCreatedArticles)
                throw new InvalidOperationException(
                    "Cannot delete account that has created news articles. " +
                    "Please reassign or delete the articles first.");

            // Optional: Update articles where this account is the updater
            var updatedArticles = await _unitOfWork.Repository<NewsArticle>()
                .FindAsync(n => n.UpdatedById == accountId);

            foreach (var article in updatedArticles)
            {
                article.UpdatedById = null;
                article.ModifiedDate = null;
                _unitOfWork.Repository<NewsArticle>().Update(article);
            }

            _unitOfWork.Repository<SystemAccount>().Delete(account);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<AccountResponse>> GetAccountsByRoleAsync(int roleId)
        {
            var accounts = await _unitOfWork.Repository<SystemAccount>()
                .FindAsync(a => a.AccountRole == roleId);

            var newsArticles = await _unitOfWork.Repository<NewsArticle>().GetAllAsync();

            return accounts.Select(a => MapToResponseDto(a, newsArticles)).ToList();
        }

        public async Task<bool> CanDeleteAccountAsync(Guid accountId)
        {
            var hasCreatedArticles = await _unitOfWork.Repository<NewsArticle>()
                .ExistsAsync(n => n.CreatedById == accountId);

            return !hasCreatedArticles;
        }

        private AccountResponse MapToResponseDto(SystemAccount account, IEnumerable<NewsArticle> newsArticles)
        {
            var createdCount = newsArticles.Count(n => n.CreatedById == account.AccountId);
            var updatedCount = newsArticles.Count(n => n.UpdatedById == account.AccountId);

            return new AccountResponse
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                RoleName = GetRoleName(account.AccountRole),
                CreatedNewsArticlesCount = createdCount,
                UpdatedNewsArticlesCount = updatedCount,
                CanDelete = createdCount == 0
            };
        }

        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Staff",
                2 => "Lecturer",
                3 => "Admin",
                _ => "Unknown"
            };
        }
    }
}
