using Sabio.Models.Requests.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface ISmsService
    {
        string Send(SmsSendRequest data);
    }
}
