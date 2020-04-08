"use strict";

// Class definition
var KTUserEdit = function () {
	// Base elements
	var formEl;
	var validator;

	var initValidation = function () {
		validator = formEl.validate({
			// Validate only visible fields
			//ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1

				eachamountlowerlimit: {
					required: true,
					integer: true,
					min: 0,
					max: function () { return $('#each-amount-upper-limit').val(); }
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
				commissioninthousandth: {
					required: true,
					integer: true,
					range: [0, 1000]
				},

				withdrawalopenfrominhour: {
					required: true
				},
				withdrawalopenfrominminute: {
					required: true
				},
				withdrawalopentoinhour: {
					required: true
				},
				withdrawalopentoinminute: {
					required: true
				},

				depositopenfrominhour: {
					required: true
				},
				depositopenfrominminute: {
					required: true
				},
				withdrawalopedepositopentoinhourntoinhour: {
					required: true
				},
				depositopentoinminute: {
					required: true
				}
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
				KTApp.progress(btn);
				//KTApp.block(formEl);

				translateToUTC();

				formEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						//KTApp.unblock(formEl);

						swal.fire({
							"title": "",
							"text": "修改成功！",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								window.location.href = "/SystemConfiguration/WithdrawalAndDeposit";
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

	var translateToLocal = function () {

		//Translate Utc Time.
		var depositOpenFromInHoursInput = formEl.find('[name="depositopenfrominhour"]');
		var depositOpenFromInMinsInput = formEl.find('[name="depositopenfrominminute"]');
		var depositOpenToInHoursInput = formEl.find('[name="depositopentoinhour"]');
		var depositOpenToInMinsInput = formEl.find('[name="depositopentoinminute"]');

		var depositOpenFromInHours = convertUTCHoursToLocalHours(
			depositOpenFromInHoursInput.val(),
			depositOpenFromInMinsInput.val()
		);
		var depositOpenFromInMins = convertUTCMinsToLocalMins(
			depositOpenFromInHoursInput.val(),
			depositOpenFromInMinsInput.val()
		);
		var depositOpenToInHours = convertUTCHoursToLocalHours(
			depositOpenToInHoursInput.val(),
			depositOpenToInMinsInput.val()
		);
		var depositOpenToInMins = convertUTCMinsToLocalMins(
			depositOpenToInHoursInput.val(),
			depositOpenToInMinsInput.val());

		if (depositOpenFromInHours < 10) {
			depositOpenFromInHours = '0' + depositOpenFromInHours;
		}
		if (depositOpenFromInMins < 10) {
			depositOpenFromInMins = '0' + depositOpenFromInMins;
		}
		if (depositOpenToInHours < 10) {
			depositOpenToInHours = '0' + depositOpenToInHours;
		}
		if (depositOpenToInMins < 10) {
			depositOpenToInMins = '0' + depositOpenToInMins;
		}

		depositOpenFromInHoursInput.val(depositOpenFromInHours).change();
		depositOpenFromInMinsInput.val(depositOpenFromInMins).change();
		depositOpenToInHoursInput.val(depositOpenToInHours).change();
		depositOpenToInMinsInput.val(depositOpenToInMins).change();


		var withdrawalOpenFromInHoursInput = formEl.find('[name="withdrawalopenfrominhour"]');
		var withdrawalOpenFromInMinsInput = formEl.find('[name="withdrawalopenfrominminute"]');
		var withdrawalOpenToInHoursInput = formEl.find('[name="withdrawalopentoinhour"]');
		var withdrawalOpenToInMinsInput = formEl.find('[name="withdrawalopentoinminute"]');

		var withdrawalOpenFromInHours = convertUTCHoursToLocalHours(
			withdrawalOpenFromInHoursInput.val(),
			withdrawalOpenFromInMinsInput.val()
		);
		var withdrawalOpenFromInMins = convertUTCMinsToLocalMins(
			withdrawalOpenFromInHoursInput.val(),
			withdrawalOpenFromInMinsInput.val()
		);
		var withdrawalOpenToInHours = convertUTCHoursToLocalHours(
			withdrawalOpenToInHoursInput.val(),
			withdrawalOpenToInMinsInput.val()
		);
		var withdrawalOpenToInMins = convertUTCMinsToLocalMins(
			withdrawalOpenToInHoursInput.val(),
			withdrawalOpenToInMinsInput.val());

		if (withdrawalOpenFromInHours < 10) {
			withdrawalOpenFromInHours = '0' + withdrawalOpenFromInHours;
		}
		if (withdrawalOpenFromInMins < 10) {
			withdrawalOpenFromInMins = '0' + withdrawalOpenFromInMins;
		}
		if (withdrawalOpenToInHours < 10) {
			withdrawalOpenToInHours = '0' + withdrawalOpenToInHours;
		}
		if (withdrawalOpenToInMins < 10) {
			withdrawalOpenToInMins = '0' + withdrawalOpenToInMins;
		}

		withdrawalOpenFromInHoursInput.val(withdrawalOpenFromInHours).change();
		withdrawalOpenFromInMinsInput.val(withdrawalOpenFromInMins).change();
		withdrawalOpenToInHoursInput.val(withdrawalOpenToInHours).change();
		withdrawalOpenToInMinsInput.val(withdrawalOpenToInMins).change();

	}

	var translateToUTC = function () {

		//Translate Utc Time.
		var depositOpenFromInHoursInput = formEl.find('[name="depositopenfrominhour"]');
		var depositOpenFromInMinsInput = formEl.find('[name="depositopenfrominminute"]');
		var depositOpenToInHoursInput = formEl.find('[name="depositopentoinhour"]');
		var depositOpenToInMinsInput = formEl.find('[name="depositopentoinminute"]');

		var depositOpenFromInHours = convertLocalHoursToUTCHours(
			depositOpenFromInHoursInput.val(),
			depositOpenFromInMinsInput.val()
		);
		var depositOpenFromInMins = convertLocalMinsToUTCMins(
			depositOpenFromInHoursInput.val(),
			depositOpenFromInMinsInput.val()
		);
		var depositOpenToInHours = convertLocalHoursToUTCHours(
			depositOpenToInHoursInput.val(),
			depositOpenToInMinsInput.val()
		);
		var depositOpenToInMins = convertLocalMinsToUTCMins(
			depositOpenToInHoursInput.val(),
			depositOpenToInMinsInput.val());

		if (depositOpenFromInHours < 10) {
			depositOpenFromInHours = '0' + depositOpenFromInHours;
		}
		if (depositOpenFromInMins < 10) {
			depositOpenFromInMins = '0' + depositOpenFromInMins;
		}
		if (depositOpenToInHours < 10) {
			depositOpenToInHours = '0' + depositOpenToInHours;
		}
		if (depositOpenToInMins < 10) {
			depositOpenToInMins = '0' + depositOpenToInMins;
		}

		depositOpenFromInHoursInput.val(depositOpenFromInHours).change();
		depositOpenFromInMinsInput.val(depositOpenFromInMins).change();
		depositOpenToInHoursInput.val(depositOpenToInHours).change();
		depositOpenToInMinsInput.val(depositOpenToInMins).change();


		var withdrawalOpenFromInHoursInput = formEl.find('[name="withdrawalopenfrominhour"]');
		var withdrawalOpenFromInMinsInput = formEl.find('[name="withdrawalopenfrominminute"]');
		var withdrawalOpenToInHoursInput = formEl.find('[name="withdrawalopentoinhour"]');
		var withdrawalOpenToInMinsInput = formEl.find('[name="withdrawalopentoinminute"]');

		var withdrawalOpenFromInHours = convertLocalHoursToUTCHours(
			withdrawalOpenFromInHoursInput.val(),
			withdrawalOpenFromInMinsInput.val()
		);
		var withdrawalOpenFromInMins = convertLocalMinsToUTCMins(
			withdrawalOpenFromInHoursInput.val(),
			withdrawalOpenFromInMinsInput.val()
		);
		var withdrawalOpenToInHours = convertLocalHoursToUTCHours(
			withdrawalOpenToInHoursInput.val(),
			withdrawalOpenToInMinsInput.val()
		);
		var withdrawalOpenToInMins = convertLocalMinsToUTCMins(
			withdrawalOpenToInHoursInput.val(),
			withdrawalOpenToInMinsInput.val());

		if (withdrawalOpenFromInHours < 10) {
			withdrawalOpenFromInHours = '0' + withdrawalOpenFromInHours;
		}
		if (withdrawalOpenFromInMins < 10) {
			withdrawalOpenFromInMins = '0' + withdrawalOpenFromInMins;
		}
		if (withdrawalOpenToInHours < 10) {
			withdrawalOpenToInHours = '0' + withdrawalOpenToInHours;
		}
		if (withdrawalOpenToInMins < 10) {
			withdrawalOpenToInMins = '0' + withdrawalOpenToInMins;
		}

		withdrawalOpenFromInHoursInput.val(withdrawalOpenFromInHours).change();
		withdrawalOpenFromInMinsInput.val(withdrawalOpenFromInMins).change();
		withdrawalOpenToInHoursInput.val(withdrawalOpenToInHours).change();
		withdrawalOpenToInMinsInput.val(withdrawalOpenToInMins).change();

	}

	return {
		// public functions
		init: function () {
			formEl = $('#kt_system_withdrawalanddeposit_form');

			initValidation();
			initSubmit();

			translateToLocal();
		}
	};
}();

jQuery(document).ready(function () {
	KTUserEdit.init();
});