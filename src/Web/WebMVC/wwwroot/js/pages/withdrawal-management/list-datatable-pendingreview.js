"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;

	const aspController = 'Withdrawal';
	const aspActionDelete = 'Delete';
	const aspActionUpdate = 'Update';

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
					noRecords: '查无提现记录',
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
				field: 'WithdrawalId',
				title: '#',
				sortable: false,
				width: 50,
				selector: {
					class: 'kt-checkbox--solid'
				},
				textAlign: 'center',
				template: function (row) {
					return row.WithdrawalId;
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
						},
						'Shop': {
							'title': '商户',
							'class': ' btn-label-success'
						},
						'ShopAgent': {
							'title': '商户代理',
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
				width: 100,
				textAlign: 'center',
				template: function (row) {
					return row.WithdrawalBankOption.BankName;
				}
			}, {
				field: 'AccountName',
				title: '银行户名',
				textAlign: 'center',
				template: function (row) {
					return row.AccountName;
				}
			}, {
				field: 'AccountNumber',
				title: '银行账号',
				textAlign: 'center',
				template: function (row) {
					return row.AccountNumber;
				}
			}, {
				field: 'TotalAmount',
				title: '提现总金额',
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
				field: 'TransactionAmount',
				title: '实际提领',
				sortable: false,
				textAlign: 'center',
				template: function (row) {
					var transactionAmount = row.TotalAmount - row.CommissionAmount;
					return transactionAmount.toFixed(3);
				}
			}, {
				field: 'ApprovedBy',
				title: '审核申请',
				width: 160,
				textAlign: 'center',
				template: function (row) {
					var output;

					if (row.ApprovedByAdminId) {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details kt-container">\
									<span class="kt-user-card-v2__name">' + row.ApprovedByAdminName + '</span>\
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
				field: "WithdrawalStatus",
				title: "状态",
				textAlign: 'center',
				width: 120,
				// callback function support for column rendering
				template: function (row) {
					var status = {
						'Submitted': {
							'title': '等待打款',
							'class': ' btn-label-danger'
						},
						'Approved': {
							'title': '等待查收',
							'class': ' btn-label-brand'
						},
						'Success': {
							'title': '成功',
							'class': ' btn-label-success'
						},
						'AwaitingCancellation': {
							'title': '等待取消',
							'class': ' btn-label-danger'
						},
						'Canceled': {
							'title': '已取消',
							'class': ' btn-label-danger'
						}
					};
					return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[row.WithdrawalStatus].class + '">' + status[row.WithdrawalStatus].title + '</span>';
				}
			}, {
				field: 'DateCreated',
				title: '申请时间',
				textAlign: 'center',
				template: function (row) {
					return convertUTCDateToLocalDate(new Date(row.DateCreated)).toLocaleString();
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
					var userAction = {
						'Submitted': {
							0: {
								'title': '取消提现',
								'datatoggle': 'show-modal-cancel',
								'class': ' btn-danger'
							}
						},
						'Approved': {
							0: {
								'title': '确认收款',
								'datatoggle': 'show-modal-confirm-payment',
								'class': ' btn-success'
							},
							1: {
								'title': '取消提现',
								'datatoggle': 'show-modal-cancel',
								'class': ' btn-danger'
							}
						},
						'Success': {
						},
						'AwaitingCancellation': {
						},
						'Canceled': {
						}
					};

					var managerAction = {
						'Submitted': {
							0: {
								'title': '通知收款',
								'datatoggle': 'show-modal-approve',
								'class': ' btn-success'
							}
						},
						'Approved': {
							0: {
								'title': '強制成功',
								'datatoggle': 'show-modal-force-success',
								'class': ' btn-success'
							}
						},
						'Success': {
						},
						'AwaitingCancellation': {
							0: {
								'title': '強制成功',
								'datatoggle': 'show-modal-force-success',
								'class': ' btn-success'
							},
							1: {
								'title': '确认取消',
								'datatoggle': 'show-modal-approve-cancellation',
								'class': ' btn-danger'
							}

						},
						'Canceled': {
						}
					};

					var contextUserType = $('#user_role').val();
					if (contextUserType == 'Manager') {
						var objectLength = Object.keys(managerAction[row.WithdrawalStatus]).length;

						if (objectLength > 0) {
							var output = '';
							for (var i = 0; i < objectLength; i++) {
								output = output + '<button data-withdrawal-id="' + row.WithdrawalId +
									'" data-toggle="' + managerAction[row.WithdrawalStatus][i].datatoggle +
									'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + managerAction[row.WithdrawalStatus][i].class + '">' +
									managerAction[row.WithdrawalStatus][i].title + '</button>';
							}
							return output;
						}
						else {
							return '-';
						}
					}
					else {
						var objectLength = Object.keys(userAction[row.WithdrawalStatus]).length;
						if (objectLength > 0) {
							var output = '';

							for (var i = 0; i < objectLength; i++) {

								output = output + '<button data-withdrawal-id="' + row.WithdrawalId +
									'" data-toggle="' + userAction[row.WithdrawalStatus][i].datatoggle +
									'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + userAction[row.WithdrawalStatus][i].class + '">' +
									userAction[row.WithdrawalStatus][i].title + '</button>'
							}
							/*return '<button data-withdrawal-id="' + row.WithdrawalId +
								'" data-toggle="' + userAction[row.WithdrawalStatus].datatoggle +
								'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + userAction[row.WithdrawalStatus].class + '">' +
								userAction[row.WithdrawalStatus].title + '</button>';*/
							return output;
						}
						else {
							return '-';
						}
					}
				}
			}]
		});

		datatable.on('click', '[data-record-id]', function () {
			modalUserDetail($(this).data('record-id'), $(this).data('user-type'));
			$('#kt_modal_quick_view_user_info').modal('show');
		});

		datatable.on('click', '[data-withdrawal-id]', function () {
			var action = $(this).data('toggle');
			if (action.length) {
				var withdrawalId = parseInt($(this).data('withdrawal-id'));
				if (action === 'show-modal-cancel') {
					modalCancel(withdrawalId);
				}
				else if (action === 'show-modal-confirm-payment') {
					modalConfirmPayment(withdrawalId);
				}
				else if (action === 'show-modal-approve') {
					modalApprove(withdrawalId);
				}
				else if (action === 'show-modal-approve-cancellation') {
					modalApproveCancellation(withdrawalId);
				}
				else if (action === 'show-modal-force-success') {
					modalForceSuccess(withdrawalId);
				}
			}
		});

	}



	// user details
	var modalUserDetail = function (id, userBaseRole) {
		var el = $('#modal_sub_datatable_ajax_source');

		var detailUrl = "";
		var dataObject = null;

		if (userBaseRole === 'Trader') {
			detailUrl = "../Trader/Detail";
			dataObject = { traderId: id };
		}
		else if (userBaseRole === 'TraderAgent') {
			detailUrl = "../TraderAgent/Detail";
			dataObject = { traderAgentId: id };
		}
		else if (userBaseRole === 'Shop') {
			detailUrl = "../Shop/Detail";
			dataObject = { shopId: id };
		}
		else if (userBaseRole === 'ShopAgent') {
			detailUrl = "S../hopAgent/Detail";
			dataObject = { shopAgentId: id };
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
				$('#quick_view_user_info_raterebatealipay').html('千分之' + data.RebateCommission.RateRebateAlipayInThousandth);
				$('#quick_view_user_info_raterebatewechat').html('千分之' + data.RebateCommission.RateRebateWechatInThousandth);
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

	// approve
	var modalApprove = function (withdrawalId) {
		if (withdrawalId) {

			//get ids and put in to a list.
			var idList = new Array();
			idList[0] = parseInt(withdrawalId);


			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "确认已打款至 " + idList.length + " 个用户的银行账户吗 ?",
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

	// modal approve cancellation
	var modalApproveCancellation = function (withdrawalId) {
		if (withdrawalId) {

			//get ids and put in to a list.
			var idList = new Array();
			idList[0] = parseInt(withdrawalId);

			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "确认要取消此笔提现吗 ?",
				type: "warning",

				confirmButtonText: "确认",
				confirmButtonClass: "btn btn-sm btn-bold btn-danger",

				showCancelButton: true,
				cancelButtonText: "取消",
				cancelButtonClass: "btn btn-sm btn-bold btn-brand",

				showLoaderOnConfirm: true,

				preConfirm: () => {
					return new Promise(function (resolve, reject) {
						$.ajax({
							url: "ApproveCancellation",
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
						title: '更新成功!',
						text: '提现已成功取消',
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

	// modal cancel
	var modalCancel = function (withdrawalId) {
		if (withdrawalId) {

			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "确认要申请取消提现吗 ?",
				type: "warning",

				confirmButtonText: "确认",
				confirmButtonClass: "btn btn-sm btn-bold btn-danger",

				showCancelButton: true,
				cancelButtonText: "取消",
				cancelButtonClass: "btn btn-sm btn-bold btn-brand",

				showLoaderOnConfirm: true,

				preConfirm: () => {
					return new Promise(function (resolve, reject) {
						$.ajax({
							url: "Cancel",
							type: "POST",
							contentType: "application/json",
							data: JSON.stringify(withdrawalId),
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
						title: '申请成功!',
						text: '可至待处理列表查看申请状态',
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

	// modal force success
	var modalForceSuccess = function (withdrawalId) {
		if (withdrawalId) {

			//get ids and put in to a list.
			var idList = new Array();
			idList[0] = parseInt(withdrawalId);

			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "确认強制更新訂單為成功吗 ?",
				type: "warning",

				confirmButtonText: "确认",
				confirmButtonClass: "btn btn-sm btn-bold btn-danger",

				showCancelButton: true,
				cancelButtonText: "取消",
				cancelButtonClass: "btn btn-sm btn-bold btn-brand",

				showLoaderOnConfirm: true,

				preConfirm: () => {
					return new Promise(function (resolve, reject) {
						$.ajax({
							url: "ForceSuccess",
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
						title: '更新成功!',
						text: '可至提现列表查看记录',
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

	// modal confirm payment
	var modalConfirmPayment = function (withdrawalId) {
		if (withdrawalId) {

			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "确认已收到款项吗 ?",
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
							url: "ComfirmPayment",
							type: "POST",
							contentType: "application/json",
							data: JSON.stringify(withdrawalId),
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
						title: '提现成功!',
						text: '可至提现列表查看记录',
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
			userType: $('#kt_user_type_filter').val(),
			status: $('#kt_status_filter').val(),
		};

		datatable.setDataSourceParam('query', queryObejct);
		datatable.load();
		datatable.search();
	};


	// search
	var search = function () {
		$('#kt_user_type_filter').on('change', function () {
			//Set query string.
			/*var queryObejct = {
				withdrawalEntryUserTypeFilter: id,
				generalSearch: ''
			};*/

			datatable.search($(this).val(), 'userType');
		});
		$('#kt_status_filter').on('change', function () {
			//Set query string.
			/*var queryObejct = {
				withdrawalEntryStatusFilter: id,
				generalSearch: ''
			};*/

			datatable.search($(this).val(), 'status');
		});
	}

	// selection
	var selection = function () {
		// init form controls
		$('#kt_user_type_filter, #kt_status_filter').selectpicker();

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

					text: "确认已打款至 " + ids.length + " 个用户的银行账户吗 ?",
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
		});
	}

	// selected records force success
	var selectedForceSuccess = function () {
		$('#kt_subheader_group_actions_force_success_all').on('click', function () {
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

					text: "确认要强制更新选取的 " + ids.length + " 笔提现状态为成功提现吗 ?",
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
								url: "ForceSuccess",
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
							text: '可至提现列表查看记录',
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
	var selectedApproveCancellation = function () {
		$('#kt_subheader_group_actions_approve_cancellation_all').on('click', function () {
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

					text: "确认要取消选取的 " + ids.length + " 笔提现吗 ?",
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
								url: "ApproveCancellation",
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
							title: '更新成功!',
							text: '提现已成功取消',
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
			daterangepickerInit();
			search();
			selection();
			selectedApprove();
			selectedForceSuccess();
			selectedApproveCancellation();
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});
