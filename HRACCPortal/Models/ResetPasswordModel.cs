using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class ResetPasswordModel
    {
        public bool IsFirstLogin { get; set; }

  
        [EmailAddress]
        public string Email { get; set; }
       

        // Selected question IDs
        [Required(ErrorMessage = "Please select a question.")]
        public string SelectedQuestion1 { get; set; }

        [Required(ErrorMessage = "Please select a question.")]
        public string SelectedQuestion2 { get; set; }

        [Required(ErrorMessage = "Please select a question.")]
        public string SelectedQuestion3 { get; set; }

        // Answers for the selected questions
        [Required(ErrorMessage = "Please provide an answer.")]
        public string Answer1 { get; set; }

        [Required(ErrorMessage = "Please provide an answer.")]
        public string Answer2 { get; set; }

        [Required(ErrorMessage = "Please provide an answer.")]
        public string Answer3 { get; set; }
        public List<SecurityQuestionModel> SecurityQuestions { get; set; }

        // Method to get answer for specific question
        public string GetAnswerForQuestion(int questionId)
        {
            if (questionId == 1)
            {
                return Answer1;  // Ensure Answer1 is not null or empty
            }
            else if (questionId == 2)
            {
                return Answer2;  // Ensure Answer2 is not null or empty
            }
            else if (questionId == 3)
            {
                return Answer3;  // Ensure Answer3 is not null or empty
            }

            return null;  // Return null if the questionId doesn't match
        }
    }
}