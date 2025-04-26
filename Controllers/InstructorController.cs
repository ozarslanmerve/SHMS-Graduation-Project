using SHMS_Project.Models;
using SHMS_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SHMS_Project.Controllers
{
    public class InstructorController : Controller
    {
        private readonly MedicalReportRecordRepository medicalReportRecordRepository = new MedicalReportRecordRepository();
        private readonly CourseRepository courseRepository = new CourseRepository();
        private readonly DiseaseRepository diseaseRepository = new DiseaseRepository();
        private readonly StudentRepository studentRepository = new StudentRepository();
        private readonly InstructorRepository instructorRepository = new InstructorRepository();
        private readonly UserRepository userRepository = new UserRepository();
        private readonly CourseStatusChangeRequestRepository courseStatusChangeRequestRepository = new CourseStatusChangeRequestRepository();

        public ActionResult InstructorPage()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string username = Session["UserName"].ToString();
            List<MedicalReportRecord> medicalReportRecords = medicalReportRecordRepository.GetMedicalReportRecordsByInstructorName(username);

            var onHoldReports = medicalReportRecords.Where(r => r.MedicalReportStatus == "On-hold").ToList();
            var otherReports = medicalReportRecords.Where(r => r.MedicalReportStatus != "On-hold").ToList();

            var viewModel = new InstructorPageViewModel
            {
                OnHoldReports = onHoldReports,
                OtherReports = otherReports
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMedicalReportStatus(int medicalReportRecordId, string medicalReportStatus)
        {
            try
            {
                medicalReportRecordRepository.UpdateMedicalReportStatus(medicalReportRecordId, medicalReportStatus);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult EpidemicControl()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string username = Session["UserName"].ToString();
            var instructorName = instructorRepository.GetInstructorNameByUsername(username);
            var courses = courseRepository.GetCoursesByInstructorName(instructorName);
            var viewModel = new List<CourseEpidemicViewModel>();

            foreach (var course in courses)
            {
                var medicalReports = medicalReportRecordRepository.GetMedicalReportRecordsByCourseCodeAndMonth(course.CourseCode, DateTime.Now.Month, DateTime.Now.Year);
                var totalStudents = studentRepository.GetNumberOfStudentsByCourseCode(course.CourseCode);

                var contagiousCount = medicalReports.Count(m => m.IsDiseaseContagious == "Yes");
                var nonContagiousCount = medicalReports.Count(m => m.IsDiseaseContagious == "No");
                var noMedicalReportCount = totalStudents - medicalReports.Count;

                var contagiousPercentage = totalStudents > 0 ? (double)contagiousCount / totalStudents * 100 : 0;
                var nonContagiousPercentage = totalStudents > 0 ? (double)nonContagiousCount / totalStudents * 100 : 0;
                var noMedicalReportPercentage = totalStudents > 0 ? (double)noMedicalReportCount / totalStudents * 100 : 100;

                viewModel.Add(new CourseEpidemicViewModel
                {
                    CourseCode = course.CourseCode,
                    CourseName = course.CourseName,
                    ContagiousPercentage = contagiousPercentage,
                    NonContagiousPercentage = nonContagiousPercentage,
                    NoMedicalReportPercentage = noMedicalReportPercentage
                });
            }

            return View(viewModel);
        }






        public ActionResult StudentDetails(string studentNo)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Student student = studentRepository.GetStudentByStudentNo(studentNo);

            if (student == null)
            {
                return HttpNotFound();
            }

            List<Course> studentCourses = studentRepository.GetCoursesByStudentNo(studentNo);

            var studentViewModel = new StudentViewModel
            {
                StudentName = student.StudentName,
                StudentNo = student.StudentNo,
                StudentMail = student.StudentMail,
                Courses = studentCourses
            };

            return View(studentViewModel);
        }

        public ActionResult CourseStatusRequests()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string username = Session["UserName"].ToString();
            string instructorName = instructorRepository.GetInstructorNameByUsername(username);

            List<CourseStatusChangeRequest> onHoldRequests = courseStatusChangeRequestRepository.GetRequestsByInstructorName(instructorName, "On-Hold");
            List<CourseStatusChangeRequest> approvedRequests = courseStatusChangeRequestRepository.GetRequestsByInstructorName(instructorName, "Approved");
            List<CourseStatusChangeRequest> rejectedRequests = courseStatusChangeRequestRepository.GetRequestsByInstructorName(instructorName, "Rejected");

            var otherRequests = approvedRequests.Concat(rejectedRequests).ToList();

            var viewModel = new CourseStatusRequestsViewModel
            {
                OnHoldRequests = onHoldRequests,
                OtherRequests = otherRequests
            };

            ViewBag.Courses = courseRepository.GetCoursesByInstructorName(instructorName).Select(c => new SelectListItem
            {
                Value = c.CourseCode,
                Text = c.CourseName
            }).ToList();

            return View(viewModel);
        }

        public ActionResult ChangeClassStatus()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string username = Session["UserName"].ToString();
            var instructorName = instructorRepository.GetInstructorNameByUsername(username);
            var courses = courseRepository.GetCoursesByInstructorName(instructorName);

            ViewBag.Courses = courses.Select(c => new SelectListItem
            {
                Value = c.CourseCode,
                Text = c.CourseName
            }).ToList();

            var viewModel = new ChangeClassStatusViewModel
            {
                Courses = ViewBag.Courses
            };

            return View(viewModel);
        }

        public ActionResult InstructorHomePage()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string username = Session["UserName"].ToString();
            string instructorName = instructorRepository.GetInstructorNameByUsername(username);
            ViewBag.InstructorName = instructorName;
            ViewBag.ProjectDescription = "Welcome to the instructor portal where you can manage medical reports, monitor epidemic control, and handle course status requests.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeClassStatus(ChangeClassStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                var request = new CourseStatusChangeRequest
                {
                    InstructorName = instructorRepository.GetInstructorNameByUsername(Session["UserName"].ToString()),
                    CourseCode = model.CourseCode,
                    RequestedStatus = model.Status,
                    RequestDate = DateTime.Now,
                    ApprovalStatus = "On-Hold"
                };

                courseStatusChangeRequestRepository.AddCourseStatusChangeRequest(request);

                return RedirectToAction("CourseStatusRequests");
            }

            string username = Session["UserName"].ToString();
            var instructorName = instructorRepository.GetInstructorNameByUsername(username);
            var courses = courseRepository.GetCoursesByInstructorName(instructorName);

            ViewBag.Courses = courses.Select(c => new SelectListItem
            {
                Value = c.CourseCode,
                Text = c.CourseName
            }).ToList();

            model.Courses = ViewBag.Courses;

            return View(model);
        }
    }
}
