using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface IKeysService
    {
        List<Key<object>> Get();
        object GetByKeyName<T>(string keyName);
        int Insert(KeysAddRequest data);
        void Update(KeysAddRequest data);
        List<DataType> GetDataTypes();
    }
}
