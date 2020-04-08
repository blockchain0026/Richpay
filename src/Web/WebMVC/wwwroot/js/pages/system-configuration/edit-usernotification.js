"use strict";

// Class definition
var KTUserEdit = function () {
	// Base elements
	var avatar;
	var formEl;
	var validator;
	var wizard;


	var initValidation = function () {
		validator = formEl.validate({
			// Validate only visible fields
			//ignore: ":hidden",

			// Validation rules
			rules: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位错误，请更正后再提交。",
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
							"text": "修改成功!",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								window.location.href = "/SystemConfiguration/UserNotification";
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

	return {
		// public functions
		init: function () {
			formEl = $('#kt_system_usernotification_form');

			initValidation();
			initSubmit();
		}
	};
}();

jQuery(document).ready(function () {
	KTUserEdit.init();
});