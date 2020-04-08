"use strict";

// Class definition
var KTContactsAdd = function () {
	// Base elements
	var wizardEl;
	var formEl;
	var validator;
	var wizard;
	var avatar;

	// Private functions
	var initWizard = function () {
		// Initialize form wizard
		wizard = new KTWizard('kt_contacts_add', {
			startStep: 1, // initial active step number
			clickableSteps: false  // allow step clicking
		});

		// Validation before going to next page
		wizard.on('beforeNext', function (wizardObj) {
			if (validator.form() !== true) {
				wizardObj.stop();  // don't go to the next step
			}

			// create review data at last step.
			var nextStep = wizardObj.currentStep + 1;
			if (nextStep === wizardObj.totalSteps) {
				//personal info
				var fullNameData = formEl.find('[name ="fullname"]').val();
				var nicknameData = formEl.find('[name ="nickname"]').val();
				var phoneNumData = formEl.find('[name ="phonenumber"]').val();
				var emailData = formEl.find('[name ="email"]').val();
				var wechatData = formEl.find('[name ="wechat"]').val();
				var qqData = formEl.find('[name ="qq"]').val();


				var personalInfo = '真实姓名: &ensp; ' + (fullNameData ? fullNameData : '-');
				personalInfo += '<br /> 昵称: &ensp; ' + (nicknameData ? nicknameData : '-');
				personalInfo += '<br /> 手机号码: &ensp; ' + (phoneNumData ? phoneNumData : '-');
				personalInfo += '<br /> 电子邮件: &ensp; ' + (emailData ? emailData : '-');
				personalInfo += '<br /> 微信: &ensp; ' + (wechatData ? wechatData : '-');
				personalInfo += '<br /> QQ: &ensp; ' + (qqData ? qqData : '-');


				//account data
				var userNameData = formEl.find('[name ="username"]').val();
				var passwordData = formEl.find('[name ="password"]').val();
				var uplineData = formEl.find('[name ="UplineId"]').val();
				var createDownlineData = formEl.find('[name ="HasGrantRight"]').is(':checked');

				var accountInfo = '用户名: &ensp; ' + (userNameData ? userNameData : '-');
				accountInfo += '<br /> 密码: &ensp; ' + (passwordData ? passwordData : '-');
				accountInfo += '<br /> 上级代理: &ensp; ' + (uplineData ? uplineData : '-');
				accountInfo += '<br /> 允许添加下级代理: &ensp; ' + (createDownlineData ? '允许' : '禁止');


				//finance data
				var withdrawalEachLowerLimit = formEl.find('input[name ="eachamountlowerlimit"]').val();
				var withdrawalEachUpperLimit = formEl.find('input[name ="eachamountupperlimit"]').val();
				var withdrawalDailyAmountLimit = formEl.find('input[name ="dailyamountlimit"]').val();
				var withdrawalDailyFrequencyLimit = formEl.find('input[name ="dailyfrequencylimit"]').val();
				var withdrawalRateInThousandth = formEl.find('input[name ="withdrawalcommissionrateinthousandth"]').val();

				var rateAlipay = formEl.find('input[name ="ratealipayinthousandth"]').val();
				var rateWechat = formEl.find('input[name ="ratewechatinthousandth"]').val();



				var riskInfo = '额度: &ensp; 单日收款金额不可超过&nbsp;' + (withdrawalDailyAmountLimit ? withdrawalDailyAmountLimit : '-')
					+ '¥，单日最多提现&nbsp;' + (withdrawalDailyFrequencyLimit ? withdrawalDailyFrequencyLimit : '')
					+ ' 笔，每笔限额&nbsp;' + (withdrawalEachLowerLimit ? withdrawalEachLowerLimit : '-') + '¥&nbsp;~&nbsp;' + (withdrawalEachUpperLimit ? withdrawalEachUpperLimit : '-') + '¥&nbsp;';
				riskInfo += '<br /> 提现手续费: &ensp; 千分之 ' + (withdrawalRateInThousandth ? withdrawalRateInThousandth : '-');
				riskInfo += '<br /> 支付宝费率: &ensp; 千分之 ' + (rateAlipay ? rateAlipay : '-');
				riskInfo += '<br /> 微信费率: &ensp; 千分之 ' + (rateWechat ? rateWechat : '-');


				formEl.find('#create_review_personal_info').html(personalInfo);
				formEl.find('#create_review_account_data').html(accountInfo);
				formEl.find('#create_review_finance').html(riskInfo);
			}
		})

		// Change event
		wizard.on('change', function (wizard) {
			KTUtil.scrollTop();
		});
	}

	var initValidation = function () {
		validator = formEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

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

				//step 2
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
				depositcommissionrateinthousandth: {
					required: true,
					integer: true,
					range: [0, 999]
				},
				withdrawalcommissionrateinthousandth: {
					required: true,
					integer: true,
					range: [0, 999]
				},
				ratealipayinthousandth: {
					required: true,
					integer: true,
					min: 0,
					max: function () {
						return parseInt($('#maxratealipay').val());
					}
				},
				ratewechatinthousandth: {
					required: true,
					integer: true,
					min: 0,
					max: function () {
						return parseInt($('#maxratewechat').val());
					}
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
							"text": "添加成功!请等候后台审核。",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								var userRole = $('#user_role').val();
								if (userRole.length) {
									if (userRole === 'TraderAgent') {
										window.location.href = "/TraderAgent/Downlines";
									}
									else {
										window.location.href = "/TraderAgent";
									}
								}
								else {
									window.location.href = "/TraderAgent";
								}
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

	var initSelect = function () {
		function formatItem(item) {
			if (item.loading) return item.text;
			var markup = "<div class='select2-result-repository clearfix'>" +
				"<div class='select2-result-repository__meta'>" +
				"<div class=''>" + item.fullname + "</div>";
			if (item.username) {
				markup += "<div class='select2-result-repository__description'>" + item.username + "</div>";
			}
			markup += "<div class='select2-result-repository__statistics'>" +
				"<div class='select2-result-repository__forks'><i class='fa fa-star'></i> 支付宝费率: " + item.rate_alipay + " </div>" +
				"<div class='select2-result-repository__stargazers'><i class='fa fa-star'></i> 微信费率: " + item.rate_wechat + " </div>" +
				"<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> 帐户状态: " + item.is_enabled + " </div>" +
				"<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> 审核状态: " + item.is_reviewed + " </div>" +
				"</div>" +
				"</div></div>";
			return markup;
		}

		function formatItemSelection(item) {
			return item.username || item.text;
		}

		$("#kt_select2_6").select2({
			placeholder: "输入代理名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "Search",
				dataType: 'json',
				delay: 250,
				data: function (params) {
					return {
						generalSearch: params.term, // search term
						page: params.page
					};
				},
				processResults: function (data, params) {
					// parse the results into the format expected by Select2
					// since we are using custom formatting functions we do not need to
					// alter the remote JSON data, except to indicate that infinite
					// scrolling can be used
					params.page = params.page || 1;

					return {
						results: $.map(data.data, function (item) {

							//Set max rate to support dynamic validation.
							var alipayRate = item.TradingCommission.RateAlipayInThousandth !== 0 ? item.TradingCommission.RateAlipayInThousandth : 999;
							var wechatRate = item.TradingCommission.RateWechatInThousandth !== 0 ? item.TradingCommission.RateWechatInThousandth : 999;

							$('#maxratealipay').val(alipayRate - 1);
							$('#maxratewechat').val(wechatRate - 1);

							return {
								//Reconstruct the data to adapt to Select2 Api.
								id: item.TraderAgentId,
								text: item.TraderAgentId,
								username: item.Username,
								fullname: item.FullName,
								rate_alipay: alipayRate,
								rate_wechat: wechatRate,
								is_enabled: item.IsEnabled ? "启用" : "停用",
								is_reviewed: item.IsReviewed ? "已通过" : "未通过",
							}
						}),
						pagination: {
							more: (params.page * 10) < data.meta.total
						}
					};
				},
				cache: true
			},
			escapeMarkup: function (markup) {
				return markup;
			}, // let our custom formatter work
			minimumInputLength: 1,
			templateResult: formatItem, // omitted for brevity, see the source of this page
			templateSelection: formatItemSelection // omitted for brevity, see the source of this page
		});
	}

	var initRateAlipaySlider = function () {
		// init slider
		var slider = document.getElementById('kt_nouislider_ratealipay');

		noUiSlider.create(slider, {
			start: [0],
			step: 1,
			range: {
				'min': [0],
				'max': [30]
			},
			format: wNumb({
				decimals: 0
			})
		});

		// init slider input
		var sliderInput = document.getElementById('kt_nouislider_ratealipay_input');

		slider.noUiSlider.on('update', function (values, handle) {
			sliderInput.value = values[handle];
		});

		sliderInput.addEventListener('change', function () {
			slider.noUiSlider.set(this.value);
		});
	}

	var initRateWechatSlider = function () {
		// init slider
		var slider = document.getElementById('kt_nouislider_ratewechat');

		noUiSlider.create(slider, {
			start: [0],
			step: 1,
			range: {
				'min': [0],
				'max': [30]
			},
			format: wNumb({
				decimals: 0
			})
		});

		// init slider input
		var sliderInput = document.getElementById('kt_nouislider_ratewechat_input');

		slider.noUiSlider.on('update', function (values, handle) {
			sliderInput.value = values[handle];
		});

		sliderInput.addEventListener('change', function () {
			slider.noUiSlider.set(this.value);
		});
	}

	return {
		// public functions
		init: function () {
			formEl = $('#kt_contacts_add_form');

			initWizard();
			initValidation();
			initSubmit();
			initSelect();
			initRateAlipaySlider();
			initRateWechatSlider();
		}
	};
}();

jQuery(document).ready(function () {
	KTContactsAdd.init();
});
