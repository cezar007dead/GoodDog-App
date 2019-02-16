using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Keys;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class KeysService : IKeysService
    {
        private IDataProvider _dataProvider;
        private MemoryCache _memoryCache;

        public KeysService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _memoryCache = MemoryCache.Default;
        }

        public int Insert(KeysAddRequest data)
        {
            int keyId = 0;
            string storeProc = "[dbo].[Keys_Insert]";
            _dataProvider.ExecuteNonQuery(storeProc
                , delegate (SqlParameterCollection sqlParams)
                {
                    sqlParams.AddWithValue("@KeyName", data.KeyName);
                    sqlParams.AddWithValue("@Value", data.Value);
                    sqlParams.AddWithValue("@DataTypeId", data.DataTypeId);
                    sqlParams.AddWithValue("@IsSecured", data.IsSecured);
                    sqlParams.AddWithValue("@ModifiedBy", data.ModifiedBy);

                    SqlParameter idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    idParameter.Direction = System.Data.ParameterDirection.Output;

                    sqlParams.Add(idParameter);

                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    Int32.TryParse(param["@Id"].Value.ToString(), out keyId);
                }
                );
            return keyId;
        }
        public void Update(KeysAddRequest data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("A parameter data is required!");
            }
            string storeProc = "[dbo].[Keys_Update]";
            _dataProvider.ExecuteNonQuery(storeProc
                , delegate (SqlParameterCollection sqlParams)
                {
                    sqlParams.AddWithValue("@KeyName", data.KeyName);
                    sqlParams.AddWithValue("@Value", data.Value);
                    sqlParams.AddWithValue("@DataTypeId", data.DataTypeId);
                    sqlParams.AddWithValue("@IsSecured", data.IsSecured);
                    sqlParams.AddWithValue("@ModifiedBy", data.ModifiedBy);
                });
        }
        public List<DataType> GetDataTypes()
        {
            List<DataType> list = null;
            string prokName = "[dbo].[DataTypeSelectAll]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  //  this is where your input params go. it works the same way as with ExecuteNonQuery. 
                  //  This proc does not have any input parameters specified so we can leave this commented out
                  //   OR we could have passed null as the parameter value 
              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  DataType type = new DataType();
                  int startingIndex = 0;
                  type.Id = reader.GetSafeInt32(startingIndex++);
                  type.DisplayName = reader.GetSafeString(startingIndex++);
                  type.Description = reader.GetSafeString(startingIndex++);

                  if (list == null)
                  {
                      list = new List<DataType>();
                  }

                  list.Add(type);
              }
              );
            return list;
        }

        public List<Key<object>> Get()
        {
            List<Key<object>> list = null;
            string prokName = "[dbo].[Keys_SelectAll]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  //  this is where your input params go. it works the same way as with ExecuteNonQuery. 
                  //  This proc does not have any input parameters specified so we can leave this commented out
                  //   OR we could have passed null as the parameter value 
              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  Key<object> sponsor = MapperForAll(reader);

                  if (list == null)
                  {
                      list = new List<Key<object>>();
                  }

                  list.Add(sponsor);
              }
              );
            return list;
        }

        public object GetByKeyName<T>(string keyName)
        {
            Key<string> key = new Key<string>();

            object cacheKey = _memoryCache.Get(keyName);


            if (cacheKey == null)
            {
                string prokName = "[dbo].[Keys_SelectByKeyName]";
                _dataProvider.ExecuteCmd(prokName
                  , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                  {
                      paramCollection.AddWithValue("@KeyName", keyName);
                  }
                  , singleRecordMapper: delegate (IDataReader reader, short set)
                  {
                      key = Mapper(reader);
                  }
                  );
                if (key.KeyName == null)
                {
                    return null;
                }
                AddKeyToCache(keyName, key);
                try
                {
                    if (new Int32() is T)
                    {
                        AddKeyToCache(keyName, key);
                        AddKeyToCache(keyName + "_Val", key.Value);
                        return Convert.ToInt32(key.Value);
                    }
                    if (new Int64() is T)
                    {
                        AddKeyToCache(keyName, key);
                        AddKeyToCache(keyName + "_Val", key.Value);
                        return Convert.ToInt64(key.Value);
                    }
                    if (new bool() is T)
                    {
                        AddKeyToCache(keyName, key);
                        AddKeyToCache(keyName + "_Val", key.Value);
                        return Convert.ToBoolean(key.Value);
                    }
                    if (typeof(String) == typeof(T))
                    {
                        AddKeyToCache(keyName, key);
                        AddKeyToCache(keyName + "_Val", key.Value);
                        return key.Value;
                    }
                }
                catch
                {
                    return null;
                }


                AddKeyToCache(keyName, key);
                AddKeyToCache(keyName + "_Val", key.Value);

                Key<object> result = new Key<object>();
                result.Id = key.Id;
                result.KeyName = key.KeyName;
                result.DataType = key.DataType;
                result.IsSecured = key.IsSecured;
                result.DateCreated = key.DateModified;
                result.DateModified = key.DateModified;
                result.ModifiedBy = key.ModifiedBy;

                string value = key.Value;
                string type = key.DataType.DisplayName;
                switch (type)
                {
                    case "numeric":
                        UInt64 number;
                        if (UInt64.TryParse(value, out number))
                            result.Value = Convert.ToInt64(value);
                        else
                            result.Value = null; break;
                    case "boolean":
                        bool flag;
                        if (bool.TryParse(value, out flag))
                            result.Value = Convert.ToBoolean(value);
                        else
                            result.Value = null; break;
                    case "string": result.Value = Convert.ToString(value); break;
                }

                return result;
            }
            else
            {
                try
                {
                    if (new Int32() is T)
                    {
                        object cacheKeyValue = _memoryCache.Get(keyName + "_Val");
                        return Convert.ToInt32(cacheKeyValue);
                    }
                    if (new Int64() is T)
                    {
                        object cacheKeyValue = _memoryCache.Get(keyName + "_Val");
                        return Convert.ToInt64(cacheKeyValue);
                    }
                    if (new bool() is T)
                    {
                        object cacheKeyValue = _memoryCache.Get(keyName + "_Val");
                        return Convert.ToBoolean(cacheKeyValue);
                    }
                    if (typeof(String) == typeof(T))
                    {
                        object cacheKeyValue = _memoryCache.Get(keyName + "_Val");
                        return cacheKeyValue;
                    }
                }
                catch
                {
                    return null;
                }
                Key<string> copy = cacheKey as Key<string>;

                Key<object> result = new Key<object>();
                result.Id = copy.Id;
                result.KeyName = copy.KeyName;
                result.DataType = copy.DataType;
                result.IsSecured = copy.IsSecured;
                result.DateCreated = copy.DateModified;
                result.DateModified = copy.DateModified;
                result.ModifiedBy = copy.ModifiedBy;

                string value = copy.Value;
                string type = copy.DataType.DisplayName;
                switch (type)
                {
                    case "numeric":
                        UInt64 number;
                        if (UInt64.TryParse(value, out number))
                            result.Value = Convert.ToInt64(value);
                        else
                            result.Value = null; break;
                    case "boolean":
                        bool flag;
                        if (bool.TryParse(value, out flag))
                            result.Value = Convert.ToBoolean(value);
                        else
                            key.Value = null; break;
                    case "string": result.Value = Convert.ToString(value); break;
                }
                Console.WriteLine();
                return result;
            }
        }

        private bool AddKeyToCache(string key, object data)
        {
            return _memoryCache.Add(key, data, DateTimeOffset.UtcNow.AddDays(1));
        }

        public static void Delete(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }
        private static Key<string> Mapper(IDataReader reader)
        {
            Key<string> key = new Key<string>();
            key.DataType = new DataType();
            int startingIndex = 0;
            key.Id = reader.GetSafeInt32(startingIndex++);
            key.KeyName = reader.GetSafeString(startingIndex++);
            key.Value = reader.GetSafeString(startingIndex++);
            key.DataType.Id = reader.GetSafeInt32(startingIndex++);
            key.DataType.DisplayName = reader.GetSafeString(startingIndex++);
            key.DataType.Description = reader.GetSafeString(startingIndex++);
            key.IsSecured = reader.GetSafeBool(startingIndex++);
            key.DateCreated = reader.GetSafeDateTime(startingIndex++);
            key.DateModified = reader.GetSafeDateTime(startingIndex++);
            key.ModifiedBy = reader.GetSafeString(startingIndex++);
            return key;
        }

        private static Key<object> MapperForAll(IDataReader reader)
        {
            Key<object> key = new Key<object>();
            key.DataType = new DataType();
            int startingIndex = 0;
            key.Id = reader.GetSafeInt32(startingIndex++);
            key.KeyName = reader.GetSafeString(startingIndex++);
            string val = reader.GetSafeString(startingIndex++);
            key.DataType.Id = reader.GetSafeInt32(startingIndex++);
            key.DataType.DisplayName = reader.GetSafeString(startingIndex++);
            key.DataType.Description = reader.GetSafeString(startingIndex++);
            key.IsSecured = reader.GetSafeBool(startingIndex++);
            key.DateCreated = reader.GetSafeDateTime(startingIndex++);
            key.DateModified = reader.GetSafeDateTime(startingIndex++);
            key.ModifiedBy = reader.GetSafeString(startingIndex++);

            string value = val;
            string type = key.DataType.DisplayName;
            switch (type)
            {
                case "numeric":
                    UInt64 number;
                    if (UInt64.TryParse(value, out number))
                        key.Value = Convert.ToInt64(value);
                    else
                        key.Value = null; break;
                case "boolean":
                    bool flag;
                    if (bool.TryParse(value, out flag))
                        key.Value = Convert.ToBoolean(value);
                    else
                        key.Value = null; break;
                case "string": key.Value = Convert.ToString(value); break;
            }

            return key;
        }
    }
}

