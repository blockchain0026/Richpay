"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;


	var createDepositFormValidator;
	var createDepositFormEl;
	var createDepositToTraderFormValidator;
	var createDepositToTraderFormEl;

	const aspController = 'Deposit';
	const aspActionDelete = 'Delete';
	const aspActionUpdate = 'Update';

	// init
	var init = function () {
		// init the datatables. Learn more: https://keenthemes.com/metronic/?page=docs&section=datatable
		datatable = $('#kt_apps_deposit_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'Deposit/Search'
					}
				},
				pageSize: 10, // display 10 records per page
				serverPaging: true,
				serverFiltering: true,
				serverSorting: true,
			},

			// layout definition
			layout: {
				scroll: true, // enable/disable datatable scroll both horizontal and vertical when needed.
				footer: false, // display/hide footer
			},
			translate: {
				records: {
					processing: '处理中...',
					noRecords: '查无入金记录',
				},
				toolbar: {
					pagination: {
						items: {
							info: '显示 {{total}} 笔中的第 {{start}} - {{end}} 笔',
							default: {
								first: 'First',
								prev: 'Previous',
								next: 'Next',
								last: 'Last',
								more: 'More pages',
								input: 'Page number',
								select: 'Select page size'
							}
						}
					}
				}
			},

			// column sorting
			sortable: true,

			pagination: true,

			search: {
				input: $('#generalSearch'),
				delay: 400,
			},

			// columns definition
			columns: [{
				field: 'DepositId',
				title: '#',
				sortable: false,
				width: 50,
				selector: false,
				textAlign: 'center',
				template: function (row) {
					return row.DepositId;
				}
			}, {
				field: "Username",
				title: "提现用户",
				sortable: true,
				autoHide: false,
				overflow: 'visible',
				width: 170,
				// callback function support for column rendering
				template: function (data, i) {
					//console.log(data);
					var number = 4 + i;
					while (number > 12) {
						number = number - 3;
					}

					var output = '';

					var stateNo = KTUtil.getRandomInt(0, 6);
					var states = [
						'success',
						'brand',
						'danger',
						'success',
						'warning',
						'primary',
						'info'
					];
					var state = states[stateNo];

					var userType = {
						'Trader': {
							'title': '交易员',
							'class': ' btn-label-danger'
						},
						'TraderAgent': {
							'title': '交易员代理',
							'class': ' btn-label-danger'
						}
					};


					output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__pic">\
									<div class="kt-badge kt-badge--xl kt-badge--' + state + '">' + data.FullName.substring(0, 1) + '</div>\
								</div>\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.UserId + '" data-user-type="' + data.UserType + '" class="kt-user-card-v2__name">' + data.FullName + '</a>\
									<span class="kt-user-card-v2__desc">' + userType[data.UserType].title + '</span>\
								</div>\
							</div>';


					return output;
				}
			}, {
				field: 'BankName',
				title: '银行名称',
				textAlign: 'center',
				width: 100,
				template: function (row) {
					return row.DepositBankAccount.BankName;
				}
			}, {
				field: 'AccountName',
				title: '银行户名',
				textAlign: 'center',
				width: 100,
				template: function (row) {
					return row.DepositBankAccount.AccountName;
				}
			}, {
				field: 'AccountNumber',
				title: '银行账号',
				textAlign: 'center',
				template: function (row) {
					return row.DepositBankAccount.AccountNumber;
				}
			}, {
				field: 'TotalAmount',
				title: '入金总金额',
				textAlign: 'center',
				template: function (row) {
					return row.TotalAmount.toFixed(3);
				}
			}, {
				field: 'CommissionAmount',
				title: '手续费',
				textAlign: 'center',
				template: function (row) {
					return row.CommissionAmount.toFixed(3);
				}
			}, {
				field: 'ActualAmount',
				title: '实际入款',
				textAlign: 'center',
				template: function (row) {
					return row.ActualAmount.toFixed(3);
				}
			}, {
				field: 'VerifiedBy',
				title: '审核申请',
				textAlign: 'center',
				width: 160,
				template: function (row) {
					var output;

					if (row.VerifiedByAdminId) {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details kt-container">\
									<span class="kt-user-card-v2__name">' + row.VerifiedByAdminName + '</span>\
								</div>\
							</div>';
					}
					else {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details kt-container">\
									<span class="kt-user-card-v2__desc">' + '-' + '</span>\
								</div>\
							</div>';
					}

					return output;
				}
			}, {
				field: "DepositStatus",
				title: "状态",
				width: 70,
				textAlign: 'center',
				// callback function support for column rendering
				template: function (row) {
					var status = {
						'Success': {
							'title': '成功',
							'class': ' btn-label-success'
						},
						'Canceled': {
							'title': '取消',
							'class': ' btn-label-danger'
						}
					};
					return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[row.DepositStatus].class + '">' + status[row.DepositStatus].title + '</span>';
				}
			}, {
				field: 'DateCreated',
				title: '时间',
				width: 150,
				textAlign: 'center',
				autohide: false,
				template: function (row) {
					var output = '';

					var dateCreated = convertUTCDateToLocalDate(new Date(row.DateCreated)).toLocaleString();
					var dateFinished = convertUTCDateToLocalDate(new Date(row.DateFinished)).toLocaleString();

					output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<span class="kt-user-card-v2__desc">申请:' + dateCreated + '</span>\
									<span class="kt-user-card-v2__desc">完成:' + dateFinished + '</span>\
								</div>\
							</div>';

					return output;
				}
			}]
		});

		datatable.on('click', '[data-record-id]', function () {
			modalUserDetail($(this).data('record-id'), $(this).data('user-type'));
			$('#kt_modal_quick_view_user_info').modal('show');
		});


		$(document).on('click', '[data-toggle="show-modal-create-deposit"]', function () {
			var modal = $('#modal__create_deposit');
			var id = $(this).data('user-id');
			var baseRole = $(this).data('user-base-role');

			if (baseRole !== 'Manager') {
				modal.find('#UserId').val(id);
			}

			modal.modal('show');
		});


		$(document).on('click', '[data-toggle="show-modal-create-deposit-to-trader"]', function () {
			var modal = $('#modal__create_deposit_to_trader');
			var id = $(this).data('user-id');
			var baseRole = $(this).data('user-base-role');
			if (baseRole == 'TraderAgent') {
				modal.modal('show');
			}
		});
	}


	// user details
	var modalUserDetail = function (id, userBaseRole) {
		var el = $('#modal_sub_datatable_ajax_source');

		var detailUrl = "";
		var dataObject = null;

		if (userBaseRole === 'Trader') {
			detailUrl = "Trader/DownlineDetail";
			dataObject = { traderId: id };
		}
		else if (userBaseRole === 'TraderAgent') {
			detailUrl = "TraderAgent/DownlineDetail";
			dataObject = { traderAgentId: id };
		}
		else {
			return;
		}

		$.ajax({
			url: detailUrl,
			type: "Get",
			contentType: "application/json",
			data: dataObject,
			success: function (data) {
				console.log("Data retrieved.");

				var hasGrantRightStatus = {
					1: { 'title': '允许', 'class': ' kt-font-success' },
					2: { 'title': '禁止', 'class': ' kt-font-danger' }
				};
				var isEnableStatus = {
					1: { 'title': '启用', 'class': ' kt-font-success' },
					2: { 'title': '停用', 'class': ' kt-font-danger' }
				};
				var isReviewedStatus = {
					1: { 'title': '已通過', 'class': ' kt-font-success' },
					2: { 'title': '未通過', 'class': ' kt-font-danger' }
				};

				//Set tab content 1
				$('#quick_view_user_info_fullname').html(data.FullName);
				$('#quick_view_user_info_nickname').html(data.Nickname);
				$('#quick_view_user_info_phonenumber').html(data.PhoneNumber);
				$('#quick_view_user_info_email').html(data.Email);
				$('#quick_view_user_info_wechat').html(data.Wechat);
				$('#quick_view_user_info_qq').html(data.QQ);

				//Set tab content 2
				if (userBaseRole === 'Trader') {
					$('#quick_view_user_info_id').html(data.TraderId);

				}
				else if (userBaseRole === 'TraderAgent') {
					$('#quick_view_user_info_id').html(data.TraderAgentId);
				}

				$('#quick_view_user_info_username').html(data.Username);
				$('#quick_view_user_info_upline').html(
					data.UplineFullName + '<br>' +
					data.UplineUserName);

				if (data.HasGrantRight) {
					$('#quick_view_user_info_hasgrantright').html(
						'<span class=" ' + hasGrantRightStatus[1].class + '">' + hasGrantRightStatus[1].title + '</span>');
				}
				else {
					$('#quick_view_user_info_hasgrantright').html(
						'<span class=" ' + hasGrantRightStatus[2].class + '">' + hasGrantRightStatus[2].title + '</span>');
				}

				if (data.IsEnabled) {
					$('#quick_view_user_info_accountstatus').html(
						'<span class=" ' + isEnableStatus[1].class + '">' + isEnableStatus[1].title + '</span>');
				}
				else {
					$('#quick_view_user_info_accountstatus').html(
						'<span class=" ' + isEnableStatus[2].class + '">' + isEnableStatus[2].title + '</span>');
				}

				if (data.IsReviewed) {
					$('#quick_view_user_info_isreviewed').html(
						'<span class=" ' + isReviewedStatus[1].class + '">' + isReviewedStatus[1].title + '</span>');
				}
				else {
					$('#quick_view_user_info_isreviewed').html(
						'<span class=" ' + isReviewedStatus[2].class + '">' + isReviewedStatus[2].title + '</span>');
				}

				$('#quick_view_user_info_datelastloggedin').html(data.DateLastLoggedIn);
				$('#quick_view_user_info_iplastloggedin').html(data.LastLoginIP);
				$('#quick_view_user_info_datecreated').html(data.DateCreated);

				//Set tab content 3
				var amountAvailable = data.Balance.AmountAvailable;
				var amountFrozen = data.Balance.AmountFrozen;
				var amountTotal = amountAvailable + amountFrozen;

				$('#quick_view_user_info_amountavailable').html(amountAvailable.toFixed(3) + ' ¥');
				$('#quick_view_user_info_amountforzen').html(amountFrozen.toFixed(3) + ' ¥');
				$('#quick_view_user_info_amounttotal').html(amountTotal.toFixed(3) + ' ¥');
				$('#quick_view_user_info_ratealipay').html('千分之' + data.TradingCommission.RateAlipayInThousandth);
				$('#quick_view_user_info_ratewechat').html('千分之' + data.TradingCommission.RateWechatInThousandth);


				$('#quick_view_user_info_eachamountlimit').html(
					data.Balance.DepositLimit.EachAmountLowerLimit + ' ¥' + ' ~ ' + data.Balance.DepositLimit.EachAmountUpperLimit + ' ¥');
				$('#quick_view_user_info_dailyamountlimit').html(data.Balance.DepositLimit.DailyAmountLimit + ' ¥');
				$('#quick_view_user_info_dailyfrequencylimit').html(data.Balance.DepositLimit.DailyFrequencyLimit);
				$('#quick_view_user_info_depositcommission').html('千分之' + data.Balance.DepositCommissionRateInThousandth);
				$('#quick_view_user_info_depositcommission').html('-');
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

		var modal = datatable.closest('.modal');

		modal.on('shown.bs.modal', function () {
			var modalContent = $(this).find('.modal-content');
			datatable.spinnerCallback(true, modalContent);

			datatable.on('kt-datatable--on-layout-updated', function () {
				datatable.show();
				datatable.spinnerCallback(false, modalContent);
				datatable.redraw();
			});
		}).on('hidden.bs.modal', function () {
			el.KTDatatable('destroy');
		});
	};

	// create deposit modal
	var modalCreateDeposit = function () {
		//Validator
		createDepositFormValidator = createDepositFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				userid: {
					required: true
				},
				depositbankaccountid: {
					required: true
				},
				depositamount: {
					required: true,
					integer: true,
					min: 1
				}
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

		$('#modal__create_deposit').on('show.bs.modal', function (e) {
			var contextUserId = $(this).find('#UserId').val();
			if (contextUserId) {
				$.ajax({
					url: "Deposit/AvailableBalance",
					type: "Get",
					contentType: "application/json",
					data: { userId: contextUserId },
					success: function (response) {
						$('#modal__create_deposit').find('#currentbalance').val(response.AvailableBalance.toFixed(3));
					},
					error: function (response) {
						console.log(response);
					}
				})
			}
			else {
				$('#UserId').on('change', function () {
					$.ajax({
						url: "Deposit/AvailableBalance",
						type: "Get",
						contentType: "application/json",
						data: { userId: this.value },
						success: function (response) {
							$('#modal__create_deposit').find('#currentbalance').val(response.AvailableBalance.toFixed(3));
						},
						error: function (response) {
							console.log(response);
						}
					})
				});

			}

			$('#DepositBankAccountId').on('change', function () {
				$.ajax({
					url: "Deposit/DepositBankAccountDetail",
					type: "Get",
					contentType: "application/json",
					data: { depositBankAccountId: this.value },
					success: function (response) {
						$('#bankname').val(response.BankName);
						$('#accountname').val(response.AccountName);
						$('#accountnumber').val(response.AccountNumber);
					},
					error: function (response) {
						console.log(response);
					}
				})
			});

		}).on('hide.bs.modal', function (e) {
			$(this).find('#UserId').val('');
			$(this).find('#currentbalance').val('');
			$(this).find('#DepositBankAccountId').val('');
			$(this).find('#bankname').val('');
			$(this).find('#accountname').val('');
			$(this).find('#accountnumber').val('');
		});

		$('#modal__create_deposit').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();

			var btn = $('#modal__create_deposit').find('[data-action="submit"]');

			if (createDepositFormValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createDepositFormEl);


				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createDepositFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createDepositFormEl);

						swal.fire({
							"title": "申请成功",
							"text": "平台确认款项后会核准并更新余额",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_deposit').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(createDepositFormEl);
						swal.fire({
							"title": "申请失败",
							"text": data.responseText,
							"type": "error",
							"buttonStyling": false,
							"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
						});
					}
				});
			}
		});
	};

	// create deposit to trader modal
	var modalCreateDepositToTrader = function () {
		//Validator
		createDepositToTraderFormValidator = createDepositToTraderFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				userid: {
					required: true
				},
				depositbankaccountid: {
					required: true
				},
				depositamount: {
					required: true,
					integer: true,
					min: 1
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

		$('#modal__create_deposit_to_trader').on('show.bs.modal', function (e) {
			$(this).find('#UserId').on('change', function () {
				$.ajax({
					url: "Deposit/AvailableBalance",
					type: "Get",
					contentType: "application/json",
					data: { userId: this.value },
					success: function (response) {
						$('#modal__create_deposit_to_trader').find('#currentbalance').val(response.AvailableBalance.toFixed(3));
					},
					error: function (response) {
						console.log(response);
					}
				})
			});

			$(this).find('#DepositBankAccountId').on('change', function () {
				$.ajax({
					url: "Deposit/DepositBankAccountDetail",
					type: "Get",
					contentType: "application/json",
					data: { depositBankAccountId: this.value },
					success: function (response) {
						$('#modal__create_deposit_to_trader').find('#bankname').val(response.BankName);
						$('#modal__create_deposit_to_trader').find('#accountname').val(response.AccountName);
						$('#modal__create_deposit_to_trader').find('#accountnumber').val(response.AccountNumber);
					},
					error: function (response) {
						console.log(response);
					}
				})
			});

		}).on('hide.bs.modal', function (e) {
			$(this).find('#UserId').val('');
			$(this).find('#currentbalance').val('');
			$(this).find('#DepositBankAccountId').val('');
			$(this).find('#bankname').val('');
			$(this).find('#accountname').val('');
			$(this).find('#accountnumber').val('');
		});

		$('#modal__create_deposit_to_trader').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();

			var btn = $('#modal__create_deposit_to_trader').find('[data-action="submit"]');

			if (createDepositToTraderFormValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createDepositFormEl);


				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createDepositToTraderFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createDepositToTraderFormEl);

						swal.fire({
							"title": "申请成功",
							"text": "平台确认款项后会核准并更新余额",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_deposit_to_trader').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(createDepositToTraderFormEl);
						swal.fire({
							"title": "申请失败",
							"text": data.responseText,
							"type": "error",
							"buttonStyling": false,
							"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
						});
					}
				});
			}
		});
	};

	var initSelect = function () {
		var userType = $('#user_role').val();
		if (userType == 'Manager') {
			function formatItem(item) {
				if (item.loading) return item.text;
				var markup = "<div class='select2-result-repository clearfix'>" +
					"<div class='select2-result-repository__meta'>" +
					"<div class=''>" + item.fullname + "</div>";
				if (item.username) {
					markup += "<div class='select2-result-repository__description'>" + item.username + "</div>";
				}
				markup += "<div class='select2-result-repository__statistics'>" +
					"<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> 帐户状态: " + item.is_enabled + " </div>" +
					"<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> 审核状态: " + item.is_reviewed + " </div>" +
					"</div>" +
					"</div></div>";
				return markup;
			}

			function formatItemSelection(item) {
				return item.username || item.text;
			}

			$("#UserId").select2({
				placeholder: "输入用户名称或ID来搜寻",
				allowClear: true,
				ajax: {
					url: "Deposit/SearchUser",
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

								/*$('#minraterebatealipay').val(alipayRate + 1);
								$('#minraterebatewechat').val(wechatRate + 1);*/

								return {
									//Reconstruct the data to adapt to Select2 Api.
									id: item.UserId,
									text: item.UserId,
									username: item.Username,
									fullname: item.FullName,
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
		if (userType == 'TraderAgent') {
			function formatItem(item) {
				if (item.loading) return item.text;
				var markup = "<div class='select2-result-repository clearfix'>" +
					"<div class='select2-result-repository__meta'>" +
					"<div class=''>" + item.fullname + "</div>";
				if (item.username) {
					markup += "<div class='select2-result-repository__description'>" + item.username + "</div>";
				}
				markup += "<div class='select2-result-repository__statistics'>" +
					"<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> 帐户状态: " + item.is_enabled + " </div>" +
					"<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> 审核状态: " + item.is_reviewed + " </div>" +
					"</div>" +
					"</div></div>";
				return markup;
			}

			function formatItemSelection(item) {
				return item.username || item.text;
			}

			$('#modal__create_deposit_to_trader').find("#UserId").select2({
				placeholder: "输入交易员名称或ID来搜寻",
				allowClear: true,
				ajax: {
					url: "Deposit/SearchTraderUser",
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

								/*$('#minraterebatealipay').val(alipayRate + 1);
								$('#minraterebatewechat').val(wechatRate + 1);*/

								return {
									//Reconstruct the data to adapt to Select2 Api.
									id: item.UserId,
									text: item.UserId,
									username: item.Username,
									fullname: item.FullName,
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

	}

	// Daterangepicker Init
	var daterangepickerInit = function () {
		if ($('#kt_dashboard_daterangepicker').length == 0) {
			return;
		}

		var picker = $('#kt_dashboard_daterangepicker');
		var start = moment();
		var end = moment();

		function cb(start, end, label) {
			var title = '';
			var range = '';

			if ((end - start) < 100 || label == '今日') {
				title = '今日:';
				range = start.format('MM/DD/YYYY');

				$('#date_search_from').val(start.format('MM/DD/YYYY'));
				$('#date_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
			} else if (label == '昨日') {
				title = '昨日:';
				range = start.format('MM/DD/YYYY');
				$('#date_search_from').val(start.format('MM/DD/YYYY'));
				$('#date_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
			} else {
				range = start.format('MM/DD/YYYY') + ' - ' + end.format('MM/DD/YYYY');
				$('#date_search_from').val(start.format('MM/DD/YYYY'));
				$('#date_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
			}
			$('#kt_dashboard_daterangepicker_date').html(range);
			//$('#kt_dashboard_daterangepicker_title').html(title);
			updateDatatable();
		}

		picker.daterangepicker({
			direction: KTUtil.isRTL(),
			startDate: start,
			endDate: end,
			opens: 'right',
			ranges: {
				'今日': [moment(), moment()],
				'昨日': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
				'过去 7 日': [moment().subtract(6, 'days'), moment()],
				'过去 30 日': [moment().subtract(29, 'days'), moment()],
				'本月': [moment().startOf('month'), moment().endOf('month')],
				'上个月': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
			},
			"locale": {
				"format": "MM/DD/YYYY",
				"separator": " - ",
				"applyLabel": "套用",
				"cancelLabel": "取消",
				"fromLabel": "由",
				"toLabel": "至",
				"customRangeLabel": "自订日期",
				"daysOfWeek": [
					"日",
					"一",
					"二",
					"三",
					"四",
					"五",
					"六"
				],
				"monthNames": [
					"一月",
					"二月",
					"三月",
					"四月",
					"五月",
					"六月",
					"七月",
					"八月",
					"九月",
					"十月",
					"十一月",
					"十二月"
				],
				"firstDay": 1
			}
		}, cb);

		cb(start, end, '');
	}

	var updateDatatable = function () {
		var query = $('#generalSearch').val();
		var from = $('#date_search_from').val();
		var to = $('#date_search_to').val();

		//Parse to utc time.
		var from = new Date(from).toLocaleString('en-US', { timeZone: 'UTC' });
		var to = new Date(to).toLocaleString('en-US', { timeZone: 'UTC' });

		var queryObejct = {
			generalSearch: query,
			DateFrom: from,
			DateTo: to,
			userType: $('#kt_user_type_filter').val()
		};

		datatable.setDataSourceParam('query', queryObejct);
		datatable.load();
		datatable.search();
	};


	// search
	var search = function () {
		$('#kt_user_type_filter').on('change', function () {
			datatable.search($(this).val(), 'userType');
		});
	}

	// selection
	var selection = function () {
		// init form controls
		$('#kt_user_type_filter').selectpicker();

		// event handler on check and uncheck on records
		datatable.on('kt-datatable--on-check kt-datatable--on-uncheck kt-datatable--on-layout-updated', function (e) {
			var checkedNodes;
			checkedNodes = datatable.rows('.kt-datatable__row--active').nodes(); // get selected records
			var count = checkedNodes.length; // selected records count

			$('#kt_subheader_group_selected_rows').html(count);

			if (count > 0) {
				$('#kt_subheader_search').addClass('kt-hidden');
				$('#kt_subheader_group_actions').removeClass('kt-hidden');
			} else {
				$('#kt_subheader_search').removeClass('kt-hidden');
				$('#kt_subheader_group_actions').addClass('kt-hidden');
			}
		});
	}

	// update total rows count text.
	var updateTotal = function () {
		datatable.on('kt-datatable--on-layout-updated', function () {
			$('#kt_subheader_total').html('共 ' + datatable.getTotalRows() + ' 笔');
		});
	};

	return {
		// public functions
		init: function () {
			createDepositFormEl = $('#form_create_deposit');
			createDepositToTraderFormEl = $('#form_create_deposit_to_trader');

			init();
			initSelect();

			modalCreateDeposit();
			modalCreateDepositToTrader();

			daterangepickerInit();

			search();
			selection();
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});
