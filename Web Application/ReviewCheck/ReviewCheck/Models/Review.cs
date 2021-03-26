using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReviewCheck.Models
{
    public class Review
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public string Text { get; set; }
        public DateTime SubmissionDate { get; set; }
        public bool IsPositive { get; set; }
        public string Result { get; set; }
    }
}