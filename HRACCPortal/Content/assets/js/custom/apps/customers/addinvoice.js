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
                    'InvoiceNumber': {
						validators: {
							notEmpty: {
								message: 'Invoice number is required'
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
					'InvoiceDate': {
						validators: {
							notEmpty: {
                                message: 'Invoice date is required'
							}
						}
					},
					'DueDate': {
						validators: {
							notEmpty: {
                                message: 'Due date Variant is required'
							}
						}
					},
					'ConsultantIdFK': {
						validators: {
							notEmpty: {
                                message: 'Consultant details is required'
							}
						}
					},
					'ConsultantPositionIdFK': {
						validators: {
							notEmpty: {
                                message: 'Consultant position detail is required'
							}
						}
                    },
                    'RegularHours': {
                        validators: {
                            notEmpty: {
                                message: 'Regular hours is required'
                            }
                        }
                    },
                    'OvertimeHours': {
                        validators: {
                            notEmpty: {
                                message: 'Overtime hours is required'
                            }
                        }
                    },
                    'InvoiceAmount': {
                        validators: {
                            notEmpty: {
                                message: 'Invoice amount is required'
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
            debugger
            // Validate form before submit
            if (validator) {
                validator.validate().then(function (status) {
                    console.log('validated!');

                    if (status == 'Valid') {
                        submitButton.setAttribute('data-kt-indicator', 'on');
                        debugger;
                        // Disable submit button whilst loading
                        var invoiceData = {};
                        invoiceData.InvoiceIdPK = form.InvoiceIdPK.value;
                        invoiceData.ConsultantIdFK = form.Consultant.value;
                        invoiceData.ConsultantPositionIdFK = form.ConsultantPosition.value;
                        invoiceData.InvoiceNumber = form.InvoiceNumber.value;
                        invoiceData.Year = form.Year.value;
                        invoiceData.Month = form.Month.value;
                        invoiceData.RegularHours = form.RegularHours.value;
                        invoiceData.InvoiceAmount = form.InvoiceAmount.value;
                        invoiceData.OvertimeHours = form.OvertimeHours.value;
                        invoiceData.InvoiceDate = form.InvoiceDate.value;
                        invoiceData.DueDate = form.DueDate.value;
                        console.log("ConsultantObj:" + JSON.stringify(invoiceData));

                        $.ajax({
                            type: "POST",
                            //url: '@Url.Action("AddConsultant", "Consultant")',
                            url: '/Invoice/AddEditInvoice',
                            data: '{model: ' + JSON.stringify(invoiceData) + '}',
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            success: function (response) {
                                submitButton.removeAttribute('data-kt-indicator');
                                console.log(response);
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
                                            window.location.href = "/Invoice/GetInvoice?id=" + response.invoiceId;
                                            // Enable submit button after loading
                                            submitButton.disabled = false;

                                            // Redirect to customers list page
                                            //window.location = form.getAttribute("data-kt-redirect");
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
                            text: "Sorry, looks like there are some errors detected,Check if all mandatory fields are filled , please try again.",
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

		closeButton.addEventListener('click', function(e){
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