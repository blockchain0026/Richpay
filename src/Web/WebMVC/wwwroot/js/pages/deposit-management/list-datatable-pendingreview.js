"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;

	const aspController = 'Deposit';
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
				width: 20,
				selector: {
					class: 'kt-checkbox--solid'
				},
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
				width: 100,
				textAlign: 'center',
				template: function (row) {
					return row.DepositBankAccount.BankName;
				}
			}, {
				field: 'AccountName',
				title: '银行户名',
				textAlign: 'center',
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
				field: 'VerifiedBy',
				title: '审核申请',
				width: 160,
				textAlign: 'center',
				template: function (row) {
					var output;

					if (row.VerifiedByAdminName) {
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
				textAlign: 'center',
				width: 120,
				// callback function support for column rendering
				template: function (row) {
					var status = {
						'Submitted': {
							'title': '等待确认',
							'class': ' btn-label-danger'
						}
					};
					return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[row.DepositStatus].class + '">' + status[row.DepositStatus].title + '</span>';
				}
			}, {
				field: 'DateCreated',
				title: '申请时间',
				textAlign: 'center',
				template: function (row) {
					var dateCreated = convertUTCDateToLocalDate(new Date(row.DateCreated)).toLocaleString();
					return dateCreated;
				}
			}, {
				field: "Actions",
				width: 100,
				title: "操作",
				textAlign: 'center',
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {
					var userAction = {
						'Submitted': {
							0: {
								'title': '取消申请',
								'datatoggle': 'show-modal-cancel',
								'class': ' btn-danger'
							}
						}
					};

					var managerAction = {
						'Submitted': {
							0: {
								'title': '确认收款',
								'datatoggle': 'show-modal-verify',
								'class': ' btn-success'
							}
						}
					};

					var contextUserType = $('#user_role').val();
					if (contextUserType == 'Manager') {
						var objectLength = Object.keys(managerAction[row.DepositStatus]).length;

						if (objectLength > 0) {
							var output = '';
							for (var i = 0; i < objectLength; i++) {
								output = output + '<button data-deposit-id="' + row.DepositId +
									'" data-toggle="' + managerAction[row.DepositStatus][i].datatoggle +
									'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + managerAction[row.DepositStatus][i].class + '">' +
									managerAction[row.DepositStatus][i].title + '</button>';
							}
							return output;
						}
						else {
							return '-';
						}
					}
					else {
						var objectLength = Object.keys(userAction[row.DepositStatus]).length;
						if (objectLength > 0) {
							var output = '';

							for (var i = 0; i < objectLength; i++) {

								output = output + '<button data-deposit-id="' + row.DepositId +
									'" data-toggle="' + userAction[row.DepositStatus][i].datatoggle +
									'" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air ' + userAction[row.DepositStatus][i].class + '">' +
									userAction[row.DepositStatus][i].title + '</button>'
							}
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

		datatable.on('click', '[data-deposit-id]', function () {
			var action = $(this).data('toggle');
			if (action.length) {
				var depositId = parseInt($(this).data('deposit-id'));
				if (action === 'show-modal-cancel') {
					modalCancel(depositId);
				}
				else if (action === 'show-modal-verify') {
					modalVerify(depositId);
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

	// verify
	var modalVerify = function (depositId) {
		if (depositId) {

			//get ids and put in to a list.
			var idList = new Array();
			idList[0] = parseInt(depositId);


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
							url: "Verify",
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

	// modal cancel
	var modalCancel = function (depositId) {
		if (depositId) {

			// learn more: https://sweetalert2.github.io/
			swal.fire({
				buttonsStyling: false,

				text: "取消后此笔入金将直接列为失败，确认取消入金吗?",
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
							data: JSON.stringify(depositId),
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
						text: '可至待入金列表查看记录',
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
						title: '已取消操作',
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

	// selected records approve
	var selectedVerify = function () {
		$('#kt_subheader_group_actions_verify_all').on('click', function () {
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

					text: "确认已收到 " + ids.length + " 个用户的款项了吗 ?",
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
								url: "Verify",
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
							text: '用户余额已更新',
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
			selectedVerify();
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});
