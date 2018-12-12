using AcmeApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeApi.Interface
{
    public interface IEmployeeService
    {  
        void RequestData();
        IList<Employee> GetEmployeeList();
        IList<int> GetExclusionsList();
        void  AddExclusionsList(int id);
    }
}
