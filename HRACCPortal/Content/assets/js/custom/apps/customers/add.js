"use strict";

// Class definition
var KTModalCustomersAdd = function () {
    var submitButton;
    var cancelButton;
    var closeButton;
    var validator;
    var form;
    var modal;

    // Init form inputs
    var handleForm = function () {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'FirstName': {
                        validators: {
                            stringLength: {
                                max: 50,
                                message: 'The First Name must be less than 50 characters',
                            },
                            notEmpty: {
                                message: 'First name is required'
                            }
                        }
                    },
                    'LastName': {
                        validators: {
                            stringLength: {
                                max: 50,
                                message: 'The Last Name must be less than 50 characters',
                            },
                            notEmpty: {
                                message: 'Last name is required'
                            }
                        }
                    },
                    'Address1': {
                        validators: {
                            stringLength: {
                                max: 100,
                                message: 'Address cannot be more than 100 characters',
                            },
                            notEmpty: {
                                message: 'Address is required'
                            }
                        }
                    },
                    'ConsultantNameAbbrv': {
                        validators: {
                            stringLength: {
                                max: 30,
                                message: 'The Consultant Name Abbreviation must be less than 30 characters',
                            },
                            notEmpty: {
                                message: 'Consultant Name Abbreviation is required'
                            }
                        }
                    },
                    'Phone': {
                        validators: {
                            notEmpty: {
                                message: 'Phone is required'
                            }
                        }
                    },
                    'Email': {
                        validators: {
                            //emailAddress: {
                            //    message: 'The value is not a valid email address'
                            //},
                            regexp: {
                                regexp: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.(com|org|net|edu|gov|mil|biz|info)$/,
                                message: 'Please enter a valid email address (e.g., demo@gmail.com)'
                            },
                            notEmpty: {
                                message: 'Email is required'
                            }
                        }
                    },
                    'StartDate': {
                        validators: {
                            notEmpty: {
                                message: 'Start Date is required'
                            }
                        }
                    },
                    'City': {
                        validators: {
                            regexp: {
                                regexp: /^[a-zA-z] ?([a-zA-z]|[a-zA-z] ).*[a-zA-z]$/,
                                message: 'Please enter valid city with min 3 characters',
                            },
                            stringLength: {
                                max: 50,
                                message: 'City cannot cannot be more than 50 characters',
                            },
                            notEmpty: {
                                message: 'City is required'
                            }
                        }
                    },
                    'State': {
                        validators: {
                            notEmpty: {
                                message: 'State is required'
                            }
                        }
                    },
                    'Zip': {
                        validators: {
                            regexp: {
                                regexp: /^\d{5}$/,
                                message: 'The US zip code must contain 5 digits',
                            },
                            notEmpty: {
                                message: 'Zip is required'
                            }
                        }
                    },
                    'WorkerType': {
                        validators: {
                            notEmpty: {
                                message: 'WorkerType is required'
                            }
                        }
                    },
                    'Title': {
                        validators: {
                            stringLength: {
                                max: 50,
                                message: 'Title must be less than 50 characters',
                            },
                            notEmpty: {
                                message: 'Title is required'
                            }
                        }
                    },

                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row',
                        eleInvalidClass: '',
                        eleValidClass: ''
                    })
                }
            }
        );

        // Revalidate country field. For more info, plase visit the official plugin site: https://select2.org/
        //$(form.querySelector('[name="country"]')).on('change', function() {
        //    // Revalidate the field when an option is chosen
        //    validator.revalidateField('country');
        //});

        // Action buttons
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            debugger;
            // Validate form before submit
            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {


                        submitButton.setAttribute('data-kt-indicator', 'on');

                        // Disable submit button whilst loading
                        //submitButton.disabled = true;
                        var ConsultantObj = {};
                        ConsultantObj.FirstName = form.FirstName.value;
                        ConsultantObj.LastName = form.LastName.value;
                        ConsultantObj.MiddleName = form.MiddleName.value;
                        ConsultantObj.ConsultantNameAbbrv = form.ConsultantNameAbbrv.value;
                        ConsultantObj.Address1 = form.Address1.value;
                        ConsultantObj.Address2 = form.Address2.value;
                        ConsultantObj.City = form.City.value;
                        ConsultantObj.State = form.State.value;
                        ConsultantObj.Zip = form.Zip.value;
                        ConsultantObj.Phone = form.Phone.value;
                        ConsultantObj.Email = form.Email.value;
                        ConsultantObj.WorkerType = form.WorkerType.value;
                        ConsultantObj.Title = form.Title.value;
                        ConsultantObj.StartDate = form.StartDate.value;
                        ConsultantObj.UserName = form.UserName.value;
                        var actv = $('input#Active').prop('checked');
                        ConsultantObj.Active = actv;

                        if (actv == false && form.InactiveReason.value == "") {
                            $("#divreason").show();
                            submitButton.setAttribute('data-kt-indicator', 'off');
                            return false;
                        } else {
                            $("#divreason").hide();
                        }
                        if (actv == false && form.InactiveDate.value == "") {
                            $("#divdate").show();
                            submitButton.setAttribute('data-kt-indicator', 'off');
                            return false;
                        } else {
                            $("#divdate").hide();
                        }
                        ConsultantObj.InactiveDate = form.InactiveDate.value;
                        ConsultantObj.InactiveReason = form.InactiveReason.value;
                        ConsultantObj.ConsultantIdPK = form.ConsultantIdPK.value;
                        console.log("ConsultantObj:" + JSON.stringify(ConsultantObj))
                        $.ajax({
                            type: "POST",
                            //url: '@Url.Action("AddConsultant", "Consultant")',
                            url: '/Consultant/AddConsultant',
                            data: '{consultant: ' + JSON.stringify(ConsultantObj) + '}',
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            success: function (response) {
                                submitButton.removeAttribute('data-kt-indicator');

                                // alert("Data has been added successfully.");
                                if (response.message == "success" || response.message == "updated") {

                                    Swal.fire({
                                        text: "Form has been successfully submitted!",
                                        icon: "success",
                                        buttonsStyling: false,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: {
                                            confirmButton: "btn btn-primary"
                                        }
                                    }).then(function (result) {
                                        if (result.isConfirmed) {
                                            // Hide modal
                                            modal.hide();

                                            // Enable submit button after loading
                                            submitButton.disabled = false;

                                            // Redirect to customers list page
                                            window.location = form.getAttribute("data-kt-redirect");
                                        }
                                    });
                                } else {
                                    console.log(response.message);
                                    Swal.fire({
                                        text: response.message,
                                        icon: "error",
                                        buttonsStyling: false,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: {
                                            confirmButton: "btn btn-primary"
                                        }
                                    });
                                }
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                submitButton.removeAttribute('data-kt-indicator');

                                console.log("Custom error : " + jqXHR.responseText + " Status: " + textStatus + " Http error:" + errorThrown);

                                Swal.fire({
                                    text: "Custom error : " + jqXHR.responseText + " Status: " + textStatus + " Http error:" + errorThrown,
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: {
                                        confirmButton: "btn btn-primary"
                                    }
                                });

                            }
                        });

                    } else {

                        Swal.fire({
                            text: "Sorry, looks like there are some errors detected,Check if all mandatory fields are filled, please try again.",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn btn-primary"
                            }
                        });
                    }
                });
            }
        });

        cancelButton.addEventListener('click', function (e) {
            e.preventDefault();

            Swal.fire({
                text: "Are you sure you would like to cancel?",
                icon: "warning",
                showCancelButton: true,
                buttonsStyling: false,
                confirmButtonText: "Yes, cancel it!",
                cancelButtonText: "No, return",
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-success"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form	
                    modal.hide(); // Hide modal				
                } else if (result.dismiss === 'cancel') {
                    Swal.fire({
                        text: "Your form has not been cancelled!.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn btn-primary",
                        }
                    });
                }
            });
        });

        closeButton.addEventListener('click', function (e) {
            e.preventDefault();

            Swal.fire({
                text: "Are you sure you would like to cancel?",
                icon: "warning",
                showCancelButton: true,
                buttonsStyling: false,
                confirmButtonText: "Yes, cancel it!",
                cancelButtonText: "No, return",
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: "btn btn-success"
                }
            }).then(function (result) {
                if (result.value) {
                    form.reset(); // Reset form	
                    modal.hide(); // Hide modal				
                } else if (result.dismiss === 'cancel') {
                    Swal.fire({
                        text: "Your form has not been cancelled!.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn btn-primary",
                        }
                    });
                }
            });
        })
    }

    return {
        // Public functions
        init: function () {
            // Elements
            modal = new bootstrap.Modal(document.querySelector('#kt_modal_add_customer'));

            form = document.querySelector('#kt_modal_add_customer_form');
            submitButton = form.querySelector('#kt_modal_add_customer_submit');
            cancelButton = form.querySelector('#kt_modal_add_customer_cancel');
            closeButton = form.querySelector('#kt_modal_add_customer_close');

            handleForm();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTModalCustomersAdd.init();
});