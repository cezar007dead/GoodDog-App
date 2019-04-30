using Sabio.Models.Requests.Sms;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Twilio;
using Twilio.AspNet.Mvc;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Sabio.Web.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api/messege")]
    public class SmsApiController : BaseApiController
    {
        ISmsService _service;
        IAuthenticationService<int> _auth = null;

        public SmsApiController(ISmsService service, IAuthenticationService<int> auth)
        {
            _service = service;
            _auth = auth;
        }

        [Route("send"), HttpPost]
        public HttpResponseMessage PostSend(SmsSendRequest modal)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ItemResponse<string> responseBody = new ItemResponse<string>();
            responseBody.Item = _service.Send(modal);
            return Request.CreateResponse(HttpStatusCode.OK, responseBody);

        }

    }
}
