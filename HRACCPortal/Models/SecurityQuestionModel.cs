using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class SecurityQuestionModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Question { get; set; }
        public string AnswerHash { get; set; }
    }
}