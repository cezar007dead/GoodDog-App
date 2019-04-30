using Sabio.Models.Domain;
using Sabio.Models.Requests.Keys;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/keysdev")]
    public class KeysAllApiController : ApiController
    {
        private IKeysService _service;
        private IAuthenticationService<int> _auth;

        public KeysAllApiController(IKeysService sponsorService, IAuthenticationService<int> auth)
        {
            _service = sponsorService;
            _auth = auth;
        }

        [Route, HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetStrByName(string keyname)
        {
            object result = _service.GetByKeyName<Key<object>>(keyname);
            ItemResponse<Key<object>> responseBody = new ItemResponse<Key<object>>();
            Key<object> key = result as Key<object>;
            responseBody.Item = key;
            HttpStatusCode statusCode = HttpStatusCode.OK;
            if (responseBody.Item == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            if (responseBody.Item.IsSecured == true)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            if (responseBody.Item == null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode, responseBody);
        }

    }
}
