using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHMS_Project.Models
{
    public class Course
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string InstructorName { get; set; }
        public string CourseTermYear { get; set; }
        public string CourseStatus { get; set; }
    }
}