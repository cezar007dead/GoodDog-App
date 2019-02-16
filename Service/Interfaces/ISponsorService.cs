using System.Collections.Generic;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;

namespace Sabio.Services
{
    public interface ISponsorService
    {
        void Delete(int id);
        List<Sponsor> Get();
        Sponsor Get(int id);
        Paged<Sponsor> Get(int pageIndex, int pageSize);
        Paged<Sponsor> Get(int pageIndex, int pageSize, int typeId);
        int Insert(SponsorAddRequest data, int userId);
        void Update(SponsorUpdateRequest data);
        List<SponsorType> GetTypes();
    }
}