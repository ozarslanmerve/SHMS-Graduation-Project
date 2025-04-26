using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHMS_Project.Models
{
    public class InstructorPageViewModel
    {
        public List<MedicalReportRecord> OnHoldReports { get; set; }
        public List<MedicalReportRecord> OtherReports { get; set; }
    }
}