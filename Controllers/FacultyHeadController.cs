using SHMS_Project.Models;
using SHMS_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SHMS_Project.Controllers
{
    public class FacultyHeadController : Controller
    {
        private readonly CourseStatusChangeRequestRepository courseStatusChangeRequestRepository = new CourseStatusChangeRequestRepository();
        private readonly CourseRepository courseRepository = new CourseRepository();
        private readonly InstructorRepository instructorRepository = new InstructorRepository();
        private readonly FacultyHeadRepository facultyHeadRepository = new FacultyHeadRepository(); 

        public ActionResult FacultyHeadPage()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<CourseStatusChangeRequest> onHoldRequests = courseStatusChangeRequestRepository.GetRequestsByStatus("On-Hold");
            List<CourseStatusChangeRequest> otherRequests = courseStatusChangeRequestRepository.GetRequestsByStatus("Approved")
                .Concat(courseStatusChangeRequestRepository.GetRequestsByStatus("Rejected")).ToList();

            var viewModel = new FacultyHeadPageViewModel
            {
                OnHoldRequests = onHoldRequests,
                OtherRequests = otherRequests
            };

            return View(viewModel);
        }


        public ActionResult FacultyHeadHomePage()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string username = Session["UserName"].ToString();
            ViewBag.FacultyHeadName = facultyHeadRepository.GetFacultyHeadNameByUsername(username); 
            ViewBag.ProjectDescription = "Welcome to the Faculty Head portal. Here you can manage course status requests.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRequestStatus(int requestId, string approvalStatus, string courseCode, string requestedStatus)
        {
            try
            {
                courseStatusChangeRequestRepository.UpdateRequestStatus(requestId, approvalStatus, courseCode, requestedStatus);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
