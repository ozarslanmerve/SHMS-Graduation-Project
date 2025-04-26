using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace SHMS_Project.Repositories
{
    public class MedicalReportRecordRepository
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;
        private readonly StudentRepository studentRepository = new StudentRepository();
        private readonly InstructorRepository instructorRepository = new InstructorRepository();

        public void AddMedicalReportRecord(MedicalReportRecord medicalReportRecord)
        {
            string query = @"INSERT INTO MedicalReportRecord 
                            (StudentNo, CourseCode, InstructorName, DiseaseName, IsDiseaseContagious, MedicalReportStartDate, MedicalReportEndDate, MedicalReportImage, MedicalReportStatus) 
                            VALUES (@StudentNo, @CourseCode, @InstructorName, @DiseaseName, @IsDiseaseContagious, @MedicalReportStartDate, @MedicalReportEndDate, @MedicalReportImage, @MedicalReportStatus)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", medicalReportRecord.StudentNo);
                    command.Parameters.AddWithValue("@CourseCode", medicalReportRecord.CourseCode);
                    command.Parameters.AddWithValue("@InstructorName", medicalReportRecord.InstructorName);
                    command.Parameters.AddWithValue("@DiseaseName", medicalReportRecord.DiseaseName);
                    command.Parameters.AddWithValue("@IsDiseaseContagious", medicalReportRecord.IsDiseaseContagious);
                    command.Parameters.AddWithValue("@MedicalReportStartDate", medicalReportRecord.MedicalReportStartDate);
                    command.Parameters.AddWithValue("@MedicalReportEndDate", medicalReportRecord.MedicalReportEndDate);
                    command.Parameters.AddWithValue("@MedicalReportImage", medicalReportRecord.MedicalReportImage);
                    command.Parameters.AddWithValue("@MedicalReportStatus", medicalReportRecord.MedicalReportStatus);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<MedicalReportRecord> GetMedicalReportRecordsByStudentNo(string username)
        {
            string studentNo = studentRepository.GetCurrentStudentNoByUsername(username);
            if (studentNo == null)
            {
                return new List<MedicalReportRecord>();
            }

            List<MedicalReportRecord> medicalReportRecords = new List<MedicalReportRecord>();

            string query = @"SELECT MRR.MedicalReportRecordId, MRR.MedicalReportStartDate, MRR.MedicalReportEndDate, MRR.CourseCode,
            MRR.DiseaseName, MRR.MedicalReportStatus, MRR.MedicalReportImage
                     FROM MedicalReportRecord MRR
                     WHERE MRR.StudentNo = @StudentNo";

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
                            MedicalReportRecordId = Convert.ToInt32(reader["MedicalReportRecordId"]),
                            MedicalReportStartDate = Convert.ToDateTime(reader["MedicalReportStartDate"]),
                            MedicalReportEndDate = Convert.ToDateTime(reader["MedicalReportEndDate"]),
                            CourseCode = reader["CourseCode"].ToString(),
                            DiseaseName = reader["DiseaseName"].ToString(),
                            MedicalReportStatus = reader["MedicalReportStatus"].ToString(),
                            MedicalReportImage = reader["MedicalReportImage"].ToString()
                        };
                        medicalReportRecords.Add(medicalReportRecord);
                    }
                }
            }

            return medicalReportRecords;
        }

        public void DeleteMedicalReportRecord(int medicalReportRecordId)
        {
            string query = @"DELETE FROM MedicalReportRecord WHERE MedicalReportRecordId = @MedicalReportRecordId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MedicalReportRecordId", medicalReportRecordId);

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error deleting medical report record:", ex.Message);
                    }
                }
            }
        }

        public List<MedicalReportRecord> GetMedicalReportRecordsByInstructorName(string username)
        {
            List<MedicalReportRecord> medicalReportRecords = new List<MedicalReportRecord>();

            string instructorName = instructorRepository.GetInstructorNameByUsername(username);

            string query = @"SELECT MRR.MedicalReportRecordId, MRR.StudentNo, MRR.MedicalReportStartDate, MRR.MedicalReportEndDate, MRR.CourseCode, MRR.DiseaseName, 
                    MRR.MedicalReportStatus, MRR.MedicalReportImage
                    FROM MedicalReportRecord MRR
                    WHERE MRR.InstructorName = @InstructorName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstructorName", instructorName);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        MedicalReportRecord medicalReportRecord = new MedicalReportRecord
                        {
                            MedicalReportRecordId = Convert.ToInt32(reader["MedicalReportRecordId"]),
                            StudentNo = reader["StudentNo"].ToString(),
                            MedicalReportStartDate = Convert.ToDateTime(reader["MedicalReportStartDate"]),
                            MedicalReportEndDate = Convert.ToDateTime(reader["MedicalReportEndDate"]),
                            CourseCode = reader["CourseCode"].ToString(),
                            DiseaseName = reader["DiseaseName"].ToString(),
                            MedicalReportStatus = reader["MedicalReportStatus"].ToString(),
                            MedicalReportImage = reader["MedicalReportImage"].ToString()
                        };
                        medicalReportRecords.Add(medicalReportRecord);
                    }
                }
            }

            return medicalReportRecords;
        }

        public List<MedicalReportRecord> GetMedicalReportRecordsByStatus(string studentNo, params string[] statuses)
        {
            List<MedicalReportRecord> medicalReportRecords = new List<MedicalReportRecord>();

            string query = @"SELECT MRR.MedicalReportRecordId, MRR.MedicalReportStartDate, MRR.MedicalReportEndDate, MRR.CourseCode, MRR.DiseaseName, MRR.MedicalReportStatus, MRR.MedicalReportImage
                             FROM MedicalReportRecord MRR
                             WHERE MRR.StudentNo = @StudentNo AND MRR.MedicalReportStatus IN (@Statuses)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", studentNo);
                    command.Parameters.AddWithValue("@Statuses", string.Join(",", statuses));
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        MedicalReportRecord record = new MedicalReportRecord
                        {
                            MedicalReportRecordId = Convert.ToInt32(reader["MedicalReportRecordId"]),
                            MedicalReportStartDate = Convert.ToDateTime(reader["MedicalReportStartDate"]),
                            MedicalReportEndDate = Convert.ToDateTime(reader["MedicalReportEndDate"]),
                            CourseCode = reader["CourseCode"].ToString(),
                            DiseaseName = reader["DiseaseName"].ToString(),
                            MedicalReportStatus = reader["MedicalReportStatus"].ToString(),
                            MedicalReportImage = reader["MedicalReportImage"].ToString()
                        };
                        medicalReportRecords.Add(record);
                    }
                }
            }

            return medicalReportRecords;
        }

        public void UpdateMedicalReportStatus(int medicalReportRecordId, string medicalReportStatus)
        {
            string query = @"UPDATE MedicalReportRecord 
                    SET MedicalReportStatus = @MedicalReportStatus
                    WHERE MedicalReportRecordId = @MedicalReportRecordId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MedicalReportStatus", medicalReportStatus);
                    command.Parameters.AddWithValue("@MedicalReportRecordId", medicalReportRecordId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<MedicalReportRecord> GetMedicalReportRecordsByCourseCodeAndMonth(string courseCode, int month, int year)
        {
            List<MedicalReportRecord> medicalReportRecords = new List<MedicalReportRecord>();
            string query = @"SELECT * FROM MedicalReportRecord
                     WHERE CourseCode = @CourseCode
                     AND MONTH(MedicalReportStartDate) = @Month
                     AND YEAR(MedicalReportStartDate) = @Year
                     AND MedicalReportStatus = 'Approved'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseCode", courseCode);
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        MedicalReportRecord record = new MedicalReportRecord
                        {
                            MedicalReportRecordId = Convert.ToInt32(reader["MedicalReportRecordId"]),
                            MedicalReportStartDate = Convert.ToDateTime(reader["MedicalReportStartDate"]),
                            MedicalReportEndDate = Convert.ToDateTime(reader["MedicalReportEndDate"]),
                            CourseCode = reader["CourseCode"].ToString(),
                            DiseaseName = reader["DiseaseName"].ToString(),
                            IsDiseaseContagious = reader["IsDiseaseContagious"].ToString(),
                            MedicalReportStatus = reader["MedicalReportStatus"].ToString(),
                            MedicalReportImage = reader["MedicalReportImage"].ToString()
                        };
                        medicalReportRecords.Add(record);
                    }
                }
            }
            return medicalReportRecords;
        }


        public List<Student> GetStudentsByCourseCode(string courseCode)
        {
            List<Student> studentsWithoutMedicalReport = new List<Student>();

            string query = @"SELECT S.StudentName, S.StudentNo
                     FROM Student S
                     LEFT JOIN MedicalReportRecord MRR ON S.StudentNo = MRR.StudentNo
                     WHERE MRR.StudentNo IS NULL OR MRR.CourseCode != @CourseCode";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseCode", courseCode);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            StudentName = reader["StudentName"].ToString(),
                            StudentNo = reader["StudentNo"].ToString()
                        };
                        studentsWithoutMedicalReport.Add(student);
                    }
                }
            }

            return studentsWithoutMedicalReport;
        }

        public List<MedicalReportRecord> GetMedicalReportRecordsByDateRange(string studentNo, DateTime startDate, DateTime endDate)
        {
            List<MedicalReportRecord> medicalReportRecords = new List<MedicalReportRecord>();

            string query = @"SELECT MedicalReportRecordId, MedicalReportStartDate, MedicalReportEndDate, CourseCode, DiseaseName, MedicalReportStatus, MedicalReportImage
                             FROM MedicalReportRecord
                             WHERE StudentNo = @StudentNo AND MedicalReportStartDate >= @StartDate AND MedicalReportEndDate <= @EndDate";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentNo", studentNo);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        MedicalReportRecord record = new MedicalReportRecord
                        {
                            MedicalReportRecordId = Convert.ToInt32(reader["MedicalReportRecordId"]),
                            MedicalReportStartDate = Convert.ToDateTime(reader["MedicalReportStartDate"]),
                            MedicalReportEndDate = Convert.ToDateTime(reader["MedicalReportEndDate"]),
                            CourseCode = reader["CourseCode"].ToString(),
                            DiseaseName = reader["DiseaseName"].ToString(),
                            MedicalReportStatus = reader["MedicalReportStatus"].ToString(),
                            MedicalReportImage = reader["MedicalReportImage"].ToString()
                        };
                        medicalReportRecords.Add(record);
                    }
                }
            }

            return medicalReportRecords;
        }
    }
}
