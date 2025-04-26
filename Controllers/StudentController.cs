using SHMS_Project.Models;
using SHMS_Project.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SHMS_Project.Controllers
{
    public class StudentController : Controller
    {
        private readonly MedicalReportRecordRepository medicalReportRecordRepository = new MedicalReportRecordRepository();
        private readonly CourseRepository courseRepository = new CourseRepository();
        private readonly DiseaseRepository diseaseRepository = new DiseaseRepository();
        private readonly StudentRepository studentRepository = new StudentRepository();
        private readonly InstructorRepository instructorRepository = new InstructorRepository();
        private readonly UserRepository userRepository = new UserRepository();

        public ActionResult StudentPage()
        {
            string username = Session["UserName"].ToString();
            string studentNo = studentRepository.GetCurrentStudentNoByUsername(username);

            List<MedicalReportRecord> medicalReportRecords = medicalReportRecordRepository.GetMedicalReportRecordsByStudentNo(username);
            var onHoldReports = medicalReportRecords.Where(r => r.MedicalReportStatus == "On-hold").ToList();
            var otherReports = medicalReportRecords.Where(r => r.MedicalReportStatus != "On-hold").ToList();

            var viewModel = new StudentPageViewModel
            {
                OnHoldReports = onHoldReports,
                OtherReports = otherReports
            };

            ViewBag.Courses = courseRepository.GetCoursesByStudentNo(studentNo).Select(c => new SelectListItem
            {
                Value = c.CourseCode,
                Text = c.CourseName
            }).ToList();

            ViewBag.Diseases = diseaseRepository.GetAllDiseases().Select(d => new SelectListItem
            {
                Value = d.DiseaseName,
                Text = d.DiseaseName
            }).ToList();

            return View(viewModel);
        }

        public ActionResult StudentCourseListPage()
        {
            string username = Session["UserName"].ToString();
            List<CourseViewModel> courseList = studentRepository.GetCourseListByStudentEmail(username);
            return View(courseList);
        }

        public ActionResult StudentHomePage()
        {
            string username = Session["UserName"].ToString();
            string studentNo = studentRepository.GetCurrentStudentNoByUsername(username);
            string studentName = studentRepository.GetStudentNameByStudentNo(studentNo);

            ViewBag.StudentName = studentName;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadMedicalReport(MedicalReportRecord medicalReportRecord, HttpPostedFileBase MedicalReportImage)
        {
            string username = Session["UserName"].ToString();
            string studentNo = studentRepository.GetCurrentStudentNoByUsername(username);

            if (ModelState.IsValid && MedicalReportImage != null && MedicalReportImage.ContentLength > 0 && MedicalReportImage.ContentType.Contains("image"))
            {
                medicalReportRecord.MedicalReportStatus = "On-hold";
                medicalReportRecord.StudentNo = studentNo;

                string courseCode = medicalReportRecord.CourseCode;
                Course selectedCourse = courseRepository.GetCourseByCode(courseCode);
                medicalReportRecord.CourseCode = selectedCourse.CourseCode;

                string instructorName = selectedCourse.InstructorName;
                medicalReportRecord.InstructorName = instructorName;

                string isDiseaseContagious = diseaseRepository.GetIsDiseaseContagiousByName(medicalReportRecord.DiseaseName);
                medicalReportRecord.IsDiseaseContagious = isDiseaseContagious;

                medicalReportRecord.MedicalReportImage = ConvertToBase64(MedicalReportImage);
                medicalReportRecordRepository.AddMedicalReportRecord(medicalReportRecord);

                return RedirectToAction("StudentPage");
            }

            ViewBag.Courses = courseRepository.GetCoursesByStudentNo(studentNo).Select(c => new SelectListItem
            {
                Value = c.CourseCode,
                Text = c.CourseName
            }).ToList();

            ViewBag.Diseases = diseaseRepository.GetAllDiseases().Select(d => new SelectListItem
            {
                Value = d.DiseaseName,
                Text = d.DiseaseName
            }).ToList();

            var onHoldReports = medicalReportRecordRepository.GetMedicalReportRecordsByStatus(studentNo, "On-hold");
            var otherReports = medicalReportRecordRepository.GetMedicalReportRecordsByStatus(studentNo, "Approved", "Rejected");

            var viewModel = new StudentPageViewModel
            {
                OnHoldReports = onHoldReports,
                OtherReports = otherReports
            };

            return View("StudentPage", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMedicalReportRecord(int? medicalReportRecordId)
        {
            if (medicalReportRecordId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                medicalReportRecordRepository.DeleteMedicalReportRecord((int)medicalReportRecordId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetInstructorName(string courseCode)
        {
            string instructorName = courseRepository.GetInstructorNameByCourseCode(courseCode);
            return Content(instructorName);
        }

        [HttpGet]
        public ActionResult FilterMedicalReportsByDateRange(DateTime? startDate, DateTime? endDate)
        {
            string username = Session["UserName"].ToString();
            string studentNo = studentRepository.GetCurrentStudentNoByUsername(username);

            if (!startDate.HasValue)
                startDate = DateTime.MinValue;
            if (!endDate.HasValue)
                endDate = DateTime.MaxValue;

            List<MedicalReportRecord> medicalReportRecords = medicalReportRecordRepository.GetMedicalReportRecordsByDateRange(studentNo, startDate.Value, endDate.Value);
            var onHoldReports = medicalReportRecords.Where(r => r.MedicalReportStatus == "On-hold").ToList();
            var otherReports = medicalReportRecords.Where(r => r.MedicalReportStatus != "On-hold").ToList();

            var viewModel = new StudentPageViewModel
            {
                OnHoldReports = onHoldReports,
                OtherReports = otherReports
            };

            ViewBag.Courses = courseRepository.GetCoursesByStudentNo(studentNo).Select(c => new SelectListItem
            {
                Value = c.CourseCode,
                Text = c.CourseName
            }).ToList();

            ViewBag.Diseases = diseaseRepository.GetAllDiseases().Select(d => new SelectListItem
            {
                Value = d.DiseaseName,
                Text = d.DiseaseName
            }).ToList();

            return View("StudentPage", viewModel);
        }

        private string ConvertToBase64(HttpPostedFileBase file)
        {
            if (file == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                file.InputStream.CopyTo(ms);
                byte[] bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
