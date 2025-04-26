using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHMS_Project.Models
{
    public class CourseStatusChangeRequest
    {
        public int RequestId { get; set; }
        public string InstructorName { get; set; }
        public string CourseCode { get; set; }
        public string RequestedStatus { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApprovalStatus { get; set; }
    }

}