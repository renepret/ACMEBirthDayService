using AcmeApi.Domain;
using AcmeApi.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AcmeApi.ApplicationLayer
{
    public class BirthdayMessageService
    {
        private IEmployeeService _service;
        private string _todaysDateFileName;
        public BirthdayMessageService(IEmployeeService service)
        {
            _service = service;
            _service.RequestData();
        }

        public List<Employee> GetBirthdayList(DateTime today)
        {
            var TodaysBirthDays = new List<Employee>();
            var year = today.Year;
            var month = today.Month;
            var day = today.Day;
            var leapDay = -1;

            if (!DateTime.IsLeapYear(year))
            {
                if (month == 2 && day == 28)
                {
                    leapDay = 29;
                }
            }

            TodaysBirthDays = _service.GetEmployeeList().Where(b => b.dateOfBirth.Value.Month == month &&
            (b.dateOfBirth.Value.Day == day || b.dateOfBirth.Value.Day == leapDay) &&
            b.employmentEndDate == null).ToList();

            //scrub exclusions
            TodaysBirthDays = TodaysBirthDays.Where(i => !_service.GetExclusionsList().Contains(i.id)).ToList();

            return TodaysBirthDays;
        }

        public void Send()
        {
            var today = DateTime.Now;
            _todaysDateFileName = today.Day + "_" + today.Month + "_" + today.Year + ".txt";

            ReadExclusionRecords();
            List<Employee> BirthdayList = GetBirthdayList(today);

            foreach (var emp in BirthdayList)
            {
                ExecuteEmail(ConfigureMailMessage(emp), today);
                WriteExclusionRecord(emp.id.ToString());
            }
        }

        private void ExecuteEmail(string message, DateTime sendDate)
        {
            //send the message
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMTP_SERVER"], int.Parse(ConfigurationManager.AppSettings["SMTP_SERVER_PORT"]));
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["TEST_EMAIL"] , ConfigurationManager.AppSettings["TEST_EMAIL_PASSWORD"]);
            MailMessage msg = null;

            try
            {
                msg = new MailMessage(ConfigurationManager.AppSettings["TEST_EMAIL"], ConfigurationManager.AppSettings["TEST_EMAIL"], ConfigurationManager.AppSettings["EMAIL_SUBJECT"], message);

                //smtp.Send(msg);
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                if (msg != null)
                {
                    msg.Dispose();
                }
            }
        }

        private string ConfigureMailMessage(Employee emp)
        {
            var message = ConfigurationManager.AppSettings["MESSAGE"];
            message = string.Format(message, emp.name + " " + emp.lastname);

            return message;
        }


        private void WriteExclusionRecord(string id)
        {
            var dir = ConfigurationManager.AppSettings["TODAYS_EXCLUSION_FILE_DIRECTORY"];
            var path = dir + _todaysDateFileName;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!System.IO.File.Exists(path))
            {
                using (var myFile = File.Create(path))
                {
                    myFile.Close();
                }
            }

            System.IO.File.AppendAllText(path, id + ";");

        }
        /// <summary>
        /// Read _todaysDateFileName for id's allready sent, then add to _service.Exclusions..
        /// </summary>
        private void ReadExclusionRecords()
        {
            var dir = ConfigurationManager.AppSettings["TODAYS_EXCLUSION_FILE_DIRECTORY"];
            var path = dir + _todaysDateFileName;

            if (!Directory.Exists(dir))
            {
                return;
            }

            if (System.IO.File.Exists(path))
            {
                var lines = File.ReadAllText(path);
                var list = lines.Split(';');
                var noSpaceId = "";
                foreach(var id in list)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        noSpaceId = id.Replace(" ", "");
                        _service.AddExclusionsList(int.Parse(noSpaceId));
                    }
                }
            }

        }
    }
}
