using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SHMS_Project.Repositories
{
    public class InstructorRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public void AddInstructor(Instructor instructor)
        {
            string query = "INSERT INTO Instructor (InstructorName, InstructorMail) VALUES (@InstructorName, @InstructorMail)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstructorName", instructor.InstructorName);
                    command.Parameters.AddWithValue("@InstructorMail", instructor.InstructorMail);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetInstructorNameForCourse(string courseCode)
        {
            string instructorName = null;
            string query = "SELECT InstructorName FROM Course WHERE CourseCode = @CourseCode";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseCode", courseCode);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        instructorName = result.ToString();
                    }
                }
            }
            return instructorName;
        }

        public string GetInstructorNameByUsername(string username)
        {
            string instructorName = null;
            string query = "SELECT InstructorName FROM Instructor WHERE InstructorMail = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        instructorName = result.ToString();
                    }
                }
            }
            return instructorName;
        }

        public List<Course> GetCoursesByInstructorMail(string username)
        {
            List<Course> courses = new List<Course>();

            string query = @"SELECT CourseCode, CourseName
                             FROM Course
                             WHERE InstructorMail = @InstructorMail";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstructorMail", username);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Course course = new Course
                        {
                            CourseCode = reader["CourseCode"].ToString(),
                            CourseName = reader["CourseName"].ToString()
                        };
                        courses.Add(course);
                    }
                }
            }

            return courses;
        }
    }

}

