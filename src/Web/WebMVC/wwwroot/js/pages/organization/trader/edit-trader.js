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
				},
				UplineId: {
					required: true
				},

				//Step 4
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
		var btn = $(document).find('[data-ktwizard-type="action-submit"]');

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
								var userRole = $('#user_role').val();
								if (userRole.length) {
									if (userRole == 'TraderAgent') {
										window.location.href = "/Trader/Downlines";
									}
									else {
										window.location.href = "/Trader";
									}
								}
								else {
									window.location.href = "/Trader";
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
				url: "../TraderAgent/Search",
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

		var maxAlipay = parseFloat($('#maxratealipay').val());
		var startRate = parseFloat($('#kt_nouislider_ratealipay_input').val());


		noUiSlider.create(slider, {
			start: startRate,
			step: 1,
			range: {
				'min': [0],
				'max': maxAlipay
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
		var maxWechat = parseFloat($('#maxratewechat').val());
		var startRate = parseFloat($('#kt_nouislider_ratewechat_input').val());

		noUiSlider.create(slider, {
			start: startRate,
			step: 1,
			range: {
				'min': [0],
				'max': maxWechat
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
			formEl = $('#kt_user_edit_form');

			initValidation();
			initSubmit();
			initSelect();
			initRateAlipaySlider();
			initRateWechatSlider();
		}
	};
}();

jQuery(document).ready(function () {
	KTUserEdit.init();
});