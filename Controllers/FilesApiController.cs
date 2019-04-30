using Sabio.Models.Domain;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{

    [Authorize(Roles = "Admin, User, Premium")]
    [RoutePrefix("api/files")]
    public class FilesApiController : BaseApiController
    {
        private IFilesService _service;
        private IAuthenticationService<int> _auth;

        public FilesApiController(IFilesService sponsorService, IAuthenticationService<int> auth)
        {
            _service = sponsorService;
            _auth = auth;
        }


        [Route, HttpPost]
        public async Task<HttpResponseMessage> Add()
        {
            ItemsResponse<AppFile> responseBody = new ItemsResponse<AppFile>();
            responseBody.Items = new List<AppFile>();
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    responseBody.Items.Add(await _service.UploadFileAsync(HttpContext.Current.Request.Files[i]));
                }
            }
            if (responseBody.Items.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, responseBody);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, responseBody);
            }
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            ItemResponse<DataFile> responseBody = new ItemResponse<DataFile>();
            HttpStatusCode statusCode = HttpStatusCode.OK;
            responseBody.Item = _service.Get(id);
            if (responseBody.Item == null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode, responseBody);
        }
    }
}
