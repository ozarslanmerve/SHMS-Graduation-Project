using SHMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SHMS_Project.Repositories
{
    public class UserRepository
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DESKTOP-D4TKUO0"].ConnectionString;

        public bool Authenticate(string username, string password)
        {
            string query = "SELECT COUNT(*) FROM [User] WHERE UserName = @Username AND UserPassword = @Password";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public string GetUserRole(string username)
        {
            string role = "";

            if (IsAdmin(username))
            {
                role = "Admin";
            }
            else if (IsStudent(username))
            {
                role = "Student";
            }
            else if (IsInstructor(username))
            {
                role = "Instructor";
            }
            else if (IsFacultyHead(username))
            {
                role = "Faculty Head";
            }

            return role;
        }

        public bool IsAdmin(string username)
        {
            string query = "SELECT COUNT(*) FROM Admin WHERE AdminMail = @Username";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool IsStudent(string username)
        {
            string query = "SELECT COUNT(*) FROM Student WHERE StudentMail = @Username";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool IsInstructor(string username)
        {
            string query = "SELECT COUNT(*) FROM Instructor WHERE InstructorMail = @Username";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool IsFacultyHead(string username)
        {
            string query = "SELECT COUNT(*) FROM FacultyHead WHERE FacultyHeadMail = @Username";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public string GetUsernameByEmail(string email)
        {
            string query = "SELECT UserName FROM [User] WHERE Username = @Email";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

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

        public void AddUser(User user, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO [User] (UserName, UserPassword) VALUES (@UserName, @UserPassword)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@UserPassword", user.UserPassword);

                    command.ExecuteNonQuery();
                }

                string roleQuery = string.Empty;
                switch (role)
                {
                    case "Admin":
                        roleQuery = "INSERT INTO Admin (AdminMail, AdminPassword) VALUES (@UserName, @UserPassword)";
                        break;
                    case "Student":
                        roleQuery = "INSERT INTO Student (StudentMail, StudentPassword) VALUES (@UserName, @UserPassword)";
                        break;
                    case "Instructor":
                        roleQuery = "INSERT INTO Instructor (InstructorMail, InstructorPassword) VALUES (@UserName, @UserPassword)";
                        break;
                    case "Faculty Head":
                        roleQuery = "INSERT INTO FacultyHead (FacultyHeadMail, FacultyHeadPassword) VALUES (@UserName, @UserPassword)";
                        break;
                }

                if (!string.IsNullOrEmpty(roleQuery))
                {
                    using (SqlCommand command = new SqlCommand(roleQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", user.UserName);
                        command.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void DeleteUser(string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string deleteUserQuery = "DELETE FROM [User] WHERE UserName = @UserName";
                using (SqlCommand command = new SqlCommand(deleteUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.ExecuteNonQuery();
                }

                string deleteRoleQuery = string.Empty;
                if (IsAdmin(userName))
                {
                    deleteRoleQuery = "DELETE FROM Admin WHERE AdminMail = @UserName";
                }
                else if (IsStudent(userName))
                {
                    deleteRoleQuery = "DELETE FROM Student WHERE StudentMail = @UserName";
                }
                else if (IsInstructor(userName))
                {
                    deleteRoleQuery = "DELETE FROM Instructor WHERE InstructorMail = @UserName";
                }
                else if (IsFacultyHead(userName))
                {
                    deleteRoleQuery = "DELETE FROM FacultyHead WHERE FacultyHeadMail = @UserName";
                }

                if (!string.IsNullOrEmpty(deleteRoleQuery))
                {
                    using (SqlCommand command = new SqlCommand(deleteRoleQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT UserName, UserPassword FROM [User]";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                UserName = reader["UserName"].ToString(),
                                UserPassword = reader["UserPassword"].ToString()
                            });
                        }
                    }
                }
            }
            return users;
        }

        public User GetUserByUsername(string username)
        {
            User user = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT UserName, UserPassword FROM [User] WHERE UserName = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserName = reader["UserName"].ToString(),
                                UserPassword = reader["UserPassword"].ToString()
                            };
                        }
                    }
                }
            }
            return user;
        }

        public void UpdateUser(User user, string role)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateUserQuery = "UPDATE [User] SET UserPassword = @UserPassword WHERE UserName = @UserName";
                using (SqlCommand command = new SqlCommand(updateUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                    command.ExecuteNonQuery();
                }

                string updateRoleQuery = string.Empty;
                switch (role)
                {
                    case "Admin":
                        updateRoleQuery = "UPDATE Admin SET AdminPassword = @UserPassword WHERE AdminMail = @UserName";
                        break;
                    case "Student":
                        updateRoleQuery = "UPDATE Student SET StudentPassword = @UserPassword WHERE StudentMail = @UserName";
                        break;
                    case "Instructor":
                        updateRoleQuery = "UPDATE Instructor SET InstructorPassword = @UserPassword WHERE InstructorMail = @UserName";
                        break;
                    case "Faculty Head":
                        updateRoleQuery = "UPDATE FacultyHead SET FacultyHeadPassword = @UserPassword WHERE FacultyHeadMail = @UserName";
                        break;
                }

                if (!string.IsNullOrEmpty(updateRoleQuery))
                {
                    using (SqlCommand command = new SqlCommand(updateRoleQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", user.UserName);
                        command.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
