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
                    'CustomerName': {
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

                    'CustomerContactPhone': {
                        validators: {
                            
                            notEmpty: {
                                message: 'Phone is required'
                            }
                        }
                    },
                    'CustomerContactEmail': {
                        validators: {
                            //emailAddress: {
                            //    message: 'The value is not a valid email address'
                            //},
                            regexp: {
                                regexp: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.(com|org|net|edu|gov|mil|biz|info)$/,
                                message: 'Please enter a valid email address (e.g., example@gmail.com)'
                            },
                            notEmpty: {
                                message: 'Email is required'
                            }
                        }
                    },
                    'CustomerContactAddress1': {
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
                    'CustomerContactCity': {
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
                    'CustomerContactState': {
                        validators: {
                            notEmpty: {
                                message: 'State is required'
                            }
                        }
                    },
                    'CustomerContactZip': {
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
                    },

                    'CustomerTerm': {
                        validators: {
                            stringLength: {
                                max: 20,
                                message: 'Term cannot cannot be more than 20 characters',
                            },
                            notEmpty: {
                                message: 'Term is required'
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
                        var customer = {};
                        customer.CustomerName = form.CustomerName.value;
                        customer.CustomerContactPhone = form.CustomerContactPhone.value;
                        customer.CustomerContactEmail = form.CustomerContactEmail.value;
                        customer.CustomerContactAddress1 = form.CustomerContactAddress1.value;
                        customer.CustomerContactAddress2 = form.CustomerContactAddress2.value;
                        customer.CustomerContactCity = form.CustomerContactCity.value;
                       customer.CustomerContactState = form.CustomerContactState.value;
                       // customer.CustomerContactState = document.getElementById('CustomerContactState').value;

                        customer.CustomerContactZip = form.CustomerContactZip.value;
                        customer.CustomerFEID = form.CustomerFEID.value;
                        customer.CustomerTerm = form.CustomerTerm.value;
                        var actv = $('input#isActive').prop('checked');
                        customer.isActive = actv;

                        customer.CustomerIdPK = form.CustomerIdPK.value;
                        console.log("customer:" + JSON.stringify(customer))
                        $.ajax({
                            type: "POST",
                            url: '/Customer/AddCustomer',
                            data: '{customer: ' + JSON.stringify(customer) + '}',
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