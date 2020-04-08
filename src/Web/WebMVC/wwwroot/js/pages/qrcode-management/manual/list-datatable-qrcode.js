"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;


	var createBarcodeFormValidator;
	var createBarcodeFormEl;

	var createMerchantCodeFormValidator;
	var createMerchantCodeFormEl;

	var createTransactionCodeValidator;
	var createTransactionCodeFormEl;

	var createBankCodeValidator;
	var createBankCodeFormEl;

	var updateBarcodeFormValidator;
	var updateBarcodeFormEl;

	var updateMerchantCodeFormValidator;
	var updateMerchantCodeFormEl;

	var updateTransactionCodeValidator;
	var updateTransactionCodeFormEl;

	var updateBankCodeValidator;
	var updateBankCodeFormEl;

	var updateQrCodeSettingsValidator;
	var updateQrCodeSettingsFormEl;

	var updateQrCodeQuotaFormValidator;
	var updateQrCodeQuotaFormEl;


	var avatar;

	const aspController = 'QrCode';
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
			'updatedatatoggle': 'show-modal-update-barcode-qrcode-data',
			'class': ' kt-font-warning'
		},
		'Merchant': {
			'title': '原生码',
			'updatedatatoggle': 'show-modal-update-merchant-qrcode-data',
			'class': ' kt-font-success'
		},
		'Transaction': {
			'title': '支转支',
			'updatedatatoggle': 'show-modal-update-transaction-qrcode-data',
			'class': ' kt-font-primary'
		},
		'Bank': {
			'title': '支转银',
			'updatedatatoggle': 'show-modal-update-bank-qrcode-data',
			'class': ' kt-font-info'
		},
		'Envelop': {
			'title': '红包',
			'updatedatatoggle': 'show-modal-update-transaction-qrcode-data',
			'class': ' kt-font-danger'
		},
		'EnvelopPassword': {
			'title': '口令红包',
			'updatedatatoggle': 'show-modal-update-transaction-qrcode-data',
			'class': ' kt-font-danger'
		}
	};

	// init
	var init = function () {
		// init the datatables. Learn more: https://keenthemes.com/metronic/?page=docs&section=datatable
		datatable = $('#kt_apps_qrcode_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'ManualCode/Search'
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
				field: 'QuotaLeftToday',
				title: '今日额度',
				template: function (row) {
					var valueMax = row.DailyAmountLimit;
					var valueNow = row.DailyAmountLimit - row.QuotaLeftToday;
					var valueNowInPercent = (valueNow / valueMax).toFixed(2) * 100;
					var title = valueMax;
					var output = '';
					if (row.QrCodeEntrySetting.AutoPairingByQuotaLeft) {
						output += '<div class="kt-widget15">\
										<div class="kt-widget15__items">\
											<div class="kt-widget15__item">\
												<span class="kt-widget15__stats kt-font-primary">'+ valueNow + '¥</span>\
												<span class="kt-widget15__text">'+ title + '</span>\
												<div class="kt-space-10"></div>\
												<div class="progress kt-widget15__chart-progress--sm kt-m0">\
													 <div class="progress-bar bg-primary" role="progressbar" style="width: '+ valueNowInPercent + '%;" aria-valuenow="' + valueNow + '" aria-valuemin="0" aria-valuemax="' + valueMax + '"></div>\
												</div>\
											</div>\
										</div>\
								  </div>';
					}
					else {
						output = '无限制';
					}

					return output;
				}
			}, {
				field: 'CurrentConsecutiveFailures',
				title: '目前连续失败',
				template: function (row) {
					var valueMax = row.QrCodeEntrySetting.CurrentConsecutiveFailuresThreshold;
					var valueNow = row.PairingInfo.CurrentConsecutiveFailures;
					var valueNowInPercent = (valueNow / valueMax).toFixed(2) * 100;
					var title = valueMax;

					var proressBarClass =
						valueNowInPercent < 50 ? ' bg-success' :
							valueNowInPercent < 75 ? ' bg-warning' :
								valueNowInPercent <= 100 ? ' bg-danger' : null;

					var statsClass =
						valueNowInPercent < 50 ? ' kt-font-success' :
							valueNowInPercent < 75 ? ' kt-font-warning' :
								valueNowInPercent <= 100 ? ' kt-font-danger' : null;

					var output = '';
					if (row.QrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures) {
						output += '<div class="kt-widget15">\
										<div class="kt-widget15__items">\
											<div class="kt-widget15__item">\
												<span class="kt-widget15__stats '+ statsClass + '">' + valueNow + '次</span>\
												<span class="kt-widget15__text">'+ title + '</span>\
												<div class="kt-space-10"></div>\
												<div class="progress kt-widget15__chart-progress--sm kt-m0">\
													 <div class="progress-bar '+ proressBarClass + '" role="progressbar" style="width: ' + valueNowInPercent + '%;" aria-valuenow="' + valueNow + '" aria-valuemin="0" aria-valuemax="' + valueMax + '"></div>\
												</div>\
											</div>\
										</div>\
								  </div>';
					}
					else {
						output = valueNow + '次';
					}

					return output;
				}
			}, {
				field: 'SuccessRateInPercent',
				title: '历史成功率',
				template: function (row) {


					var valueMax = 100;
					var valueNow = row.PairingInfo.SuccessRateInPercent;
					var valueNowInPercent = row.PairingInfo.SuccessRateInPercent;
					var title = row.PairingInfo.TotalSuccess + '/' + (row.PairingInfo.TotalSuccess + row.PairingInfo.TotalFailures);

					var proressBarClass =
						valueNowInPercent < 50 ? ' bg-danger' :
							valueNowInPercent < 70 ? ' bg-warning' :
								valueNowInPercent <= 100 ? ' bg-success' : null;

					var statsClass =
						valueNowInPercent < 50 ? ' kt-font-danger' :
							valueNowInPercent < 70 ? ' kt-font-warning' :
								valueNowInPercent <= 100 ? ' kt-font-success' : null;


					var output = '';
					output += '<div class="kt-widget15">\
                                   <div class="kt-widget15__items">\
                                       <div class="kt-widget15__item">\
										   <span class="kt-widget15__stats '+ statsClass + '">' + valueNow + '%</span>\
									       <span class="kt-widget15__text">'+ title + '</span>\
									       <div class="kt-space-10"></div>\
									       <div class="progress kt-widget15__chart-progress--sm kt-m0">\
									           <div class="progress-bar '+ proressBarClass + '" role="progressbar" style="width: ' + valueNowInPercent + '%;" aria-valuenow="' + valueNow + '" aria-valuemin="0" aria-valuemax="' + valueMax + '"></div>\
									       </div>\
								      </div>\
							      </div>\
						      </div>';
					return output;
				}
			}, {
				field: "QrCodeStatus",
				title: "启用状态",
				width: 70,
				textAlign: 'center',
				// callback function support for column rendering
				template: function (row) {
					var status = {
						'Disabled': {
							'title': '停用',
							'class': ' btn-label-danger'
						},
						'Enabled': {
							'title': '启用',
							'class': ' btn-label-success'
						},
						'AutoDisabled': {
							'title': '自动停用',
							'class': ' btn-label-warning'
						}
					};
					return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[row.QrCodeStatus].class + '">' + status[row.QrCodeStatus].title + '</span>';
				}
			}, {
				field: "PairingStatus",
				title: "抢单状态",
				width: 100,
				textAlign: 'center',
				// callback function support for column rendering
				template: function (row) {
					var status = {
						'DisableBySystem': {
							'title': '自动封禁',
							'class': ' btn-label-danger'
						},
						'NormalDisable': {
							'title': '闲置',
							'class': ' btn-label-info'
						},
						'Pairing': {
							'title': '抢单中',
							'class': ' btn-label-success'
						},
						'ProcessingOrder': {
							'title': '处理订单中',
							'class': ' btn-label-warning'
						}
					};
					return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[row.PairingStatus].class + '">' + status[row.PairingStatus].title +
						(row.PairingStatusDescription ? '<i class="flaticon2-information" data-toggle="kt-tooltip" data-placement="right" title="' + row.PairingStatusDescription + '"></i>' : '') +
						'</span>';
				}
			}, {
				field: 'DateLastTraded',
				title: '上次交易',
				textAlign: 'center',
				autohide: false,
				template: function (row) {
					var dateLastTraded = row.DateLastTraded ? row.DateLastTraded : '-';

					return dateLastTraded;
				},
			}, {
				field: "Actions",
				width: 100,
				title: "动作",
				textAlign: 'center',
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {

					var updateLink = '/' + aspController + '/' + aspActionUpdate + '?traderAgentId=' + row.QrCodeId;
					var output = '';

					var contextUserType = $('#user_role').val();
					if (contextUserType == 'Manager') {
						output = output + '<button data-qrcode-id="' + row.QrCodeId +
							'" data-toggle="show_modal_create_test_order_for_qrcode" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air btn-success ">' +
							'测试' + '</button>';
					}
					output = output + '\
							<div class="dropdown">\
								<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
									<i class="flaticon-more-1"></i>\
								</a>\
								<div id="row_action_dropdown_menu" class="dropdown-menu dropdown-menu-right">\
									<ul class="kt-nav">\
										<li class="kt-nav__item">\
											<a href="#" data-qrcode-id="' + row.QrCodeId + '" class="kt-nav__link" data-toggle="' + paymentSchemeType[row.PaymentScheme].updatedatatoggle + '" >\
												<i class="kt-nav__link-icon flaticon2-expand"></i>\
												<span class="kt-nav__link-text">更改二维码</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#" data-qrcode-id="' + row.QrCodeId + '" class="kt-nav__link" data-toggle="show-modal-update-update-qrcode-settings" >\
												<i class="kt-nav__link-icon flaticon2-expand"></i>\
												<span class="kt-nav__link-text">轮巡设置</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#" data-qrcode-id="' + row.QrCodeId + '" class="kt-nav__link" data-toggle="show-modal-update-qrcode-quota" >\
												<i class="kt-nav__link-icon flaticon2-expand"></i>\
												<span class="kt-nav__link-text">额度设置</span>\
											</a>\
										</li>\
									</ul>\
								</div>\
							</div>\
						';
					return output;
				}
			}]
		});

		datatable.on('click', '[data-record-id]', function () {
			var userType = $(this).data('user-type');
			var contextUserRole = $('#user_role').val();
			if (contextUserRole == 'Trader') {
				//Trader can not see detail of trader agents and shops.
				if (userType !== 'Trader') {
					return;
				}
			}
			else if (contextUserRole == 'TraderAgent') {
				//Trader agent can not see detail of shops.
				if (userType !== 'Trader' && userType !== 'TraderAgent') {
					return;
				}
			}


			modalUserDetail($(this).data('record-id'), userType);
			$('#kt_modal_quick_view_user_info').modal('show');
		});

		$(document).on('click', '[data-qrcode-id]', function () {
			if ($(this).data('toggle') === 'show-modal-update-barcode-qrcode-data') {
				modalUpdateBarcodeData($(this).data('qrcode-id'));
				$('#modal__update_barcode_qrcode_data').modal('show');
			}
			if ($(this).data('toggle') === 'show-modal-update-transaction-qrcode-data') {
				modalUpdateTransactionCodeData($(this).data('qrcode-id'));
				$('#modal__update_transaction_qrcode_data').modal('show');
			}
			if ($(this).data('toggle') === 'show-modal-update-merchant-qrcode-data') {
				modalUpdateMerchantCodeData($(this).data('qrcode-id'));
				$('#modal__update_merchant_qrcode_data').modal('show');
			}
			if ($(this).data('toggle') === 'show-modal-update-bank-qrcode-data') {
				modalUpdateBankCodeData($(this).data('qrcode-id'));
				$('#modal__update_bank_qrcode_data').modal('show');
			}
			if ($(this).data('toggle') === 'show-modal-update-update-qrcode-settings') {
				modalUpdateQrCodeSettings($(this).data('qrcode-id'));
				$('#modal__update_qrcode_settings').modal('show');
			}
			if ($(this).data('toggle') === 'show-modal-update-qrcode-quota') {
				modalUpdateQrCodeQuota($(this).data('qrcode-id'));
				$('#modal__update_qrcode_quota').modal('show');
			}
		});

		$(document).on('click', '[data-toggle="show-modal-create-barcode"]', function () {
			var modal = $('#modal__create_barcode');
			var id = $(this).data('user-id');
			var baseRole = $(this).data('user-base-role');

			/*if (baseRole !== 'Manager') {
				modal.find('#UserId').val(id);
			}*/

			modal.modal('show');
		});
		$(document).on('click', '[data-toggle="show-modal-create-merchant"]', function () {
			var modal = $('#modal__create_merchant');

			modal.modal('show');
		});
		$(document).on('click', '[data-toggle="show-modal-create-transaction"]', function () {
			var modal = $('#modal__create_transaction');
			createTransactionCodeFormEl.find('[name ="paymentscheme"]').val('Transaction');
			modal.modal('show');
		});
		$(document).on('click', '[data-toggle="show-modal-create-bank"]', function () {
			var modal = $('#modal__create_bank');

			modal.modal('show');
		});
		$(document).on('click', '[data-toggle="show-modal-create-envelop"]', function () {
			var modal = $('#modal__create_transaction');
			createTransactionCodeFormEl.find('[name ="paymentscheme"]').val('Envelop');
			modal.modal('show');
		});
		$(document).on('click', '[data-toggle="show-modal-create-envelop-password"]', function () {
			var modal = $('#modal__create_transaction');
			createTransactionCodeFormEl.find('[name ="paymentscheme"]').val('EnvelopPassword');
			modal.modal('show');
		});

		$(document).on('click', '[data-toggle="show-test-qrcode"]', function () {
			//var baseRole = $(this).data('user-base-role');
			var qrCodeId = $(this).data('qrcode-id');
			$.ajax({
				url: "ManualCode/GenerateQrCode",
				type: "POST",
				contentType: "application/json",
				data: JSON.stringify(qrCodeId),
				success: function (response) {
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
					reject(response.responseText)
				}
			});
		});
		
		$(document).on('click', '[data-toggle="show_modal_create_test_order_for_qrcode"]', function () {
			var qrCodeId = $(this).data('qrcode-id');
			modalCreateTestOrderForQrCode(qrCodeId);
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

	//create temp data url.
	let getObjectURL = function (file) {
		let url = null;
		if (window.createObjectURL != undefined) { // basic
			url = window.createObjectURL(file);
		} else if (window.URL != undefined) { // mozilla(firefox)
			url = window.URL.createObjectURL(file);
		} else if (window.webkitURL != undefined) { // webkit or chrome
			url = window.webkitURL.createObjectURL(file);
		}
		return url;
	}

	// create barcode modal
	var modalCreateBarcode = function () {
		// Initialize form wizard
		var wizard = new KTWizard('kt_wizard_create_barcode', {
			startStep: 1, // initial active step number
			clickableSteps: false  // allow step clicking
		});

		wizard.on('beforeNext', function (wizardObj) {
			// Validation before going to next page
			if (createBarcodeFormValidator.form() !== true) {
				//wizardObj.stop();  // don't go to the next step
			}

			// create review data at last step.
			var nextStep = wizardObj.currentStep + 1;
			if (nextStep === wizardObj.totalSteps) {
				//base info
				var traderData = createBarcodeFormEl.find('[name ="userid"]').val();
				var fullNameData = createBarcodeFormEl.find('[name ="fullname"]').val();
				var shopData = createBarcodeFormEl.find('[name ="specifiedshopid"]').val();
				var paymentChannelData = createBarcodeFormEl.find('[name ="paymentchannel"]').val();


				var baseInfo = '交易员: &ensp; ' + (traderData ? traderData : '-');
				baseInfo += '<br /> 真实姓名: &ensp; ' + (fullNameData ? fullNameData : '-');
				baseInfo += '<br /> 指定商户: &ensp; ' + (shopData ? shopData : '-');
				baseInfo += '<br /> 支付类型: &ensp; ' + (paymentChannelData ? paymentChannelData : '-');


				//qrcode data
				var qrcodeUrl = createBarcodeFormEl.find('[name ="qrcodeurl"]').val();
				var qrcodeAmount = createBarcodeFormEl.find('[name ="amount"]').val();

				var qrcodeInfo = '个码链接: &ensp; ' + (qrcodeUrl ? qrcodeUrl : '-');
				qrcodeInfo += '<br /> 个码金额: &ensp; ' + (qrcodeAmount ? qrcodeAmount : '-');


				//risk control data
				var autoBySuccessRate = createBarcodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').is(':checked');
				var successRateThreshold = createBarcodeFormEl.find('[name ="SuccessRateThresholdInHundredth"]').val();
				var successRateMinOrders = createBarcodeFormEl.find('[name ="SuccessRateMinOrders"]').val();

				var autoByQuotaLeft = createBarcodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').is(':checked');
				var quotaLeftThreshold = createBarcodeFormEl.find('[name ="QuotaLeftThreshold"]').val();

				var autoByCurrentConsecutiveFailures = createBarcodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').is(':checked');
				var currentConsecutiveFailuresThreshold = createBarcodeFormEl.find('[name ="CurrentConsecutiveFailuresThreshold"]').val();

				var autoByAvailableBalance = createBarcodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').is(':checked');
				var availableBalanceThreshold = createBarcodeFormEl.find('[name ="AvailableBalanceThreshold"]').val();

				var autoByBusinessHours = createBarcodeFormEl.find('input[name ="AutoPairingByBusinessHours"]').is(':checked');


				var dailyLimit = createBarcodeFormEl.find('[name ="DailyAmountLimit"]').val();
				var lowerLimit = createBarcodeFormEl.find('[name ="OrderAmountLowerLimit"]').val();
				var upperLimit = createBarcodeFormEl.find('[name ="OrderAmountUpperLimit"]').val();

				var riskInfo = '成功率: &ensp;' + (autoBySuccessRate ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (successRateThreshold ? successRateThreshold : '-') + '%&ensp;</label>且已&nbsp;<label class="text-danger">累计&nbsp;' + (successRateMinOrders ? successRateMinOrders : '-') + '&nbsp;笔</label>&nbsp;(含)以上订单时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 剩余额度: &ensp;' + (autoByQuotaLeft ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (quotaLeftThreshold ? quotaLeftThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 连续失败: &ensp;' + (autoByCurrentConsecutiveFailures ? '连续失败&nbsp;<label class="text-danger font-weight-normal">超过&nbsp;' + (currentConsecutiveFailuresThreshold ? currentConsecutiveFailuresThreshold : '-') + '次&nbsp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 可用余额: &ensp;' + (autoByAvailableBalance ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (availableBalanceThreshold ? availableBalanceThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 营业时间: &ensp;' + (autoByBusinessHours ? '营业时间外自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 额度: &ensp; 单日收款金额不可超过&nbsp;' + (dailyLimit ? dailyLimit : '-') + '¥，每笔限额&nbsp;' + (lowerLimit ? lowerLimit : '-') + '¥&nbsp;~&nbsp;' + (upperLimit ? upperLimit : '-') + '¥&nbsp;';


				createBarcodeFormEl.find('#create_barcode_review_base_info').html(baseInfo);
				createBarcodeFormEl.find('#create_barcode_review_qrcode_data').html(qrcodeInfo);
				createBarcodeFormEl.find('#create_barcode_review_risk_template').html(riskInfo);

				console.log(baseInfo);
				console.log(qrcodeInfo);
				console.log(riskInfo);
			}
		})

		// Change event
		wizard.on('change', function (wizard) {
			//KTUtil.scrollTop(); <-this won't work on pop up modal.
			$('#modal__create_barcode').find('.kt-scroll').animate({ scrollTop: 0 }, 'normal');
		});

		//Validator
		createBarcodeFormValidator = createBarcodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				userid: {
					required: true
				},
				fullname: {
					required: true
				},
				paymentchannel: {
					required: true,
					minlength: 1
				},

				// Step 2
				qrcodefile: {
					accept: "image/*"
				},
				qrcodeurl: {
					required: true,
					url: true
				},
				amount: {
					integer: true,
					min: 1
				},

				// Step 3
				SuccessRateThresholdInHundredth: {
					required: true,
					integer: true,
					min: 1,
					max: 100
				},
				SuccessRateMinOrders: {
					required: true,
					integer: true,
					min: 0
				},
				QuotaLeftThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				CurrentConsecutiveFailuresThreshold: {
					required: true,
					integer: true,
					min: 0
				},
				AvailableBalanceThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				DailyAmountLimit: {
					required: true,
					integer: true,
					min: 1
				},
				OrderAmountLowerLimit: {
					required: true,
					integer: true,
					min: 0,
					max: function () { return parseInt($('#create_barcode_order_amount_upper_limit').val()); }
				},
				OrderAmountUpperLimit: {
					required: true,
					integer: true
				}
			},
			messages: {
				qrcodeurl: {
					required: "请上传个码",
					url: "无效的个码数据"
				}
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Toggle input handler
		createBarcodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBarcodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', false);
				createBarcodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createBarcodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', true);
				createBarcodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', true);
			}
		});
		createBarcodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBarcodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createBarcodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', true);
			}
		});
		createBarcodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBarcodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createBarcodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', true);
			}
		});
		createBarcodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBarcodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createBarcodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', true);
			}
		});

		//File input handler
		var qrCodeFileInput = document.getElementById('kt_create_barcode_qrcode_file_input');
		qrCodeFileInput.addEventListener('change', function () {

			//Extract qr code file data and put into input.
			var localData = getObjectURL($('#kt_create_barcode_qrcode_file_input')[0].files[0])
			qrcode.decode(localData);
			qrcode.callback = function (qrCodeUrl) {
				//$('#kt_create_barcode_qrcode_url').val(qrCodeUrl.toString());
				$('#kt_create_barcode_qrcode_url').attr('value', qrCodeUrl);
			}
		});

		$('#modal__create_barcode').on('show.bs.modal', function (e) {
			//$(this).find('.kt-scroll').attr('data-height', document.documentElement.clientHeight)
			//$(this).find('.kt-scroll').attr('style', 'height: ' + document.documentElement.clientHeight+'px; overflow: hidden;')
		}).on('hide.bs.modal', function (e) {
			$(this).find('#UserId').val('');
			$(this).find('#currentbalance').val('');
		});


		//Submit handler
		$('#modal__create_barcode').on('click', '[data-ktwizard-type="action-submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__create_barcode').find('[data-ktwizard-type="action-submit"]');


			//$('#kt_create_barcode_qrcode_input').val(qrCodeData);

			if (createBarcodeFormValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createBarcodeFormEl);


				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createBarcodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createBarcodeFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "请等待管理员审核",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_barcode').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(createBarcodeFormEl);
						swal.fire({
							"title": "添加失败",
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

	// create merchant code modal
	var modalCreateMerchantCode = function () {
		// Initialize form wizard
		var wizard = new KTWizard('kt_wizard_create_merchant', {
			startStep: 1, // initial active step number
			clickableSteps: false  // allow step clicking
		});

		wizard.on('beforeNext', function (wizardObj) {
			// Validation before going to next page
			if (createMerchantCodeFormValidator.form() !== true) {
				//wizardObj.stop();  // don't go to the next step
			}

			// create review data at last step.
			var nextStep = wizardObj.currentStep + 1;
			if (nextStep === wizardObj.totalSteps) {
				//base info
				var traderData = createMerchantCodeFormEl.find('[name ="userid"]').val();
				var fullNameData = createMerchantCodeFormEl.find('[name ="fullname"]').val();
				var shopData = createMerchantCodeFormEl.find('[name ="specifiedshopid"]').val();
				var paymentChannelData = createMerchantCodeFormEl.find('[name ="paymentchannel"]').val();

				var baseInfo = '交易员: &ensp; ' + (traderData ? traderData : '-');
				baseInfo += '<br /> 真实姓名: &ensp; ' + (fullNameData ? fullNameData : '-');
				baseInfo += '<br /> 指定商户: &ensp; ' + (shopData ? shopData : '-');
				baseInfo += '<br /> 支付类型: &ensp; ' + (paymentChannelData ? paymentChannelData : '-');


				//qrcode data
				var appid = createMerchantCodeFormEl.find('[name ="appid"]').val();
				var alipayPublicKey = createMerchantCodeFormEl.find('[name ="alipaypublickey"]').val();
				var wechatapicertificate = createMerchantCodeFormEl.find('[name ="wechatapicertificate"]').val();
				var privatekey = createMerchantCodeFormEl.find('[name ="privatekey"]').val();
				var merchantid = createMerchantCodeFormEl.find('[name ="merchantid"]').val();

				var qrcodeInfo = '应用ID: &ensp; ' + (appid ? appid : '-');
				qrcodeInfo += '<br /> 支付宝公钥: &ensp; ' + (alipayPublicKey ? alipayPublicKey : '-');
				qrcodeInfo += '<br /> 微信API证书: &ensp; ' + (wechatapicertificate ? wechatapicertificate : '-');
				qrcodeInfo += '<br /> 私钥: &ensp; ' + (privatekey ? privatekey : '-');
				qrcodeInfo += '<br /> 商户ID: &ensp; ' + (merchantid ? merchantid : '-');


				//risk control data
				var autoBySuccessRate = createMerchantCodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').is(':checked');
				var successRateThreshold = createMerchantCodeFormEl.find('[name ="SuccessRateThresholdInHundredth"]').val();
				var successRateMinOrders = createMerchantCodeFormEl.find('[name ="SuccessRateMinOrders"]').val();

				var autoByQuotaLeft = createMerchantCodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').is(':checked');
				var quotaLeftThreshold = createMerchantCodeFormEl.find('[name ="QuotaLeftThreshold"]').val();

				var autoByCurrentConsecutiveFailures = createMerchantCodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').is(':checked');
				var currentConsecutiveFailuresThreshold = createMerchantCodeFormEl.find('[name ="CurrentConsecutiveFailuresThreshold"]').val();

				var autoByAvailableBalance = createMerchantCodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').is(':checked');
				var availableBalanceThreshold = createMerchantCodeFormEl.find('[name ="AvailableBalanceThreshold"]').val();

				var autoByBusinessHours = createMerchantCodeFormEl.find('input[name ="AutoPairingByBusinessHours"]').is(':checked');


				var dailyLimit = createMerchantCodeFormEl.find('[name ="DailyAmountLimit"]').val();
				var lowerLimit = createMerchantCodeFormEl.find('[name ="OrderAmountLowerLimit"]').val();
				var upperLimit = createMerchantCodeFormEl.find('[name ="OrderAmountUpperLimit"]').val();

				var riskInfo = '成功率: &ensp;' + (autoBySuccessRate ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (successRateThreshold ? successRateThreshold : '-') + '%&ensp;</label>且已&nbsp;<label class="text-danger">累计&nbsp;' + (successRateMinOrders ? successRateMinOrders : '-') + '&nbsp;笔</label>&nbsp;(含)以上订单时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 剩余额度: &ensp;' + (autoByQuotaLeft ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (quotaLeftThreshold ? quotaLeftThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 连续失败: &ensp;' + (autoByCurrentConsecutiveFailures ? '连续失败&nbsp;<label class="text-danger font-weight-normal">超过&nbsp;' + (currentConsecutiveFailuresThreshold ? currentConsecutiveFailuresThreshold : '-') + '次&nbsp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 可用余额: &ensp;' + (autoByAvailableBalance ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (availableBalanceThreshold ? availableBalanceThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 营业时间: &ensp;' + (autoByBusinessHours ? '营业时间外自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 额度: &ensp; 单日收款金额不可超过&nbsp;' + (dailyLimit ? dailyLimit : '-') + '¥，每笔限额&nbsp;' + (lowerLimit ? lowerLimit : '-') + '¥&nbsp;~&nbsp;' + (upperLimit ? upperLimit : '-') + '¥&nbsp;';


				createMerchantCodeFormEl.find('#create_merchant_review_base_info').html(baseInfo);
				createMerchantCodeFormEl.find('#create_merchant_review_qrcode_data').html(qrcodeInfo);
				createMerchantCodeFormEl.find('#create_merchant_review_risk_template').html(riskInfo);
			}
		})

		// Change event
		wizard.on('change', function (wizard) {
			//KTUtil.scrollTop(); <-this won't work on pop up modal.
			$('#modal__create_merchant').find('.kt-scroll').animate({ scrollTop: 0 }, 'normal');
		});

		//Validator
		createMerchantCodeFormValidator = createMerchantCodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				userid: {
					required: true
				},
				fullname: {
					required: true
				},
				paymentchannel: {
					required: true,
					minlength: 1
				},

				// Step 2
				appid: {
					required: true
				},
				privatekey: {
					required: true
				},
				merchantid: {
					required: true
				},

				// Step 3
				SuccessRateThresholdInHundredth: {
					required: true,
					integer: true,
					min: 1,
					max: 100
				},
				SuccessRateMinOrders: {
					required: true,
					integer: true,
					min: 0
				},
				QuotaLeftThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				CurrentConsecutiveFailuresThreshold: {
					required: true,
					integer: true,
					min: 0
				},
				AvailableBalanceThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				DailyAmountLimit: {
					required: true,
					integer: true,
					min: 1
				},
				OrderAmountLowerLimit: {
					required: true,
					integer: true,
					min: 0,
					max: function () { return parseInt($('#create_merchant_order_amount_upper_limit').val()); }
				},
				OrderAmountUpperLimit: {
					required: true,
					integer: true
				},
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Toggle input handler
		createMerchantCodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createMerchantCodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', false);
				createMerchantCodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createMerchantCodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', true);
				createMerchantCodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', true);
			}
		});
		createMerchantCodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createMerchantCodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createMerchantCodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', true);
			}
		});
		createMerchantCodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createMerchantCodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createMerchantCodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', true);
			}
		});
		createMerchantCodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createMerchantCodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', false);
			}
			else {
				console.log($(this).checked);
				//Not checked, disable relevant inputs.
				createMerchantCodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', true);
			}
		});


		$('#modal__create_merchant').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});


		//Submit handler
		$('#modal__create_merchant').on('click', '[data-ktwizard-type="action-submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__create_merchant').find('[data-ktwizard-type="action-submit"]');



			if (createMerchantCodeFormValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createMerchantCodeFormEl);


				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createMerchantCodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createMerchantCodeFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "请等待管理员审核",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_merchant').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(createMerchantCodeFormEl);
						swal.fire({
							"title": "添加失败",
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

	// create transaction code modal
	var modalCreateTransactionCode = function () {
		// Initialize form wizard
		var wizard = new KTWizard('kt_wizard_create_transaction', {
			startStep: 1, // initial active step number
			clickableSteps: false  // allow step clicking
		});

		wizard.on('beforeNext', function (wizardObj) {
			// Validation before going to next page
			if (createTransactionCodeValidator.form() !== true) {
				//wizardObj.stop();  // don't go to the next step
			}

			// create review data at last step.
			var nextStep = wizardObj.currentStep + 1;
			if (nextStep === wizardObj.totalSteps) {
				//base info
				var traderData = createTransactionCodeFormEl.find('[name ="userid"]').val();
				var fullNameData = createTransactionCodeFormEl.find('[name ="fullname"]').val();
				var shopData = createTransactionCodeFormEl.find('[name ="specifiedshopid"]').val();
				var paymentSchemeData = createTransactionCodeFormEl.find('[name ="paymentscheme"]').val();

				var baseInfo = '交易员: &ensp; ' + (traderData ? traderData : '-');
				baseInfo += '<br /> 真实姓名: &ensp; ' + (fullNameData ? fullNameData : '-');
				baseInfo += '<br /> 指定商户: &ensp; ' + (shopData ? shopData : '-');
				baseInfo += '<br /> 支付通道: &ensp; ' + (paymentSchemeData ? paymentSchemeData : '-');


				//qrcode data
				var alipayUserId = createTransactionCodeFormEl.find('[name ="alipayuserid"]').val();

				var qrcodeInfo = '支付宝用户ID: &ensp; ' + (alipayUserId ? alipayUserId : '-');


				//risk control data
				var autoBySuccessRate = createTransactionCodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').is(':checked');
				var successRateThreshold = createTransactionCodeFormEl.find('[name ="SuccessRateThresholdInHundredth"]').val();
				var successRateMinOrders = createTransactionCodeFormEl.find('[name ="SuccessRateMinOrders"]').val();

				var autoByQuotaLeft = createTransactionCodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').is(':checked');
				var quotaLeftThreshold = createTransactionCodeFormEl.find('[name ="QuotaLeftThreshold"]').val();

				var autoByCurrentConsecutiveFailures = createTransactionCodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').is(':checked');
				var currentConsecutiveFailuresThreshold = createTransactionCodeFormEl.find('[name ="CurrentConsecutiveFailuresThreshold"]').val();

				var autoByAvailableBalance = createTransactionCodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').is(':checked');
				var availableBalanceThreshold = createTransactionCodeFormEl.find('[name ="AvailableBalanceThreshold"]').val();

				var autoByBusinessHours = createTransactionCodeFormEl.find('input[name ="AutoPairingByBusinessHours"]').is(':checked');


				var dailyLimit = createTransactionCodeFormEl.find('[name ="DailyAmountLimit"]').val();
				var lowerLimit = createTransactionCodeFormEl.find('[name ="OrderAmountLowerLimit"]').val();
				var upperLimit = createTransactionCodeFormEl.find('[name ="OrderAmountUpperLimit"]').val();

				var riskInfo = '成功率: &ensp;' + (autoBySuccessRate ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (successRateThreshold ? successRateThreshold : '-') + '%&ensp;</label>且已&nbsp;<label class="text-danger">累计&nbsp;' + (successRateMinOrders ? successRateMinOrders : '-') + '&nbsp;笔</label>&nbsp;(含)以上订单时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 剩余额度: &ensp;' + (autoByQuotaLeft ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (quotaLeftThreshold ? quotaLeftThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 连续失败: &ensp;' + (autoByCurrentConsecutiveFailures ? '连续失败&nbsp;<label class="text-danger font-weight-normal">超过&nbsp;' + (currentConsecutiveFailuresThreshold ? currentConsecutiveFailuresThreshold : '-') + '次&nbsp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 可用余额: &ensp;' + (autoByAvailableBalance ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (availableBalanceThreshold ? availableBalanceThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 营业时间: &ensp;' + (autoByBusinessHours ? '营业时间外自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 额度: &ensp; 单日收款金额不可超过&nbsp;' + (dailyLimit ? dailyLimit : '-') + '¥，每笔限额&nbsp;' + (lowerLimit ? lowerLimit : '-') + '¥&nbsp;~&nbsp;' + (upperLimit ? upperLimit : '-') + '¥&nbsp;';


				createTransactionCodeFormEl.find('#create_transaction_review_base_info').html(baseInfo);
				createTransactionCodeFormEl.find('#create_transaction_review_qrcode_data').html(qrcodeInfo);
				createTransactionCodeFormEl.find('#create_transaction_review_risk_template').html(riskInfo);
			}
		})

		// Change event
		wizard.on('change', function (wizard) {
			//KTUtil.scrollTop(); <-this won't work on pop up modal.
			$('#modal__create_transaction').find('.kt-scroll').animate({ scrollTop: 0 }, 'normal');
		});

		//Validator
		createTransactionCodeValidator = createTransactionCodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				userid: {
					required: true
				},
				fullname: {
					required: true
				},
				paymentscheme: {
					required: true,
					minlength: 1
				},

				// Step 2
				alipayuserid: {
					required: true,
				},

				// Step 3
				SuccessRateThresholdInHundredth: {
					required: true,
					integer: true,
					min: 1,
					max: 100
				},
				SuccessRateMinOrders: {
					required: true,
					integer: true,
					min: 0
				},
				QuotaLeftThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				CurrentConsecutiveFailuresThreshold: {
					required: true,
					integer: true,
					min: 0
				},
				AvailableBalanceThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				DailyAmountLimit: {
					required: true,
					integer: true,
					min: 1
				},
				OrderAmountLowerLimit: {
					required: true,
					integer: true,
					min: 0,
					max: function () { return parseInt($('#create_transaction_order_amount_upper_limit').val()); }
				},
				OrderAmountUpperLimit: {
					required: true,
					integer: true
				}
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Toggle input handler
		createTransactionCodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createTransactionCodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', false);
				createTransactionCodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createTransactionCodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', true);
				createTransactionCodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', true);
			}
		});
		createTransactionCodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createTransactionCodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createTransactionCodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', true);
			}
		});
		createTransactionCodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createTransactionCodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createTransactionCodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', true);
			}
		});
		createTransactionCodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createTransactionCodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createTransactionCodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', true);
			}
		});


		$('#modal__create_transaction').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});


		//Submit handler
		$('#modal__create_transaction').on('click', '[data-ktwizard-type="action-submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__create_transaction').find('[data-ktwizard-type="action-submit"]');

			if (createTransactionCodeValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createTransactionCodeFormEl);


				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createTransactionCodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createTransactionCodeFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "请等待管理员审核",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_transaction').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(createTransactionCodeFormEl);
						swal.fire({
							"title": "添加失败",
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

	// create bank code modal
	var modalCreateBankCode = function () {
		// Initialize form wizard
		var wizard = new KTWizard('kt_wizard_create_bank', {
			startStep: 1, // initial active step number
			clickableSteps: false  // allow step clicking
		});

		wizard.on('beforeNext', function (wizardObj) {
			// Validation before going to next page
			if (createBankCodeValidator.form() !== true) {
				//wizardObj.stop();  // don't go to the next step
			}

			// create review data at last step.
			var nextStep = wizardObj.currentStep + 1;
			if (nextStep === wizardObj.totalSteps) {
				//base info
				var traderData = createBankCodeFormEl.find('[name ="userid"]').val();
				var fullNameData = createBankCodeFormEl.find('[name ="fullname"]').val();
				var shopData = createBankCodeFormEl.find('[name ="specifiedshopid"]').val();

				var baseInfo = '交易员: &ensp; ' + (traderData ? traderData : '-');
				baseInfo += '<br /> 真实姓名: &ensp; ' + (fullNameData ? fullNameData : '-');
				baseInfo += '<br /> 指定商户: &ensp; ' + (shopData ? shopData : '-');


				//qrcode data
				var bankname = createBankCodeFormEl.find('[name ="bankname"]').val();
				var bankmark = createBankCodeFormEl.find('[name ="bankmark"]').val();
				var accountname = createBankCodeFormEl.find('[name ="accountname"]').val();
				var cardnumber = createBankCodeFormEl.find('[name ="cardnumber"]').val();

				var qrcodeInfo = '银行名称: &ensp; ' + (bankname ? bankname : '-');
				qrcodeInfo += '<br />银行缩写: &ensp; ' + (bankmark ? bankmark : '-');
				qrcodeInfo += '<br />银行户名: &ensp; ' + (accountname ? accountname : '-');
				qrcodeInfo += '<br />银行卡号: &ensp; ' + (cardnumber ? cardnumber : '-');


				//risk control data
				var autoBySuccessRate = createBankCodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').is(':checked');
				var successRateThreshold = createBankCodeFormEl.find('[name ="SuccessRateThresholdInHundredth"]').val();
				var successRateMinOrders = createBankCodeFormEl.find('[name ="SuccessRateMinOrders"]').val();

				var autoByQuotaLeft = createBankCodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').is(':checked');
				var quotaLeftThreshold = createBankCodeFormEl.find('[name ="QuotaLeftThreshold"]').val();

				var autoByCurrentConsecutiveFailures = createBankCodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').is(':checked');
				var currentConsecutiveFailuresThreshold = createBankCodeFormEl.find('[name ="CurrentConsecutiveFailuresThreshold"]').val();

				var autoByAvailableBalance = createBankCodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').is(':checked');
				var availableBalanceThreshold = createBankCodeFormEl.find('[name ="AvailableBalanceThreshold"]').val();

				var autoByBusinessHours = createBankCodeFormEl.find('input[name ="AutoPairingByBusinessHours"]').is(':checked');


				var dailyLimit = createBankCodeFormEl.find('[name ="DailyAmountLimit"]').val();
				var lowerLimit = createBankCodeFormEl.find('[name ="OrderAmountLowerLimit"]').val();
				var upperLimit = createBankCodeFormEl.find('[name ="OrderAmountUpperLimit"]').val();

				var riskInfo = '成功率: &ensp;' + (autoBySuccessRate ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (successRateThreshold ? successRateThreshold : '-') + '%&ensp;</label>且已&nbsp;<label class="text-danger">累计&nbsp;' + (successRateMinOrders ? successRateMinOrders : '-') + '&nbsp;笔</label>&nbsp;(含)以上订单时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 剩余额度: &ensp;' + (autoByQuotaLeft ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (quotaLeftThreshold ? quotaLeftThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 连续失败: &ensp;' + (autoByCurrentConsecutiveFailures ? '连续失败&nbsp;<label class="text-danger font-weight-normal">超过&nbsp;' + (currentConsecutiveFailuresThreshold ? currentConsecutiveFailuresThreshold : '-') + '次&nbsp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 可用余额: &ensp;' + (autoByAvailableBalance ? '<label class="text-danger font-weight-normal">低于&nbsp;' + (availableBalanceThreshold ? availableBalanceThreshold : '-') + '¥&ensp;</label>时自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 营业时间: &ensp;' + (autoByBusinessHours ? '营业时间外自动关闭轮巡/抢单' : '-');
				riskInfo += '<br /> 额度: &ensp; 单日收款金额不可超过&nbsp;' + (dailyLimit ? dailyLimit : '-') + '¥，每笔限额&nbsp;' + (lowerLimit ? lowerLimit : '-') + '¥&nbsp;~&nbsp;' + (upperLimit ? upperLimit : '-') + '¥&nbsp;';


				createBankCodeFormEl.find('#create_bank_review_base_info').html(baseInfo);
				createBankCodeFormEl.find('#create_bank_review_qrcode_data').html(qrcodeInfo);
				createBankCodeFormEl.find('#create_bank_review_risk_template').html(riskInfo);
			}
		})

		// Change event
		wizard.on('change', function (wizard) {
			//KTUtil.scrollTop(); <-this won't work on pop up modal.
			$('#modal__create_bank').find('.kt-scroll').animate({ scrollTop: 0 }, 'normal');
		});

		//Validator
		createBankCodeValidator = createBankCodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				userid: {
					required: true
				},
				fullname: {
					required: true
				},

				// Step 2
				bankselect: {
					required: true
				},
				accountname: {
					required: true
				},
				cardnumber: {
					required: true
				},

				// Step 3
				SuccessRateThresholdInHundredth: {
					required: true,
					integer: true,
					min: 1,
					max: 100
				},
				SuccessRateMinOrders: {
					required: true,
					integer: true,
					min: 0
				},
				QuotaLeftThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				CurrentConsecutiveFailuresThreshold: {
					required: true,
					integer: true,
					min: 0
				},
				AvailableBalanceThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				DailyAmountLimit: {
					required: true,
					integer: true,
					min: 1
				},
				OrderAmountLowerLimit: {
					required: true,
					integer: true,
					min: 0,
					max: function () { return parseInt($('#create_bank_order_amount_upper_limit').val()); }
				},
				OrderAmountUpperLimit: {
					required: true,
					integer: true
				}
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Toggle input handler
		createBankCodeFormEl.find('input[name ="AutoPairingBySuccessRate"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBankCodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', false);
				createBankCodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createBankCodeFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', true);
				createBankCodeFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', true);
			}
		});
		createBankCodeFormEl.find('input[name ="AutoPairingByQuotaLeft"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBankCodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createBankCodeFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', true);
			}
		});
		createBankCodeFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBankCodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createBankCodeFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', true);
			}
		});
		createBankCodeFormEl.find('input[name ="AutoPairngByAvailableBalance"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				createBankCodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				createBankCodeFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', true);
			}
		});

		//Bank select handler
		createBankCodeFormEl.find('select[name ="bankselect"]').on('change', function () {
			var bankObject = $(this).children("option:selected");
			if (bankObject) {
				createBankCodeFormEl.find('input[name ="bankname"]').val(bankObject.data('bankname'));
				createBankCodeFormEl.find('input[name ="bankmark"]').val(bankObject.data('bankmark'));
			}
		});


		$('#modal__create_bank').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});


		//Submit handler
		$('#modal__create_bank').on('click', '[data-ktwizard-type="action-submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__create_bank').find('[data-ktwizard-type="action-submit"]');



			if (createBankCodeValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createBankCodeFormEl);


				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createBankCodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createBankCodeFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "请等待管理员审核",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_bank').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(createBankCodeFormEl);
						swal.fire({
							"title": "添加失败",
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


	// update barcode data modal
	var modalUpdateBarcodeData = function (id) {
		//Validator
		updateBarcodeFormValidator = updateBarcodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				qrcodeid: {
					required: true
				},

				// Step 2
				qrcodefile: {
					accept: "image/*"
				},
				qrcodeurl: {
					required: true,
					url: true
				},
				amount: {
					integer: true,
					min: 1
				},
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//File input handler
		var qrCodeFileInput = document.getElementById('kt_update_barcode_qrcode_file_input');
		qrCodeFileInput.addEventListener('change', function () {
			//Extract qr code file data and put into input.
			var localData = getObjectURL($('#kt_update_barcode_qrcode_file_input')[0].files[0])
			qrcode.decode(localData);
			qrcode.callback = function (qrCodeUrl) {
				//$('#kt_create_barcode_qrcode_url').val(qrCodeUrl.toString());
				$('#kt_update_barcode_qrcode_url').attr('value', qrCodeUrl);
			}
		});

		//Set default data.
		$.ajax({
			url: '../ManualCode/BarcodeInfo',
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				//Set qrcode id.
				updateBarcodeFormEl.find('[name="qrcodeid"]').val(data.QrCodeId);

				//Set qrcode url.
				updateBarcodeFormEl.find('[name="qrcodeurl"]').val(data.QrCodeUrl);

				//Set qrcode amount.
				updateBarcodeFormEl.find('[name="amount"]').val(data.Amount);
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

		$('#modal__update_barcode_qrcode_data').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});

		//Submit handler
		$('#modal__update_barcode_qrcode_data').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__update_barcode_qrcode_data').find('[data-action="submit"]');

			if (updateBarcodeFormValidator.form()) {
				KTApp.progress(btn);
				KTApp.block(updateBarcodeFormEl);

				updateBarcodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(updateBarcodeFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "请等待管理员审核",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__update_barcode_qrcode_data').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(updateBarcodeFormEl);
						swal.fire({
							"title": "添加失败",
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

	// update merchant code data modal
	var modalUpdateMerchantCodeData = function (id) {
		//Validator
		updateMerchantCodeFormValidator = updateMerchantCodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				qrcodeid: {
					required: true
				},

				appid: {
					required: true,
				},
				privatekey: {
					required: true,
				},
				merchantid: {
					required: true,
				},
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Set default data.
		$.ajax({
			url: '../ManualCode/MerchantInfo',
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				//Set qrcode id.
				updateMerchantCodeFormEl.find('[name="qrcodeid"]').val(data.QrCodeId);

				//Set app id.
				updateMerchantCodeFormEl.find('[name="appid"]').val(data.AppId);

				//Set public key alipay.
				updateMerchantCodeFormEl.find('[name="alipaypublickey"]').val(data.AlipayPublicKey);

				//Set qrcode amount.
				updateMerchantCodeFormEl.find('[name="wechatapicertificate"]').val(data.WechatApiCertificate);

				//Set private key.
				updateMerchantCodeFormEl.find('[name="privatekey"]').val(data.PrivateKey);

				//Set merchant id.
				updateMerchantCodeFormEl.find('[name="merchantid"]').val(data.MerchantId);
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

		$('#modal__update_merchant_qrcode_data').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});

		//Submit handler
		$('#modal__update_merchant_qrcode_data').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__update_merchant_qrcode_data').find('[data-action="submit"]');

			if (updateMerchantCodeFormValidator.form()) {
				KTApp.progress(btn);
				KTApp.block(updateMerchantCodeFormEl);

				updateMerchantCodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(updateMerchantCodeFormEl);

						swal.fire({
							"title": "更新成功",
							"text": "",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__update_merchant_qrcode_data').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(updateMerchantCodeFormEl);
						swal.fire({
							"title": "更新失败",
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

	// update transaction code data modal
	var modalUpdateTransactionCodeData = function (id) {
		//Validator
		updateTransactionCodeValidator = updateTransactionCodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				qrcodeid: {
					required: true
				},

				alipayuserid: {
					required: true,
				},
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Set default data.
		$.ajax({
			url: '../ManualCode/TransactionInfo',
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				//Set qrcode id.
				updateTransactionCodeFormEl.find('[name="qrcodeid"]').val(data.QrCodeId);

				//Set app id.
				updateTransactionCodeFormEl.find('[name="alipayuserid"]').val(data.AlipayUserId);
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

		$('#modal__update_transaction_qrcode_data').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});

		//Submit handler
		$('#modal__update_transaction_qrcode_data').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__update_transaction_qrcode_data').find('[data-action="submit"]');

			if (updateTransactionCodeValidator.form()) {
				KTApp.progress(btn);
				KTApp.block(updateTransactionCodeFormEl);

				updateTransactionCodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(updateTransactionCodeFormEl);

						swal.fire({
							"title": "更新成功",
							"text": "",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__update_transaction_qrcode_data').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(updateTransactionCodeFormEl);
						swal.fire({
							"title": "添加失败",
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

	// update bank code data modal
	var modalUpdateBankCodeData = function (id) {
		//Validator
		updateBankCodeValidator = updateBankCodeFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				qrcodeid: {
					required: true
				},

				bankselect: {
					required: true,
				},
				cardnumber: {
					required: true,
				},
				accountname: {
					required: true,
				},
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Bank select handler
		updateBankCodeFormEl.find('select[name ="bankselect"]').on('change', function () {
			var bankObject = $(this).children("option:selected");
			if (bankObject) {
				updateBankCodeFormEl.find('input[name ="bankname"]').val(bankObject.data('bankname'));
				updateBankCodeFormEl.find('input[name ="bankmark"]').val(bankObject.data('bankmark'));
			}
		});

		//Set default data.
		$.ajax({
			url: '../ManualCode/BankInfo',
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				//Set qrcode id.
				updateBankCodeFormEl.find('[name="qrcodeid"]').val(data.QrCodeId);

				//Set bank select list.
				updateBankCodeFormEl.find('[name="bankselect"]').val(data.BankName);

				//Set bank name
				updateBankCodeFormEl.find('[name="bankname"]').val(data.BankName);

				//Set bank mark
				updateBankCodeFormEl.find('[name="bankmark"]').val(data.BankMark);

				//Set card number
				updateBankCodeFormEl.find('[name="cardnumber"]').val(data.CardNumber);

				//Set account name
				updateBankCodeFormEl.find('[name="accountname"]').val(data.AccountName);
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

		$('#modal__update_bank_qrcode_data').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});

		//Submit handler
		$('#modal__update_bank_qrcode_data').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__update_bank_qrcode_data').find('[data-action="submit"]');

			if (updateBankCodeValidator.form()) {
				KTApp.progress(btn);
				KTApp.block(updateBankCodeFormEl);

				updateBankCodeFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(updateBankCodeFormEl);

						swal.fire({
							"title": "更新成功",
							"text": "",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__update_bank_qrcode_data').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(updateBankCodeFormEl);
						swal.fire({
							"title": "添加失败",
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


	// update qr settings modal
	var modalUpdateQrCodeSettings = function (id) {
		//Validator
		updateQrCodeSettingsValidator = updateQrCodeSettingsFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",
			// Validation rules
			rules: {
				qrcodeid: {
					required: true
				},

				SuccessRateThresholdInHundredth: {
					required: true,
					integer: true,
					min: 1,
					max: 100
				},
				SuccessRateMinOrders: {
					required: true,
					integer: true,
					min: 0
				},
				QuotaLeftThreshold: {
					required: true,
					integer: true,
					min: 1
				},
				CurrentConsecutiveFailuresThreshold: {
					required: true,
					integer: true,
					min: 0
				},
				AvailableBalanceThreshold: {
					required: true,
					integer: true,
					min: 1
				},
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Toggle input handler
		updateQrCodeSettingsFormEl.find('input[name ="AutoPairingBySuccessRate"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				updateQrCodeSettingsFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', false);
				updateQrCodeSettingsFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				updateQrCodeSettingsFormEl.find('input[name ="SuccessRateThresholdInHundredth"]').prop('disabled', true);
				updateQrCodeSettingsFormEl.find('input[name ="SuccessRateMinOrders"]').prop('disabled', true);
			}
		});
		updateQrCodeSettingsFormEl.find('input[name ="AutoPairingByQuotaLeft"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				updateQrCodeSettingsFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				updateQrCodeSettingsFormEl.find('input[name ="QuotaLeftThreshold"]').prop('disabled', true);
			}
		});
		updateQrCodeSettingsFormEl.find('input[name ="AutoPairingByCurrentConsecutiveFailures"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				updateQrCodeSettingsFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				updateQrCodeSettingsFormEl.find('input[name ="CurrentConsecutiveFailuresThreshold"]').prop('disabled', true);
			}
		});
		updateQrCodeSettingsFormEl.find('input[name ="AutoPairngByAvailableBalance"]').on('change', function () {
			//If checked, enable relevant inputs.
			if ($(this).is(':checked')) {
				updateQrCodeSettingsFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', false);
			}
			else {
				//Not checked, disable relevant inputs.
				updateQrCodeSettingsFormEl.find('input[name ="AvailableBalanceThreshold"]').prop('disabled', true);
			}
		});


		//Set default data.
		$.ajax({
			url: '../ManualCode/Detail',
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				//Set qrcode id.
				updateQrCodeSettingsFormEl.find('[name="qrcodeid"]').val(data.QrCodeId);

				updateQrCodeSettingsFormEl.find('[name="AutoPairingBySuccessRate"]').val(data.QrCodeEntrySetting.AutoPairingBySuccessRate);
				updateQrCodeSettingsFormEl.find('[name="AutoPairingByQuotaLeft"]').val(data.QrCodeEntrySetting.AutoPairingByQuotaLeft);
				updateQrCodeSettingsFormEl.find('[name="AutoPairingByBusinessHours"]').val(data.QrCodeEntrySetting.AutoPairingByBusinessHours);
				updateQrCodeSettingsFormEl.find('[name="AutoPairingByCurrentConsecutiveFailures"]').val(data.QrCodeEntrySetting.AutoPairingByCurrentConsecutiveFailures);
				updateQrCodeSettingsFormEl.find('[name="AutoPairngByAvailableBalance"]').val(data.QrCodeEntrySetting.AutoPairngByAvailableBalance);

				updateQrCodeSettingsFormEl.find('[name="SuccessRateThresholdInHundredth"]').val(data.QrCodeEntrySetting.SuccessRateThresholdInHundredth);
				updateQrCodeSettingsFormEl.find('[name="SuccessRateMinOrders"]').val(data.QrCodeEntrySetting.SuccessRateMinOrders);
				updateQrCodeSettingsFormEl.find('[name="QuotaLeftThreshold"]').val(data.QrCodeEntrySetting.QuotaLeftThreshold);
				updateQrCodeSettingsFormEl.find('[name="CurrentConsecutiveFailuresThreshold"]').val(data.QrCodeEntrySetting.CurrentConsecutiveFailuresThreshold);
				updateQrCodeSettingsFormEl.find('[name="AvailableBalanceThreshold"]').val(data.QrCodeEntrySetting.AvailableBalanceThreshold);
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

		$('#modal__update_qrcode_settings').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});

		//Submit handler
		$('#modal__update_qrcode_settings').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__update_qrcode_settings').find('[data-action="submit"]');

			if (updateQrCodeSettingsValidator.form()) {
				KTApp.progress(btn);
				KTApp.block(updateQrCodeSettingsFormEl);

				updateQrCodeSettingsFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(updateQrCodeSettingsFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "请等待管理员审核",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__update_qrcode_settings').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(updateQrCodeSettingsFormEl);
						swal.fire({
							"title": "添加失败",
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


	// update qr quota modal
	var modalUpdateQrCodeQuota = function (id) {
		//Validator
		updateQrCodeQuotaFormValidator = updateQrCodeQuotaFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",
			// Validation rules
			rules: {
				qrcodeid: {
					required: true
				},

				DailyAmountLimit: {
					required: true,
					integer: true,
					min: 1
				},
				OrderAmountLowerLimit: {
					required: true,
					integer: true,
					min: 0,
					max: function () { return parseInt($('#update_qrcode_quota_order_amount_upper_limit').val()); }
				},
				OrderAmountUpperLimit: {
					required: true,
					integer: true
				}
			},
			messages: {
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份栏位中有一些错误。 请更正它们。",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		//Set default data.
		$.ajax({
			url: '../ManualCode/Detail',
			type: "Get",
			contentType: "application/json",
			data: { qrCodeId: id },
			success: function (data) {
				//Set qrcode id.
				updateQrCodeQuotaFormEl.find('[name="qrcodeid"]').val(data.QrCodeId);

				updateQrCodeQuotaFormEl.find('[name="DailyAmountLimit"]').val(data.DailyAmountLimit);
				updateQrCodeQuotaFormEl.find('[name="OrderAmountLowerLimit"]').val(data.OrderAmountLowerLimit);
				updateQrCodeQuotaFormEl.find('[name="OrderAmountUpperLimit"]').val(data.OrderAmountUpperLimit);
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

		$('#modal__update_qrcode_quota').on('show.bs.modal', function (e) {
		}).on('hide.bs.modal', function (e) {
		});

		//Submit handler
		$('#modal__update_qrcode_quota').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__update_qrcode_quota').find('[data-action="submit"]');

			if (updateQrCodeQuotaFormValidator.form()) {
				KTApp.progress(btn);
				KTApp.block(updateQrCodeQuotaFormEl);

				updateQrCodeQuotaFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(updateQrCodeQuotaFormEl);

						swal.fire({
							"title": "更新成功",
							"text": "",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__update_qrcode_quota').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(updateQrCodeQuotaFormEl);
						swal.fire({
							"title": "添加失败",
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

	// create test order page modal
	var modalCreateTestOrderForQrCode = function (id) {
		swal.mixin({
			buttonsStyling: false,
			input: 'text',
			type: "info",

			confirmButtonText: "确认",
			confirmButtonClass: "btn btn-sm btn-bold btn-success",

			showCancelButton: true,
			cancelButtonText: "取消",
			cancelButtonClass: "btn btn-sm btn-bold btn-brand",

			showLoaderOnConfirm: true,

			progressSteps: ['1'],

			allowOutsideClick: () => !Swal.isLoading(),
		}).queue([
			'订单金额'
		]).then(function (result) {
			if (result.value) {
				var amount = parseInt(result.value[0]);

				let vm = {
					QrCodeId: id,
					OrderAmount: amount,
				};

				$.ajax({
					url: "Order/CreateTestOrderToPlatform",
					type: "POST",
					contentType: "application/json",
					data: JSON.stringify(vm),
					success: function (response) {
						if (response.success) {
							var localUrl = response.localUrl;
							console.log(response);
							window.open(
								'https://localhost/' + localUrl,
								'_blank' // <- This is what makes it open in a new window.
							);
						}
						else {
							swal.fire({
								title: '失败',
								text: response.responseText,
								type: 'error',
								buttonsStyling: false,
								confirmButtonText: "OK",
								confirmButtonClass: "btn btn-sm btn-bold btn-brand",
							});
						}
					},
					error: function (response) {
						console.log('Create Order Result: Error.');
						console.log(response);

						swal.fire({
							title: '已取消',
							text: response.responseText,
							type: 'error',
							buttonsStyling: false,
							confirmButtonText: "OK",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						});
						//reject(response.responseText)
					}
				})

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

	//select trader
	var initSelectTrader = function () {
		var userType = $('#user_role').val();
		if (userType !== 'Manager' && userType !== 'TraderAgent') {
			return;
		}

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

		$("#kt_select2_create_barcode_trader").select2({
			placeholder: "输入交易员名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchTrader",
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
					console.log('here');
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
		$("#kt_select2_create_merchant_trader").select2({
			placeholder: "输入交易员名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchTrader",
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
					console.log('here');
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
		$("#kt_select2_create_transaction_trader").select2({
			placeholder: "输入交易员名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchTrader",
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
					console.log('here');
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
		$("#kt_select2_create_bank_trader").select2({
			placeholder: "输入交易员名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchTrader",
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
					console.log('here');
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

	//select shop
	var initSelectShop = function () {
		var userType = $('#user_role').val();
		if (userType !== 'Manager') {
			return;
		}

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

		$("#kt_select2_create_barcode_shop").select2({
			placeholder: "输入商户名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchShop",
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
					console.log('here');
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
		$("#kt_select2_create_merchant_shop").select2({
			placeholder: "输入商户名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchShop",
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
					console.log('here');
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
		$("#kt_select2_create_transaction_shop").select2({
			placeholder: "输入商户名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchShop",
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
					console.log('here');
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
		$("#kt_select2_create_bank_shop").select2({
			placeholder: "输入商户名称或ID来搜寻",
			allowClear: true,
			ajax: {
				url: "ManualCode/SearchShop",
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
					console.log('here');
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


	// search
	var search = function () {
		$('#kt_payment_channel_filter').on('change', function () {
			datatable.search($(this).val(), 'paymentChannel');
		});
		$('#kt_payment_scheme_filter').on('change', function () {
			datatable.search($(this).val(), 'paymentScheme');
		});
		$('#kt_qrcode_status_filter').on('change', function () {
			datatable.search($(this).val(), 'qrCodeStatus');
		});
		$('#kt_pairing_status_filter').on('change', function () {
			datatable.search($(this).val(), 'pairingStatus');
		});
	}

	// selection
	var selection = function () {
		// init form controls
		$('#kt_payment_channel_filter, #kt_payment_scheme_filter, #kt_qrcode_status_filter, #kt_pairing_status_filter').selectpicker();

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



	// selected records enable
	var selectedEnable = function () {
		$('#kt_subheader_group_actions_enable_all').on('click', function () {
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

					text: "确认启用 " + ids.length + " 个二维码吗 ?",
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
								url: "ManualCode/Enable",
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
							text: '二维码已启用',
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

	// selected records disable
	var selectedDisable = function () {
		$('#kt_subheader_group_actions_disable_all').on('click', function () {
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

					text: "确认要停用 " + ids.length + " 个二维码吗 ?",
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
								url: "ManualCode/Disable",
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
							text: '二维码已全数停用',
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


	// selected records pairing
	var selectedPairing = function () {
		$('#kt_subheader_group_actions_pairing_all').on('click', function () {
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

					text: "确认" + ids.length + " 个二维码开始抢卖吗 ?",
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
								url: "ManualCode/StartPairing",
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
							text: '二维码抢卖中',
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

	// selected records stop pairing
	var selectedStop = function () {
		$('#kt_subheader_group_actions_stop_all').on('click', function () {
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

					text: "确认要停止 " + ids.length + " 个二维码抢卖吗 ?",
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
								url: "ManualCode/StopPairing",
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
							text: '二维码已全数停用',
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


	var initAvatar = function () {
		avatar = new KTAvatar('kt_create_barcode_add_qrcode');
	}

	return {
		// public functions
		init: function () {
			createBarcodeFormEl = $('#form_create_barcode');
			createMerchantCodeFormEl = $('#form_create_merchant');
			createTransactionCodeFormEl = $('#form_create_transaction');
			createBankCodeFormEl = $('#form_create_bank');

			updateBarcodeFormEl = $('#form_update_barcode_qrcode_data');
			updateMerchantCodeFormEl = $('#form_update_merchant_qrcode_data');
			updateTransactionCodeFormEl = $('#form_update_transaction_qrcode_data');
			updateBankCodeFormEl = $('#form_update_bank_qrcode_data');

			updateQrCodeSettingsFormEl = $('#form_update_qrcode_settings');
			updateQrCodeQuotaFormEl = $('#form_update_qrcode_quota');

			init();
			initSelectTrader();
			initSelectShop();

			modalCreateBarcode();
			modalCreateMerchantCode();
			modalCreateTransactionCode();
			modalCreateBankCode();


			search();
			selection();
			updateTotal();

			selectedEnable();
			selectedDisable();
			selectedPairing();
			selectedStop();

			initAvatar();

		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});
