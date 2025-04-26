using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SHMS_Project.Models;
using SHMS_Project.Repositories;

namespace SHMS_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserRepository userRepository = new UserRepository();

        public ActionResult AdminPage()
        {
            var users = userRepository.GetAllUsers();
            var userRoles = users.Select(u => userRepository.GetUserRole(u.UserName)).ToList();
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        public ActionResult AddUser()
        {
            return View();
        }


        public ActionResult AdminHomePage()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userName = Session["UserName"].ToString();
            ViewBag.UserName = userName;
            ViewBag.ProjectDescription = "Welcome to the Admin Portal. Here you can manage users and oversee the system's operations.";

            return View();
        }

      

        public ActionResult EditUser(string userName)
        {
            var user = userRepository.GetUserByUsername(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User user, string role)
        {
            try
            {
                userRepository.UpdateUser(user, role);
                return RedirectToAction("AdminPage");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }


        public ActionResult GetUser(string userName)
        {
            var user = userRepository.GetUserByUsername(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            var userRole = userRepository.GetUserRole(userName); 
            var result = new
            {
                UserName = user.UserName,
                UserPassword = user.UserPassword,
               
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
