using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHMS_Project.Models
{
    public class CourseStatusRequestsViewModel
    {
        public List<CourseStatusChangeRequest> OnHoldRequests { get; set; }
        public List<CourseStatusChangeRequest> OtherRequests { get; set; }
        public List<string> InstructorCourses { get; set; }
    }

}
