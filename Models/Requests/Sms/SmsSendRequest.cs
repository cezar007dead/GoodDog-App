using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Sms
{
    public class SmsSendRequest
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Messege { get; set; }
    }
}
