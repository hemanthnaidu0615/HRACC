"use strict";


// Class definition
var KTModalEmployersAdd = function () {
    var submitButton;
    var cancelButton;
    var closeButton;
    var validator;
    var form;
    var modal;

    // Init form inputs
    var handleForm = function () {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        debugger;
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'EmployerName': {
                        validators: {
                            stringLength: {
                                max: 60,
                                message: 'The name must be less than 60 characters',
                            },
                            notEmpty: {
                                message: 'Name is required'
                            }
                        }
                    },

                    'EmployerContactPhone': {
                        validators: {
                            notEmpty: {
                                message: 'Phone is required'
                            }
                        }
                    },
                    'EmployerContactEmail': {
                        validators: {
                            //emailAddress: {
                            //    message: 'The value is not a valid email address'
                            //},
                            //notEmpty: {
                            //    message: 'Email is required'
                            //}
                            

                            regexp: {
                                regexp: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.(com|org|net|edu|gov|mil|biz|info)$/,
                                message: 'Please enter a valid email address (e.g., example@gmail.com)'
                            },
                            notEmpty: {
                                message: 'Email is required'
                            }
                        }
                    },
                    'EmployerContactAddress1': {
                        validators: {

                            stringLength: {
                                max: 100,
                                message: 'Address cannot cannot be more than 100 characters',
                            },
                            notEmpty: {
                                message: 'Address is required'
                            }
                        }
                    },
                    'EmployerContactCity': {
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
                    'EmployerContactState': {
                        validators: {
                            notEmpty: {
                                message: 'State is required'
                            }
                        }
                    },
                    'EmployerContactZip': {
                        validators: {
                            regexp: {
                                regexp: /^\d{5}$/,
                                message: 'The US zip code must contain 5 digits',
                            },
                            notEmpty: {
                                message: 'ZipCode is required'
                            }
                        }
                    },
                    'EmployerFEID': {
                        validators: {
                            stringLength: {
                                max: 20,
                                message: 'FEID cannot be more than 20 characters & must be alphanumeric',
                            },
                            notEmpty: {
                                message: 'FEID is required'
                            }
                        }
                    }

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
            //debugger;
            // Validate form before submit
            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {


                        submitButton.setAttribute('data-kt-indicator', 'on');

                        // Disable submit button whilst loading
                        //submitButton.disabled = true;
                        var employer = {};
                        
                        employer.EmployerName = form.EmployerName.value;
                        employer.EmployerContactPhone = form.EmployerContactPhone.value;
                        employer.EmployerContactEmail = form.EmployerContactEmail.value;
                        employer.EmployerContactAddress1 = form.EmployerContactAddress1.value;
                        employer.EmployerContactAddress2 = form.EmployerContactAddress2.value;
                        employer.EmployerContactCity = form.EmployerContactCity.value;
                        employer.EmployerContactState = form.EmployerContactState.value;
                        employer.EmployerContactZip = form.EmployerContactZip.value;
                        employer.EmployerFEID = form.EmployerFEID.value;
                        var actv = $('input#isActive').prop('checked');
                        employer.isActive = actv;

                        employer.EmployerIdPK = form.EmployerIdPK.value;
                        console.log("employer:" + JSON.stringify(employer))
                        $.ajax({
                            type: "POST",
                            url: '/Employer/AddEmployer',
                            data: '{employer: ' + JSON.stringify(employer) + '}',
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
                                            debugger
                                            if (response.redirectUrl != "old") {
                                                window.location = response.redirectUrl;
                                            }
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
                            text: "Sorry, looks like there are some errors detected,Check if all Mandatory fields are present , please try again.",
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

        //function isFormDirty(form) {
        //    debugger
        //    // Check if any input, textarea, or select field in the form has a value
        //    //const formElements = Array.from(form.elements);
        //    //return formElements.some(element => {
        //    //    debugger
        //    //    // Consider different types of form elements
        //    //    if (element.tagName === "INPUT" || element.tagName === "TEXTAREA" || element.tagName === "SELECT") {
        //    //        debugger
        //    //        return element.value.trim() !== ""; // Check if value is not empty
        //    //    }
        //    //    return false; // Ignore elements that are not form inputs



        //    });
        //}



        function isFormDirty(form) {
            debugger
            const formElements = Array.from(form.elements);
            return formElements.some(element => {
                debugger
                // Consider different types of form elements
                if (element.tagName === "INPUT" && element.value.trim() || element.tagName === "TEXTAREA" || element.tagName === "SELECT") {
                    debugger
                    return element.value.trim() !== ""; // Check if value is not empty
                }
                return false; // Ignore elements that are not form inputs



            });
        }



        //cancelButton.addEventListener('click', function (e) {
        //    debugger
        //    e.preventDefault();


        //    const isFormEmpty = Array.from(form.elements).every(element => {
        //        debugger
        //        if (element.innerHTML == "")
        //        {
        //            return true;

        //        }
        //        else {
        //            return false;
        //        }

        //    });

        //    if (isFormEmpty == "false") {
        //        debugger
        //        Swal.fire({
        //            text: "Are you sure you would like to cancel and lose data?",
        //            icon: "warning",
        //            showCancelButton: true,
        //            buttonsStyling: false,
        //            confirmButtonText: "Yes, cancel it!",
        //            cancelButtonText: "No, return",
        //            customClass: {
        //                confirmButton: "btn btn-primary",
        //                cancelButton: "btn btn-success"
        //            }
        //        }).then(function (result) {
        //            if (result.value) {
        //                form.reset(); // Reset form
        //                modal.hide(); // Hide modal
        //            } else if (result.dismiss === 'cancel') {
        //                Swal.fire({
        //                    text: "Your form has not been cancelled!.",
        //                    icon: "error",
        //                    buttonsStyling: false,
        //                    confirmButtonText: "Ok, got it!",
        //                    customClass: {
        //                        confirmButton: "btn btn-primary",
        //                    }
        //                });
        //            }
        //        });
        //    }

        //    else {

        //        modal.hide();
        //    }


        //});


        cancelButton.addEventListener('click', function (e) {
            debugger
            e.preventDefault();

            // Check if the form is empty
            const employerName = document.querySelector('[name="EmployerName"]').value.trim();
            const employerPhone = document.querySelector('[name="EmployerContactPhone"]').value.trim();
            const employerEmail = document.querySelector('[name="EmployerContactEmail"]').value.trim();
            const employeraddress1 = document.querySelector('[name="EmployerContactAddress2"]').value.trim();
            const employeraddress = document.querySelector('[name="EmployerContactAddress1"]').value.trim();
            const employerState = document.querySelector('[name="EmployerContactState"]').value.trim();
            const city = document.querySelector('[name="EmployerContactCity"]').value.trim();
            const employerZip = document.querySelector('[name="EmployerContactZip"]').value.trim();
            const employerFEID = document.querySelector('[name="EmployerFEID"]').value.trim();
            
            // Check if any of the critical fields are filled
            const isFormEmpty = !(employerName || employerPhone || employerEmail || employeraddress1 || employeraddress || city || employerZip || employerFEID);

            if (!isFormEmpty) {
                debugger
                // If the form contains data, show the SweetAlert confirmation dialog
                Swal.fire({
                    text: "Are you sure you would like to cancel and lose data?",
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
                        form.reset(); // Reset the form	
                        modal.hide(); // Hide the modal				
                    } else if (result.dismiss === 'cancel') {
                        Swal.fire({
                            text: "Your form has not been cancelled!",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Ok, got it!",
                            customClass: {
                                confirmButton: "btn btn-primary",
                            }
                        });
                    }
                });
            } else {
                // If the form is empty, directly hide the modal
                modal.hide();

            }
        });



        closeButton.addEventListener('click', function (e) {
            e.preventDefault();
            debugger
            // Check if the form is empty
            const employerName = document.querySelector('[name="EmployerName"]').value.trim();
            const employerPhone = document.querySelector('[name="EmployerContactPhone"]').value.trim();
            const employerEmail = document.querySelector('[name="EmployerContactEmail"]').value.trim();
            const employeraddress1 = document.querySelector('[name="EmployerContactAddress2"]').value.trim();
            const employeraddress = document.querySelector('[name="EmployerContactAddress1"]').value.trim();
            const employerState = document.querySelector('[name="EmployerContactState"]').value.trim();
            const city = document.querySelector('[name="EmployerContactCity"]').value.trim();
            const employerZip = document.querySelector('[name="EmployerContactZip"]').value.trim();
            const employerFEID = document.querySelector('[name="EmployerFEID"]').value.trim();

            // Check if any of the critical fields are filled
            const isFormEmpty = !(employerName || employerPhone || employerEmail || employeraddress1 || employeraddress || city || employerZip || employerFEID);

            if (!isFormEmpty) {
            Swal.fire({
                text: "Are you sure you would like to cancel and lose data?",
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
            } else {
                // If the form is empty, directly hide the modal
                modal.hide();
            }
        })
    }

    return {
        // Public functions
        init: function () {
            // Elements
            modal = new bootstrap.Modal(document.querySelector('#kt_modal_add_employer'));

            form = document.querySelector('#kt_modal_add_employer_form');
            submitButton = form.querySelector('#kt_modal_add_employer_submit');
            cancelButton = form.querySelector('#kt_modal_add_employer_cancel');
            closeButton = form.querySelector('#kt_modal_add_employer_close');

            handleForm();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTModalEmployersAdd.init();
});