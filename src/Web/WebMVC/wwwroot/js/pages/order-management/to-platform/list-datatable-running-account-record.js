"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;

	var avatar;

	const aspController = 'Order';
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
			'updatedatatoggle': 'show-modal-update-barcode-order-data',
			'class': ' kt-font-warning'
		},
		'Merchant': {
			'title': '原生码',
			'updatedatatoggle': 'show-modal-update-merchant-order-data',
			'class': ' kt-font-success'
		},
		'Transaction': {
			'title': '支转支',
			'updatedatatoggle': 'show-modal-update-transaction-order-data',
			'class': ' kt-font-primary'
		},
		'Bank': {
			'title': '支转银',
			'updatedatatoggle': 'show-modal-update-bank-order-data',
			'class': ' kt-font-info'
		},
		'Envelop': {
			'title': '红包',
			'updatedatatoggle': 'show-modal-update-transaction-order-data',
			'class': ' kt-font-danger'
		},
		'EnvelopPassword': {
			'title': '口令红包',
			'updatedatatoggle': 'show-modal-update-transaction-order-data',
			'class': ' kt-font-danger'
		}
	};

	// init
	var init = function () {
		var contextUserId = $('#user_id').val();
		// init the datatables. Learn more: https://keenthemes.com/metronic/?page=docs&section=datatable
		datatable = $('#kt_apps_running_account_record_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'RunningAccountRecords',
						params: {
							query: {
								userId: contextUserId
							}
						}
					},
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
					noRecords: '查无订单',
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
				field: 'RunningAccountRecordId',
				title: '#',
				sortable: false,
				width: 50,
				selector: false,
				textAlign: 'center',
				template: function (row) {
					return row.Id;
				}
			}, {
				field: 'TrackingNumber',
				title: '订单号',
				width: 250,
				autoHide: false,
				template: function (row) {
					var output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<span href="#" class="kt-user-card-v2__name ">\
									<a href="#" data-order-tracking-number="' + row.OrderTrackingNumber + '" data-toggle="show-info-order" class="kt-link kt-link--state kt-link--dark kt-font-boldest">平台订单号:' + row.OrderTrackingNumber + '</a></span>\
						            <span href="#" class="kt-user-card-v2__name " data-shop-order-id="' + row.ShopOrderId + '">商户订单号:' + row.ShopOrderId + '</span>\
								</div>\
							</div>';

					return output;
				}
			}, {
				field: 'ShopUserName',
				title: '商户',
				template: function (data) {
					var output;
					if (data.ShopId) {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.ShopId + '" data-user-type="Shop" class="kt-user-card-v2__name">' + data.ShopFullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.ShopUserName + '</span>\
								</div>\
							</div>';
					}
					else {
						output = '-';
					}

					return output;
				}
			}, {
				field: "TraderUserName",
				title: "交易员",
				sortable: true,
				overflow: 'visible',
				template: function (data, i) {
					var output = '-';
					if (data.TraderUserName) {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.TraderId + '" data-user-type="Trader" class="kt-user-card-v2__name">' + data.TraderFullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.TraderUserName + '</span>\
								</div>\
							</div>';
					}

					return output;
				}
			}, {
				field: 'Amount',
				title: '金额',
				width: 100,
				textAlign: 'center',
				template: function (row) {
					var amount = (row.Amount).toFixed(0);
					return amount + '¥';
				}
			}, {
				field: 'DistributedAmount',
				title: '分红',
				width: 100,
				textAlign: 'center',
				template: function (row) {
					var distributedAmount = '-';
					if (row.DistributedAmount) {
						distributedAmount = (row.DistributedAmount).toFixed(3) + '¥';
					}
					return distributedAmount;
				}
			}, {
				field: 'DateCreated',
				title: '建立時間',
				textAlign: 'center',
				template: function (row) {
					return row.DateCreated;
				}
			}, {
				field: "Status",
				title: "订单状态",
				textAlign: 'center',
				// callback function support for column rendering
				template: function (row) {
					var status = {
						'Submitted': {
							'title': '分配中',
							'class': ' btn-label-danger'
						},
						'AwaitingPayment': {
							'title': '支付中',
							'class': ' btn-label-warning'
						},
						'Success': {
							'title': '成功',
							'class': ' btn-label-success'
						},
						'Failed': {
							'title': '失敗',
							'class': ' btn-label-danger'
						}
					};

					return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[row.Status].class + '">' + status[row.Status].title + '</span>';
				}
			}, {
				field: "Actions",
				width: 100,
				title: "动作",
				textAlign: 'center',
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {
					//var updateLink = '/' + aspController + '/' + aspActionUpdate + '?traderAgentId=' + row.OrderId;

					var traderAction = {
						'Submitted': {
						},
						'AwaitingPayment': {
							0: {
								'title': '确认收款',
								'datatoggle': 'show-modal-confirm-payment',
								'class': ' btn-success'
							}
						},
						'Success': {
						},
						'Failed': {
						}
					};


					var contextUserType = $('#user_role').val();
					if (contextUserType == 'Trader') {
						var objectLength = Object.keys(traderAction[row.Status]).length;
						if (objectLength > 0) {
							var output = '';

							for (var i = 0; i < objectLength; i++) {

								output = output + '<button data-order-tracking-number="' + row.OrderTrackingNumber +
									'" data-order-amount="' + row.Amount + '" data-toggle="' + traderAction[row.Status][i].datatoggle +
									'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + traderAction[row.Status][i].class + '">' +
									traderAction[row.Status][i].title + '</button>'
							}
							return output;
						}
						else {
							return '-';
						}
					}
					else {
						return '-';
					}
				}
			}]
		});

		datatable.on('click', '[data-record-id]', function () {
			var userType = $(this).data('user-type');

			/*
			var contextUserRole = $('#user_role').val();
			if (contextUserRole == 'Trader') {
				//Trader can not see detail of trader agents and shops.
				if (userType !== 'Trader') {
					return;
				}
			}
			else if (contextUserRole == 'TraderAgent') {
				//Trader agent can not see detail of shops.
				if (userType !== 'Trader') {
					return;
				}
			}
			else if (contextUserRole == 'Manager') {
				//Manager can see everything.
			}
			else if (contextUserRole == 'Shop') {
				//Shop can only see detail of shop.
				if (userType !== 'Shop') {
					return;
				}
			}
			else if (contextUserRole == 'ShopAgent') {
				//Shop agent can only see detail of shop
				if (userType !== 'Shop') {
					return;
				}
			}
			*/

			modalUserDetail($(this).data('record-id'), userType);
			$('#kt_modal_quick_view_user_info').modal('show');
		});

		$(document).on('click', '[data-order-tracking-number]', function () {
			if ($(this).data('toggle') === 'show-modal-confirm-payment') {
				modalConfirmPayment(
					$(this).data('order-tracking-number'),
					$(this).data('order-amount'));
			}
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

	// order details
	var modalOrderDetail = function (id, paymentScheme) {
		var detailUrl = "../ManualCode/Detail";
		var orderDataUrl = "";

		if (paymentScheme === 'Barcode') {
			orderDataUrl = "../ManualCode/BarcodeInfo";
		}
		else if (paymentScheme === 'Merchant') {
			orderDataUrl = "../ManualCode/MerchantInfo";
		}
		else if (paymentScheme === 'Bank') {
			orderDataUrl = "../ManualCode/BankInfo";
		}
		else if (paymentScheme === 'Transaction'
			|| paymentScheme === 'Envelop'
			|| paymentScheme === 'EnvelopPassword') {
			orderDataUrl = "../ManualCode/TransactionInfo";
		}
		else {
			return;
		}

		//Map info.
		$.ajax({
			url: detailUrl,
			type: "Get",
			contentType: "application/json",
			data: { orderId: id },
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
				$('#quick_view_order_info_fullname').html(data.FullName);
				$('#quick_view_order_info_trader').html(
					data.UserFullName + '<br>' +
					data.Username);

				var shopFullName = data.PairingInfo.SpecifiedShopFullName;
				$('#quick_view_order_info_specified_shop').html(
					(shopFullName ? shopFullName : '-') + '<br>' +
					(shopFullName ? data.PairingInfo.SpecifiedShopUsername : ''));
				$('#quick_view_order_info_payment_channel').html(paymentChannelType[data.PaymentChannel].title);
				$('#quick_view_order_info_payment_scheme').html(paymentSchemeType[data.PaymentScheme].title);
				$('#quick_view_order_info_datecreated').html(data.DateCreated);


				$('#quick_view_order_info_isapproved').html(
					'<span class=" ' + isApprovedStatus[data.IsApproved].class + '">' + isApprovedStatus[data.IsApproved].title + '</span>');

				$('#quick_view_order_info_code_status').html(
					'<span class=" ' + codeStatus[data.OrderStatus].class + '">' + codeStatus[data.OrderStatus].title + '</span>');

				$('#quick_view_order_info_pairing_status').html(
					'<span class=" ' + pairingStatus[data.PairingStatus].class + '">' + pairingStatus[data.PairingStatus].title + '</span>');

				$('#quick_view_user_info_upline').html(
					data.UplineFullName + '<br>' +
					data.UplineUserName);


				$('#quick_view_order_info_by_success_rate').html(
					'<span class=" ' + isEnabledStatus[data.OrderEntrySetting.AutoPairingBySuccessRate].class + '">' + isEnabledStatus[data.OrderEntrySetting.AutoPairingBySuccessRate].title + '</span>' + '<br>' +
					(data.OrderEntrySetting.AutoPairingBySuccessRate ? '阈值: ' + data.OrderEntrySetting.SuccessRateThresholdInHundredth + '%' : '-') + '<br>' +
					(data.OrderEntrySetting.AutoPairingBySuccessRate ? '最低笔数: ' + data.OrderEntrySetting.SuccessRateMinOrders + '笔' : '-'));
				$('#quick_view_order_info_by_quota_left').html(
					'<span class=" ' + isEnabledStatus[data.OrderEntrySetting.AutoPairingByQuotaLeft].class + '">' + isEnabledStatus[data.OrderEntrySetting.AutoPairingByQuotaLeft].title + '</span>' + '<br>' +
					(data.OrderEntrySetting.AutoPairingByQuotaLeft ? '阈值: ' + data.OrderEntrySetting.QuotaLeftThreshold + '¥' : '-'));
				$('#quick_view_order_info_by_currrent_consecutive_failures').html(
					'<span class=" ' + isEnabledStatus[data.OrderEntrySetting.AutoPairingByCurrentConsecutiveFailures].class + '">' + isEnabledStatus[data.OrderEntrySetting.AutoPairingByCurrentConsecutiveFailures].title + '</span>' + '<br>' +
					(data.OrderEntrySetting.AutoPairingByCurrentConsecutiveFailures ? '阈值: ' + data.OrderEntrySetting.CurrentConsecutiveFailuresThreshold + '笔' : '-'));
				$('#quick_view_order_info_by_available_balance').html(
					'<span class=" ' + isEnabledStatus[data.OrderEntrySetting.AutoPairngByAvailableBalance].class + '">' + isEnabledStatus[data.OrderEntrySetting.AutoPairngByAvailableBalance].title + '</span>' + '<br>' +
					(data.OrderEntrySetting.AutoPairngByAvailableBalance ? '阈值: ' + data.OrderEntrySetting.AvailableBalanceThreshold + '¥' : '-'));
				$('#quick_view_order_info_by_bussiness_hours').html(
					'<span class=" ' + isEnabledStatus[data.OrderEntrySetting.AutoPairingByBusinessHours].class + '">' + isEnabledStatus[data.OrderEntrySetting.AutoPairingByBusinessHours].title + '</span>' + '<br>');

				$('#quick_view_order_info_daily_amount_limit').html(data.DailyAmountLimit + ' ¥');
				$('#quick_view_order_info_order_amount_limit').html(data.OrderAmountLowerLimit + '¥ ~' + data.OrderAmountUpperLimit + '¥');

				$('#quick_view_order_info_min_commission').html('千分之' + data.PairingInfo.MinCommissionRateInThousandth);
				$('#quick_view_order_info_available_balance').html(data.PairingInfo.AvailableBalance.toFixed(3) + ' ¥');
				$('#quick_view_order_info_quota_left_today').html(data.PairingInfo.QuotaLeftToday.toFixed(3) + ' ¥');
				$('#quick_view_order_info_last_traded').html(data.PairingInfo.DateLastTraded ? data.PairingInfo.DateLastTraded : ' -');
				$('#quick_view_order_info_total_success').html(data.PairingInfo.TotalSuccess + ' 次');
				$('#quick_view_order_info_total_failures').html(data.PairingInfo.TotalFailures + ' 次');
				$('#quick_view_order_info_highest_consecutive_success').html(data.PairingInfo.HighestConsecutiveSuccess + ' 次');
				$('#quick_view_order_info_highest_consecutive_failures').html(data.PairingInfo.HighestConsecutiveFailures + ' 次');
				$('#quick_view_order_info_current_consecutive_success').html(data.PairingInfo.CurrentConsecutiveSuccess + ' 次');
				$('#quick_view_order_info_current_consecutive_failures').html(data.PairingInfo.CurrentConsecutiveFailures + ' 次');
				$('#quick_view_order_info_success_rate').html(data.PairingInfo.SuccessRateInPercent + ' %');

			},
			error: function (response) {
				reject(response.responseText)
			}
		})

		//Map code data
		$.ajax({
			url: orderDataUrl,
			type: "Get",
			contentType: "application/json",
			data: { orderId: id },
			success: function (data) {
				var orderDataEl = '';
				if (paymentScheme === 'Barcode') {
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								收款链接:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.OrderUrl + '\
							</span>\
						</div>';
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								金额:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.Amount ? data.Amount : ' -') + '\
							</span>\
						</div>';
				}
				else if (paymentScheme === 'Merchant') {
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								应用Id:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.AppId + '\
							</span>\
						</div>';
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								支付宝公钥:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.AlipayPublicKey ? data.AlipayPublicKey : ' -') + '\
							</span>\
						</div>';
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								微信API证书:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.WechatApiCertificate ? data.WechatApiCertificate : ' -') + '\
							</span>\
						</div>';
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								私钥:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.PrivateKey ? data.PrivateKey : ' -') + '\
							</span>\
						</div>';
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								商户ID:\
                            </span>\
						    <span class="kt-widget13__text">'+ (data.MerchantId ? data.MerchantId : ' -') + '\
							</span>\
						</div>';
				}
				else if (paymentScheme === 'Bank') {
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								银行名称:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.BankName + '\
							</span>\
						</div>';
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								银行代号:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.BankMark + '\
							</span>\
						</div>';
					orderDataEl +=
						'<div class="kt-widget13__item">\
							<span class="kt-widget13__desc kt-align-right">\
								户名:\
                            </span>\
						    <span class="kt-widget13__text">'+ data.AccountName + '\
							</span>\
						</div>';
					orderDataEl +=
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
					orderDataEl +=
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


				$('#kt_modal_quick_view_order_info_widget13_order_data').html(orderDataEl)

			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})
	};


	// modal confirm payment
	var modalConfirmPayment = function (orderTrackingNumber, amount) {
		if (orderTrackingNumber) {

			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "确认已收到款项(" + amount + "¥)吗 ?",
				type: "info",

				confirmButtonText: "确认",
				confirmButtonClass: "btn btn-sm btn-bold btn-success",

				showCancelButton: true,
				cancelButtonText: "取消",
				cancelButtonClass: "btn btn-sm btn-bold btn-brand",

				showLoaderOnConfirm: true,

				preConfirm: () => {
					return new Promise(function (resolve, reject) {
						$.ajax({
							url: "ConfirmPayment",
							type: "POST",
							contentType: "application/json",
							data: JSON.stringify(orderTrackingNumber),
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
						title: '成功上分!',
						text: '可至订单列表查看纪录',
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

				$('#order_search_from').val(start.format('MM/DD/YYYY'));
				$('#order_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
			} else if (label == '昨日') {
				title = '昨日:';
				range = start.format('MM/DD/YYYY');
				$('#order_search_from').val(start.format('MM/DD/YYYY'));
				$('#order_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
			} else {
				range = start.format('MM/DD/YYYY') + ' - ' + end.format('MM/DD/YYYY');
				$('#order_search_from').val(start.format('MM/DD/YYYY'));
				$('#order_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
			}
			$('#kt_dashboard_daterangepicker_date').html(range);
			//$('#kt_dashboard_daterangepicker_title').html(title);
			updateDatatable();
			updateSumData();
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
		var contextUserId = $('#user_id').val();
		var query = $('#generalSearch').val();
		var from = $('#order_search_from').val();
		var to = $('#order_search_to').val();

		//Parse to utc time.
		var from = new Date(from).toLocaleString('en-US', { timeZone: 'UTC' });
		var to = new Date(to).toLocaleString('en-US', { timeZone: 'UTC' });

		var queryObejct = {
			generalSearch: query,
			userId: contextUserId,
			DateFrom: from,
			DateTo: to,
			OrderStatus: $('#kt_order_status_filter').val()
		};

		datatable.setDataSourceParam('query', queryObejct);
		datatable.load();
		datatable.search();
	};
	var updateSumData = function () {
		var contextUserId = $('#user_id').val();
		var query = $('#generalSearch').val();
		var from = $('#order_search_from').val();
		var to = $('#order_search_to').val();

		//Parse to utc time.
		var from = new Date(from).toLocaleString('en-US', { timeZone: 'UTC' });
		var to = new Date(to).toLocaleString('en-US', { timeZone: 'UTC' });

		var orderStatus = $('#kt_order_status_filter').val();

		var dataUrl = 'RunningAccountRecordSumData?userId=' + contextUserId + '&query=' + query + '&from=' + from + '&to=' + to
			+ '&orderStatus=' + orderStatus;

		$.ajax({
			url: dataUrl,
			type: "Get",
			contentType: "application/json",
			success: function (response) {
				console.log(response);
				$('#sum_data_order_total_count').html(response.TotalCount + '笔');
				$('#sum_data_order_total_success_amount').html(response.TotalSuccessOrderAmount + '¥');
				$('#sum_data_order_total_commission').html(response.TotalCommissionAmount + '¥');
			},
			error: function (response) {
				console.log(response);
			}
		});
	};


	// search
	var search = function () {
		/*$('#kt_order_type_filter').on('change', function () {
			datatable.search($(this).val(), 'OrderType');
		});*/
		$('#kt_order_status_filter').on('change', function () {
			datatable.search($(this).val(), 'OrderStatus');
			updateSumData();
		});
	}

	// selection
	var selection = function () {
		// init form controls
		$('#kt_order_status_filter').selectpicker();

		// event handler on check and uncheck on records
		datatable.on('kt-datatable--on-check kt-datatable--on-uncheck kt-datatable--on-layout-updated', function (e) {
			var checkedNodes;
			checkedNodes = datatable.rows('.kt-datatable__row--active').nodes(); // get selected records
			var count = checkedNodes.length; // selected records count

			$('#kt_subheader_group_selected_rows').html(count);
			console.log(count);
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
			init();

			search();
			selection();
			daterangepickerInit();
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});