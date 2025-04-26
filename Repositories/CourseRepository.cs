using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SHMS_Project.Repositories
{
    public class CourseRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public List<Course> GetAllCourses()
        {
            List<Course> courses = new List<Course>();

            string query = "SELECT * FROM Course";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Course course = new Course
                        {
                            CourseCode = reader["CourseCode"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            InstructorName = reader["InstructorName"].ToString(),
                            CourseTermYear = reader["CourseTermYear"].ToString(),
                            CourseStatus = reader["CourseStatus"].ToString()
                        };
                        courses.Add(course);
                    }
                }
            }

            return courses;
        }

        public Course GetCourseByCode(string courseCode)
        {
            Course course = null;
            string query = "SELECT * FROM Course WHERE CourseCode = @CourseCode";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseCode", courseCode);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        course = new Course
                        {
                            CourseCode = reader["CourseCode"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            InstructorName = reader["InstructorName"].ToString(),
                            CourseTermYear = reader["CourseTermYear"].ToString(),
                            CourseStatus = reader["CourseStatus"].ToString()
                        };
                    }
                }
            }
            return course;


        }



        public List<Course> GetCoursesByStudentNo(string studentNo)
        {
            List<Course> courses = new List<Course>();

            string query = @"SELECT C.CourseCode, C.CourseName, C.InstructorName
                     FROM Course C
                     INNER JOIN CourseList L ON C.CourseCode = L.CourseCode
                     WHERE L.StudentNo = @StudentNo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", studentNo);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Course course = new Course
                        {
                            CourseCode = reader["CourseCode"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            InstructorName = reader["InstructorName"].ToString()
                        };
                        courses.Add(course);
                    }
                }
            }

            return courses;
        }


        public string GetInstructorNameByCourseCode(string courseCode)
        {
            string instructorName = "";
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

        public List<Course> GetCoursesByInstructorName(string instructorName)
        {
            List<Course> courses = new List<Course>();

            string query = @"SELECT CourseCode, CourseName
                             FROM Course
                             WHERE InstructorName = @InstructorName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstructorName", instructorName);
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

        public void UpdateCourseStatus(string courseCode, string courseStatus)
        {
            string query = @"UPDATE Course SET Status = @CourseStatus WHERE CourseCode = @CourseCode";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseCode", courseCode);
                    command.Parameters.AddWithValue("@CourseStatus", courseStatus);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
