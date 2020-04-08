"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;

	var userRole = $('#user_role').val();


	const accountStatus = {
		1: { 'value': true, 'title': '启用' },
		2: { 'value': false, 'title': '停用' }
	};
	const aspController = 'Manager';
	const aspActionDelete = 'Delete';
	const aspActionUpdate = 'Update';

	// init
	var init = function () {
		// init the datatables. Learn more: https://keenthemes.com/metronic/?page=docs&section=datatable

		var selectorVisibility = true;
		if (userRole.length) {
			if (userRole == 'TraderAgent') {
				selectorVisibility = false;
			}
		}


		datatable = $('#kt_apps_pendingreview_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						//url: 'https://keenthemes.com/metronic/tools/preview/api/datatables/demos/default.php',
						url: 'PendingReview'
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
					noRecords: '查无代理',
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
				field: 'TraderAgentId',
				title: '',
				sortable: false,
				width: 20,
				textAlign: 'center',
				selector: {
					class: 'kt-checkbox--solid'
				},
				template: function (row) {
					return row.TraderAgentId;
				}
			}, {
				field: "UserName",
				title: "用户名",
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
					var user_img = '100_' + number + '.jpg';

					var pos = KTUtil.getRandomInt(0, 5);
					/*var position = [
						'Developer',
						'Designer',
						'CEO',
						'Manager',
						'Architect',
						'Sales'
					];*/

					var output = '';
					if (number > 5) {
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

						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__pic">\
									<div class="kt-badge kt-badge--xl kt-badge--' + state + '">' + data.FullName.substring(0, 1) + '</div>\
								</div>\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.TraderAgentId + '" class="kt-user-card-v2__name">' + data.FullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.Username + '</span>\
								</div>\
							</div>';
					} else {
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

						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__pic">\
									<div class="kt-badge kt-badge--xl kt-badge--' + state + '">' + data.FullName.substring(0, 1) + '</div>\
								</div>\
								<div class="kt-user-card-v2__details">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.TraderAgentId + '" class="kt-user-card-v2__name">' + data.FullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.Username + '</span>\
								</div>\
							</div>';
					}

					return output;
				}
			}, {
				field: 'NickName',
				title: '昵称',
				sortable: true,
				template: function (row) {
					return row.Nickname;
				}
			}, {
				field: 'UplineUserName',
				title: '上级代理',
				sortable: true,
				textAlign: 'center',
				template: function (row) {
					var uplineFullName = row.UplineFullName ? row.UplineFullName : '';
					var uplineUserName = row.UplineUserName ? row.UplineUserName : '-';
					var uplineUserId = row.UplineUserId ? row.UplineUserId : '';

					var output;

					if (row.UplineUserId) {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details kt-container">\
									<a href="#kt_modal_quick_view_user_info " data-record-id="' + uplineUserId + '" class="kt-user-card-v2__name">' + uplineFullName + '</a>\
								</div>\
							</div>';
					}
					else {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details kt-container">\
									<span class="kt-user-card-v2__desc">' + uplineUserName + '</span>\
								</div>\
							</div>';
					}

					return output;
				}
			}, {
				field: 'AmountAvailable',
				title: '可用余额',
				sortable: true,
				textAlign: 'center',
				template: function (row) {
					return row.Balance.AmountAvailable.toFixed(3);
				}
			}, {
				field: 'AmountFrozen',
				title: '冻结余额',
				sortable: true,
				textAlign: 'center',
				template: function (row) {
					return row.Balance.AmountFrozen.toFixed(3);
				}
			}, {
				field: 'RateAlipayInThousandth',
				title: '支付宝费率',
				sortable: true,
				textAlign: 'center',
				template: function (row) {
					var commission = row.TradingCommission.RateAlipayInThousandth / 1000;
					return commission.toFixed(3);
				}
			}, {
				field: 'RateWechatInThousandth',
				title: '微信费率',
				sortable: true,
				textAlign: 'center',
				template: function (row) {
					var commission = row.TradingCommission.RateWechatInThousandth / 1000;
					return commission.toFixed(3);
				}
			}, {
				field: 'LastLogin',
				title: '上次登入',
				sortable: true,
				textAlign: 'center',
				template: function (row) {
					var iplast = row.LastLoginIP ? row.LastLoginIP : '-';
					var datelast = row.DateLastLoggedIn ? row.DateLastLoggedIn : '';
					var output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details kt-container">\
						            <span class="kt-user-card-v2__desc">' + iplast + '<br>'
						+ datelast + '</span>\
								</div>\
							</div>';
					return output;
				}
			}, {
				field: 'HasGrantRight',
				title: '添加代理',
				sortable: true,
				textAlign: 'center',
				template: function (row) {
					var status = {
						1: { 'title': '允许', 'class': ' btn-label-success' },
						2: { 'title': '禁止', 'class': ' btn-label-danger' }
					};
					if (row.HasGrantRight) {
						return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[1].class + '">' + status[1].title + '</span>';
					}
					else {
						return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[2].class + '">' + status[2].title + '</span>';
					}
				}
			}, {
				field: "IsReviewed",
				title: "审核状态",
				textAlign: 'center',
				sortable: true,
				// callback function support for column rendering
				template: function (row) {
					var status = {
						1: {
							'title': '已审核',
							'class': ' btn-label-success'
						},
						2: {
							'title': '未审核',
							'class': ' btn-label-danger'
						}
					};
					if (row.IsReviewed) {
						return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[1].class + '">' + status[1].title + '</span>';
					}
					else {
						return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[2].class + '">' + status[2].title + '</span>';
					}
				}
			},]
		});
		//Set selector visibility depend on user role.
		datatable.columns('TraderAgentId').visible(selectorVisibility);

		datatable.on('click', '[data-record-id]', function () {
			modalSubRemoteDatatable($(this).data('record-id'));
			$('#kt_modal_quick_view_user_info').modal('show');
		});
	}

	var modalSubRemoteDatatable = function (id) {
		var el = $('#modal_sub_datatable_ajax_source');
		$.ajax({
			url: "DownlineDetail",
			type: "Get",
			contentType: "application/json",
			data: { traderAgentId: id },
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
				$('#quick_view_user_info_id').html(data.TraderAgentId);
				$('#quick_view_user_info_username').html(data.Username);
				$('#quick_view_user_info_password').html(data.Password);
				var uplineFullName = data.UplineFullName ? data.UplineFullName : '-';
				var uplineUsername = data.UplineUserName ? data.UplineUserName : '';
				$('#quick_view_user_info_upline').html(
					uplineFullName + '<br>' + uplineUsername);

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
					data.Balance.WithdrawalLimit.EachAmountLowerLimit + ' ¥' + ' ~ ' + data.Balance.WithdrawalLimit.EachAmountUpperLimit + ' ¥');
				$('#quick_view_user_info_dailyamountlimit').html(data.Balance.WithdrawalLimit.DailyAmountLimit + ' ¥');
				$('#quick_view_user_info_dailyfrequencylimit').html(data.Balance.WithdrawalLimit.DailyFrequencyLimit);
				$('#quick_view_user_info_withdrawalcommission').html('千分之' + data.Balance.WithdrawalCommissionRateInThousandth);
				$('#quick_view_user_info_depositcommission').html('千分之' + data.Balance.DepositCommissionRateInThousandth);
			},
			error: function (response) {
				console.log(response);
				reject(response.responseText)
			}
		})

	};

	// search
	var search = function () {
		$('#kt_form_status').on('change', function () {
			datatable.search($(this).val().toLowerCase(), 'Status');
		});
	}

	// selection
	var selection = function () {
		// init form controls
		//$('#kt_form_status, #kt_form_type').selectpicker();

		// event handler on check and uncheck on records
		datatable.on('kt-datatable--on-check kt-datatable--on-uncheck kt-datatable--on-layout-updated', function (e) {
			var checkedNodes = datatable.rows('.kt-datatable__row--active').nodes(); // get selected records
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

	// fetch selected records
	var selectedFetch = function () {
		// event handler on selected records fetch modal launch
		$('#kt_datatable_records_fetch_modal').on('show.bs.modal', function (e) {
			// show loading dialog
			var loading = new KTDialog({ 'type': 'loader', 'placement': 'top center', 'message': 'Loading ...' });
			loading.show();

			setTimeout(function () {
				loading.hide();
			}, 1000);

			// fetch selected IDs
			var ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
				return $(chk).val();
			});

			// populate selected IDs
			var c = document.createDocumentFragment();

			for (var i = 0; i < ids.length; i++) {
				var li = document.createElement('li');
				li.setAttribute('data-id', ids[i]);
				li.innerHTML = 'Selected record ID: ' + ids[i];
				c.appendChild(li);
			}

			$(e.target).find('#kt_apps_user_fetch_records_selected').append(c);
		}).on('hide.bs.modal', function (e) {
			$(e.target).find('#kt_apps_user_fetch_records_selected').empty();
		});
	};

	// selected records status update
	var selectedStatusUpdate = function () {
		$('#kt_subheader_group_actions_status_change').on('click', "[data-toggle='status-change']", function () {
			var status = $(this).find(".kt-nav__link-text").html();
			var statusText = $(this).find(".kt-badge").text();

			var isEnabled;

			//Set account status value
			if (statusText === accountStatus[1].title) {
				isEnabled = accountStatus[1].value;
			}
			else if (statusText === accountStatus[2].title) {
				isEnabled = accountStatus[2].value;
			}

			// fetch selected IDs
			var ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
				return $(chk).val();
			});



			if (ids.length > 0) {
				// learn more: https://sweetalert2.github.io/

				//create post data.
				var accounts = new Array();

				//get ids and put in to a list.
				var idList = new Array();
				for (var i = 0; i < ids.length; i++) {
					idList[i] = ids[i];
				}

				//build post data.
				idList.forEach(function (managerId) {
					let account = {
						UserId: managerId, IsEnabled: isEnabled
					};

					accounts.push(account);
				});


				swal.fire({
					buttonsStyling: false,

					html: "确定要修改选取的 " + ids.length + " 個管理員帳戶狀態為 " + status + " 吗?",
					type: "info",

					confirmButtonText: "确认修改",
					confirmButtonClass: "btn btn-sm btn-bold btn-brand",

					showCancelButton: true,
					cancelButtonText: "取消",
					cancelButtonClass: "btn btn-sm btn-bold btn-default",

					showLoaderOnConfirm: true,
					/*preConfirm: (login) => {
						return fetch('Manager/UpdateAccountStatus', {
							method: 'POST',
							body: JSON.stringify(accounts),
							headers: new Headers({
								'Content-Type': 'application/json'
							})
						})
							.then(response => {
								if (!response.ok) {
									throw new Error(response.statusText)
								}

								console.log(response.text);
								return response.json()
							})
							.catch(error => {
								console.log(error);
								Swal.showValidationMessage(
									'Request failed: ' + error
								)
							})
					},*/
					preConfirm: () => {
						return new Promise(function (resolve, reject) {
							$.ajax({
								url: "Manager/UpdateAccountStatus",
								type: "POST",
								contentType: "application/json",
								data: JSON.stringify(accounts),
								success: function (response) {
									console.log("Data submitted.");
									console.log(response);
									resolve(response)
								},
								error: function (response) {
									console.log(response);
									reject(response.responseText)
								}
							})
						}).then(response => {
							if (!response.success) {
								throw new Error(response.responseText);
							}
							return response;
						}).catch(error => {
							Swal.showValidationMessage(
								'Request failed: ' + error
							)
						})
					},
					allowOutsideClick: () => !Swal.isLoading()
				}).then(function (result) {
					if (result.value) {
						swal.fire({
							title: '修改成功!',
							text: '你选取的管理员帐户状态已经修改!',
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
							title: '已取消修改',
							text: '你选取的管理员帐户状态尚未更改!',
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

	// selected records delete
	var selectedDelete = function () {
		$('#kt_subheader_group_actions_delete_all').on('click', function () {
			// fetch selected IDs
			var ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
				return $(chk).val();
			});

			if (ids.length > 0) {

				//create post data.
				var accounts = new Array();

				//get ids and put in to a list.
				var idList = new Array();
				for (var i = 0; i < ids.length; i++) {
					idList[i] = ids[i];
				}

				//build post data.
				idList.forEach(function (traderAgentId) {
					let account = {
						UserId: traderAgentId, IsReviewed: false
					};
					accounts.push(account);
				});

				// learn more: https://sweetalert2.github.io/
				swal.fire({
					buttonsStyling: false,

					text: "审核失败的帐户将会全数删除，" +
						"确定要拒绝选取的 " + ids.length + " 个代理帐户吗 ?",
					type: "danger",

					confirmButtonText: "确认删除",
					confirmButtonClass: "btn btn-sm btn-bold btn-danger",

					showCancelButton: true,
					cancelButtonText: "取消",
					cancelButtonClass: "btn btn-sm btn-bold btn-brand",

					showLoaderOnConfirm: true,

					preConfirm: () => {
						return new Promise(function (resolve, reject) {
							$.ajax({
								url: "Review",
								type: "POST",
								contentType: "application/json",
								data: JSON.stringify(accounts),
								success: function (response) {
									console.log("Data submitted.");
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
							console.log(response);
							return response;
						}).catch(error => {
							console.log(error);
							Swal.showValidationMessage(
								'Request failed: ' + error
							)
						})
					},
					allowOutsideClick: () => !Swal.isLoading()
				}).then(function (result) {
					if (result.value) {
						swal.fire({
							title: '删除成功!',
							text: '你选取的代理帐户已删除!',
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
							text: '你选取的代理帐户尚在等待审核!',
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

	// selected records delete
	var selectedReview = function () {
		$('#kt_subheader_group_actions_review_all').on('click', function () {
			// fetch selected IDs
			var ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
				return $(chk).val();
			});

			if (ids.length > 0) {

				//create post data.
				var accounts = new Array();

				//get ids and put in to a list.
				var idList = new Array();
				for (var i = 0; i < ids.length; i++) {
					idList[i] = ids[i];
				}
				console.log(idList);
				//build post data.
				idList.forEach(function (traderAgentId) {
					console.log(traderAgentId);
					let account = {
						UserId: traderAgentId, IsReviewed: true
					};
					accounts.push(account);
				});

				// learn more: https://sweetalert2.github.io/
				swal.fire({
					buttonsStyling: false,

					text: "确定要让选取的 " + ids.length + " 个代理帐户通过审核吗 ?",
					type: "danger",

					confirmButtonText: "确认审核",
					confirmButtonClass: "btn btn-sm btn-bold btn-danger",

					showCancelButton: true,
					cancelButtonText: "取消",
					cancelButtonClass: "btn btn-sm btn-bold btn-brand",

					showLoaderOnConfirm: true,

					preConfirm: () => {
						return new Promise(function (resolve, reject) {
							$.ajax({
								url: "Review",
								type: "POST",
								contentType: "application/json",
								data: JSON.stringify(accounts),
								success: function (response) {
									console.log("Data submitted.");
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
							console.log(response);
							return response;
						}).catch(error => {
							console.log(error);
							Swal.showValidationMessage(
								'Request failed: ' + error
							)
						})
					},
					allowOutsideClick: () => !Swal.isLoading()
				}).then(function (result) {
					if (result.value) {
						swal.fire({
							title: '审核通过!',
							text: '你选取的代理帐户已通过审核!',
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
							text: '你选取的代理帐户尚在等待审核!',
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
			selectedFetch();
			selectedStatusUpdate();
			selectedDelete();
			selectedReview();
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});
