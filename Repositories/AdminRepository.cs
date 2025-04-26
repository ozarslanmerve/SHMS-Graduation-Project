using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SHMS_Project.Repositories
{
    public class AdminRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public void AddAdmin(Admin admin)
        {
            string query = "INSERT INTO Admin (AdminMail) VALUES (@AdminMail)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AdminMail", admin.AdminMail);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}