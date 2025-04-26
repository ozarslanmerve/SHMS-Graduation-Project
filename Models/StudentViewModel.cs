using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace SHMS_Project.Models
{
    public class StudentViewModel
    {
        public string StudentName { get; set; }
        public string StudentNo { get; set; }
        
        public string StudentMail { get; set; }
        public List<Course> Courses { get; set; }
    }
}
