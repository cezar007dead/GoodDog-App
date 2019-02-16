using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Keys
{
    public class KeysAddRequest
    {
        public string KeyName { get; set; }

        public string Value { get; set; }

        public int DataTypeId { get; set; }

        public bool IsSecured { get; set; }

        public string ModifiedBy { get; set; }
    }
}
