using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using HRACCPortal.Edmx;
using HRACCPortal.ObjectModel;
using Xamarin.Essentials;
using Contact = HRACCPortal.Edmx.Contact;

namespace HRACCPortal.Models
{
    public class clsCrud
    {
        public HRACCDBEntities entities;
        public CustomerModel customerModel;
        public ConsultantModel consultantModel;
        public InvoiceModel invoiceModel;
        public InvoiceObjectModel invoiceObjectModel;
        public InvoiceGenerationModel invoicegenerationModel;
        public InvoiceGenerationObjectModel invoicegenerationObjectModel;
        public ConsultantPositionDetailsModel consultantPositionDetailsModel;
        public EmployerModel employerModel; //For Employer Table
        public SubContractorModel subcontractorModel; //for subcontractor
      //  public PaymentsReceivedModel PaymentsReceivedModel;
        public EmployeeModel employeeModel;//For Employee Table
        public WorkerModel workerModel;
        public ContactModel contactModel;

        /// </summary>
        public clsCrud()
        {
            customerModel = new CustomerModel();
            consultantModel = new ConsultantModel();
            invoiceModel = new InvoiceModel();
            invoiceObjectModel = new InvoiceObjectModel();
            invoicegenerationModel = new InvoiceGenerationModel();
            invoicegenerationObjectModel = new InvoiceGenerationObjectModel();
            consultantPositionDetailsModel = new ConsultantPositionDetailsModel();
            employerModel = new EmployerModel(); //For Employer Table
            subcontractorModel = new SubContractorModel(); //for subcontractor
            employeeModel = new EmployeeModel();
            workerModel = new WorkerModel();
            contactModel = new ContactModel();
            //  PaymentsReceivedModel = new PaymentsReceivedModel();
            entities = new HRACCDBEntities();
        }
        public List<CustomerModel> CustomerList { get; set; }
        public List<EmployerModel> EmployerList { get; set; } //For Employer Table
        public List<SubContractorModel> SubContractorList { get; set; } // for subcontractor
        public List<ConsultantModel> ConsultantList { get; set; }
        public List<ConsultantPositionDetailsModel> ConsultantPositionDetailsList { get; set; }
        public List<InvoiceSubmissionModel> InvoiceSubmissionList { get; set; }
        public List<EmployeeModel> EmployeeList { get; set; }
        public List<WorkerModel> WorkerList { get; set; }
        public List<ContactModel> ContactList { get; set; }
        // public List<PaymentsReceivedModel> PaymentsReceivedList { get; set; }

        public IEnumerable<SelectListItem> ddlPositionsList
        {
            get
            {
                var customer = entities.Positions.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in customer
                                                    select new SelectListItem
                                                    {
                                                        Text = value.PositionTitle,
                                                        Value = value.PositionIdPK.ToString(),
                                                    };
                return items;
            }
        }
        public IEnumerable<SelectListItem> ddlInvoiceSubmissionList
        {
            get
            {
                var customer = entities.InvoiceSubmissions.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in customer
                                                    select new SelectListItem
                                                    {
                                                        Text = value.Year,
                                                        Value = value.InvoiceSubmissionId.ToString(),
                                                    };
                return items;
            }
        }
        public IEnumerable<SelectListItem> ddlPositionsRateList
        {
            get
            {
                var customer = entities.PositionRates.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in customer
                                                    select new SelectListItem
                                                    {
                                                        Text = value.Rate.ToString(),
                                                        Value = value.PositionRateIdPK.ToString(),
                                                    };
                return items;
            }
        }
        public IEnumerable<SelectListItem> ddlConsultantsList
        {
            get
            {
                var customer = entities.Consultants.AsEnumerable().ToList();
                IEnumerable<SelectListItem> items = from value in customer
                                                    select new SelectListItem
                                                    {
                                                        Text = value.FirstName,
                                                        Value = value.ConsultantIdPK.ToString(),
                                                    };
                return items;
            }
        }

        #region Customer
        public string AddCustomer(CustomerModel customer)
        {
            if (customer.CustomerIdPK > 0)
            {
                var customerModel = entities.Customers.Where(x => x.CustomerIdPK == customer.CustomerIdPK).FirstOrDefault();
                customerModel.AddedBy = "Admin";
                customerModel.CustomerContactAddress1 = customer.CustomerContactAddress1;
                customerModel.CustomerContactAddress2 = customer.CustomerContactAddress2;
                customerModel.CustomerContactCity = customer.CustomerContactCity;
                customerModel.CustomerContactEmail = customer.CustomerContactEmail;
                customerModel.CustomerContactPhone = customer.CustomerContactPhone;
                customerModel.CustomerContactState = customer.CustomerContactState;
                

                customerModel.CustomerContactZip = customer.CustomerContactZip;
                customerModel.CustomerName = customer.CustomerName;
                customerModel.CustomerFEID = customer.CustomerFEID;

                customerModel.CustomerTerm = customer.CustomerTerm;
                customerModel.DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/");
                customerModel.UpdatedBy = "ADMIN";
                customerModel.CustomerIdPK = customer.CustomerIdPK;
                customerModel.isActive = customer.isActive;
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }
            }
            else
            {
                var cons = entities.Consultants.Where(x => x.Email == customer.CustomerContactEmail).ToList();
                if (cons.Count > 0)
                {
                    return "Email already exist";
                }

                Customer scust = new Customer
                {
                    AddedBy = customer.AddedBy,
                    CustomerContactAddress1 = customer.CustomerContactAddress1,
                    CustomerContactAddress2 = customer.CustomerContactAddress2,
                    CustomerContactCity = customer.CustomerContactCity,
                    CustomerContactEmail = customer.CustomerContactEmail,
                    CustomerContactPhone = customer.CustomerContactPhone,
                    CustomerContactState = customer.CustomerContactState,
                    CustomerContactZip = customer.CustomerContactZip,
                    CustomerName = customer.CustomerName,
                    CustomerFEID = customer.CustomerFEID,
                    CustomerTerm = customer.CustomerTerm,
                    CustomerIdPK = customer.CustomerIdPK,
                    DateAdded = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    UpdatedBy = "Admin",
                    isActive = customer.isActive
                };

                entities.Customers.AddObject(scust);
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public void GetCustomers()
        {
            CustomerList = (from g in entities.Customers
                            select g
                               ).AsEnumerable().Select(customer => new CustomerModel
                               {
                                   AddedBy = customer.AddedBy,
                                   CustomerContactAddress1 = customer.CustomerContactAddress1,
                                   CustomerContactAddress2 = customer.CustomerContactAddress2,
                                   CustomerContactCity = customer.CustomerContactCity,
                                   CustomerContactEmail = customer.CustomerContactEmail,
                                   CustomerContactPhone = customer.CustomerContactPhone,
                                   CustomerContactState = customer.CustomerContactState,
                                   CustomerContactZip = customer.CustomerContactZip,
                                   CustomerName = customer.CustomerName,
                                   CustomerFEID = customer.CustomerFEID,
                                   CustomerTerm = customer.CustomerTerm,
                                   DateAdded = customer.DateAdded,
                                   // DateUpdated = Convert.ToDateTime(customer.DateUpdated).ToString("MMM,dd, yyyy"),
                                   DateUpdated = DateTime.TryParse(customer.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                                   // DateUpdated = customer.DateUpdated,
                                   UpdatedBy = customer.UpdatedBy,
                                   CustomerIdPK = customer.CustomerIdPK,
                                   isActive = customer.isActive
                               }).ToList();
        }

        public void GetCustomersForEmployer(string employerEmail)
        {
            CustomerList = (from ce in entities.CustomerEmployers
                            join c in entities.Customers on ce.CustomerIdFK equals c.CustomerIdPK
                            join e in entities.Employers on ce.EmployerIdFK equals e.EmployerIdPK
                            where e.EmployerContactEmail == employerEmail
                            select c)
                            .AsEnumerable()
                            .Select(customer => new CustomerModel
                            {
                                AddedBy = customer.AddedBy,
                                CustomerContactAddress1 = customer.CustomerContactAddress1,
                                CustomerContactAddress2 = customer.CustomerContactAddress2,
                                CustomerContactCity = customer.CustomerContactCity,
                                CustomerContactEmail = customer.CustomerContactEmail,
                                CustomerContactPhone = customer.CustomerContactPhone,
                                CustomerContactState = customer.CustomerContactState,
                                CustomerContactZip = customer.CustomerContactZip,
                                CustomerName = customer.CustomerName,
                                CustomerFEID = customer.CustomerFEID,
                                CustomerTerm = customer.CustomerTerm,
                                DateAdded = customer.DateAdded,
                                DateUpdated = DateTime.TryParse(customer.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                                UpdatedBy = customer.UpdatedBy,
                                CustomerIdPK = customer.CustomerIdPK,
                                isActive = customer.isActive
                            }).ToList();
        }


        public void GetCustomersForEmployee(string employeeEmail)
        {
            CustomerList = (from ee in entities.EmployeeEmployers
                            join ce in entities.CustomerEmployers on ee.EmployerIdFK equals ce.EmployerIdFK
                            join c in entities.Customers on ce.CustomerIdFK equals c.CustomerIdPK
                            join e in entities.Employees on ee.EmployeeIdFK equals e.EmployeeIdPk
                            where e.EmployeeEmail == employeeEmail
                            select c)
                            .Distinct() // Ensure unique customers if an employee has multiple employers
                            .AsEnumerable()
                            .Select(customer => new CustomerModel
                            {
                                AddedBy = customer.AddedBy,
                                CustomerContactAddress1 = customer.CustomerContactAddress1,
                                CustomerContactAddress2 = customer.CustomerContactAddress2,
                                CustomerContactCity = customer.CustomerContactCity,
                                CustomerContactEmail = customer.CustomerContactEmail,
                                CustomerContactPhone = customer.CustomerContactPhone,
                                CustomerContactState = customer.CustomerContactState,
                                CustomerContactZip = customer.CustomerContactZip,
                                CustomerName = customer.CustomerName,
                                CustomerFEID = customer.CustomerFEID,
                                CustomerTerm = customer.CustomerTerm,
                                DateAdded = customer.DateAdded,
                                DateUpdated = DateTime.TryParse(customer.DateUpdated, out var parsedDate)
                                 ? parsedDate.ToString("MMM,dd,yyyy")
                                 : string.Empty,
                                UpdatedBy = customer.UpdatedBy,
                                CustomerIdPK = customer.CustomerIdPK,
                                isActive = customer.isActive
                            }).ToList();
        }

        public CustomerModel GetCustomerById(int id)
        {
            var customer = entities.Customers.Where(x => x.CustomerIdPK == id).FirstOrDefault();
            customerModel.AddedBy = customer.AddedBy;
            customerModel.CustomerContactAddress1 = customer.CustomerContactAddress1;
            customerModel.CustomerContactAddress2 = customer.CustomerContactAddress2;
            customerModel.CustomerContactCity = customer.CustomerContactCity;
            customerModel.CustomerContactEmail = customer.CustomerContactEmail;
            customerModel.CustomerContactPhone = customer.CustomerContactPhone;
            customerModel.CustomerContactState = customer.CustomerContactState;
            customerModel.CustomerContactZip = customer.CustomerContactZip;
            customerModel.CustomerName = customer.CustomerName;
            customerModel.CustomerFEID = customer.CustomerFEID;
            customerModel.CustomerTerm = customer.CustomerTerm;
            customerModel.DateAdded = customer.DateAdded;
            customerModel.DateUpdated = customer.DateUpdated;
            customerModel.UpdatedBy = customer.UpdatedBy;
            customerModel.CustomerIdPK = customer.CustomerIdPK;
            customerModel.isActive = customer.isActive;
            return customerModel;
        }


        #endregion

        

        #region Employer

        public string AddEmployer(EmployerModel employer)
        {
            if (employer.EmployerIdPK > 0)
            {
                var employerModel = entities.Employers.Where(x => x.EmployerIdPK == employer.EmployerIdPK).FirstOrDefault();
                employerModel.AddedBy = "Admin";
                employerModel.EmployerContactAddress1 = employer.EmployerContactAddress1;
                employerModel.EmployerContactAddress2 = employer.EmployerContactAddress2;
                employerModel.EmployerContactCity = employer.EmployerContactCity;
                employerModel.EmployerContactEmail = employer.EmployerContactEmail;
                employerModel.EmployerContactPhone = employer.EmployerContactPhone;
                employerModel.EmployerContactState = employer.EmployerContactState;
                employerModel.EmployerContactZip = employer.EmployerContactZip;
                employerModel.EmployerName = employer.EmployerName;
                employerModel.EmployerFEID = employer.EmployerFEID;
                employerModel.DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/");
                employerModel.UpdatedBy = "ADMIN";
                employerModel.EmployerIdPK = employer.EmployerIdPK;
                employerModel.isActive = employer.isActive;
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }
            }
            else
            {
                var cons = entities.Consultants.Where(x => x.Email == employer.EmployerContactEmail).ToList();
                if (cons.Count > 0)
                {
                    return "Email already exist";
                }

                Employer scust = new Employer
                {
                    AddedBy = employer.AddedBy,
                    EmployerContactAddress1 = employer.EmployerContactAddress1,
                    EmployerContactAddress2 = employer.EmployerContactAddress2,
                    EmployerContactCity = employer.EmployerContactCity,
                    EmployerContactEmail = employer.EmployerContactEmail,
                    EmployerContactPhone = employer.EmployerContactPhone,
                    EmployerContactState = employer.EmployerContactState,
                    EmployerContactZip = employer.EmployerContactZip,
                    EmployerName = employer.EmployerName,
                    EmployerFEID = employer.EmployerFEID,
                    EmployerIdPK = employer.EmployerIdPK,
                    DateAdded = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    UpdatedBy = "Admin",
                    isActive = employer.isActive
                };

                entities.Employers.AddObject(scust);
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public void GetEmployers()
        {
            EmployerList = (from g in entities.Employers
                            select g
                               ).AsEnumerable().Select(employer => new EmployerModel
                               {
                                   AddedBy = employer.AddedBy,
                                   EmployerContactAddress1 = employer.EmployerContactAddress1,
                                   EmployerContactAddress2 = employer.EmployerContactAddress2,
                                   EmployerContactCity = employer.EmployerContactCity,
                                   EmployerContactEmail = employer.EmployerContactEmail,
                                   EmployerContactPhone = employer.EmployerContactPhone,
                                   EmployerContactState = employer.EmployerContactState,
                                   EmployerContactZip = employer.EmployerContactZip,
                                   EmployerName = employer.EmployerName,
                                   EmployerFEID = employer.EmployerFEID,
                                   DateAdded = employer.DateAdded,
                                   // DateUpdated = Convert.ToDateTime(customer.DateUpdated).ToString("MMM,dd, yyyy"),
                                   //DateUpdated = DateTime.Now.ToString("MMM,dd,yyyy"),
                                   DateUpdated = DateTime.TryParse(employer.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,

                                   // DateUpdated = customer.DateUpdated,
                                   UpdatedBy = employer.UpdatedBy,
                                   EmployerIdPK = employer.EmployerIdPK,
                                   isActive = employer.isActive
                               }).ToList();
        }
        public EmployerModel GetEmployerById(int id)
        {
            var employer = entities.Employers.Where(x => x.EmployerIdPK == id).FirstOrDefault();
            employerModel.AddedBy = employer.AddedBy;
            employerModel.EmployerContactAddress1 = employer.EmployerContactAddress1;
            employerModel.EmployerContactAddress2 = employer.EmployerContactAddress2;
            employerModel.EmployerContactCity = employer.EmployerContactCity;
            employerModel.EmployerContactEmail = employer.EmployerContactEmail;
            employerModel.EmployerContactPhone = employer.EmployerContactPhone;
            employerModel.EmployerContactState = employer.EmployerContactState;
            employerModel.EmployerContactZip = employer.EmployerContactZip;
            employerModel.EmployerName = employer.EmployerName;
            employerModel.EmployerFEID = employer.EmployerFEID;
            employerModel.DateAdded = employer.DateAdded;
            employerModel.DateUpdated = employer.DateUpdated;
            employerModel.UpdatedBy = employer.UpdatedBy;
            employerModel.EmployerIdPK = employer.EmployerIdPK;
            employerModel.isActive = employer.isActive;
            return employerModel;
        }

        public EmployerModel GetEmployerByEmail(string email)
        {
            var employer = entities.Employers.Where(x => x.EmployerContactEmail == email).FirstOrDefault();

            // Check if employer exists to prevent null reference issues.
            if (employer == null)
                return null;

            var employerModel = new EmployerModel
            {
                AddedBy = employer.AddedBy,
                EmployerContactAddress1 = employer.EmployerContactAddress1,
                EmployerContactAddress2 = employer.EmployerContactAddress2,
                EmployerContactCity = employer.EmployerContactCity,
                EmployerContactEmail = employer.EmployerContactEmail,
                EmployerContactPhone = employer.EmployerContactPhone,
                EmployerContactState = employer.EmployerContactState,
                EmployerContactZip = employer.EmployerContactZip,
                EmployerName = employer.EmployerName,
                EmployerFEID = employer.EmployerFEID,
                DateAdded = employer.DateAdded,
                 DateUpdated = DateTime.TryParse(employer.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                UpdatedBy = employer.UpdatedBy,
                EmployerIdPK = employer.EmployerIdPK,
                isActive = employer.isActive
            };

            // Initialize the EmployerList for compatibility with the view.
            EmployerList = new List<EmployerModel> { employerModel };

            return employerModel;
        }

        public void GetEmployersForEmployee(string email)
        {
            // Get EmployeeIdPK for the logged-in email
            var employee = entities.Employees.FirstOrDefault(x => x.EmployeeEmail == email);
            if (employee == null)
            {
                EmployerList = new List<EmployerModel>(); // No employers found
                return;
            }

            var employeeIdPK = employee.EmployeeIdPk;

            // Get EmployerIdFKs from EmployeeEmployer table
            var employerIds = entities.EmployeeEmployers
                                      .Where(x => x.EmployeeIdFK == employeeIdPK)
                                      .Select(x => x.EmployerIdFK)
                                      .ToList();

            // Fetch employer details
            EmployerList = entities.Employers
                                   .Where(x => employerIds.Contains(x.EmployerIdPK))
                                   .Select(employer => new EmployerModel
                                   {
                                       EmployerIdPK = employer.EmployerIdPK,
                                       EmployerName = employer.EmployerName,
                                       EmployerContactEmail = employer.EmployerContactEmail,
                                       // Add other fields as needed
                                       AddedBy = employer.AddedBy,
                                       EmployerContactAddress1 = employer.EmployerContactAddress1,
                                       EmployerContactAddress2 = employer.EmployerContactAddress2,
                                       EmployerContactCity = employer.EmployerContactCity,
                                     
                                       EmployerContactPhone = employer.EmployerContactPhone,
                                       EmployerContactState = employer.EmployerContactState,
                                       EmployerContactZip = employer.EmployerContactZip,
                                        
                                       EmployerFEID = employer.EmployerFEID,
                                       DateAdded = employer.DateAdded,
                                       DateUpdated = !string.IsNullOrEmpty(employer.DateUpdated)
                                        ? Convert.ToDateTime(employer.DateUpdated).ToString("MMM,dd,yyyy")
                                        : string.Empty,

                                       // DateUpdated = Convert.ToDateTime(customer.DateUpdated).ToString("MMM,dd, yyyy"),

                                       // DateUpdated = customer.DateUpdated,
                                       UpdatedBy = employer.UpdatedBy,
                                    
                                       isActive = employer.isActive
                                   }).ToList();
        }

        #endregion


        #region Employee

        public string AddEmployee(EmployeeModel employee)
        {
            if (employee.EmployeeIdPk > 0)
            {
              var employeeModel = entities.Employees.Where(x => x.EmployeeIdPk == employee.EmployeeIdPk).FirstOrDefault();
                employeeModel.AddedBy = "Admin";
                employeeModel.EmployeeAddress1 = employee.EmployeeAddress1;
                employeeModel.EmployeeAddress2 = employee.EmployeeAddress2;
                employeeModel.EmployeeCity = employee.EmployeeCity;
                employeeModel.EmployeeEmail = employee.EmployeeEmail;
                employeeModel.EmployeePhone = employee.EmployeePhone;
                employeeModel.EmployeeState = employee.EmployeeState;
                employeeModel.EmployeeType = employee.EmployeeType;
                employeeModel.EmployeeZip = employee.EmployeeZip;
                employeeModel.EmployeeName = employee.EmployeeName;
                employeeModel.EmployeeTitle = employee.EmployeeTitle;
                employeeModel.DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/");
                employeeModel.UpdatedBy = "ADMIN";
                employeeModel.EmployeeIdPk = employee.EmployeeIdPk;
                employeeModel.isActive = employee.isActive;
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }
            }
            else
            {
                var cons = entities.Consultants.Where(x => x.Email == employee.EmployeeEmail).ToList();
                if (cons.Count > 0)
                {
                    return "Email already exist";
                }

                Employee scust = new Employee
                {
                    AddedBy = employee.AddedBy,
                    EmployeeAddress1 = employee.EmployeeAddress1,
                    EmployeeAddress2 = employee.EmployeeAddress2,
                    EmployeeCity = employee.EmployeeCity,
                    EmployeeEmail = employee.EmployeeEmail,
                    EmployeePhone = employee.EmployeePhone,
                    EmployeeState = employee.EmployeeState,
                    EmployeeType = employee.EmployeeType,
                    EmployeeZip = employee.EmployeeZip,
                    EmployeeName = employee.EmployeeName,
                    EmployeeIdPk = employee.EmployeeIdPk,
                    EmployeeTitle = employee.EmployeeTitle,
                    isActive = employee.isActive,
                    DateAdded = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    UpdatedBy = "Admin",
                    
                };

                entities.Employees.AddObject(scust);
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public void GetEmployees()
        {
            EmployeeList = (from g in entities.Employees
                            select g
                               ).AsEnumerable().Select(employee => new EmployeeModel
                               {
                                   AddedBy = employee.AddedBy,
                                   EmployeeAddress1 = employee.EmployeeAddress1,
                                   EmployeeAddress2 = employee.EmployeeAddress2,
                                   EmployeeCity = employee.EmployeeCity,
                                   EmployeeEmail = employee.EmployeeEmail,
                                   EmployeePhone = employee.EmployeePhone,
                                   EmployeeState = employee.EmployeeState,
                                   EmployeeType = employee.EmployeeType,
                                   EmployeeZip = employee.EmployeeZip,
                                   EmployeeName = employee.EmployeeName,
                                   isActive = employee.isActive,
                                   EmployeeTitle = employee.EmployeeTitle,
                                   DateAdded = employee.DateAdded,
                                   // DateUpdated = Convert.ToDateTime(customer.DateUpdated).ToString("MMM,dd, yyyy"),
                                   DateUpdated = DateTime.TryParse(employee.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                                   // DateUpdated = customer.DateUpdated,
                                   UpdatedBy = employee.UpdatedBy,
                                   EmployeeIdPk = employee.EmployeeIdPk,

                               }).ToList();
        }

        public void GetEmployeesForEmployer(string employerEmail)
        {
            // Get EmployerIdPK based on the logged-in employer's email
            var employer = entities.Employers.FirstOrDefault(x => x.EmployerContactEmail == employerEmail);

            if (employer == null)
            {
                EmployeeList = new List<EmployeeModel>(); // No employees found
                return;
            }

            var employerIdPK = employer.EmployerIdPK;

            // Fetch employees for this employer from EmployeeEmployer table
            var employeeIds = entities.EmployeeEmployers
                                      .Where(x => x.EmployerIdFK == employerIdPK)
                                      .Select(x => x.EmployeeIdFK)
                                      .Distinct()
                                      .ToList();

            EmployeeList = entities.Employees
                                   .Where(x => employeeIds.Contains(x.EmployeeIdPk))
                                   .Select(employee => new EmployeeModel
                                   {
                                       EmployeeIdPk = employee.EmployeeIdPk,
                                       EmployeeName = employee.EmployeeName,
                                       EmployeeEmail = employee.EmployeeEmail,
                                       EmployeePhone = employee.EmployeePhone,
                                       EmployeeTitle = employee.EmployeeTitle,
                                       EmployeeAddress1 = employee.EmployeeAddress1,
                                       EmployeeCity = employee.EmployeeCity,
                                       EmployeeState = employee.EmployeeState,
                                       EmployeeZip = employee.EmployeeZip,
                                       isActive = employee.isActive,
                                       DateAdded = employee.DateAdded,
                                       DateUpdated = !string.IsNullOrEmpty(employee.DateUpdated)
                                        ? Convert.ToDateTime(employee.DateUpdated).ToString("MMM,dd,yyyy")
                                        : string.Empty,
                                       UpdatedBy = employee.UpdatedBy,
                                   }).ToList();
        }

        public void GetEmployeesForEmployee(string employeeEmail)
        {
            // Get EmployeeIdPK for the logged-in email
            var employee = entities.Employees.FirstOrDefault(x => x.EmployeeEmail == employeeEmail);

            if (employee == null)
            {
                EmployeeList = new List<EmployeeModel>(); // No employees found
                return;
            }

            var employeeIdPK = employee.EmployeeIdPk;

            // Get EmployerIdFKs from EmployeeEmployer table for this employee
            var employerIds = entities.EmployeeEmployers
                                      .Where(x => x.EmployeeIdFK == employeeIdPK)
                                      .Select(x => x.EmployerIdFK)
                                      .Distinct()
                                      .ToList();

            // Fetch all employees for these employers
            var employeeIds = entities.EmployeeEmployers
                                      .Where(x => employerIds.Contains(x.EmployerIdFK))
                                      .Select(x => x.EmployeeIdFK)
                                      .Distinct()
                                      .ToList();

            EmployeeList = entities.Employees
                                   .Where(x => employeeIds.Contains(x.EmployeeIdPk))
                                   .Select(employees => new EmployeeModel
                                   {
                                       EmployeeIdPk = employees.EmployeeIdPk,
                                       EmployeeName = employees.EmployeeName,
                                       EmployeeEmail = employees.EmployeeEmail,
                                       EmployeePhone = employees.EmployeePhone,
                                       EmployeeTitle = employees.EmployeeTitle,
                                       EmployeeAddress1 = employees.EmployeeAddress1,
                                       EmployeeCity = employees.EmployeeCity,
                                       EmployeeState = employees.EmployeeState,
                                       EmployeeZip = employees.EmployeeZip,
                                       isActive = employees.isActive,
                                       DateAdded = employees.DateAdded,
                                       //   DateUpdated = DateTime.Now.ToString("MMM,dd,yyyy"),
                                       DateUpdated = !string.IsNullOrEmpty(employee.DateUpdated)
                                         ? Convert.ToDateTime(employee.DateUpdated).ToString("MMM,dd,yyyy")
                                         : string.Empty,
                                       UpdatedBy = employees.UpdatedBy,
                                   }).ToList();
        }
        public EmployeeModel GetEmployeeById(int id)
        {
            var employee = entities.Employees.Where(x => x.EmployeeIdPk == id).FirstOrDefault();
            employeeModel.AddedBy = employee.AddedBy;
            employeeModel.EmployeeAddress1 = employee.EmployeeAddress1;
            employeeModel.EmployeeAddress2 = employee.EmployeeAddress2;
            employeeModel.EmployeeCity = employee.EmployeeCity;
            employeeModel.EmployeeEmail = employee.EmployeeEmail;
            employeeModel.EmployeePhone = employee.EmployeePhone;
            employeeModel.EmployeeState = employee.EmployeeState;
            employeeModel.EmployeeType = employee.EmployeeType;
            employeeModel.EmployeeZip = employee.EmployeeZip;
            employeeModel.EmployeeName = employee.EmployeeName;
            employeeModel.EmployeeTitle = employee.EmployeeTitle;
            employeeModel.DateAdded = employee.DateAdded;
            employeeModel.DateUpdated = employee.DateUpdated;
            employeeModel.UpdatedBy = employee.UpdatedBy;
            employeeModel.EmployeeIdPk = employee.EmployeeIdPk;
            employeeModel.isActive = employee.isActive;
            return employeeModel;
        }


        #endregion



        #region SubContractor
        public string AddSubContractor(SubContractorModel subcontractor)
        {
            if (subcontractor.SubContractorIdPK > 0)
            {
                var subcontractorModel = entities.SubContractors.Where(x => x.SubContractorIdPK == subcontractor.SubContractorIdPK).FirstOrDefault();
                subcontractorModel.AddedBy = "Admin";
                subcontractorModel.SubContractorContactAddress1 = subcontractor.SubContractorContactAddress1;
                subcontractorModel.SubContractorContactAddress2 = subcontractor.SubContractorContactAddress2;
                subcontractorModel.SubContractorContactCity = subcontractor.SubContractorContactCity;
                subcontractorModel.SubContractorContactEmail = subcontractor.SubContractorContactEmail;
                subcontractorModel.SubContractorContactPhone = subcontractor.SubContractorContactPhone;
                subcontractorModel.SubContractorContactState = subcontractor.SubContractorContactState;
                subcontractorModel.SubContractorContactZip = subcontractor.SubContractorContactZip;
                subcontractorModel.SubContractorName = subcontractor.SubContractorName;
                subcontractorModel.SubContractorFEID = subcontractor.SubContractorFEID;
                subcontractorModel.SubContractorTerm = subcontractor.SubContractorTerm;
                subcontractorModel.DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/");
                subcontractorModel.UpdatedBy = "ADMIN";
                subcontractorModel.SubContractorIdPK = subcontractor.SubContractorIdPK;
                subcontractorModel.isActive = subcontractor.isActive;
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }
            }
            else
            {
                var cons = entities.Consultants.Where(x => x.Email == subcontractor.SubContractorContactEmail).ToList();
                if (cons.Count > 0)
                {
                    return "Email already exist";
                }

                SubContractor scust = new SubContractor
                {
                    AddedBy = subcontractor.AddedBy,
                    SubContractorContactAddress1 = subcontractor.SubContractorContactAddress1,
                    SubContractorContactAddress2 = subcontractor.SubContractorContactAddress2,
                    SubContractorContactCity = subcontractor.SubContractorContactCity,
                    SubContractorContactEmail = subcontractor.SubContractorContactEmail,
                    SubContractorContactPhone = subcontractor.SubContractorContactPhone,
                    SubContractorContactState = subcontractor.SubContractorContactState,
                    SubContractorContactZip = subcontractor.SubContractorContactZip,
                    SubContractorName = subcontractor.SubContractorName,
                    SubContractorFEID = subcontractor.SubContractorFEID,
                    SubContractorTerm = subcontractor.SubContractorTerm,
                    SubContractorIdPK = subcontractor.SubContractorIdPK,
                    DateAdded = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    UpdatedBy = "Admin",
                    isActive = subcontractor.isActive
                };

                entities.SubContractors.AddObject(scust);
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public void GetSubContractors()
        {
            SubContractorList = (from g in entities.SubContractors
                                 select g
                               ).AsEnumerable().Select(subcontractor => new SubContractorModel
                               {
                                   AddedBy = subcontractor.AddedBy,
                                   SubContractorContactAddress1 = subcontractor.SubContractorContactAddress1,
                                   SubContractorContactAddress2 = subcontractor.SubContractorContactAddress2,
                                   SubContractorContactCity = subcontractor.SubContractorContactCity,
                                   SubContractorContactEmail = subcontractor.SubContractorContactEmail,
                                   SubContractorContactPhone = subcontractor.SubContractorContactPhone,
                                   SubContractorContactState = subcontractor.SubContractorContactState,
                                   SubContractorContactZip = subcontractor.SubContractorContactZip,
                                   SubContractorName = subcontractor.SubContractorName,
                                   SubContractorFEID = subcontractor.SubContractorFEID,
                                   SubContractorTerm = subcontractor.SubContractorTerm,
                                   DateAdded = subcontractor.DateAdded,
                                   // DateUpdated = Convert.ToDateTime(customer.DateUpdated).ToString("MMM,dd, yyyy"),
                                   DateUpdated = DateTime.Now.ToString("MMM,dd,yyyy"),
                                   // DateUpdated = customer.DateUpdated,
                                   UpdatedBy = subcontractor.UpdatedBy,
                                   SubContractorIdPK = subcontractor.SubContractorIdPK,
                                   isActive = subcontractor.isActive,
                               }).ToList();
        }
        public SubContractorModel GetSubContractorById(int id)
        {
            var subcontractor = entities.SubContractors.Where(x => x.SubContractorIdPK == id).FirstOrDefault();
            subcontractorModel.AddedBy = subcontractor.AddedBy;
            subcontractorModel.SubContractorContactAddress1 = subcontractor.SubContractorContactAddress1;
            subcontractorModel.SubContractorContactAddress2 = subcontractor.SubContractorContactAddress2;
            subcontractorModel.SubContractorContactCity = subcontractor.SubContractorContactCity;
            subcontractorModel.SubContractorContactEmail = subcontractor.SubContractorContactEmail;
            subcontractorModel.SubContractorContactPhone = subcontractor.SubContractorContactPhone;
            subcontractorModel.SubContractorContactState = subcontractor.SubContractorContactState;
            subcontractorModel.SubContractorContactZip = subcontractor.SubContractorContactZip;
            subcontractorModel.SubContractorName = subcontractor.SubContractorName;
            subcontractorModel.SubContractorFEID = subcontractor.SubContractorFEID;
            subcontractorModel.SubContractorTerm = subcontractor.SubContractorTerm;
            subcontractorModel.DateAdded = subcontractor.DateAdded;
            subcontractorModel.DateUpdated = subcontractor.DateUpdated;
            subcontractorModel.UpdatedBy = subcontractor.UpdatedBy;
            subcontractorModel.SubContractorIdPK = subcontractor.SubContractorIdPK;
            subcontractorModel.isActive = subcontractor.isActive;
            return subcontractorModel;
        }


        #endregion

        #region Worker

        public string AddWorker(WorkerModel worker)
        {
            if (worker.WorkerIdPK > 0)
            {
                var workerModel = entities.Workers.Where(x => x.WorkerIdPK == worker.WorkerIdPK).FirstOrDefault();
                workerModel.AddedBy = "Admin";
                workerModel.WorkerAddress1 = worker.WorkerAddress1;
                workerModel.WorkerAddress2 = worker.WorkerAddress2;
                workerModel.WorkerCity = worker.WorkerCity;
                workerModel.WorkerEmail = worker.WorkerEmail;
                workerModel.WorkerPhone = worker.WorkerPhone;
                workerModel.WorkerState = worker.WorkerState;


                workerModel.WorkerZip = worker.WorkerZip;
                workerModel.WorkerName = worker.WorkerName;

                workerModel.WorkerTitle = worker.WorkerTitle;
                workerModel.WorkerSalary = worker.WorkerSalary;
                workerModel.DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/");
                workerModel.UpdatedBy = "ADMIN";
                workerModel.WorkerIdPK = worker.WorkerIdPK;
                workerModel.isActive = worker.isActive;
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }
            }
            else
            {
                var cons = entities.Consultants.Where(x => x.Email == worker.WorkerEmail).ToList();
                if (cons.Count > 0)
                {
                    return "Email already exist";
                }

                Worker scust = new Worker
                {
                    AddedBy = worker.AddedBy,
                    WorkerAddress1 = worker.WorkerAddress1,
                    WorkerAddress2 = worker.WorkerAddress2,
                    WorkerCity = worker.WorkerCity,
                    WorkerEmail = worker.WorkerEmail,
                    WorkerPhone = worker.WorkerPhone,
                    WorkerState = worker.WorkerState,
                    WorkerZip = worker.WorkerZip,
                    WorkerName = worker.WorkerName,
                    WorkerTitle = worker.WorkerTitle,
                    WorkerIdPK = worker.WorkerIdPK,
                    WorkerSalary = worker.WorkerSalary,
                    DateAdded = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    UpdatedBy = "Admin",
                    isActive = worker.isActive
                };

                entities.Workers.AddObject(scust);
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public void GetWorkers()
        {
            WorkerList = (from g in entities.Workers
                          select g
                               ).AsEnumerable().Select(worker => new WorkerModel
                               {
                                   AddedBy = worker.AddedBy,
                                   WorkerAddress1 = worker.WorkerAddress1,
                                   WorkerAddress2 = worker.WorkerAddress2,
                                   WorkerCity = worker.WorkerCity,
                                   WorkerEmail = worker.WorkerEmail,
                                   WorkerPhone = worker.WorkerPhone,
                                   WorkerState = worker.WorkerState,
                                   WorkerZip = worker.WorkerZip,
                                   WorkerName = worker.WorkerName,
                                   WorkerTitle = worker.WorkerTitle,
                                   WorkerSalary = worker.WorkerSalary,
                                   DateAdded = worker.DateAdded,
                                   // DateUpdated = Convert.ToDateTime(customer.DateUpdated).ToString("MMM,dd, yyyy"),
                                   DateUpdated = DateTime.Now.ToString("MMM,dd,yyyy"),
                                   // DateUpdated = customer.DateUpdated,
                                   UpdatedBy = worker.UpdatedBy,
                                   WorkerIdPK = worker.WorkerIdPK,
                                   isActive = worker.isActive
                               }).ToList();
        }
        public WorkerModel GetWorkerById(int id)
        {
            var worker = entities.Workers.Where(x => x.WorkerIdPK == id).FirstOrDefault();
            workerModel.AddedBy = worker.AddedBy;
            workerModel.WorkerAddress1 = worker.WorkerAddress1;
            workerModel.WorkerAddress2 = worker.WorkerAddress2;
            workerModel.WorkerCity = worker.WorkerCity;
            workerModel.WorkerEmail = worker.WorkerEmail;
            workerModel.WorkerPhone = worker.WorkerPhone;
            workerModel.WorkerState = worker.WorkerState;
            workerModel.WorkerZip = worker.WorkerZip;
            workerModel.WorkerName = worker.WorkerName;
            workerModel.WorkerTitle = worker.WorkerTitle;
            workerModel.WorkerSalary = worker.WorkerSalary;
            workerModel.DateAdded = worker.DateAdded;
            workerModel.DateUpdated = worker.DateUpdated;
            workerModel.UpdatedBy = worker.UpdatedBy;
            workerModel.WorkerIdPK = worker.WorkerIdPK;
            workerModel.isActive = worker.isActive;
            return workerModel;
        }


        #endregion

        #region Consultant
        public string AddConsultant(ConsultantModel obj)
        {

            if (obj.ConsultantIdPK > 0)
            {
                var obj1 = entities.Consultants.Where(x => x.ConsultantIdPK == obj.ConsultantIdPK).FirstOrDefault();
                obj1.Active = obj.Active;
                obj1.Address1 = obj.Address1;
                obj1.Address2 = obj.Address2;
                obj1.City = obj.City;
                obj1.ConsultantNameAbbrv = obj.ConsultantNameAbbrv;
                obj1.Email = obj.Email;
                obj1.FirstName = obj.FirstName;
                obj1.InactiveDate = obj.InactiveDate;
                obj1.InactiveReason = obj.InactiveReason;
                obj1.LastName = obj.LastName;
                obj1.MiddleName = obj.MiddleName;
                obj1.Phone = obj.Phone;
                obj1.StartDate = obj.StartDate;
                obj1.DateUpdated = DateTime.Now.ToString();
                obj1.UpdatedBy = "Admin";
                obj1.State = obj.State;
                obj1.WorkerType = obj.WorkerType;
                obj1.Title = obj.Title;
                obj1.Zip = obj.Zip;
                obj1.UserName = obj.UserName;
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }

            }
            else
            {
                var cons = entities.Consultants.Where(x => x.Email == obj.Email).ToList();
                if (cons.Count > 0)
                {
                    return "Email already exist";
                }
                Consultant eobj = new Consultant
                {
                    AddedBy = "Admin",
                    Active = obj.Active,
                    Address1 = obj.Address1,
                    Address2 = obj.Address2,
                    City = obj.City,
                    ConsultantNameAbbrv = obj.ConsultantNameAbbrv,
                    Email = obj.Email,
                    FirstName = obj.FirstName,
                    InactiveDate = obj.InactiveDate,
                    InactiveReason = obj.InactiveReason,
                    LastName = obj.LastName,
                    MiddleName = obj.MiddleName,
                    Phone = obj.Phone,
                    StartDate = obj.StartDate,
                    DateAdded = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    UpdatedBy = "Admin",
                    State = obj.State,
                    WorkerType = obj.WorkerType,
                    Title = obj.Title,
                    Zip = obj.Zip,
                    UserName = obj.UserName
                };
                entities.Consultants.AddObject(eobj);

                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public void GetConsultants()
        {
            ConsultantList = (from g in entities.Consultants
                              select g
                               ).AsEnumerable().Select(obj => new ConsultantModel
                               {
                                   AddedBy = obj.AddedBy,
                                   Active = obj.Active,
                                   Address1 = obj.Address1,
                                   Address2 = obj.Address2,
                                   City = obj.City,
                                   ConsultantNameAbbrv = obj.ConsultantNameAbbrv,
                                   Email = obj.Email,
                                   FirstName = obj.FirstName,
                                   InactiveDate = obj.InactiveDate,
                                   InactiveReason = obj.InactiveReason,
                                   LastName = obj.LastName,
                                   MiddleName = obj.MiddleName,
                                   Phone = obj.Phone,
                                   UserName = obj.UserName,
                                   StartDate = obj.StartDate,
                                   DateAdded = obj.DateAdded, //string.IsNullOrEmpty(obj.DateAdded) == true ? DateTime.Now.ToString("MM/dd/yyyy") : DateTime.Parse(obj.DateAdded).ToString("MM/dd/yyyy"),
                                                              //  DateUpdated = obj.DateUpdated,
                                                              //DateUpdated = Convert.ToDateTime(obj.DateUpdated).ToString("MMM,dd, yyyy"),
                                                              //DateUpdated = DateTime.Now.ToString("MMM,dd,yyyy"),
                                   DateUpdated = DateTime.TryParse(obj.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                                   UpdatedBy = obj.UpdatedBy,
                                   State = obj.State,
                                   WorkerType = obj.WorkerType,
                                   Title = obj.Title,
                                   Zip = obj.Zip,
                                   ConsultantIdPK = obj.ConsultantIdPK
                               }).ToList();
        }

        public void GetConsultantsForEmployer(string employerEmail)
        {
            ConsultantList = (from ce in entities.ConsultantEmployers
                              join c in entities.Consultants on ce.ConsultantIdFK equals c.ConsultantIdPK
                              join e in entities.Employers on ce.EmployerIdFK equals e.EmployerIdPK
                              where e.EmployerContactEmail == employerEmail
                              select c)
                              .AsEnumerable()
                              .Select(obj => new ConsultantModel
                              {
                                  AddedBy = obj.AddedBy,
                                  Active = obj.Active,
                                  Address1 = obj.Address1,
                                  Address2 = obj.Address2,
                                  City = obj.City,
                                  ConsultantNameAbbrv = obj.ConsultantNameAbbrv,
                                  Email = obj.Email,
                                  FirstName = obj.FirstName,
                                  InactiveDate = obj.InactiveDate,
                                  InactiveReason = obj.InactiveReason,
                                  LastName = obj.LastName,
                                  MiddleName = obj.MiddleName,
                                  Phone = obj.Phone,
                                  UserName = obj.UserName,
                                  StartDate = obj.StartDate,
                                  DateAdded = obj.DateAdded,
                                  //DateUpdated = DateTime.Now.ToString("MMM,dd,yyyy"),
                                  DateUpdated = DateTime.TryParse(obj.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                                  UpdatedBy = obj.UpdatedBy,
                                  State = obj.State,
                                  WorkerType = obj.WorkerType,
                                  Title = obj.Title,
                                  Zip = obj.Zip,
                                  ConsultantIdPK = obj.ConsultantIdPK
                              }).ToList();
        }

        public void GetConsultantsForEmployee(string employeeEmail)
        {
            ConsultantList = (from ee in entities.EmployeeEmployers
                              join ce in entities.ConsultantEmployers on ee.EmployerIdFK equals ce.EmployerIdFK
                              join c in entities.Consultants on ce.ConsultantIdFK equals c.ConsultantIdPK
                              join e in entities.Employees on ee.EmployeeIdFK equals e.EmployeeIdPk
                              where e.EmployeeEmail == employeeEmail
                              select c)
                              .Distinct() // Ensure unique consultants if an employee has multiple employers
                              .AsEnumerable()
                              .Select(obj => new ConsultantModel
                              {
                                  AddedBy = obj.AddedBy,
                                  Active = obj.Active,
                                  Address1 = obj.Address1,
                                  Address2 = obj.Address2,
                                  City = obj.City,
                                  ConsultantNameAbbrv = obj.ConsultantNameAbbrv,
                                  Email = obj.Email,
                                  FirstName = obj.FirstName,
                                  InactiveDate = obj.InactiveDate,
                                  InactiveReason = obj.InactiveReason,
                                  LastName = obj.LastName,
                                  MiddleName = obj.MiddleName,
                                  Phone = obj.Phone,
                                  UserName = obj.UserName,
                                  StartDate = obj.StartDate,
                                  DateAdded = obj.DateAdded,
                                  //DateUpdated = DateTime.Now.ToString("MMM,dd,yyyy"),
                                  DateUpdated = DateTime.TryParse(obj.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                                  UpdatedBy = obj.UpdatedBy,
                                  State = obj.State,
                                  WorkerType = obj.WorkerType,
                                  Title = obj.Title,
                                  Zip = obj.Zip,
                                  ConsultantIdPK = obj.ConsultantIdPK
                              }).ToList();
        }

        public ConsultantModel GetConsultantById(int id)
        {
            var obj = entities.Consultants.Where(x => x.ConsultantIdPK == id).FirstOrDefault();
            consultantModel.AddedBy = obj.AddedBy;
            consultantModel.Active = obj.Active;
            consultantModel.Address1 = obj.Address1;
            consultantModel.Address2 = obj.Address2;
            consultantModel.City = obj.City;
            consultantModel.ConsultantNameAbbrv = obj.ConsultantNameAbbrv;
            consultantModel.Email = obj.Email;
            consultantModel.FirstName = obj.FirstName;
            consultantModel.InactiveDate = string.IsNullOrEmpty(obj.InactiveDate) == true ? DateTime.Now.ToString("dd/MM/yyyy") : obj.InactiveDate;
            consultantModel.InactiveReason = obj.InactiveReason;
            consultantModel.LastName = obj.LastName;
            consultantModel.MiddleName = obj.MiddleName;
            consultantModel.Phone = obj.Phone;
            consultantModel.StartDate = string.IsNullOrEmpty(obj.StartDate) == true ? DateTime.Now.ToString("dd/MM/yyyy") : obj.StartDate;
            consultantModel.DateAdded = string.IsNullOrEmpty(obj.DateAdded) == true ? DateTime.Now.ToString("dd/MM/yyyy") : obj.DateAdded;
            consultantModel.UserName = obj.UserName;
            //   consultantModel.DateUpdated = obj.DateUpdated;
            // consultantModel.UpdatedBy = obj.UpdatedBy;
            consultantModel.State = obj.State;
            consultantModel.WorkerType = obj.WorkerType;
            consultantModel.Title = obj.Title;
            consultantModel.Zip = obj.Zip;
            consultantModel.ConsultantIdPK = obj.ConsultantIdPK;
            return consultantModel;
        }
        #endregion

        #region Contact

        public string AddContact(ContactModel contact)
        {
            if (contact.ContactIdPK > 0)
            {
                var contactModel = entities.Contacts.Where(x => x.ContactIdPK == contact.ContactIdPK).FirstOrDefault();
                contactModel.AddedBy = "Admin";
                contactModel.ContactAddress1 = contact.ContactAddress1;
                contactModel.ContactAddress2 = contact.ContactAddress2;
                contactModel.ContactCity = contact.ContactCity;
                contactModel.ContactEmail = contact.ContactEmail;
                contactModel.ContactPhone = contact.ContactPhone;
                contactModel.ContactState = contact.ContactState;
                contactModel.ContactType = contact.ContactType;
                contactModel.ContactZip = contact.ContactZip;
                contactModel.ContactName = contact.ContactName;
                contactModel.ContactFEID = contact.ContactFEID;
                contactModel.DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/");
                contactModel.UpdatedBy = "ADMIN";
                contactModel.ContactIdPK = contact.ContactIdPK;
                contactModel.isActive = contact.isActive;
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }
            }
            else
            {
                var cons = entities.Consultants.Where(x => x.Email == contact.ContactEmail).ToList();
                if (cons.Count > 0)
                {
                    return "Email already exist";
                }

                Contact scust = new Contact
                {
                    AddedBy = contact.AddedBy,
                    ContactAddress1 = contact.ContactAddress1,
                    ContactAddress2 = contact.ContactAddress2,
                    ContactCity = contact.ContactCity,
                    ContactEmail = contact.ContactEmail,
                    ContactPhone = contact.ContactPhone,
                    ContactState = contact.ContactState,
                    ContactType = contact.ContactType,
                    ContactZip = contact.ContactZip,
                    ContactName = contact.ContactName,
                    ContactFEID = contact.ContactFEID,
                    ContactIdPK = contact.ContactIdPK,
                    DateAdded = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    DateUpdated = DateTime.Now.ToString("MM/dd/yyyy").Replace("-", "/"),
                    UpdatedBy = "Admin",
                    isActive = contact.isActive
                };

                entities.Contacts.AddObject(scust);
                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }
        public void GetContacts()
        {
            ContactList = (from g in entities.Contacts
                            select g
                               ).AsEnumerable().Select(contact => new ContactModel
                               {
                                   AddedBy = contact.AddedBy,
                                   ContactAddress1 = contact.ContactAddress1,
                                   ContactAddress2 = contact.ContactAddress2,
                                   ContactCity = contact.ContactCity,
                                   ContactEmail = contact.ContactEmail,
                                   ContactPhone = contact.ContactPhone,
                                   ContactState = contact.ContactState,
                                   ContactType = contact.ContactType,
                                   ContactZip = contact.ContactZip,
                                   ContactName = contact.ContactName,
                                   ContactFEID = contact.ContactFEID,
                                   ContactIdPK = contact.ContactIdPK,

                                   DateAdded = contact.DateAdded,
                                   // DateUpdated = Convert.ToDateTime(customer.DateUpdated).ToString("MMM,dd, yyyy"),
                                   DateUpdated = DateTime.TryParse(contact.DateUpdated, out var parsedDate)
                                    ? parsedDate.ToString("MMM,dd,yyyy")
                                    : string.Empty,
                                   // DateUpdated = customer.DateUpdated,
                                   UpdatedBy = contact.UpdatedBy,
                                   
                                   isActive = contact.isActive
                               }).ToList();
        }
        public ContactModel GetContactById(int id)
        {
            var contact = entities.Contacts.Where(x => x.ContactIdPK == id).FirstOrDefault();
            contactModel.AddedBy = contact.AddedBy;
            contactModel.ContactAddress1 = contact.ContactAddress1;
            contactModel.ContactAddress2 = contact.ContactAddress2;
            contactModel.ContactCity = contact.ContactCity;
            contactModel.ContactEmail = contact.ContactEmail;
            contactModel.ContactPhone = contact.ContactPhone;
            contactModel.ContactState = contact.ContactState;
            contactModel.ContactType = contact.ContactType;
            contactModel.ContactZip = contact.ContactZip;
            contactModel.ContactName = contact.ContactName;
            contactModel.ContactFEID = contact.ContactFEID;
            contactModel.DateAdded = contact.DateAdded;
           contactModel.DateUpdated = contact.DateUpdated;
            contactModel.UpdatedBy = contact.UpdatedBy;
            contactModel.ContactIdPK = contact.ContactIdPK;
            contactModel.isActive = contact.isActive;
            return contactModel;
        }

        #endregion

        #region consultantPositionDetails

        public void GetconsultantPositionDetails()
        {
            ConsultantPositionDetailsList = (from g in entities.ConsultantPositionDetails
                                             select g
                               ).AsEnumerable().Select(obj => new ConsultantPositionDetailsModel
                               {
                                   ConsultantPositionIdPK = obj.ConsultantPositionIdPK,
                                   ConsultantIdFK = obj.ConsultantIdFK,
                                   PositionIdFK = obj.PositionIdFK,
                                   PositionRateIdFK = obj.PositionRateIdFK,
                                   PositionStartDate = obj.PositionStartDate,
                                   PositionEndDate = obj.PositionEndDate,
                                   PositionActive = obj.PositionActive,
                                   AddedBy = obj.AddedBy,
                                   DateAdded = obj.DateAdded, //string.IsNullOrEmpty(obj.DateAdded) == true ? DateTime.Now.ToString("MM/dd/yyyy") : DateTime.Parse(obj.DateAdded).ToString("MM/dd/yyyy"),
                                   //DateUpdated = Convert.ToDateTime(obj.DateUpdated).ToString("MMM,dd, yyyy"),
                                   //chANGED BY UZAIR
                                   DateUpdated = !string.IsNullOrWhiteSpace(obj.DateUpdated) && DateTime.TryParse(obj.DateUpdated, out var dateUpdated)
                                    ? dateUpdated.ToString("MMM, dd, yyyy")
                                    : "N/A",
                                   UpdatedBy = obj.UpdatedBy,

                               }).ToList();
        }

        public ConsultantPositionDetailsModel GetConsultantPositionDetailsById(int id)
        {
            var obj = entities.ConsultantPositionDetails.Where(x => x.ConsultantPositionIdPK == id).FirstOrDefault();
            consultantPositionDetailsModel.AddedBy = obj.AddedBy;
            consultantPositionDetailsModel.PositionActive = obj.PositionActive;
            consultantPositionDetailsModel.PositionStartDate = string.IsNullOrEmpty(obj.PositionStartDate) == true ? DateTime.Now.ToString("dd/MM/yyyy") : obj.PositionStartDate;
            consultantPositionDetailsModel.DateAdded = string.IsNullOrEmpty(obj.DateAdded) == true ? DateTime.Now.ToString("dd/MM/yyyy") : obj.DateAdded;

            consultantPositionDetailsModel.PositionEndDate = obj.PositionEndDate;
            consultantPositionDetailsModel.ConsultantPositionIdPK = obj.ConsultantPositionIdPK;
            consultantPositionDetailsModel.ConsultantIdFK = obj.ConsultantIdFK;
            consultantPositionDetailsModel.PositionIdFK = obj.PositionIdFK;
            consultantPositionDetailsModel.PositionRateIdFK = obj.PositionRateIdFK;
            return consultantPositionDetailsModel;
        }

        public string AddConsultantPositionDetails(ConsultantPositionDetailsModel obj)
        {

            if (obj.ConsultantPositionIdPK > 0)
            {
                var obj1 = entities.ConsultantPositionDetails.Where(x => x.ConsultantPositionIdPK == obj.ConsultantPositionIdPK).FirstOrDefault();
                obj1.PositionActive = obj.PositionActive;
                obj1.AddedBy = "Admin";
                obj1.ConsultantIdFK = obj.ConsultantIdFK;
                obj1.PositionIdFK = obj.PositionIdFK;
                obj1.PositionRateIdFK = obj.PositionRateIdFK;
                obj1.PositionStartDate = obj.PositionStartDate;
                obj1.PositionEndDate = obj.PositionEndDate;
                obj1.DateAdded = DateTime.Now.ToString();
                obj1.DateUpdated = DateTime.Now.ToString();
                obj1.UpdatedBy = "Admin";

                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "updated";
                }
                else
                {
                    return "fail";
                }

            }
            else
            {

                ConsultantPositionDetail eobj = new ConsultantPositionDetail
                {
                    PositionActive = obj.PositionActive,
                    AddedBy = "Admin",
                    ConsultantIdFK = obj.ConsultantIdFK,
                    PositionIdFK = obj.PositionIdFK,
                    PositionRateIdFK = obj.PositionRateIdFK,
                    PositionStartDate = obj.PositionStartDate,
                    PositionEndDate = obj.PositionEndDate,
                    DateAdded = DateTime.Now.ToString(),
                    DateUpdated = DateTime.Now.ToString(),
                    UpdatedBy = "Admin",

                };
                entities.ConsultantPositionDetails.AddObject(eobj);

                int i = entities.SaveChanges();
                if (i > 0)
                {
                    return "success";
                }
                else
                {
                    return "fail";
                }
            }
        }

        #endregion
        #region Invoice pdf

        public InvoicePdfModel GenratePdf(int id)
        {
                InvoicePdfModel invoicePdfModel = new InvoicePdfModel();
            invoicePdfModel.invDetails = (from inv in entities.Invoices
                                          join cpd in entities.ConsultantPositionDetails on inv.ConsultantPositionIdFK equals cpd.ConsultantPositionIdPK
                                          join ct in entities.Consultants on cpd.ConsultantIdFK equals ct.ConsultantIdPK
                                          join p in entities.Positions on cpd.PositionIdFK equals p.PositionIdPK
                                          join pr in entities.PositionRates on p.PositionIdPK equals pr.PositionIdFK
                                          join c in entities.Customers on p.CustomerIdFK equals c.CustomerIdPK
                                          //      join e in entities.Employers on p.EmployerIdFK equals e.EmployerIdPK
                                          where inv.InvoiceIdPK == id
                                          select new { inv, cpd, p, c, ct, pr }).AsEnumerable().Select(x =>     new InvoicePdfModel
                                          {
                                              CustomerName = x.c.CustomerName,
                                              CustomerContactAddress1 = x.c.CustomerContactAddress1,
                                              CustomerContactAddress2 = x.c.CustomerContactAddress2,
                                              CustomerContactCity = x.c.CustomerContactCity,
                                              CustomerContactState = x.c.CustomerContactState,
                                              CustomerContactZip = x.c.CustomerContactZip,
                                              CustomerContactEmail = x.c.CustomerContactEmail,
                                              CustomerContactPhone = x.c.CustomerContactPhone,

                                              FirstName = x.ct.FirstName,
                                              LastName = x.ct.LastName,
                                              MiddleName = x.ct.MiddleName,

                                              PositionTitle = x.p.PositionTitle,
                                              PurchaseOrderNo = x.p.PurchaseOrderNo,
                                              PositionNumber = x.p.PositionNumber,
                                              PositionFamily = x.p.PositionFamily,
                                              PositionScopeVariant = x.p.PositionScopeVariant,

                                              RegularHours = x.inv.RegularHours,
                                              Rate = x.pr.Rate,

                                              TotalAmount = Convert.ToDecimal(x.inv.RegularHours) * Convert.ToDecimal(x.pr.Rate),
                                              InvoiceNo = x.inv.Month + x.inv.Year.Substring(Math.Max(x.inv.Year.Length - 2, 0)) + x.p.PurchaseOrderNo + x.p.PurchaseOrderNo.Substring(Math.Max(x.p.PurchaseOrderNo.Length - 4, 0)),
                                              InvoiceIdPK = x.inv.InvoiceIdPK,

                                              ConsultatntName = x.ct.FirstName + x.ct.LastName,
                                              Month = x.inv.Month,
                                              Year = x.inv.Year,
                                              MonthStartDate = FirstDayOfMonthFromDateTime(x.inv.Year, x.inv.Month),
                                              MonthEndDate = LastDayOfMonthFromDateTime(x.inv.Year, x.inv.Month),
                                              DateAdded = x.inv.DateAdded,
                                              //   InvoiceDate = Convert.ToDateTime(x.inv.InvoiceDate).ToString("MMM,dd, yyyy"),
                                              InvoiceDate = DateTime.Now.ToString("MMM,dd,yyyy"),
                                              DueDate = string.IsNullOrEmpty(x.inv.DueDate) ? "" : DateTime.Now.ToString("MMM,dd,yyyy"),
                                              //  DueDate = string.IsNullOrEmpty(x.inv.DueDate) ? "" : Convert.ToDateTime(x.inv.DueDate).ToString("MMM,dd, yyyy"),
                                              Term = x.c.CustomerTerm,

                                          }).ToList();


            var comapany = entities.Companies.Where(x => x.CompanyIdPK == 1).FirstOrDefault();
            invoicePdfModel.CompanyName = comapany.CompanyName;
            invoicePdfModel.CompanyPhone = comapany.CompanyPhone;
            invoicePdfModel.CompanyEmail = comapany.CompanyEmail;
            invoicePdfModel.CompanyAddress1 = comapany.CompanyAddress1;
            invoicePdfModel.CompanyAddress2 = comapany.CompanyAddress2;
            invoicePdfModel.CompanyCity = comapany.CompanyCity;
            invoicePdfModel.CompanyState = comapany.CompanyState;
            invoicePdfModel.CompanyZip = comapany.CompanyZip;
            invoicePdfModel.CompanyFax = comapany.CompanyFax;
            return invoicePdfModel;
        }
        public string FirstDayOfMonthFromDateTime(string syear, string smonth)
        {
            try
            {
                // Validate and convert inputs to integers
                if (!int.TryParse(syear, out int year) || year < 1)
                {
                    throw new ArgumentException("Invalid year provided.");
                }
                if (!int.TryParse(smonth, out int month) || month < 1 || month > 12)
                {
                    throw new ArgumentException("Invalid month provided.");
                }

                // Create DateTime for the first day of the month
                DateTime firstDayOfTheMonth = new DateTime(year, month, 1);

                // Return formatted date
                return firstDayOfTheMonth.ToString("MMM,dd, yyyy");
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                return $"Error: {ex.Message}";
            }
        }
        public string LastDayOfMonthFromDateTime(string syear, string smonth)
        {
            try
            {
                // Validate and convert inputs to integers
                if (!int.TryParse(syear, out int year) || year < 1)
                {
                    throw new ArgumentException("Invalid year provided.");
                }
                if (!int.TryParse(smonth, out int month) || month < 1 || month > 12)
                {
                    throw new ArgumentException("Invalid month provided.");
                }

                // Calculate the last day of the month
                DateTime firstDayOfTheMonth = new DateTime(year, month, 1);
                DateTime lastDayOfTheMonth = firstDayOfTheMonth.AddMonths(1).AddDays(-1);

                // Return formatted date
                return lastDayOfTheMonth.ToString("MMM,dd, yyyy");
            }
            catch (Exception ex)
            {
                // Handle exceptions gracefully
                return $"Error: {ex.Message}";
            }
        }

        #endregion
    }

}
