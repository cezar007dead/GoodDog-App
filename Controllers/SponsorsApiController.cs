using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api.Tests
{

    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/sponsors")]
    public class SponsorsApiController : BaseApiController
    {
        private ISponsorService _service;
        private IAuthenticationService<int> _auth;

        public SponsorsApiController(ISponsorService sponsorService, IAuthenticationService<int> auth)
        {
            _service = sponsorService;
            _auth = auth;
        }

        [Route, HttpPost]
        public HttpResponseMessage Add(SponsorAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return CreateErrorResponse();
            }
            ItemResponse<int> responseBody = new ItemResponse<int>();
            int userId = _auth.GetCurrentUserId();
            responseBody.Item = _service.Insert(model, userId);
            return Request.CreateResponse(HttpStatusCode.Created, responseBody);
        }

        [Route, HttpGet]
        public HttpResponseMessage GetAll()
        {
            ItemsResponse<Sponsor> responseBody = new ItemsResponse<Sponsor>();
            responseBody.Items = _service.Get();
            HttpStatusCode statusCode = HttpStatusCode.OK;
            if (responseBody.Items == null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode, responseBody);

        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            ItemResponse<Sponsor> responseBody = new ItemResponse<Sponsor>();
            responseBody.Item = _service.Get(id);
            HttpStatusCode statusCode = HttpStatusCode.OK;
            if (responseBody.Item == null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode, responseBody);
        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(SponsorUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return CreateErrorResponse();
            }
            _service.Update(model);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            _service.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, new SuccessResponse());
        }

        [Route("{pageIndex:int}/{pageSize:int}"), HttpGet]
        public HttpResponseMessage Get(int pageIndex, int pageSize)
        {
            ItemResponse<Paged<Sponsor>> responseBody = new ItemResponse<Paged<Sponsor>>();
            responseBody.Item = _service.Get(pageIndex, pageSize);
            HttpStatusCode statusCode = HttpStatusCode.OK;
            if (responseBody.Item == null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode, responseBody);
        }

        [Route(), HttpGet]
        public HttpResponseMessage Get(int pageIndex, int pageSize, int typeId)
        {
            ItemResponse<Paged<Sponsor>> responseBody = new ItemResponse<Paged<Sponsor>>();
            responseBody.Item = _service.Get(pageIndex, pageSize, typeId);
            HttpStatusCode statusCode = HttpStatusCode.OK;
            if (responseBody.Item == null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode, responseBody);
        }

        [Route("types"), HttpGet]
        public HttpResponseMessage GetTypes()
        {
            ItemsResponse<SponsorType> responseBody = new ItemsResponse<SponsorType>();
            responseBody.Items = _service.GetTypes();
            HttpStatusCode statusCode = HttpStatusCode.OK;
            if (responseBody.Items == null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode, responseBody);
        }
    }
}
