using System.Collections.Generic;
using System.Web.Mvc;

namespace SHMS_Project.Models
{
    public class ChangeClassStatusViewModel
    {
        public string InstructorName { get; set; }  
        public string CourseCode { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> Courses { get; set; }
    }
}
