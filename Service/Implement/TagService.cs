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
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TagResponseDto>> GetAllTagsAsync()
        {
            var tags = await _unitOfWork.Repository<Tag>().GetAllAsync();

            return tags.Select(t => new TagResponseDto
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Note = t.Note
            }).ToList();
        }

        public async Task<TagResponseDto> GetTagByIdAsync(Guid tagId)
        {
            var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(tagId);

            if (tag == null)
                return null;

            return new TagResponseDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };
        }

        public async Task<TagResponseDto> CreateTagAsync(TagRequest request)
        {
            // Check if tag with same name already exists
            var existingTag = await _unitOfWork.Repository<Tag>()
                .FindAsync(t => t.TagName.ToLower() == request.TagName.ToLower());

            if (existingTag.Any())
                throw new ArgumentException("A tag with this name already exists");

            var tag = new Tag
            {
                TagId = Guid.NewGuid(),
                TagName = request.TagName,
                Note = request.Note
            };

            await _unitOfWork.Repository<Tag>().AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            return new TagResponseDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };
        }

        public async Task<TagResponseDto> UpdateTagAsync(UpdateTagRequest request)
        {
            var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(request.TagId);

            if (tag == null)
                throw new KeyNotFoundException("Tag not found");

            // Check if another tag with the same name exists
            var existingTag = await _unitOfWork.Repository<Tag>()
                .FindAsync(t => t.TagName.ToLower() == request.TagName.ToLower()
                    && t.TagId != request.TagId);

            if (existingTag.Any())
                throw new ArgumentException("A tag with this name already exists");

            tag.TagName = request.TagName;
            tag.Note = request.Note;

            _unitOfWork.Repository<Tag>().Update(tag);
            await _unitOfWork.SaveChangesAsync();

            return new TagResponseDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };
        }

        public async Task<bool> DeleteTagAsync(Guid tagId)
        {
            var tag = await _unitOfWork.Repository<Tag>().GetByIdAsync(tagId);

            if (tag == null)
                throw new KeyNotFoundException("Tag not found");

            // Tags can be deleted even if associated with news articles
            // The many-to-many relationship will be handled by EF Core

            _unitOfWork.Repository<Tag>().Delete(tag);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
