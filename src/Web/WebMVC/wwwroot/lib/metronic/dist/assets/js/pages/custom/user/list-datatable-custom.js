"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;
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
		datatable = $('#kt_apps_user_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						//url: 'https://keenthemes.com/metronic/tools/preview/api/datatables/demos/default.php',
						url: 'Manager/Search'
					},
				},
				pageSize: 10, // display 20 records per page
				serverPaging: true,
				serverFiltering: true,
				serverSorting: true,
			},

			// layout definition
			layout: {
				scroll: false, // enable/disable datatable scroll both horizontal and vertical when needed.
				footer: false, // display/hide footer
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
				field: 'ManagerID',
				title: '#',
				sortable: false,
				width: 20,
				selector: {
					class: 'kt-checkbox--solid'
				},
				textAlign: 'center',
				template: function (row) {
					return row.ManagerId;
				}
			}, {
				field: "UserName",
				title: "用户名",
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
									<a href="#" class="kt-user-card-v2__name">' + data.FullName + '</a>\
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
									<a href="#" class="kt-user-card-v2__name">' + data.FullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.Username + '</span>\
								</div>\
							</div>';
					}

					return output;
				}
			}, {
				field: 'RoleName',
				title: '权限',
				template: function (row) {
					var status = {
						1: { 'title': '超级管理员', 'state': 'primary' },
						2: { 'title': '普通管理员', 'state': 'warning' }
					};
					/*if (row.RoleName === "Admin") {
						return '<span class="kt-badge kt-badge--' + status[1].state + ' kt-badge--dot"></span>&nbsp;<span class="kt-font-bold kt-font-' + status[1].state + '">' +
							status[1].title + '</span>';
					}
					else {
						return '<span class="kt-badge kt-badge--' + status[2].state + ' kt-badge--dot"></span>&nbsp;<span class="kt-font-bold kt-font-' + status[2].state + '">' +
							row.RoleName + '</span>';
					}*/

					return '<span class="kt-badge kt-badge--' + status[1].state + ' kt-badge--dot"></span>&nbsp;<span class="kt-font-bold kt-font-' + status[1].state + '">' +
						row.RoleName + '</span>';
				}
			}
				, {
				field: 'NickName',
				title: '昵称',
				template: function (row) {
					return row.Nickname;
				}
			}, {
				field: 'LastLogin',
				title: '上次登入',
				textAlign: 'center',
				template: function (row) {
					var output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
						            <span class="kt-user-card-v2__desc">' + (row.LastLoginIP ? row.LastLoginIP : '-') + '</span>\
									<span class="kt-user-card-v2__desc">' + (row.DateLastLoggedIn ? row.DateLastLoggedIn : '') + '</span>\
								</div>\
							</div>';
					return output;
				}
			}, {
				field: "Status",
				title: "帐户状态",
				width: 100,
				// callback function support for column rendering
				template: function (row) {
					var status = {
						1: {
							'title': '启用',
							'class': ' btn-label-success'
						},
						2: {
							'title': '停用',
							'class': ' btn-label-danger'
						}
					};
					if (row.IsEnabled) {
						return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[1].class + '">' + status[1].title + '</span>';
					}
					else {
						return '<span class="btn btn-bold btn-sm btn-font-sm ' + status[2].class + '">' + status[2].title + '</span>';
					}
				}
			}, /*{
				width: 110,
				field: 'Type',
				title: 'Type',
				autoHide: false,
				// callback function support for column rendering
				template: function (row) {
					var status = {
						1: { 'title': 'Online', 'state': 'danger' },
						2: { 'title': 'Retail', 'state': 'primary' },
						3: { 'title': 'Direct', 'state': 'success' },
					};
					return '<span class="kt-badge kt-badge--' + status[row.Type].state + ' kt-badge--dot"></span>&nbsp;<span class="kt-font-bold kt-font-' + status[row.Type].state + '">' +
						status[row.Type].title + '</span>';
				},
			},*/ {
				field: "Actions",
				width: 80,
				title: "动作",
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {
					var updateLink = '/' + aspController + '/' + aspActionUpdate + '?managerId=' + row.ManagerId;
					return '\
							<div class="dropdown">\
								<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
									<i class="flaticon-more-1"></i>\
								</a>\
								<div class="dropdown-menu dropdown-menu-right">\
									<ul class="kt-nav">\
										<li class="kt-nav__item">\
											<a href="'+ updateLink + '" class="kt-nav__link">\
												<i class="kt-nav__link-icon flaticon2-contract"></i>\
												<span class="kt-nav__link-text">修改</span>\
											</a>\
										</li>\
									</ul>\
								</div>\
							</div>\
						';
				},
			}]
		});
		datatable.reload();
	}

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

				//get ids and put in to a list.
				var idList = new Array();
				for (var i = 0; i < ids.length; i++) {
					idList[i] = ids[i];
				}

				// learn more: https://sweetalert2.github.io/
				swal.fire({
					buttonsStyling: false,

					text: "确定要删除选取的 " + ids.length + " 个管理员帐户吗 ?",
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
								url: "Manager/Delete",
								type: "POST",
								contentType: "application/json",
								data: JSON.stringify(idList),
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
							text: '你选取的管理员帐户已删除!',
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
							title: 'Cancelled',
							text: '你选取的管理员帐户尚未删除!',
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
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});
