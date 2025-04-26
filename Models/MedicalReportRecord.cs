using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHMS_Project.Models
{
    public class MedicalReportRecord
    {
        public int MedicalReportRecordId { get; set; }
        public string StudentNo { get; set; }
        public string CourseCode { get; set; }
        public string InstructorName { get; set; }
        public string DiseaseName { get; set; }
        public string IsDiseaseContagious { get; set; }
        public DateTime MedicalReportStartDate { get; set; }
        public DateTime MedicalReportEndDate { get; set; }
        public string MedicalReportImage { get; set; }
        public string MedicalReportStatus { get; set; }
    }
}

