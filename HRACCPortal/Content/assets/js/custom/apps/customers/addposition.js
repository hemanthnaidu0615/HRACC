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
							notEmpty: {
								message: 'Customer name is required'
							}
						}
					},
                    'PositionNumber': {
                        validators: {
                            regexp: {
                                regexp: /[a-zA-Z0-9]/,
                                message: 'Position number must be alphanumeric',
                            },
                            stringLength: {
                                max: 20,
                                message: 'The position number must be less than 20 characters',
                            },
							notEmpty: {
                                message: 'Position Number is required'
							}
						}
					},
					'PositionTitle': {
						validators: {
							notEmpty: {
                                message: 'Position Title is required'
							}
						}
					},
					'PositionFamily': {
                        validators: {
							notEmpty: {
                               message: 'PositionFamily is required'
							}
						}
					},
					'PositionScopeVariant': {
						validators: {
							notEmpty: {
                                message: 'Position Scope Variant is required'
							}
						}
					},
					'PurchaseOrderNo': {
                        validators: {
                            regexp: {
                                regexp: /[a-zA-Z0-9]/,
                                message: 'Purchase order number must be alphanumeric',
                            },
                            stringLength: {
                                max: 20,
                                message: 'The purchase order number must be less than 20 characters',
                            },
							notEmpty: {
                                message: 'Purchase Order No is required'
							}
						}
					},
					'PurchaseRequisitionNo': {
                        validators: {
                            regexp: {
                                regexp: /[a-zA-Z0-9]/,
                                message: 'Purchase requisition number must be alphanumeric',
                            },
                            stringLength: {
                                max: 20,
                                message: 'The purchase requisition number must be less than 20 characters',
                            },
							notEmpty: {
                                message: 'Purchase Requisition No is required'
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
						var positionData = {};
                        positionData.CustomerIdFK = form.CustomerName.value;
                        positionData.PositionNumber = form.PositionNumber.value;
                        positionData.PositionTitle = form.PositionTitle.value;
                        positionData.PositionFamily = form.PositionFamily.value;
                        positionData.PositionIdPK = form.PositionIdPK.value;
                        positionData.PositionScopeVariant = form.PositionScopeVariant.value;
                        positionData.PurchaseOrderNo = form.PurchaseOrderNo.value;
						var actv = $('input#Status').prop('checked');
						positionData.Status = actv;
                        positionData.PurchaseRequisitionNo = form.PurchaseRequisitionNo.value;
                        positionData.InactiveDate = form.InactiveDate.value;
                        positionData.InactiveReason = form.InactiveReason.value;
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

						console.log("ConsultantObj:" + JSON.stringify(positionData));

                        $.ajax({
                            type: "POST",
                            //url: '@Url.Action("AddConsultant", "Consultant")',
                            url: '/Position/AddPosition',
                            data: '{pmodel: ' + JSON.stringify(positionData) + '}',
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