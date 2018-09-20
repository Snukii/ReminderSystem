using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Net;
using System.Net.Mime;
using System.Net.Mail;
using System.IO;

namespace Reminder
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseInteraction DBI = new DatabaseInteraction();
            while (true)
            {
                //Check database and send Emails to people who have an appointment in 3 days
                DBI.remindEmail();

                //Check database and send SMS to people who have an appointment in a day
                DBI.remindSMS();

                //Wait 5 seconds before running the loop again
                Thread.Sleep(5000);
            }
        }
    }
}
