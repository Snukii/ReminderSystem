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
    class DatabaseInteraction
    {
        //Setup
        private SqlConnection conn = new SqlConnection(@"data source = .\SQLSERVERNAMEHERE; integrated security = true; database = DATABASENAMEHERE; MultipleActiveResultSets = True");
        private SqlCommand cmd = null;
        private SqlDataReader rdr = null;
        private string sqlSel = "";

        //Method to send reminders
        private void remind(string sqlString, string reminderType, int currentState, int intermediateState, int destinationState)
        {
            Reminders rmnd = new Reminders();

            conn.Open();

            sqlSel = sqlString;

            cmd = new SqlCommand(sqlSel, conn);

            rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    int id = rdr.GetInt32(rdr.GetOrdinal("id"));

                    int affectedRows = UpdateState(conn, id, currentState, intermediateState);

                    if (affectedRows > 0)
                    {
                        switch (reminderType)
                        {
                            case "EMAIL":
                                rmnd.SendEMAIL(rdr["email"].ToString(), "Remember that you have an appointment at: " + rdr["appTime"]);
                                break;
                            case "SMS":
                                rmnd.SendSMS(rdr["phone"].ToString(), "Remember that you have an appointment at: " + rdr["appTime"]);
                                break;
                        }
                        UpdateState(conn, id, intermediateState, destinationState);
                        Console.WriteLine("\n");
                    }
                }
            }
            else
            {
                Console.WriteLine("No valid " + reminderType + " transitions to be made");
            }
            rdr.Close();
            conn.Close();
        }

        //Method to call the reminder method with Email configuration
        public void remindEmail()
        {
            remind("select * from AppointmentTable where state = 0 and appTime <= DATEADD(DAY, 3, CONVERT(datetime, CURRENT_TIMESTAMP))", "EMAIL", 0, 1, 2);
        }

        //Method to call the reminder method with SMS configuration
        public void remindSMS()
        {
            remind("select * from AppointmentTable where state = 2 and appTime <= DATEADD(DAY, 1, CONVERT(datetime, CURRENT_TIMESTAMP))", "SMS", 2, 3, 4);
        }

        //Method to update the state column in the database
        static int UpdateState(SqlConnection connection, int id, int stateOld, int stateNew)
        {
            Console.Write("Updating state for id:" + id + " from state: " + stateOld + " to state: " + stateNew);
            using (var updateState = new SqlCommand("update AppointmentTable set state = " + stateNew + " where state = " + stateOld + " and id =" + id, connection))
            {
                int affectedRows = updateState.ExecuteNonQuery();

                return affectedRows;
            }
        }
    }
}
