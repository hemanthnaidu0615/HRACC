using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRACCPortal.Models
{
    public class LoginModel : LoginObjectModel
    {
        private HRACCDBEntities hRACCDBEntities;

        public LoginModel()
        {
            hRACCDBEntities = new HRACCDBEntities();
        }
        public string ValidateLoginDetails(LoginModel model)
        {
            var loginDetails = hRACCDBEntities.UserDetails.Where(x => x.UserName == model.EmailId).ToList();
            if (loginDetails.Count==1)
            {
                var validateLogin=hRACCDBEntities.UserDetails.Where(x => x.Password == model.Password).ToList();
                if (validateLogin.Count == 1)
                {
                    return "Success";
                }
            }
            return "Failed";
        }
    }
}