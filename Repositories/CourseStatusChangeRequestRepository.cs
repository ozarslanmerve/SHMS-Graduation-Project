using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SHMS_Project.Repositories
{
    public class CourseStatusChangeRequestRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public void AddCourseStatusChangeRequest(CourseStatusChangeRequest request)
        {
            string query = @"INSERT INTO CourseStatusChangeRequest (InstructorName, CourseCode, RequestedStatus, RequestDate, ApprovalStatus)
                             VALUES (@InstructorName, @CourseCode, @RequestedStatus, @RequestDate, @ApprovalStatus)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstructorName", request.InstructorName);
                    command.Parameters.AddWithValue("@CourseCode", request.CourseCode);
                    command.Parameters.AddWithValue("@RequestedStatus", request.RequestedStatus);
                    command.Parameters.AddWithValue("@RequestDate", request.RequestDate);
                    command.Parameters.AddWithValue("@ApprovalStatus", request.ApprovalStatus);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<CourseStatusChangeRequest> GetRequestsByStatus(string status)
        {
            List<CourseStatusChangeRequest> requests = new List<CourseStatusChangeRequest>();

            string query = @"SELECT * FROM CourseStatusChangeRequest WHERE ApprovalStatus = @Status";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", status);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CourseStatusChangeRequest request = new CourseStatusChangeRequest
                        {
                            RequestId = (int)reader["RequestId"],
                            InstructorName = reader["InstructorName"].ToString(),
                            CourseCode = reader["CourseCode"].ToString(),
                            RequestedStatus = reader["RequestedStatus"].ToString(),
                            RequestDate = (DateTime)reader["RequestDate"],
                            ApprovalStatus = reader["ApprovalStatus"].ToString()
                        };
                        requests.Add(request);
                    }
                }
            }
            return requests;
        }


        public List<CourseStatusChangeRequest> GetRequestsByInstructorName(string instructorName, string status)
        {
            List<CourseStatusChangeRequest> requests = new List<CourseStatusChangeRequest>();

            string query = @"SELECT * FROM CourseStatusChangeRequest WHERE InstructorName = @InstructorName AND ApprovalStatus = @Status";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstructorName", instructorName);
                    command.Parameters.AddWithValue("@Status", status);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CourseStatusChangeRequest request = new CourseStatusChangeRequest
                        {
                            RequestId = (int)reader["RequestId"],
                            InstructorName = reader["InstructorName"].ToString(),
                            CourseCode = reader["CourseCode"].ToString(),
                            RequestedStatus = reader["RequestedStatus"].ToString(),
                            RequestDate = (DateTime)reader["RequestDate"],
                            ApprovalStatus = reader["ApprovalStatus"].ToString()
                        };
                        requests.Add(request);
                    }
                }
            }
            return requests;
        }

        public void UpdateRequestStatus(int requestId, string status, string courseCode, string requestedStatus)
        {
            string query1 = @"UPDATE CourseStatusChangeRequest SET ApprovalStatus = @Status WHERE RequestId = @RequestId";
            string query2 = @"UPDATE Course SET CourseStatus = @RequestedStatus WHERE CourseCode = @CourseCode";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand command1 = new SqlCommand(query1, connection, transaction))
                        {
                            command1.Parameters.AddWithValue("@RequestId", requestId);
                            command1.Parameters.AddWithValue("@Status", status);
                            command1.ExecuteNonQuery();
                        }

                        if (status == "Approved")
                        {
                            using (SqlCommand command2 = new SqlCommand(query2, connection, transaction))
                            {
                                command2.Parameters.AddWithValue("@CourseCode", courseCode);
                                command2.Parameters.AddWithValue("@RequestedStatus", requestedStatus);
                                command2.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
