using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SHMS_Project.Repositories
{
    public class FacultyHeadRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public void AddFacultyHead(FacultyHead facultyHead)
        {
            string query = "INSERT INTO FacultyHead (FacultyHeadName, FacultyHeadMail) VALUES (@FacultyHeadName, @FacultyHeadMail)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FacultyHeadName", facultyHead.FacultyHeadName);
                    command.Parameters.AddWithValue("@FacultyHeadMail", facultyHead.FacultyHeadMail);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetFacultyHeadNameByUsername(string username)
        {
            string query = "SELECT FacultyHeadName FROM FacultyHead WHERE FacultyHeadMail = @Username";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }
            return null;
        }
    }
}