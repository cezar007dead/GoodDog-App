using Sabio.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sabio.Services.Interfaces
{
    public interface IFilesService
    {
        Task<AppFile> UploadFileAsync(HttpPostedFile file);
        DataFile Get(int id);

    }
}
