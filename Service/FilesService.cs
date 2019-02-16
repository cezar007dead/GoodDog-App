using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using System.Web;
using System.Security.Cryptography;
using Amazon.S3.Model;
using Sabio.Data.Providers;
using System.Data.SqlClient;
using Sabio.Models.Domain;
using System.Web.Configuration;
using Sabio.Services.Interfaces;
using System.Web.Services;
using System.Data;
using Sabio.Data;

namespace Sabio.Services
{
    public class FilesService : IFilesService
    {
        private IKeysService _keysService;
        private string key;
        private string secretKey;
        private string bucketName;
        private string regionName;
        private IDataProvider _dataProvider;
        private string filePath;


        public FilesService(IDataProvider dataProvider, IKeysService keysService)
        {
            _keysService = keysService;
            key = Convert.ToString(_keysService.GetByKeyName<string>("AWS.KeyId"));
            secretKey = Convert.ToString(_keysService.GetByKeyName<string>("AWS.AccessSecret"));
            bucketName = Convert.ToString(_keysService.GetByKeyName<string>("Bucket"));
            filePath = Convert.ToString(_keysService.GetByKeyName<string>("Aws.FilePath"));
            regionName = Convert.ToString(_keysService.GetByKeyName<string>("Aws.RegionName"));
            _dataProvider = dataProvider;
        }

        public async Task<AppFile> UploadFileAsync(HttpPostedFile file)
        {
            try
            {
                Stream streamFile = file.InputStream;
                RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(regionName);
                IAmazonS3 s3Client = new AmazonS3Client(key, secretKey, bucketRegion);
                var fileTransferUtility = new TransferUtility(s3Client);

                Guid guid = Guid.NewGuid();

                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                request.BucketName = bucketName;
                request.Key = string.Format(filePath, guid, file.FileName);
                request.InputStream = streamFile;

                await fileTransferUtility.UploadAsync(request);

                int id = Insert("/" + request.Key, file.FileName);
                AppFile responseObj = new AppFile();
                responseObj.Id = id;
                responseObj.Path = Convert.ToString(_keysService.GetByKeyName<string>("AWS.PathLink")) + "/" + request.Key;
                return responseObj;

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                return null;
            }

        }
        public DataFile Get(int id)
        {
            DataFile file = null;
            string prokName = "[dbo].[Files_GetById]";
            _dataProvider.ExecuteCmd(prokName
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Id", id);
              }
              , singleRecordMapper: delegate (IDataReader reader, short set)
              {
                  file = new DataFile();
                  int startingIndex = 0;
                  file.Id = reader.GetSafeInt32(startingIndex++);
                  file.Path = reader.GetSafeString(startingIndex++);
                  file.Name = reader.GetSafeString(startingIndex++);
                  file.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  file.DateModified = reader.GetSafeDateTime(startingIndex++);
              }
              );
            file.Path = Convert.ToString(_keysService.GetByKeyName<string>("AWS.PathLink")) + file.Path;
            return file;
        }

        private int Insert(string Path, string name)
        {
            int fileId = 0;
            string storeProc = "[dbo].[Files_Insert]";
            _dataProvider.ExecuteNonQuery(storeProc
                , delegate (SqlParameterCollection sqlParams)
                {
                    sqlParams.AddWithValue("@Name", name);
                    sqlParams.AddWithValue("@Path", Path);

                    SqlParameter idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    idParameter.Direction = System.Data.ParameterDirection.Output;

                    sqlParams.Add(idParameter);

                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    Int32.TryParse(param["@Id"].Value.ToString(), out fileId);
                }
                );
            return fileId;
        }
    }
}
