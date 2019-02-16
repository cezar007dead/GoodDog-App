using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain
{
    public class Key<T>
    {
        public int Id { get; set; }

        public string KeyName { get; set; }

        public T Value { get; set; }

        public DataType DataType { get; set; }

        public bool IsSecured { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public string ModifiedBy { get; set; }

    }
}
