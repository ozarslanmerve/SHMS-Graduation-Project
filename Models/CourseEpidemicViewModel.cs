using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHMS_Project.Models
{
    public class CourseEpidemicViewModel
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public double ContagiousPercentage { get; set; }
        public double NonContagiousPercentage { get; set; }
        public double NoMedicalReportPercentage { get; set; }
    }
}
