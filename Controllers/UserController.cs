using SHMS_Project.Repositories;
using System.Web.Mvc;

namespace SHMS_Project.Controllers
{
    public class UserController : Controller
    {
        private UserRepository userRepository = new UserRepository();
        private StudentRepository studentRepository = new StudentRepository();

        [HttpPost]
        public ActionResult LoginPage(string email, string password)
        {
            string username = userRepository.GetUsernameByEmail(email);
            bool isAuthenticated = userRepository.Authenticate(username, password);
            if (isAuthenticated)
            {
                Session["UserName"] = username;
                if (userRepository.IsAdmin(username))
                {
                    return RedirectToAction("AdminHomePage", "Admin");
                }
                else if (userRepository.IsStudent(username))
                {
                    string studentNo = studentRepository.GetCurrentStudentNoByEmail(email);
                    return RedirectToAction("StudentHomePage", "Student", new { studentNo });
                }
                else if (userRepository.IsInstructor(username))
                {
                    return RedirectToAction("InstructorHomePage", "Instructor");
                }
                else if (userRepository.IsFacultyHead(username))
                {
                    return RedirectToAction("FacultyHeadHomePage", "FacultyHead");
                }
                else
                {
                    return RedirectToAction("LoginPage", "User");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email address or password";
                return View("LoginPage");
            }
        }

        public ActionResult LoginPage()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("LoginPage", "User");
        }

        public ActionResult AdminPage()
        {
            return View();
        }

       
    }
}
