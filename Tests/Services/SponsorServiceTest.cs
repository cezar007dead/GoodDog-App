using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Services;

namespace Sabio.Tests.Services
{
    [TestClass]
    public class SponsorServiceTest
    {
        private readonly ISponsorService _sponsorService;

        public SponsorServiceTest()
        {
            List<Sponsor> sponsorProfileList = new List<Sponsor>();
            Sponsor s1 = new Sponsor();
            s1.Address = new Address();
            s1.SponsorType = new SponsorType();
            Sponsor s2 = new Sponsor();
            s2.Address = new Address();
            s2.SponsorType = new SponsorType();
            Sponsor s3 = new Sponsor();
            s3.Address = new Address();
            s3.SponsorType = new SponsorType();

            s1.Id = 1;
            s1.Name = "Google";
            s1.CompanyUrl = new Uri("https://www.google.com/");
            s1.Address.Id = 1;
            s1.PhoneNumber = "8184239283";
            s1.ContactPerson = "Sundar Pichai";
            s1.SponsorType.Id = 1;

            s2.Id = 2;
            s2.Name = "Facebook";
            s2.CompanyUrl = new Uri("https://www.facebook.com/");
            s2.Address.Id = 2;
            s2.PhoneNumber = "8185309283";
            s2.ContactPerson = "Mark Zuckerberg";
            s2.SponsorType.Id = 2;

            s3.Id = 3;
            s3.Name = "VK";
            s3.CompanyUrl = new Uri("https://www.facebook.com/");
            s3.Address.Id = 3;
            s3.PhoneNumber = "1235309283";
            s3.ContactPerson = "Pavel Durov";
            s3.SponsorType.Id = 3;

            sponsorProfileList.Add(s1);
            sponsorProfileList.Add(s2);
            sponsorProfileList.Add(s3);

            List<SponsorAddRequest> sponsorAddProfileList = new List<SponsorAddRequest>();
            SponsorAddRequest sar1 = new SponsorAddRequest();
            int userId1 = 1;
            sar1.Name = "Microsoft";
            sar1.CompanyUrl = "https://www.microsoft.com/";
            sar1.AddressId = 1;
            sar1.PhoneNumber = "8184309283";
            sar1.ContactPerson = "Bill Gates";
            sar1.PrimarySponsorTypeId = 1;

            sponsorAddProfileList.Add(sar1);

            List<SponsorUpdateRequest> sponsorUpdateProfileList = new List<SponsorUpdateRequest>();
            SponsorUpdateRequest sur1 = new SponsorUpdateRequest();
            sur1.Id = 1;
            sur1.Name = "Twitter";
            sur1.CompanyUrl = "https://twitter.com/";
            sur1.AddressId = 1;
            sur1.PhoneNumber = "2215309283";
            sur1.ContactPerson = "Jack Dorsey";
            sur1.PrimarySponsorTypeId = 1;

            sponsorUpdateProfileList.Add(sur1);

            List<SponsorType> sponsorTypesList = new List<SponsorType>();

            SponsorType st1 = new SponsorType();
            SponsorType st2 = new SponsorType();
            SponsorType st3 = new SponsorType();

            st1.Id = 0;
            st1.Name = "int";

            st2.Id = 1;
            st2.Name = "bool";

            st3.Id = 2;
            st3.Name = "string";

            sponsorTypesList.Add(st1);
            sponsorTypesList.Add(st2);
            sponsorTypesList.Add(st3);


            var mock = new Mock<ISponsorService>();



            mock.Setup(m => m.Insert(It.IsAny<SponsorAddRequest>(), It.IsAny<int>())).Returns(
                (SponsorAddRequest insertRequestModel, int userId) =>
                {
                    List<ValidationResult> validationResults = ValidateModal(insertRequestModel);

                    if (validationResults.Count > 0)
                    {
                        throw new ValidationException(validationResults[0], null, insertRequestModel);
                    }
                    return sponsorAddProfileList.Count + 1;
                }
                );

            mock.Setup(m => m.Get()).Returns(
                () =>
                {
                    List<Sponsor> sponsorsList = new List<Sponsor>();
                    foreach (var item in sponsorProfileList)
                    {
                        Sponsor model = new Sponsor();
                        model.Address = new Address();
                        model.SponsorType = new SponsorType();

                        model.Id = item.Id;
                        model.Name = item.Name;
                        model.CompanyUrl = item.CompanyUrl;
                        model.Address.Id = item.Address.Id;
                        model.PhoneNumber = item.PhoneNumber;
                        model.ContactPerson = item.ContactPerson;
                        model.SponsorType.Id = item.SponsorType.Id;
                        sponsorsList.Add(model);
                    }
                    return sponsorsList;
                });

            mock.Setup(m => m.Get(It.IsAny<int>())).Returns(
              (int id) =>
              {
                  Sponsor modal = sponsorProfileList.Where(m => m.Id == id).FirstOrDefault();

                  Sponsor newModal = null;
                  if (modal != null)
                  {
                      newModal = new Sponsor();
                      newModal.Address = new Address();
                      newModal.SponsorType = new SponsorType();

                      newModal.Id = modal.Id;
                      newModal.Name = modal.Name;
                      newModal.CompanyUrl = modal.CompanyUrl;
                      newModal.Address.Id = modal.Address.Id;
                      newModal.PhoneNumber = modal.PhoneNumber;
                      newModal.ContactPerson = modal.ContactPerson;
                      newModal.SponsorType.Id = modal.SponsorType.Id;
                  }
                  return newModal;
              });


            mock.Setup(m => m.Update(It.IsAny<SponsorUpdateRequest>())).Callback(
               (SponsorUpdateRequest updateRequestModel) =>
               {
                   List<ValidationResult> validationResults = ValidateModal(updateRequestModel);

                   if (validationResults.Count > 0)
                   {
                       throw new ValidationException(validationResults[0], null, updateRequestModel);
                   }
                   Sponsor model = sponsorProfileList.Where(m => m.Id == updateRequestModel.Id).Single();
                   model.Name = updateRequestModel.Name;
                   model.CompanyUrl = new Uri(updateRequestModel.CompanyUrl);
                   model.Address.Id = updateRequestModel.AddressId;
                   model.PhoneNumber = updateRequestModel.PhoneNumber;
                   model.ContactPerson = updateRequestModel.ContactPerson;
                   model.SponsorType.Id = updateRequestModel.PrimarySponsorTypeId;

               }
               );

            mock.Setup(m => m.Delete(It.IsAny<int>())).Callback(
                (int id) =>
                {
                    Sponsor sponsor = sponsorProfileList.Where(m => m.Id == id).Single();
                    sponsorProfileList.Remove(sponsor);
                });

            mock.Setup(m => m.Get(It.IsAny<int>(), It.IsAny<int>())).Returns(
                (int pageIndex, int pageSize) =>
                {
                    List<Sponsor> sponsorsList = new List<Sponsor>();
                    int count = (pageIndex) * pageSize;

                    for (int i = count; i < sponsorProfileList.Count; i++)
                    {
                        if (i < count + pageSize)
                        {
                            sponsorsList.Add(sponsorProfileList[i]);
                        }
                        else
                        {
                            break;
                        }

                    }
                    Paged<Sponsor> paged = new Paged<Sponsor>(sponsorsList, pageIndex, pageSize, sponsorProfileList.Count);
                    return paged;
                });

            mock.Setup(m => m.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(
                (int pageIndex, int pageSize, int typeId) =>
                {
                    List<Sponsor> sponsorsListResult = new List<Sponsor>();

                    List<Sponsor> sponsorsByType = new List<Sponsor>();
                    for (int i = 0; i < sponsorProfileList.Count; i++)
                    {
                        if (sponsorProfileList[i].SponsorType.Id == typeId)
                        {
                            sponsorsByType.Add(sponsorProfileList[i]);
                        }
                    }
                    int count = (pageIndex) * pageSize;

                    for (int i = count; i < sponsorsByType.Count; i++)
                    {
                        if (i < count + pageSize)
                        {
                            sponsorsListResult.Add(sponsorsByType[i]);
                        }
                        else
                        {
                            break;
                        }

                    }
                    Paged<Sponsor> paged = new Paged<Sponsor>(sponsorsListResult, pageIndex, pageSize, sponsorsByType.Count);
                    return paged;
                });

            mock.Setup(m => m.GetTypes()).Returns(
             () =>
             {
                 List<SponsorType> sponsorsTypesList = new List<SponsorType>();
                 foreach (var item in sponsorTypesList)
                 {
                     SponsorType model = new SponsorType();

                     model.Id = item.Id;
                     model.Name = item.Name;
                     sponsorsTypesList.Add(model);
                 }
                 return sponsorsTypesList;
             });

            _sponsorService = mock.Object;

        }

        private List<ValidationResult> ValidateModal(object insertRequestModel)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext ctx = new ValidationContext(insertRequestModel, null, null);
            Validator.TryValidateObject(insertRequestModel, ctx, validationResults, true);
            return validationResults;
        }

        [TestMethod]
        public void Insert_Test()
        {
            //Arrange
            int userId = 1;
            SponsorAddRequest sponsor = new SponsorAddRequest
            {
                Name = "Yandex",
                CompanyUrl = "https://yandex.com/",
                AddressId = 1,
                PhoneNumber = "9184309283",
                ContactPerson = "Arkady Volozhich",
                PrimarySponsorTypeId = 1
            };

            //Act
            int result = _sponsorService.Insert(sponsor, userId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(int), "Id has to be int");
            Assert.IsTrue(result > 0);
        }

        [TestMethod, ExpectedException(typeof(ValidationException))]
        public void Insert_Failed_Name_Required_Test()
        {
            //Arrange
            int userId = 1;
            SponsorAddRequest sponsor = new SponsorAddRequest
            {
                //Name = "Yandex",
                CompanyUrl = "https://yandex.com/",
                AddressId = 1,
                PhoneNumber = "9184309283",
                ContactPerson = "Arkady Volozhich",
                PrimarySponsorTypeId = 1
            };

            //Act
            int result = _sponsorService.Insert(sponsor, userId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(int), "Id has to be int");
            Assert.IsTrue(result > 0);
        }

        [TestMethod, ExpectedException(typeof(ValidationException))]
        public void Insert_Failed_CompanyUrl_Required_Test()
        {
            //Arrange
            int userId = 1;
            SponsorAddRequest sponsor = new SponsorAddRequest
            {
                Name = "Yandex",
                //CompanyUrl = "https://yandex.com/",
                AddressId = 1,
                PhoneNumber = "9184309283",
                ContactPerson = "Arkady Volozhich",
                PrimarySponsorTypeId = 1
            };

            //Act
            int result = _sponsorService.Insert(sponsor, userId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(int), "Id has to be int");
            Assert.IsTrue(result > 0);
        }

        [TestMethod, ExpectedException(typeof(ValidationException))]
        public void Insert_Failed_PhoneNumber_Required_Test()
        {
            //Arrange
            int userId = 1;
            SponsorAddRequest sponsor = new SponsorAddRequest
            {
                Name = "Yandex",
                CompanyUrl = "https://yandex.com/",
                AddressId = 1,
                //PhoneNumber = "9184309283",
                ContactPerson = "Arkady Volozhich",
                PrimarySponsorTypeId = 1
            };

            //Act
            int result = _sponsorService.Insert(sponsor, userId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(int), "Id has to be int");
            Assert.IsTrue(result > 0);
        }

        [TestMethod, ExpectedException(typeof(ValidationException))]
        public void Insert_Failed_ContactPerson_Required_Test()
        {
            //Arrange
            int userId = 1;
            SponsorAddRequest sponsor = new SponsorAddRequest
            {
                Name = "Yandex",
                CompanyUrl = "https://yandex.com/",
                AddressId = 1,
                PhoneNumber = "9184309283",
                //ContactPerson = "Arkady Volozhich",
                PrimarySponsorTypeId = 1
            };

            //Act
            int result = _sponsorService.Insert(sponsor, userId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(int), "Id has to be int");
            Assert.IsTrue(result > 0);
        }

        [TestMethod, ExpectedException(typeof(ValidationException))]
        public void Insert_Failed_PrimarySponsorTypeId_Required_Test()
        {
            //Arrange
            int userId = 1;
            SponsorAddRequest sponsor = new SponsorAddRequest
            {
                Name = "Yandex",
                CompanyUrl = "https://yandex.com/",
                AddressId = 1,
                PhoneNumber = "9184309283",
                ContactPerson = "Arkady Volozhich",
                //PrimarySponsorTypeId = 1
            };

            //Act
            int result = _sponsorService.Insert(sponsor, userId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(int), "Id has to be int");
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void Update_Test()
        {
            Sponsor sponsor = new Sponsor();
            sponsor.Address = new Address();
            sponsor.SponsorType = new SponsorType();
            sponsor = _sponsorService.Get(1);

            //Arrange
            SponsorUpdateRequest sponsorUpdate = new SponsorUpdateRequest
            {
                Id = sponsor.Id,
                Name = "Instagram",
                CompanyUrl = "https://yandex.com/",
                AddressId = sponsor.Address.Id + 1,
                PhoneNumber = "9184309283",
                ContactPerson = "Kevin Systrom",
                PrimarySponsorTypeId = sponsor.SponsorType.Id + 1
            };

            //Act
            _sponsorService.Update(sponsorUpdate);
            Sponsor anotherSponsor = new Sponsor();
            anotherSponsor.Address = new Address();
            anotherSponsor.SponsorType = new SponsorType();
            anotherSponsor = _sponsorService.Get(1);

            //Assert
            Assert.IsTrue(sponsor.Id == anotherSponsor.Id, "Id don't match");
            Assert.IsFalse(sponsor.Name == anotherSponsor.Name, "Name hasn't changed");
            Assert.IsFalse(sponsor.CompanyUrl == anotherSponsor.CompanyUrl, "CompanyUrl hasn't changed");
            Assert.IsFalse(sponsor.Address.Id == anotherSponsor.Address.Id, "Address hasn't changed");
            Assert.IsFalse(sponsor.PhoneNumber == anotherSponsor.PhoneNumber, "PhoneNumber hasn't changed");
            Assert.IsFalse(sponsor.ContactPerson == anotherSponsor.ContactPerson, "ContactPerson hasn't changed");
            Assert.IsFalse(sponsor.SponsorType.Id == anotherSponsor.SponsorType.Id, "SponsorType Id hasn't changed");

        }

        [TestMethod]
        public void GetAll_Test()
        {
            List<Sponsor> sponsorsLsit = _sponsorService.Get();
            Assert.IsNotNull(sponsorsLsit, "Sponsor Profile List can't be null");
            Assert.IsTrue(sponsorsLsit.Count > 0, "Sponsor Profile List has to have a count greater then 0");
        }

        [TestMethod]
        public void GetById_Test()
        {
            Sponsor sponsors = _sponsorService.Get(1);
            Assert.IsNotNull(sponsors, "Sponsor can't be null");
            Assert.IsInstanceOfType(sponsors, typeof(Sponsor), "The type returned is not correct");
        }

        [TestMethod]
        public void GetByPageIndexSize_Test()
        {
            Paged<Sponsor> sponsorsLsitPaginated = _sponsorService.Get(1, 2);

            Assert.IsNotNull(sponsorsLsitPaginated, "Sponsor Profile Pagination can't be null");
            Assert.IsTrue(sponsorsLsitPaginated.PagedItems.Count > 0, "Sponsor Profile Paginated List has to have a count greater then 0");
        }

        [TestMethod]
        public void GetByPageIndexSizeType_Test()
        {
            Paged<Sponsor> sponsorsLsitPaginated = _sponsorService.Get(0, 1, 1);

            Assert.IsNotNull(sponsorsLsitPaginated, "Sponsor Profile Pagination can't be null");
            Assert.IsTrue(sponsorsLsitPaginated.PagedItems.Count > 0, "Sponsor Profile Paginated List has to have a count greater then 0");
        }

        [TestMethod]
        public void GetAllSponsorsTypes_Test()
        {
            List<SponsorType> sponsorsLsit = _sponsorService.GetTypes();
            Assert.IsNotNull(sponsorsLsit, "Sponsor Profile List can't be null");
            Assert.IsTrue(sponsorsLsit.Count > 0, "Sponsor Profile List has to have a count greater then 0");
        }

        [TestMethod]
        public void DeleteById_Test()
        {
            Sponsor sponsors = _sponsorService.Get(1);

            _sponsorService.Delete(1);

            Sponsor sponsorsDefault = _sponsorService.Get(1);

            Assert.IsNull(sponsorsDefault, "Delete is not a success");
        }
    }
}
