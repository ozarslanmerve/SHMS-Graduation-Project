using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SHMS_Project.Repositories
{
    public class DiseaseRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public List<Disease> GetAllDiseases()
        {
            List<Disease> diseases = new List<Disease>();

            string query = "SELECT * FROM Disease";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Disease disease = new Disease
                        {
                            DiseaseName = reader["DiseaseName"].ToString(),
                            IsDiseaseContagious = reader["IsDiseaseContagious"].ToString()
                        };
                        diseases.Add(disease);
                    }
                }
            }

            return diseases;
        }

        public string GetIsDiseaseContagiousByName(string diseaseName)
        {
            string isDiseaseContagious = string.Empty;
            string query = "SELECT IsDiseaseContagious FROM Disease WHERE DiseaseName = @DiseaseName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DiseaseName", diseaseName);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isDiseaseContagious = reader["IsDiseaseContagious"].ToString();
                        }
                    }
                }
            }

            return isDiseaseContagious;
        }
    }
}
