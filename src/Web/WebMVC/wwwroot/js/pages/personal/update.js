"use strict";

// Class definition
var KTUserProfile = function () {
	// Base elements
	var avatar;
	var offcanvas;

	var formEl;
	var validator;

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
				username: {
					required: true,
					minlength: 5,
					maxlength: 100
				},
				password: {
					required: true,
					minlength: 6,
					maxlength: 128
				},

				//Step 3
				eachamountlowerlimit: {
					required: true,
					integer: true,
					min: 0,
					max: function () { return parseInt($('#each-amount-upper-limit').val()); }
				},
				eachamountupperlimit: {
					required: true,
					integer: true
				},
				dailyamountlimit: {
					required: true,
					integer: true,
					min: 0
				},
				dailyfrequencylimit: {
					required: true,
					integer: true,
					min: 0
				},

				//Step 4
				/*password: {
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
				},*/
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "栏位信息有误, 请更正",
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
		var btn = formEl.find('[data-action-type="action-submit"]');

		btn.on('click', function (e) {
			e.preventDefault();

			if (validator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(formEl);

				// See: http://malsup.com/jquery/form/#ajaxSubmit
				formEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(formEl);

						swal.fire({
							"title": "更新成功",
							"text": "账户信息已成功更改",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								window.location.href = "/Personal/Update";
								//window.location.reload(true);
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(formEl);
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

	// Private functions
	var initAside = function () {
		// Mobile offcanvas for mobile mode
		offcanvas = new KTOffcanvas('kt_user_profile_aside', {
			overlay: true,
			baseClass: 'kt-app__aside',
			closeBy: 'kt_user_profile_aside_close',
			toggleBy: 'kt_subheader_mobile_toggle'
		});
	}

	var initUserForm = function () {
		avatar = new KTAvatar('kt_user_avatar');
	}

	return {
		// public functions
		init: function () {
			formEl = $('#user_edit_form');

			initValidation();
			initSubmit();
			initSelect();

			initAside();
			initUserForm();
		}
	};
}();

KTUtil.ready(function () {
	KTUserProfile.init();
});