using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeApi.Domain
{
    public class Employee
    {

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("lastname")]
        public string lastname { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime? dateOfBirth { get; set; }

        [JsonProperty("employmentStartDate")]
        public DateTime? employmentStartDate { get; set; }

        [JsonProperty("employmentEndDate")]
        public DateTime? employmentEndDate { get; set; }
    }
}
