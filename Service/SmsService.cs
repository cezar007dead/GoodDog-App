using Sabio.Data.Providers;
using Sabio.Models.Requests.Sms;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Twilio;
using Twilio.AspNet.Mvc;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Sabio.Services
{
    public class SmsService : ISmsService
    {
        private string accountsid;
        private string authtoken;
        private string serverPhoneNumber;

        private IKeysService _keysService;
        private IDataProvider _dataProvider;

        public SmsService(IDataProvider dataProvider, IKeysService keysService)
        {
            _keysService = keysService;
            _dataProvider = dataProvider;
            accountsid = Convert.ToString(_keysService.GetByKeyName<string>("Account.Sid"));
            authtoken = Convert.ToString(_keysService.GetByKeyName<string>("AuthToken"));
            serverPhoneNumber = Convert.ToString(_keysService.GetByKeyName<string>("ServerPhoneNumber"));

        }
        public string Send(SmsSendRequest data)
        {
            try
            {
                TwilioClient.Init(accountsid, authtoken);
                PhoneNumber to = new PhoneNumber(data.PhoneNumber);
                PhoneNumber from = new PhoneNumber(serverPhoneNumber);

                MessageResource messege = MessageResource.Create(
                    to: to,
                    from: from,
                    body: data.Messege);
                return "Succsess";
            }
            catch (Twilio.Exceptions.ApiException e)
            {
                throw new Exception(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
