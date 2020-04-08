"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;

	const aspController = 'ManualCode';
	const aspActionDelete = 'Delete';
	const aspActionUpdate = 'Update';



	const paymentChannelType = {
		'Alipay': {
			'title': '支付宝',
			'class': ' kt-font-info font-weight-normal'
		},
		'Wechat': {
			'title': '微信',
			'class': ' btn-font-success font-weight-normal'
		}
	};
	const paymentSchemeType = {
		'Barcode': {
			'title': '个码',
			'class': ' kt-font-warning'
		},
		'Merchant': {
			'title': '原生码',
			'datatoggle': 'show-modal-approve',
			'class': ' kt-font-success'
		},
		'Transaction': {
			'title': '支转支',
			'datatoggle': 'kt-font-primary',
			'class': ' kt-font-primary'
		},
		'Bank': {
			'title': '支转银',
			'datatoggle': 'show-modal-approve',
			'class': ' kt-font-info'
		},
		'Envelop': {
			'title': '红包',
			'datatoggle': 'show-modal-approve',
			'class': ' kt-font-danger'
		},
		'EnvelopPassword': {
			'title': '口令红包',
			'datatoggle': 'show-modal-approve',
			'class': ' kt-font-danger'
		}
	};


	// init
	var init = function () {
		// init the datatables. Learn more: https://keenthemes.com/metronic/?page=docs&section=datatable
		datatable = $('#kt_apps_pendingreview_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'PendingReview'
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
					noRecords: '查无二维码',
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
				field: 'QrCodeId',
				title: '#',
				sortable: false,
				width: 50,
				selector: {
					class: 'kt-checkbox--solid'
				},
				textAlign: 'center',
				template: function (row) {
					return row.QrCodeId;
				}
			}, {
				field: 'FullName',
				title: '名称',
				width: 170,
				autoHide: false,
				template: function (row) {

					var output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__pic">\
									<button type="button" data-qrcode-id="' + row.QrCodeId + '" data-toggle="show-test-qrcode" class="btn btn-secondary btn-elevate btn-icon float-left"><i class="fa fa-qrcode"></i></button>\
								</div>\
								<div class="kt-user-card-v2__details">\
									<span href="#" class="kt-user-card-v2__name ">\
									<a href="#" data-qrcode-id="' + row.QrCodeId + '"  data-payment-scheme="' + row.PaymentScheme + '" data-toggle="show-info-qrcode" class="kt-link kt-link--state kt-link--dark kt-font-boldest">' + row.FullName + '</a></span>\
									<span class="kt-user-card-v2__desc"> <label class="'+ paymentChannelType[row.PaymentChannel].class + '"> ' + paymentChannelType[row.PaymentChannel].title + '</label> · \
                                    <label class="' + paymentSchemeType[row.PaymentScheme].class + '"> ' + paymentSchemeType[row.PaymentScheme].title + '</label> </span>\
								</div>\
							</div>';

					//output += '&emsp;<button type="button" data-qrcode-id="' + row.QrCodeId + '" data-toggle="show-test-qrcode" class="btn btn-secondary btn-elevate btn-icon float-left"><i class="fa fa-qrcode"></i></button>';
					//output += '<a href="#" data-qrcode-id="' + row.QrCodeId + '" data-toggle="show-test-qrcode" class="kt-link kt-link--state kt-link--primary kt-font-boldest">' + row.FullName + '</a>';
					//output += '<a href="#" data-qrcode-id="' + row.QrCodeId + '" data-toggle="show-test-qrcode" class="kt-user-card-v2__desc">' + row.FullName + '</a>';

					return output;
				}
			}, {
				field: "Username",
				title: "交易员",
				sortable: true,
				overflow: 'visible',
				template: function (data, i) {
					var output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.UserId + '" data-user-type="Trader" class="kt-user-card-v2__name">' + data.UserFullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.Username + '</span>\
								</div>\
							</div>';


					return output;
				}
			}, {
				field: 'UplineUserName',
				title: '上级代理',
				template: function (data) {
					var output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.UplineUserId + '" data-user-type="TraderAgent" class="kt-user-card-v2__name">' + data.UplineFullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.UplineUserName + '</span>\
								</div>\
							</div>';

					return output;
				}
			}, {
				field: 'SpecifiedShopUsername',
				title: '指定商户',
				template: function (data) {
					var output;

					if (data.SpecifiedShopId) {

						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.SpecifiedShopId + '" data-user-type="Shop" class="kt-user-card-v2__name">' + data.SpecifiedShopFullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.SpecifiedShopUsername + '</span>\
								</div>\
							</div>';
					}
					else {
						output = '-';
					}

					return output;
				}
			}, {
				field: 'DateCreated',
				title: '申请时间',
				textAlign: 'center',
				template: function (row) {
					return row.DateCreated;
				}
			}, {
				field: "Actions",
				width: 135,
				title: "动作",
				textAlign: 'center',
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {
					var action = {
						0: {
							'title': '拒绝',
							'datatoggle': 'show-modal-reject',
							'class': ' btn-danger'
						},
						1: {
							'title': '核准',
							'datatoggle': 'show-modal-approve',
							'class': ' btn-success'
						}
					};

					var contextUserType = $('#user_role').val();
					if (contextUserType == 'Manager') {
						var output = '';
						output += '<div class="row">'

						output = output + '<div class="col-sm-6"><button data-qrcode-id="' + row.QrCodeId +
							'" data-toggle="' + action[0].datatoggle +
							'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + action[0].class + '">' +
							action[0].title + '</button></div>';
						output = output + '<div class="col-sm-6"><button data-qrcode-id="' + row.QrCodeId +
							'" data-toggle="' + action[1].datatoggle +
							'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + action[1].class + '">' +
							action[1].title + '</button></div>';

						output += '</div>'

						return output;
					}
				}
			}]
		});

		datatable.on('click', '[data-record-id]', function () {
			var userType = $(this).data('user-type');
			var contextUserRole = $('#user_role').val();
			console.log(contextUserRole);
			if (contextUserRole == 'Trader') {
				//Trader can not see detail of trader agents and shops.
				if (userType !== 'Trader') {
					return;
				}
			}
			else if (contextUserRole == 'TraderAgent') {
				//Trader agent can not see detail of shops.
				if (userType !== 'Trader' && userType!=='TraderAgent') {
					return;
				}
			}

			modalUserDetail($(this).data('record-id'), userType );
			$('#kt_modal_quick_view_user_info').modal('show');
		});

		datatable.on('click', '[data-qrcode-id]', function () {
			var action = $(this).data('toggle');
			if (action.length) {
				var qrcodeId = parseInt($(this).data('qrcode-id'));
				if (action === 'show-modal-approve') {
					modalApprove(qrcodeId);
				}
				else if (action === 'show-modal-reject') {
					modalReject(qrcodeId);
				}
			}
		});

		$(document).on('click', '[data-toggle="show-test-qrcode"]', function () {
			var qrCodeId = $(this).data('qrcode-id');
			var btn = $(this);
			KTApp.progress(btn);

			$.ajax({
				url: "GenerateQrCode",
				type: "POST",
				contentType: "application/json",
				data: JSON.stringify(qrCodeId),
				success: function (response) {
					KTApp.unprogress(btn);

					Swal.fire({
						title: '二维码',
						text: '扫码或存照来测试收付款',
						imageUrl: response.QrCodeData,
						imageWidth: 200,
						imageHeight: 200,
						imageAlt: 'QR Code',
					})
				},
				error: function (response) {
					KTApp.unprogress(btn);
					reject(response.responseText)
				}
			});

			/*if (baseRole !== 'Manager') {
				modal.find('#UserId').val(id);
			}*/
		});
		$(document).on('click', '[data-toggle="show-info-qrcode"]', function () {
			var qrCodeId = $(this).data('qrcode-id');

			modalQrCodeDetail(qrCodeId, $(this).data('payment-scheme'));
			$('#kt_modal_quick_view_qrcode_info').modal('show');
		});
	}


	// user details
	var modalUserDetail = function (id, userBaseRole) {
		var detailUrl = "";
		var dataObject = null;

		if (userBaseRole === 'Trader') {
			detailUrl = "../Trader/DownlineDetail";
			dataObject = { traderId: id };
		}
		else if (userBaseRole === 'TraderAgent') {
			detailUrl = "../TraderAgent/DownlineDetail";
			dataObject = { traderAgentId: id };
		}
		else if (userBaseRole === 'Shop') {
			detailUrl = "../Shop/Detail";
			dataObject = { shopId: id };
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
				else if (userBaseRole === 'Shop') {
					$('#quick_view_user_info_id').html(data.ShopId);
				}
				else if (userBaseRole === 'ShopAgent') {
					$('#quick_view_user_info_id').html(data.ShopAgentId);
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
				if (userBaseRole === 'Trader' || userBaseRole === 'TraderAgent') {
					$('#quick_view_user_info_ratealipay').html('千分之' + data.TradingCommission.RateAlipayInThousandth);
					$('#quick_view_user_info_ratewechat').html('千分之' + data.TradingCommission.RateWechatInThousandth);
				}
				else if (userBaseRole === 'Shop' || userBaseRole === 'ShopAgent') {
					$('#quick_view_user_info_ratealipay').html('千分之' + data.RebateCommission.RateRebateAlipayInThousandth);
					$('#quick_view_user_info_ratewechat').html('千分之' + data.RebateCommission.RateRebateWechatInThousandth);
				}
				$('#quick_view_user_info_eachamountlimit').html(
					data.Balance.WithdrawalLimit.EachAmountLowerLimit + ' ¥' + ' ~ ' + data.Balance.WithdrawalLimit.EachAmountUpperLimit + ' ¥');
				$('#quick_view_user_info_dailyamountlimit').html(data.Balance.WithdrawalLimit.DailyAmountLimit + ' ¥');
				$('#quick_view_user_info_dailyfrequencylimit').html(data.Balance.WithdrawalLimit.DailyFrequencyLimit);
				$('#quick_view_user_info_withdrawalcommission').html('千分之' + data.Balance.WithdrawalCommissionRateInThousandth);
				$('#quick_view_user_info_depositcommission').html('-');
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})
	};

	// qrcode details
	var modalQrCodeDetail = function (id, paymentScheme) {
		var detailUrl = "../ManualCode/Detail";
		var qrCodeDataUrl = "";

		if (paymentScheme === 'Barcode') {
			qrCodeDataUrl = "../ManualCode/BarcodeInfo";
		}
		else if (paymentScheme === 'Merchant') {
			qrCodeDataUrl = "../ManualCode/MerchantInfo";
		}
		else if (paymentScheme === 'Bank') {
			qrCodeDataUrl = "../ManualCode/BankInfo";
		}
		else if (paymentScheme === 'Transaction'
			|| paymentScheme === 'Envelop'
			|| paymentScheme === 'EnvelopPassword') {
			qrCodeDataUrl = "../ManualCode/TransactionInfo";
		}
		else {
			return;
		}

		//Map info.
		$.ajax({
			url: detailUrl,
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				console.log("Data retrieved.");


				var isEnabledStatus = {
					'true': { 'title': '启用', 'class': ' kt-font-success' },
					'false': { 'title': '停用', 'class': ' kt-font-danger' }
				};

				var pairingStatus = {
					'DisableBySystem': { 'title': '自动封禁', 'class': ' kt-font-danger' },
					'NormalDisable': { 'title': '闲置', 'class': ' kt-font-primary' },
					'Pairing': { 'title': '抢单中', 'class': ' kt-font-success' },
					'ProcessingOrder': { 'title': '处理订单中', 'class': ' kt-font-warning' }
				};
				var codeStatus = {
					'Disabled': { 'title': '停用', 'class': ' kt-font-danger' },
					'Enabled': { 'title': '启用', 'class': ' kt-font-success' },
					'AutoDisabled': { 'title': '自动停用', 'class': ' kt-font-danger' }
				};
				var isApprovedStatus = {
					'true': { 'title': '已通過', 'class': ' kt-font-success' },
					'false': { 'title': '未通過', 'class': ' kt-font-danger' }
				};

				//Set tab content 1
				$('#quick_view_qrcode_info_fullname').html(data.FullName);
				$('#quick_view_qrcode_info_trader').html(
					data.UserFullName + '<br>' +
					data.Username);

				var shopFullName = data.SpecifiedShopFullName;
				$('#quick_view_qrcode_info_specified_shop').html(
					(shopFullName ? shopFullName : '-') + '<br>' +
					(shopFullName ? data.SpecifiedShopUsername : ''));
				$('#quick_view_qrcode_info_payment_channel').html(paymentChannelType[data.PaymentChannel].title);
				$('#quick_view_qrcode_info_payment_scheme').html(paymentSchemeType[data.PaymentScheme].title);
				$('#quick_view_qrcode_info_datecreated').html(data.DateCreated);


				$('#quick_view_qrcode_info_isapproved').html(
					'<span class=" ' + isApprovedStatus[data.IsApproved].class + '">' + isApprovedStatus[data.IsApproved].title + '</span>');

				$('#quick_view_qrcode_info_code_status').html(
					'<span class=" ' + codeStatus[data.QrCodeStatus].class + '">' + codeStatus[data.QrCodeStatus].title + '</span>');

				$('#quick_view_qrcode_info_pairing_status').html(
					'<span class=" ' + pairingStatus[data.PairingStatus].class + '">' + pairingStatus[data.PairingStatus].title + '</span>');


				$('#quick_view_qrcode_info_by_success_rate').html(
					'<span class=" ' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingBySuccessRate].class + '">' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingBySuccessRate].title + '</span>' + '<br>' +
					(data.QrCodeEntrySetting.AutoPairingBySuccessRate ? '阈值: ' + data.QrCodeEntrySetting.SuccessRateThresholdInHundredth + '%' : '-') + '<br>' +
					(data.QrCodeEntrySetting.AutoPairingBySuccessRate ? '最低笔数: ' + data.QrCodeEntrySetting.SuccessRateMinOrders + '笔' : '-'));
				$('#quick_view_qrcode_info_by_quota_left').html(
					'<span class=" ' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingByQuotaLeft].class + '">' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingByQuotaLeft].title + '</span>' + '<br>' +
					(data.QrCodeEntrySetting.AutoPairingByQuotaLeft ? '阈值: ' + data.QrCodeEntrySetting.QuotaLeftThreshold + '¥' : '-'));
				$('#quick_view_qrcode_info_by_currrent_consecutive_failures').html(
					'<span class=" ' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures].class + '">' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures].title + '</span>' + '<br>' +
					(data.QrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures ? '阈值: ' + data.QrCodeEntrySetting.CurrentConsecutiveFailuresThreshold + '笔' : '-'));
				$('#quick_view_qrcode_info_by_available_balance').html(
					'<span class=" ' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairngByAvailableBalance].class + '">' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairngByAvailableBalance].title + '</span>' + '<br>' +
					(data.QrCodeEntrySetting.AutoPairngByAvailableBalance ? '阈值: ' + data.QrCodeEntrySetting.AvailableBalanceThreshold + '¥' : '-'));
				$('#quick_view_qrcode_info_by_bussiness_hours').html(
					'<span class=" ' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingByBusinessHours].class + '">' + isEnabledStatus[data.QrCodeEntrySetting.AutoPairingByBusinessHours].title + '</span>' + '<br>');

				$('#quick_view_qrcode_info_daily_amount_limit').html(data.DailyAmountLimit + ' ¥');
				$('#quick_view_qrcode_info_order_amount_limit').html(data.OrderAmountLowerLimit + '¥ ~' + data.OrderAmountUpperLimit + '¥');

				$('#quick_view_qrcode_info_min_commission').html('千分之' + data.MinCommissionRateInThousandth);
				$('#quick_view_qrcode_info_available_balance').html(data.AvailableBalance.toFixed(3) + ' ¥');
				$('#quick_view_qrcode_info_quota_left_today').html(data.QuotaLeftToday.toFixed(3) + ' ¥');
				$('#quick_view_qrcode_info_last_traded').html(data.DateLastTraded ? data.DateLastTraded : ' -');
				$('#quick_view_qrcode_info_total_success').html(data.PairingInfo.TotalSuccess + ' 次');
				$('#quick_view_qrcode_info_total_failures').html(data.PairingInfo.TotalFailures + ' 次');
				$('#quick_view_qrcode_info_highest_consecutive_success').html(data.PairingInfo.HighestConsecutiveSuccess + ' 次');
				$('#quick_view_qrcode_info_highest_consecutive_failures').html(data.PairingInfo.HighestConsecutiveFailures + ' 次');
				$('#quick_view_qrcode_info_current_consecutive_success').html(data.PairingInfo.CurrentConsecutiveSuccess + ' 次');
				$('#quick_view_qrcode_info_current_consecutive_failures').html(data.PairingInfo.CurrentConsecutiveFailures + ' 次');
				$('#quick_view_qrcode_info_success_rate').html(data.PairingInfo.SuccessRateInPercent + ' %');

			},
			error: function (response) {
				reject(response.responseText)
			}
		})


		//Map code data
		$.ajax({
			url: qrCodeDataUrl,
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				var qrCodeDataEl = '';
				if (paymentScheme === 'Barcode') {
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								收款链接:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.QrCodeUrl + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								金额:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.Amount ? data.Amount : ' -') + '\
							</span>\
						</div>';
				}
				else if (paymentScheme === 'Merchant') {
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								应用Id:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.AppId + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								支付宝公钥:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.AlipayPublicKey ? data.AlipayPublicKey : ' -') + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								微信API证书:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.WechatApiCertificate ? data.WechatApiCertificate : ' -') + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								私钥:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.PrivateKey ? data.PrivateKey : ' -') + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								商户ID:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.MerchantId ? data.MerchantId : ' -') + '\
							</span>\
						</div>';
				}
				else if (paymentScheme === 'Bank') {
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								银行名称:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.BankName + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								银行代号:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.BankMark + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								户名:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.AccountName + '\
							</span>\
						</div>';
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								银行账号:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.CardNumber + '\
							</span>\
						</div>';
				}
				else if (paymentScheme === 'Transaction'
					|| paymentScheme === 'Envelop'
					|| paymentScheme === 'EnvelopPassword') {
					qrCodeDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								支付宝用户ID:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.AlipayUserId + '\
							</span>\
						</div>';
				}
				else {
					return;
				}


				$('#kt_modal_quick_view_qrcode_info_widget13_qrcode_data').html(qrCodeDataEl)

			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})
	};

	// modal approve
	var modalApprove = function (qrcodeId) {
		if (qrcodeId) {

			//get ids and put in to a list.
			var idList = new Array();
			idList[0] = parseInt(qrcodeId);


			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "确认核准此二维码吗 ?",
				type: "info",

				confirmButtonText: "确认",
				confirmButtonClass: "btn btn-sm btn-bold btn-success",

				showCancelButton: true,
				cancelButtonText: "取消",
				cancelButtonClass: "btn btn-sm btn-bold btn-secondary",

				showLoaderOnConfirm: true,

				preConfirm: () => {
					return new Promise(function (resolve, reject) {
						$.ajax({
							url: "Approve",
							type: "POST",
							contentType: "application/json",
							data: JSON.stringify(idList),
							success: function (response) {
								resolve(response)
							},
							error: function (response) {
								console.log(response);
								reject(response.responseText)
							}
						})
					}).then(response => {
						if (!response.success) {
							console.log(response);
							throw new Error(response.responseText);
						}
						return response;
					}).catch(error => {
						Swal.showValidationMessage(
							'发生错误: ' + error
						)
					})
				},
				allowOutsideClick: () => !Swal.isLoading()
			}).then(function (result) {
				if (result.value) {
					swal.fire({
						title: '更新成功!',
						text: '等待用户确认收款来完成提现',
						type: 'success',
						buttonsStyling: false,
						confirmButtonText: "OK",
						confirmButtonClass: "btn btn-sm btn-bold btn-brand",
					}).then((result) => {
						if (result.value) {
							datatable.reload();
						}
					})
					// result.dismiss can be 'cancel', 'overlay',
					// 'close', and 'timer'
				} else if (result.dismiss === 'cancel') {
					swal.fire({
						title: '已取消',
						text: '',
						type: 'error',
						buttonsStyling: false,
						confirmButtonText: "OK",
						confirmButtonClass: "btn btn-sm btn-bold btn-brand",
					});
				}
			});
		}
	}

	// modal reject
	var modalReject = function (qrcodeId) {
		if (qrcodeId) {
			//get ids and put in to a list.
			var idList = new Array();
			idList[0] = parseInt(qrcodeId);

			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "审核失败的二维码将会被删除，确定吗 ?",
				type: "warning",

				confirmButtonText: "确认",
				confirmButtonClass: "btn btn-sm btn-bold btn-danger",

				showCancelButton: true,
				cancelButtonText: "取消",
				cancelButtonClass: "btn btn-sm btn-bold btn-secondary",

				showLoaderOnConfirm: true,

				preConfirm: () => {
					return new Promise(function (resolve, reject) {
						$.ajax({
							url: "Reject",
							type: "POST",
							contentType: "application/json",
							data: JSON.stringify(idList),
							success: function (response) {
								resolve(response);
							},
							error: function (response) {
								console.log(response);
								reject(response.responseText)
							}
						})
					}).then(response => {
						if (!response.success) {
							console.log(response);
							throw new Error(response.responseText);
						}
						return response;
					}).catch(error => {
						Swal.showValidationMessage(
							'发生错误: ' + error
						)
					})
				},
				allowOutsideClick: () => !Swal.isLoading()
			}).then(function (result) {
				if (result.value) {
					swal.fire({
						title: '操作成功!',
						text: '',
						type: 'success',
						buttonsStyling: false,
						confirmButtonText: "OK",
						confirmButtonClass: "btn btn-sm btn-bold btn-brand",
					}).then((result) => {
						if (result.value) {
							datatable.reload();
						}
					})
					// result.dismiss can be 'cancel', 'overlay',
					// 'close', and 'timer'
				} else if (result.dismiss === 'cancel') {
					swal.fire({
						title: '已取消',
						text: '',
						type: 'error',
						buttonsStyling: false,
						confirmButtonText: "OK",
						confirmButtonClass: "btn btn-sm btn-bold btn-brand",
					});
				}
			});
		}
	}



	// search
	var search = function () {
		$('#kt_payment_channel_filter').on('change', function () {

			datatable.search($(this).val(), 'paymentChannel');
		});
		$('#kt_payment_scheme_filter').on('change', function () {

			datatable.search($(this).val(), 'paymentScheme');
		});
	}

	// selection
	var selection = function () {
		// init form controls
		$('#kt_payment_channel_filter, #kt_payment_scheme_filter').selectpicker();

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

	// selected records approve
	var selectedApprove = function () {
		$('#kt_subheader_group_actions_approve_all').on('click', function () {
			// fetch selected IDs
			var ids;
			ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
				return $(chk).val();
			});


			if (ids.length > 0) {

				//get ids and put in to a list.
				var idList = new Array();
				for (var i = 0; i < ids.length; i++) {
					idList[i] = parseInt(ids[i]);
				}

				// learn more: https://sweetalert2.github.io/
				swal.fire({
					buttonsStyling: false,

					text: "确认核准 " + ids.length + " 个二维码吗 ?",
					type: "info",

					confirmButtonText: "确认",
					confirmButtonClass: "btn btn-sm btn-bold btn-danger",

					showCancelButton: true,
					cancelButtonText: "取消",
					cancelButtonClass: "btn btn-sm btn-bold btn-brand",

					showLoaderOnConfirm: true,

					preConfirm: () => {
						return new Promise(function (resolve, reject) {
							$.ajax({
								url: "Approve",
								type: "POST",
								contentType: "application/json",
								data: JSON.stringify(idList),
								success: function (response) {
									resolve(response)
								},
								error: function (response) {
									console.log(response);
									reject(response.responseText)
								}
							})
						}).then(response => {
							if (!response.success) {
								console.log(response);
								throw new Error(response.responseText);
							}
							return response;
						}).catch(error => {
							Swal.showValidationMessage(
								'发生错误: ' + error
							)
						})
					},
					allowOutsideClick: () => !Swal.isLoading()
				}).then(function (result) {
					if (result.value) {
						swal.fire({
							title: '操作成功!',
							text: '可至二维码列表查看',
							type: 'success',
							buttonsStyling: false,
							confirmButtonText: "OK",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						}).then((result) => {
							if (result.value) {
								datatable.reload();
							}
						})
						// result.dismiss can be 'cancel', 'overlay',
						// 'close', and 'timer'
					} else if (result.dismiss === 'cancel') {
						swal.fire({
							title: '已取消',
							text: '',
							type: 'error',
							buttonsStyling: false,
							confirmButtonText: "OK",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						});
					}
				});
			}
		});
	}

	// selected records approve cancellation
	var selectedReject = function () {
		$('#kt_subheader_group_actions_reject_all').on('click', function () {
			// fetch selected IDs
			var ids;
			ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
				return $(chk).val();
			});


			if (ids.length > 0) {

				//get ids and put in to a list.
				var idList = new Array();
				for (var i = 0; i < ids.length; i++) {
					idList[i] = parseInt(ids[i]);
				}

				// learn more: https://sweetalert2.github.io/
				swal.fire({
					buttonsStyling: false,

					text: "审核失败的二维码会全数删除，确认要删除 " + ids.length + " 个二维码吗 ?",
					type: "danger",

					confirmButtonText: "确认",
					confirmButtonClass: "btn btn-sm btn-bold btn-danger",

					showCancelButton: true,
					cancelButtonText: "取消",
					cancelButtonClass: "btn btn-sm btn-bold btn-brand",

					showLoaderOnConfirm: true,

					preConfirm: () => {
						return new Promise(function (resolve, reject) {
							$.ajax({
								url: "Reject",
								type: "POST",
								contentType: "application/json",
								data: JSON.stringify(idList),
								success: function (response) {
									resolve(response);
								},
								error: function (response) {
									console.log(response);
									reject(response.responseText)
								}
							})
						}).then(response => {
							if (!response.success) {
								console.log(response);
								throw new Error(response.responseText);
							}
							return response;
						}).catch(error => {
							Swal.showValidationMessage(
								'发生错误: ' + error
							)
						})
					},
					allowOutsideClick: () => !Swal.isLoading()
				}).then(function (result) {
					if (result.value) {
						swal.fire({
							title: '操作成功!',
							text: '二维码已全数删除',
							type: 'success',
							buttonsStyling: false,
							confirmButtonText: "OK",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						}).then((result) => {
							if (result.value) {
								datatable.reload();
							}
						})
						// result.dismiss can be 'cancel', 'overlay',
						// 'close', and 'timer'
					} else if (result.dismiss === 'cancel') {
						swal.fire({
							title: '已取消',
							text: '',
							type: 'error',
							buttonsStyling: false,
							confirmButtonText: "OK",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						});
					}
				});
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
			init();
			search();
			selection();
			selectedApprove();
			selectedReject();
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});