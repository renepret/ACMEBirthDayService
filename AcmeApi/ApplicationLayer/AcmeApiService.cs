using AcmeApi.Domain;
using AcmeApi.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static AcmeApi.AcmeApiService;

namespace AcmeApi
{
    public class AcmeApiService: IEmployeeService
    {
        private List<Employee> _employees;
        private List<int> _exclusions;
        public AcmeApiService()
        {
            _employees = new List<Employee>();
            _exclusions = new List<int>();
        }

        public List<Employee> Employees { get { return _employees; } }
        public List<int> Exclusions { get { return _exclusions; } }


        IList<Employee> IEmployeeService.GetEmployeeList()
        {
            return _employees;
        }

        IList<int> IEmployeeService.GetExclusionsList()
        {
            return _exclusions;
        }

        public void AddExclusionsList(int id)
        {
            _exclusions.Add(id);
        }


        public enum ObjectType
        {
            Employees,
            Exlusions,
            All
        }

        public void RequestData()
        {
            GetRequest(ObjectType.Employees);
            GetRequest(ObjectType.Exlusions);
        }
        public void GetRequest(ObjectType type)
        {
          
            var TARGETURL = "";
            if (type == ObjectType.Employees)
            {
                TARGETURL = ConfigurationManager.AppSettings["EMPLOYEE_URL"];
            }
            else
            {
                TARGETURL = ConfigurationManager.AppSettings["EXCLUSIONS_URL"];
            }

            RunRequest(type, TARGETURL);
        }

        private void RunRequest(ObjectType type, string TARGETURL)
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(TARGETURL);

                //should cater for proxy in real life

                webRequest.Timeout = 12000;

                string json = "";

                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            json = streamReader.ReadToEnd();

                            Populatetype(type, json);

                            streamReader.Close();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                var logServicemessage = ex.Message;
                //log api exceptions here.
            }
        }

        private void Populatetype(ObjectType type, string json)
        {
            if (type == ObjectType.Employees)
            {
                _employees = JsonConvert.DeserializeObject<List<Employee>>(json);
            }
            else
            {
                _exclusions = JsonConvert.DeserializeObject<List<int>>(json);
            }
        }

  
    }
}
