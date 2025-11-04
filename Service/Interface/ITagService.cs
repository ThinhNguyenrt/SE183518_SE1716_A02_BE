using Repository.Requests;
using Repository.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ITagService
    {
        Task<IEnumerable<TagResponseDto>> GetAllTagsAsync();
        Task<TagResponseDto> GetTagByIdAsync(Guid tagId);
        Task<TagResponseDto> CreateTagAsync(TagRequest request);
        Task<TagResponseDto> UpdateTagAsync(UpdateTagRequest request);
        Task<bool> DeleteTagAsync(Guid tagId);
    }
}
