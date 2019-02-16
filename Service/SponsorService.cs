using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class SponsorService : ISponsorService
    {
        private IDataProvider _dataProvider;

        public SponsorService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public int Insert(SponsorAddRequest data, int userId)
        {
            int sponsorId = 0;
            string storeProc = "[dbo].[Sponsors_Insert]";
            _dataProvider.ExecuteNonQuery(storeProc
                , delegate (SqlParameterCollection sqlParams)
                {
                    sqlParams.AddWithValue("@Name", data.Name);
                    sqlParams.AddWithValue("@CompanyUrl", data.CompanyUrl.ToString());
                    sqlParams.AddWithValue("@AddressId", data.AddressId);
                    sqlParams.AddWithValue("@PhoneNumber", data.PhoneNumber);
                    sqlParams.AddWithValue("@ContactPerson", data.ContactPerson);
                    sqlParams.AddWithValue("@PrimarySponsorTypeId", data.PrimarySponsorTypeId);
                    sqlParams.AddWithValue("@UserId", userId);

                    SqlParameter idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    idParameter.Direction = System.Data.ParameterDirection.Output;

                    sqlParams.Add(idParameter);

                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    Int32.TryParse(param["@Id"].Value.ToString(), out sponsorId);
                }
                );
            return sponsorId;
        }

        public List<Sponsor> Get()
        {
            List<Sponsor> list = null;
            string prokName = "[dbo].[Sponsors_SelectAll]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  //  this is where your input params go. it works the same way as with ExecuteNonQuery. 
                  //  This proc does not have any input parameters specified so we can leave this commented out
                  //   OR we could have passed null as the parameter value 
              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  Sponsor sponsor = Mapper(reader);

                  if (list == null)
                  {
                      list = new List<Sponsor>();
                  }

                  list.Add(sponsor);
              }
              );
            return list;
        }

        public Sponsor Get(int id)
        {
            Sponsor sponsor = null;
            string prokName = "[dbo].[Sponsors_SelectById]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Id", id);
              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  sponsor = Mapper(reader);
              }
              );
            return sponsor;
        }

        public Paged<Sponsor> Get(int pageIndex, int pageSize)
        {
            Paged<Sponsor> response = null;
            List<Sponsor> list = null;
            int totalCount = 0;
            string prokName = "[dbo].[Sponsors_GetByPageIndexPageSize]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@PageNumber", pageIndex);
                  paramCollection.AddWithValue("@PageSize", pageSize);

              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  Sponsor sponsor = new Sponsor();
                  sponsor = Mapper(reader);
                  if (totalCount == 0)
                  {
                      totalCount = reader.GetSafeInt32(20);
                  }
                  if (list == null)
                  {
                      list = new List<Sponsor>();
                  }

                  list.Add(sponsor);
              }
              );
            if (list != null)
            {
                response = new Paged<Sponsor>(list, pageIndex, pageSize, totalCount);
                return response;
            }
            return response;
        }
        public Paged<Sponsor> Get(int pageIndex, int pageSize, int typeId)
        {
            Paged<Sponsor> response = null;
            List<Sponsor> list = null;
            int totalCount = 0;
            string prokName = "[dbo].[Sponsors_GetByPageIndexPageSizeType]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@PageNumber", pageIndex);
                  paramCollection.AddWithValue("@PageSize", pageSize);
                  paramCollection.AddWithValue("@TypeId", typeId);
              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  Sponsor sponsor = new Sponsor();
                  sponsor = Mapper(reader);
                  if (totalCount == 0)
                  {
                      totalCount = reader.GetSafeInt32(20);
                  }
                  if (list == null)
                  {
                      list = new List<Sponsor>();
                  }

                  list.Add(sponsor);
              }
              );
            if (list != null)
            {
                response = new Paged<Sponsor>(list, pageIndex, pageSize, totalCount);
                return response;
            }
            return response;
        }

        public void Update(SponsorUpdateRequest data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("A parameter data is required!");
            }
            string storeProc = "[dbo].[Sponsors_Update]";
            _dataProvider.ExecuteNonQuery(storeProc
                , delegate (SqlParameterCollection sqlParams)
                {
                    sqlParams.AddWithValue("@Id", data.Id);
                    sqlParams.AddWithValue("@Name", data.Name);
                    sqlParams.AddWithValue("@CompanyUrl", data.CompanyUrl);
                    sqlParams.AddWithValue("@AddressId", data.AddressId);
                    sqlParams.AddWithValue("@PhoneNumber", data.PhoneNumber);
                    sqlParams.AddWithValue("@ContactPerson", data.ContactPerson);
                    sqlParams.AddWithValue("@PrimarySponsorTypeId", data.PrimarySponsorTypeId);
                });
        }

        public void Delete(int id)
        {
            string storeProc = "[dbo].[Sponsors_Delete]";
            _dataProvider.ExecuteNonQuery(storeProc
                , delegate (SqlParameterCollection sqlParams)
                {
                    sqlParams.AddWithValue("@Id", id);
                });
        }

        //Types
        public List<SponsorType> GetTypes()
        {
            List<SponsorType> list = null;
            string prokName = "[dbo].[SponsorTypes_SelectAll]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  //  this is where your input params go. it works the same way as with ExecuteNonQuery. 
                  //  This proc does not have any input parameters specified so we can leave this commented out
                  //   OR we could have passed null as the parameter value 
              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  SponsorType sponsor = new SponsorType();
                  int startingIndex = 0;
                  sponsor.Id = reader.GetSafeInt32(startingIndex++);
                  sponsor.Name = reader.GetSafeString(startingIndex++);
                  if (list == null)
                  {
                      list = new List<SponsorType>();
                  }
                  list.Add(sponsor);
              }
              );
            return list;
        }

        private static Sponsor Mapper(IDataReader reader)
        {
            Sponsor sponsor = new Sponsor();
            sponsor.SponsorType = new SponsorType();
            sponsor.Address = new Address();
            sponsor.Address.StateProvince = new StateProvince();
            sponsor.User = new User();
            int startingIndex = 0;
            sponsor.Id = reader.GetSafeInt32(startingIndex++);
            sponsor.User.Id = reader.GetSafeInt32(startingIndex++);  //User
            sponsor.User.UserName = reader.GetSafeString(startingIndex++);
            sponsor.Name = reader.GetSafeString(startingIndex++);
            sponsor.CompanyUrl = reader.GetSafeUri(startingIndex++);
            sponsor.Address.Id = reader.GetSafeInt32(startingIndex++); //Addresss
            sponsor.Address.LineOne = reader.GetSafeString(startingIndex++);
            sponsor.Address.LineTwo = reader.GetSafeString(startingIndex++);
            sponsor.Address.City = reader.GetSafeString(startingIndex++);
            sponsor.Address.StateProvince.Id = reader.GetSafeInt32(startingIndex++); //stateProvince
            sponsor.Address.StateProvince.CountryId = reader.GetSafeInt32(startingIndex++);
            sponsor.Address.StateProvince.Code = reader.GetSafeString(startingIndex++);
            sponsor.Address.StateProvince.CountryRegionCode = reader.GetSafeString(startingIndex++);
            sponsor.Address.PostalCode = reader.GetSafeString(startingIndex++);
            sponsor.PhoneNumber = reader.GetSafeString(startingIndex++);
            sponsor.ContactPerson = reader.GetSafeString(startingIndex++);
            sponsor.SponsorType.Id = reader.GetSafeInt32(startingIndex++);  // SponsorType
            sponsor.SponsorType.Name = reader.GetSafeString(startingIndex++);
            sponsor.DateCreated = reader.GetSafeDateTime(startingIndex++);
            sponsor.DateModified = reader.GetSafeDateTime(startingIndex++);
            return sponsor;
        }
    }
}
