"use strict";

// Class definition
var KTUserEdit = function () {
	// Base elements
	var avatar;
	var formEl;
	var validator;
	var wizard;

	var isPasswordPresent = function () {
		return $('#accountPassword').val().length > 0;
	}

	var initValidation = function () {
		validator = formEl.validate({
			// Validate only visible fields
			//ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1

				fullname: {
					required: true
				},
				phonenumber: {
					required: true
				},
				email: {
					required: true,
					email: true
				},
				managerId: {
					required: true
				},
				username: {
					required: true
				},
				rolename: {
					required: true,
					minlength: 1
				},
				password: {
					minlength: {
						depends: isPasswordPresent,
						param: 6
					}
				},
				rpassword: {
					required: isPasswordPresent,
					minlength: {
						depends: isPasswordPresent,
						param: 6
					},
					equalTo: {
						depends: isPasswordPresent,
						param: "#accountPassword"
					}
				},
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "There are some errors in your submission. Please correct them.",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});
	}

	var initSubmit = function () {
		var btn = formEl.find('[data-ktwizard-type="action-submit"]');

		btn.on('click', function (e) {
			e.preventDefault();

			if (validator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				//KTApp.block(formEl);

				// See: http://malsup.com/jquery/form/#ajaxSubmit
				formEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						//KTApp.unblock(formEl);

						swal.fire({
							"title": "",
							"text": "The application has been successfully submitted!",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								window.location.href = "/Manager";
								//window.location.reload(true);
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						swal.fire({
							"title": "",
							"text": data.responseText,
							"type": "error",
							"buttonStyling": false,
							"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
						});
					}
				});
			}
		});
	}

	var initUserForm = function () {
		avatar = new KTAvatar('kt_user_edit_avatar');
	}

	return {
		// public functions
		init: function () {
			formEl = $('#kt_user_edit_form');

			initValidation();
			initSubmit();
			initUserForm();
		}
	};
}();

jQuery(document).ready(function () {
	KTUserEdit.init();
});