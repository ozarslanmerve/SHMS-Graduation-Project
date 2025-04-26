using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SHMS_Project.Repositories
{
    public class StudentRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public void AddStudent(Student student)
        {
            string query = "INSERT INTO Student (StudentName, StudentNo, StudentMail, StudentPassword) VALUES (@StudentName, @StudentNo, @StudentMail, @StudentPassword)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentName", student.StudentName);
                    command.Parameters.AddWithValue("@StudentNo", student.StudentNo);
                    command.Parameters.AddWithValue("@StudentMail", student.StudentMail);
                    command.Parameters.AddWithValue("@StudentPassword", student.StudentPassword);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();

            string query = "SELECT * FROM Student";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Student student = new Student
                            {
                                StudentName = reader["StudentName"].ToString(),
                                StudentNo = reader["StudentNo"].ToString(),
                                StudentMail = reader["StudentMail"].ToString(),
                                StudentPassword = reader["StudentPassword"].ToString()
                            };
                            students.Add(student);
                        }
                    }
                }
            }
            return students;
        }

        public string GetCurrentStudentNoByEmail(string email)
        {
            string studentNo = null;
            string query = "SELECT StudentNo FROM Student WHERE StudentMail = @StudentMail";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentMail", email);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        studentNo = result.ToString();
                    }
                }
            }
            return studentNo;
        }

        public string GetCurrentStudentNoByUsername(string username)
        {
            string studentNo = null;
            string query = "SELECT StudentNo FROM Student WHERE StudentMail = @Username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        studentNo = result.ToString();
                    }
                }
            }
            return studentNo;
        }

        public string GetStudentNameByStudentNo(string studentNo)
        {
            string studentName = null;
            string query = "SELECT StudentName FROM Student WHERE StudentNo = @StudentNo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", studentNo);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        studentName = result.ToString();
                    }
                }
            }

            return studentName;
        }

        public string GetStudentMailByStudentNo(string studentNo)
        {
            string studentMail = null;
            string query = "SELECT StudentMail FROM Student WHERE StudentNo = @StudentNo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", studentNo);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        studentMail = result.ToString();
                    }
                }
            }

            return studentMail;
        }

        public List<MedicalReportRecord> GetMedicalReportRecordsByStudentNo(string studentNo)
        {
            List<MedicalReportRecord> medicalReportRecords = new List<MedicalReportRecord>();

            string query = @"SELECT MRR.MedicalReportStartDate, MRR.MedicalReportEndDate, MRR.CourseCode, MRR.DiseaseName, MRR.MedicalReportStatus
                             FROM MedicalReportRecord MRR
                             JOIN Student S ON MRR.StudentNo = S.StudentNo
                             WHERE S.StudentNo = @StudentNo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", studentNo);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        MedicalReportRecord medicalReportRecord = new MedicalReportRecord
                        {
                            MedicalReportStartDate = Convert.ToDateTime(reader["MedicalReportStartDate"]),
                            MedicalReportEndDate = Convert.ToDateTime(reader["MedicalReportEndDate"]),
                            CourseCode = reader["CourseCode"].ToString(),
                            DiseaseName = reader["DiseaseName"].ToString(),
                            MedicalReportStatus = reader["MedicalReportStatus"].ToString()
                        };
                        medicalReportRecords.Add(medicalReportRecord);
                    }
                }
            }

            return medicalReportRecords;
        }

        public List<CourseViewModel> GetCourseListByStudentEmail(string username)
        {
            List<CourseViewModel> courseList = new List<CourseViewModel>();

            string query = @"SELECT C.CourseCode, C.CourseName, C.InstructorName, C.CourseTermYear, C.CourseStatus
                             FROM Course C
                             JOIN CourseList CL ON C.CourseCode = CL.CourseCode
                             JOIN Student S ON CL.StudentNo = S.StudentNo
                             WHERE S.StudentMail = @StudentMail";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentMail", username);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CourseViewModel course = new CourseViewModel
                        {
                            CourseCode = reader["CourseCode"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            InstructorName = reader["InstructorName"].ToString(),
                            CourseTermYear = reader["CourseTermYear"].ToString(),
                            CourseStatus = reader["CourseStatus"].ToString()
                        };
                        courseList.Add(course);
                    }
                }
            }

            return courseList;
        }

        public List<Course> GetCoursesByStudentNo(string studentNo)
        {
            List<Course> courses = new List<Course>();

            string query = @"SELECT C.CourseCode, C.CourseName
                             FROM Course C
                             JOIN CourseList CL ON C.CourseCode = CL.CourseCode
                             WHERE CL.StudentNo = @StudentNo";

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
                            CourseName = reader["CourseName"].ToString()
                        };
                        courses.Add(course);
                    }
                }
            }

            return courses;
        }

        public Student GetStudentByStudentNo(string studentNo)
        {
            Student student = null;

            string query = "SELECT * FROM Student WHERE StudentNo = @StudentNo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", studentNo);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        student = new Student
                        {
                            StudentName = reader["StudentName"].ToString(),
                            StudentNo = reader["StudentNo"].ToString(),
                            StudentMail = reader["StudentMail"].ToString(),
                            StudentPassword = reader["StudentPassword"].ToString()
                        };
                    }
                }
            }

            return student;
        }

        public int GetNumberOfStudentsByCourseCode(string courseCode)
        {
            int numberOfStudents = 0;

            string query = @"SELECT COUNT(*) AS NumberOfStudents
                             FROM CourseList
                             WHERE CourseCode = @CourseCode";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseCode", courseCode);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        numberOfStudents = Convert.ToInt32(reader["NumberOfStudents"]);
                    }
                }
            }

            return numberOfStudents;
        }
    }
}
