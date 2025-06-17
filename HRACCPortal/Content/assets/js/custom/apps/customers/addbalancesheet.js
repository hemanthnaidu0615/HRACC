"use strict";

// Class definition
var KTModalBalanceSheetsAdd = function () {
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
                            notEmpty: {
                                message: 'Customer name is required'
                            }
                        }
                    },
                    'Year': {
                        validators: {
                            notEmpty: {
                                message: 'Year is required'
                            }
                        }
                    },
                    'Month': {
                        validators: {
                            notEmpty: {
                                message: 'Month is required'
                            }
                        }
                    },
                    'InvoiceNumber': {
                        validators: {
                            regexp: {
                                regexp: /^[0-9]+$/,
                                message: 'Invoice Number must be numeric',
                            },
                            notEmpty: {
                                message: 'Invoice Number is required'
                            }
                        }
                    },
                    'InvoiceAmount': {
                        validators: {
                            regexp: {
                                regexp: /^[0-9]+$/,
                                message: 'Invoice Amount must be numeric',
                            },
                            notEmpty: {
                                message: 'Invoice Amount is required'
                            }
                        }
                    },
                    'PaymentReceived': {
                        validators: {
                            regexp: {
                                regexp: /^[0-9]+$/,
                                message: 'Payment Received must be numeric',
                            },
                            notEmpty: {
                                message: 'Payment Received is required'
                            }
                        }
                    },
                    'Balance': {
                        validators: {
                            regexp: {
                                regexp: /^[0-9]+$/,
                                message: 'Balance must be numeric',
                            },
                            notEmpty: {
                                message: 'Balance is required'
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
        //$(form.querySelector('[name="CustomerName"]')).on('change', function() {
        //    // Revalidate the field when an option is chosen
        //    validator.revalidateField('CustomerName');
        //});

        // Action buttons
        submitButton.addEventListener('click', function (e) {
            e.preventDefault();
            /*debugger;*/
            // Validate form before submit
            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {
                        submitButton.setAttribute('data-kt-indicator', 'on');
                        /*debugger;*/
                        // Disable submit button whilst loading
                        var balancesheetData = {};
                        balancesheetData.CustomerIdFK = form.CustomerName.value;
                        balancesheetData.BalanceSheetId = form.BalanceSheetId.value;
                        balancesheetData.Year = form.Year.value;
                        balancesheetData.Month = form.Month.value;
                        balancesheetData.InvoiceNumber = form.InvoiceNumber.value;
                        balancesheetData.InvoiceAmount = form.InvoiceAmount.value;
                        balancesheetData.PaymentReceived = form.PaymentReceived.value;
                        balancesheetData.Balance = form.Balance.value;
                        var actv = $('input#Status').prop('checked');
                        balancesheetData.Status = actv;
                        balancesheetData.InactiveDate = form.InactiveDate.value;
                        balancesheetData.InactiveReason = form.InactiveReason.value;
                        debugger;
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

                        console.log("ConsultantObj:" + JSON.stringify(balancesheetData));

                        $.ajax({
                            type: "POST",
                            //url: '@Url.Action("AddConsultant", "Consultant")',
                            url: '/BalanceSheet/AddBalanceSheet',
                            data: '{bsmodel: ' + JSON.stringify(balancesheetData) + '}',
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            success: function (response) {
                                submitButton.removeAttribute('data-kt-indicator');

                                // alert("Data has been added successfully.");
                                if (response.message == "Success") {
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
            modal = new bootstrap.Modal(document.querySelector('#kt_modal_add_balancesheet'));

            form = document.querySelector('#kt_modal_add_balancesheet_form');
            submitButton = form.querySelector('#kt_modal_add_balancesheet_submit');
            cancelButton = form.querySelector('#kt_modal_add_balancesheet_cancel');
            closeButton = form.querySelector('#kt_modal_add_balancesheet_close');

            handleForm();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTModalBalanceSheetsAdd.init();
});