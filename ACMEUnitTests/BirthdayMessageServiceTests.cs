using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcmeApi;
using AcmeApi.ApplicationLayer;
using AcmeApi.Domain;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Tests for both the ACME API calls and for the Message Service.
/// </summary>
namespace ACMEUnitTests
{
    [TestClass]
    public class BirthdayMessageServiceTests
    {
        [TestMethod]
        public void TestEmployeeGetRequest()
        {
            var service = new AcmeApiService();
            service.GetRequest(AcmeApiService.ObjectType.Employees); ;
            Assert.IsTrue(service.Employees.Count > 0);
        }

        [TestMethod]
        public void TestExclusionsGetRequest()
        {
            var service = new AcmeApiService();
            service.GetRequest(AcmeApiService.ObjectType.Exlusions); ;
            Assert.IsTrue(service.Exclusions.Count > 0);
        }

        [TestMethod]
        public void TestGetBirthdayListPositive()
        {
            var testDate = new DateTime(1955, 10, 28);
            var service = new BirthdayMessageService(new AcmeApiService());
            var TodaysBirthDays = service.GetBirthdayList(testDate);
           

            service.Send();
            Assert.IsTrue(TodaysBirthDays.Count > 0);
        }

        [TestMethod]
        public void TestGetBirthdayListLeapYear()
        {
            var testDate = new DateTime(2016, 2, 29);
            var service = new BirthdayMessageService(new AcmeApiService());
            var TodaysBirthDays = service.GetBirthdayList(testDate);
            Assert.IsTrue(TodaysBirthDays.Count > 0);
        }


        [TestMethod]
        public void TestGetBirthdayListNonLeapYear()
        {
            var testDate = new DateTime(2015, 2, 28);
            var service = new BirthdayMessageService(new AcmeApiService());
            var TodaysBirthDays = service.GetBirthdayList(testDate);
            Assert.IsTrue(TodaysBirthDays.Count > 0);
        }

        [TestMethod]
        public void TestGetBirthdayList()
        {

            var service = new BirthdayMessageService(new AcmeApiService());
            var TodaysBirthDays = service.GetBirthdayList(DateTime.Today);
            Assert.IsTrue(TodaysBirthDays.Count > 0);
        }

        //new AcmeApi.Domain.Employee
        //{
        //	dateOfBirth = new DateTime(1967, 12, 21, 0, 0, 0),
        //	employmentEndDate = null,
        //	employmentStartDate = new DateTime(2012, 11, 1, 0, 0, 0),
        //	id = 223,
        //	lastname = "Hedstrom",
        //	name = "Earlene"
        //}

        [TestMethod]
        public void TestGetBirthdayListExclusions()
        {
            var testDate = new DateTime(2015, 12, 21);
            var service = new BirthdayMessageService(new AcmeApiService());
            List<Employee> TodaysBirthDays = service.GetBirthdayList(testDate);
            Employee excludedEmployee = TodaysBirthDays.Where(s => s.id == 223).SingleOrDefault();
            Assert.IsTrue(excludedEmployee==null);
        }
        /// <summary>
        /// The Main Method
        /// </summary>
        [TestMethod]
        public void TestSendMessage()
        {

            var service = new BirthdayMessageService(new AcmeApiService());
            service.Send();
            
        }
    }
}
