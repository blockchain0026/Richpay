"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;
	var downline_datatable;

	var bankbook_record_datatable;
	var running_account_record_datatable;
	var frozen_balance_datatable;

	var currentTableIndex = 1;
	var changeBalanceFormValidator;
	var changeBalanceFormEl;

	const accountStatus = {
		1: { 'value': true, 'title': '启用' },
		2: { 'value': false, 'title': '停用' }
	};
	const aspController = 'ShopAgent';
	const aspActionDelete = 'Delete';
	const aspActionUpdate = 'Update';

	// init
	var init = function () {
		// init the datatables. Learn more: https://keenthemes.com/metronic/?page=docs&section=datatable
		datatable = $('#kt_apps_shopagent_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'ShopAgent/Search'
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
					noRecords: '查无商户代理',
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
				field: 'ShopAgentID',
				title: '#',
				sortable: false,
				width: 20,
				selector: {
					class: 'kt-checkbox--solid'
				},
				textAlign: 'center',
				template: function (row) {
					return row.ShopAgentId;
				}
			}, {
				field: "Username",
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
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.ShopAgentId + '" class="kt-user-card-v2__name">' + data.FullName + '</a>\
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
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + data.ShopAgentId + '" class="kt-user-card-v2__name">' + data.FullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.Username + '</span>\
								</div>\
							</div>';
					}

					return output;
				}
			}, {
				field: 'Nickname',
				title: '昵称',
				template: function (row) {
					return row.Nickname;
				}
			}, {
				field: 'UplineUserName',
				title: '上级代理',
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
				textAlign: 'center',
				template: function (row) {
					return row.Balance.AmountAvailable.toFixed(3);
				}
			}, {
				field: 'AmountFrozen',
				title: '冻结余额',
				textAlign: 'center',
				template: function (row) {
					return row.Balance.AmountFrozen.toFixed(3);
				}
			}, {
				field: 'RateRebateAlipayInThousandth',
				title: '支付宝返佣',
				textAlign: 'center',
				template: function (row) {
					var commission = row.RebateCommission.RateRebateAlipayInThousandth / 1000;
					return commission.toFixed(3);
				}
			}, {
				field: 'RateRebateWechatInThousandth',
				title: '微信返佣',
				textAlign: 'center',
				template: function (row) {
					var commission = row.RebateCommission.RateRebateWechatInThousandth / 1000;
					return commission.toFixed(3);
				}
			}, {
				field: 'LastLogin',
				title: '上次登入',
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
				field: "Status",
				title: "帐户状态",
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
			}, {
				field: "Actions",
				width: 80,
				title: "动作",
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {
					var updateLink = '/' + aspController + '/' + aspActionUpdate + '?shopAgentId=' + row.ShopAgentId;
					return '\
							<div class="dropdown">\
								<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
									<i class="flaticon-more-1"></i>\
								</a>\
								<div id="row_action_dropdown_menu" class="dropdown-menu dropdown-menu-right">\
									<ul class="kt-nav">\
										<li class="kt-nav__item">\
											<a href="'+ updateLink + '" class="kt-nav__link">\
												<i class="kt-nav__link-icon flaticon2-contract"></i>\
												<span class="kt-nav__link-text">帐户设置</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#" data-user-id="' + row.ShopAgentId + '" class="kt-nav__link" data-toggle="show-modal_bankbook_records_list_datatable" data-target="#modal_bankbook_records_list_datatable">\
												<i class="kt-nav__link-icon fa fa-search-dollar"></i>\
												<span class="kt-nav__link-text">帐变纪录</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#" data-user-id="' + row.ShopAgentId + '" class="kt-nav__link" data-toggle="show-modal_running_account_records_datatable" data-target="#modal_running_account_records_datatable">\
												<i class="kt-nav__link-icon fa fa-money-bill-wave"></i>\
												<span class="kt-nav__link-text">流水纪录</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#modal__change_balance" data-user-id="' + row.ShopAgentId + '" data-current-balance="' + row.Balance.AmountAvailable.toFixed(3) + '" data-toggle="show-modal-change-balance" class="kt-nav__link" >\
												<i class="kt-nav__link-icon fa fa-cash-register"></i>\
												<span class="kt-nav__link-text">变帐</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#" class="kt-nav__link">\
												<i class="kt-nav__link-icon fa fa-exchange-alt"></i>\
												<span class="kt-nav__link-text">调点</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#modal__frozen_balance" data-user-id="' + row.ShopAgentId + '" data-current-balance="' + row.Balance.AmountAvailable.toFixed(3) + '" data-toggle="show-modal_frozen_balance_list_datatable" class="kt-nav__link" >\
												<i class="kt-nav__link-icon flaticon2-circular-arrow"></i>\
												<span class="kt-nav__link-text">解冻</span>\
											</a>\
										</li>\
									</ul>\
								</div>\
							</div>\
						';
					//data-toggle="modal" data-target="#modal__change_balance"
				},
			}]
		});

		datatable.on('click', '[data-record-id]', function () {
			modalUserDetail($(this).data('record-id'));
			$('#kt_modal_quick_view_user_info').modal('show');
		});

		downline_datatable = $('#kt_apps_downline_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						//url: 'https://keenthemes.com/metronic/tools/preview/api/datatables/demos/default.php',
						url: 'ShopAgent/SearchDownlines',
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
					noRecords: '查无商户代理',
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
				input: $('#downlineSearch'),
				delay: 400

			},
			rows: {
				callback: function (row, data, index) {
					//console.log(data);
				}
			},

			// columns definition
			columns: [{
				field: 'ShopAgentID',
				title: '#',
				sortable: false,
				width: 20,
				selector: {
					class: 'kt-checkbox--solid'
				},
				textAlign: 'center',
				template: function (row) {
					return row.ShopAgentId;
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
									<a href="#'+ data.ShopAgentId + '" data-record-id="' + data.ShopAgentId + '" class="kt-user-card-v2__name">' + data.FullName + '</a>\
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
									<a href="#'+ data.ShopAgentId + '" data-record-id="' + data.ShopAgentId + '" class="kt-user-card-v2__name">' + data.FullName + '</a>\
									<span class="kt-user-card-v2__desc">' + data.Username + '</span>\
								</div>\
							</div>';
					}

					return output;
				}
			}, {
				field: 'NickName',
				title: '昵称',
				template: function (row) {
					return row.Nickname;
				}
			}, {
				field: 'UplineUserName',
				title: '上级代理',
				textAlign: 'center',
				template: function (row) {
					var uplineFullName = row.UplineFullName ? row.UplineFullName : '';
					var uplineUserName = row.UplineUserName ? row.UplineUserName : '-';
					var uplineUserId = row.UplineUserId ? row.UplineUserId : '';

					var output;

					if (row.UplineUserId) {
						output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details kt-container">\
									<a href="#kt_modal_quick_view_user_info" data-record-id="' + uplineUserId + '" class="kt-user-card-v2__name">' + uplineFullName + '</a>\
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
				textAlign: 'center',
				template: function (row) {
					return row.Balance.AmountAvailable.toFixed(3);
				}
			}, {
				field: 'AmountFrozen',
				title: '冻结余额',
				textAlign: 'center',
				template: function (row) {
					return row.Balance.AmountFrozen.toFixed(3);
				}
			}, {
				field: 'RateRebateAlipayInThousandth',
				title: '支付宝返佣',
				textAlign: 'center',
				template: function (row) {
					var commission = row.RebateCommission.RateRebateAlipayInThousandth / 1000;
					return commission.toFixed(3);
				}
			}, {
				field: 'RateRebateWechatInThousandth',
				title: '微信返佣',
				textAlign: 'center',
				template: function (row) {
					var commission = row.RebateCommission.RateRebateWechatInThousandth / 1000;
					return commission.toFixed(3);
				}
			}, {
				field: 'LastLogin',
				title: '上次登入',
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
				field: "Status",
				title: "帐户状态",
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
			}, {
				field: "Actions",
				width: 80,
				title: "动作",
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {
					var updateLink = '/' + aspController + '/' + aspActionUpdate + '?shopAgentId=' + row.ShopAgentId;
					return '\
							<div class="dropdown">\
								<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
									<i class="flaticon-more-1"></i>\
								</a>\
								<div id="row_action_dropdown_menu" class="dropdown-menu dropdown-menu-right">\
									<ul class="kt-nav">\
										<li class="kt-nav__item">\
											<a href="'+ updateLink + '" class="kt-nav__link">\
												<i class="kt-nav__link-icon flaticon2-contract"></i>\
												<span class="kt-nav__link-text">帐户设置</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#" data-user-id="' + row.ShopAgentId + '" class="kt-nav__link" data-toggle="show-modal_bankbook_records_list_datatable" data-target="#modal_bankbook_records_list_datatable">\
												<i class="kt-nav__link-icon flaticon2-expand"></i>\
												<span class="kt-nav__link-text">帐变纪录</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#modal__change_balance" data-user-id="' + row.ShopAgentId + '" data-current-balance="' + row.Balance.AmountAvailable.toFixed(3) + '" data-toggle="show-modal-change-balance" class="kt-nav__link" >\
												<i class="kt-nav__link-icon flaticon2-trash"></i>\
												<span class="kt-nav__link-text">变帐</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#" class="kt-nav__link">\
												<i class="kt-nav__link-icon flaticon2-mail-1"></i>\
												<span class="kt-nav__link-text">调点</span>\
											</a>\
										</li>\
										<li class="kt-nav__item">\
											<a href="#modal__frozen_balance" data-user-id="' + row.ShopAgentId + '" data-current-balance="' + row.Balance.AmountAvailable.toFixed(3) + '" data-toggle="show-modal_frozen_balance_list_datatable" class="kt-nav__link" >\
												<i class="kt-nav__link-icon flaticon2-mail-1"></i>\
												<span class="kt-nav__link-text">解冻</span>\
											</a>\
										</li>\
									</ul>\
								</div>\
							</div>\
						';
				},
			}]
		});
		downline_datatable.hide();

		downline_datatable.on('kt-datatable--on-ajax-done', function (event, data) {
			//Update agent navigations after table update.
			var currentLayerAgentId = $('#layer_user_id').val();

			var agentNav = $('#navs_agent_' + currentLayerAgentId);
			console.log(agentNav.length);
			if (agentNav.length) {
				//The user who's downline is been searching is exist in agent nav.
				//If agentNav exist, remove element after it.
				agentNav.nextAll().remove();

				//Set previous user id to the one who is before current layer user on nav.
				//Property can be use when the user's downline has been delete and there is no other downlines,
				//then use the property to track the user's upline and loads his downlines.
				$('#layer_user_id_previous').val(
					agentNav.prev().prev().attr('data-record-id')
				);

				console.log($('#layer_user_id_previous').val());
			}
			else {
				if (data.length > 0) {
					//The user has downlines and he isn't exist in agent navs.

					//If this is not in first layer, append user to navs.
					if (currentLayerAgentId.length) {
						//Append new agent nav to  #navs_agent.
						var newAgentNav = '<span class="kt-subheader__breadcrumbs-separator"></span>' +
							'<a href="#" id="navs_agent_' + currentLayerAgentId + '" data-record-id="' + currentLayerAgentId + '" class="kt-subheader__breadcrumbs-link" >' +
							data[0].UplineUserName +
							'</a>';

						//Set previous user id to #layer_user_id, since it is the id of new layer user's upline.
						$('#layer_user_id_previous').val(
							$('#layer_user_id').val()
						);

						console.log(data);
						$('#navs_agent').append(newAgentNav);

					}
					else {
						//The current page is at nav item 'home'.

						//Clear data.
						$('#layer_user_id_previous').val('');

						//Remove all content after  #navs_agent_home.
						console.log('Remove all content after  #navs_agent_home.');
						$('#navs_agent_home').nextAll().remove();
					}
				}
				else {
					console.log('Current layer agent: ' + currentLayerAgentId);
					if (currentLayerAgentId.length) {
						//The user has no downlines and he isn't exist in agent navs.
						console.log('The user has no downlines and he isnt exist in agent navs.');

						//Pop up warning window.
						swal.fire({
							title: '查无结果',
							text: '你选取的商户代理帐户没有下级代理!',
							type: 'warning',
							buttonsStyling: false,
							confirmButtonText: "确认",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						}).then((result) => {
							//Nav to the page of user's upline.
							$('#layer_user_id').val($('#layer_user_id_previous').val());

							var queryObejct = {
								userId: $('#layer_user_id_previous').val(),
								downlineSearch: ''
							};

							downline_datatable.setDataSourceParam('query', queryObejct);
							downline_datatable.load();
							downline_datatable.search();
						});
					}
					else {
						//There isn't any agent. The current page is at nav item 'home'.

						//Clear data.
						$('#layer_user_id_previous').val('');

						//Remove all content after  #navs_agent_home.
						console.log('Remove all content after  #navs_agent_home.');
						$('#navs_agent_home').nextAll().remove();
					}
				}

			}

			$('#layer_user_id').val(currentLayerAgentId);
		});

		downline_datatable.on('click', '[data-record-id]', function () {
			console.log('clicked');
			var showModal = $(this).attr('href') == '#kt_modal_quick_view_user_info' ? true : false;

			//If the item been clicked is upline agent property, then show his info details.
			if (showModal) {
				modalUserDetail($(this).data('record-id'));
				$('#kt_modal_quick_view_user_info').modal('show');
			}
			else {
				$('#layer_user_id_previous').val(
					$('#layer_user_id').val()
				);
				$('#layer_user_id').val($(this).data('record-id'));

				var agentId = $(this).data('record-id');

				var queryObejct = {
					userId: agentId,
					downlineSearch: ''
				};
				downline_datatable.setDataSourceParam('query', queryObejct);
				downline_datatable.load();
				downline_datatable.search();
			}
		});


		bankbook_record_datatable = $('#datatable_bankbook_records').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'ShopAgent/Bankbook',
						params: {
							query: {
								userId: ''
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
				height: 460, // datatable's body's fixed height
				minHeight: 460,
				footer: false, // display/hide footer
				class: 'kt-datatable--brand'
			},

			toobar: {
				items: {
					pagination: {
						pageSizeSelect: [5, 10, 20, 30, 50, 100]
					}
				}
			},

			// column sorting
			sortable: true,

			pagination: true,

			search: {
				input: $('#modal_bankbook_records_list_datatable').find('#generalSearch'),
				delay: 400,
			},

			// columns definition
			columns: [
				{
					field: 'DateOccurred',
					sortable: 'desc',
					selector: false,
					title: '时间',
				}, {
					field: 'Type',
					title: '类型',
					autoHide: false,
					// callback function support for column rendering
					template: function (row) {
						var status = {
							'提现': { 'title': '提现', 'state': 'primary' },
							'取消提现': { 'title': '取消提现', 'state': 'primary' },
							'入金': { 'title': '入金', 'state': 'info' },
							'接收订单': { 'title': '接收订单', 'state': 'warning' },
							'取消订单': { 'title': '取消订单', 'state': 'warning' },
							'管理员减款': { 'title': '管理员减款', 'state': 'danger' },
							'管理员加款': { 'title': '管理员加款', 'state': 'danger' },
							'管理员冻结': { 'title': '管理员冻结', 'state': 'danger' },
							'管理员取消冻结': { 'title': '管理员取消冻结', 'state': 'danger' },
							'调点转入': { 'title': '调点转入', 'state': 'brand' },
							'调点转出': { 'title': '调点转出', 'state': 'brand' },
						};
						return '<span class="kt-badge kt-badge--' + status[row.Type].state + ' kt-badge--dot"></span>&nbsp;<span class="kt-font-' + status[row.Type].state + '">' +
							status[row.Type].title + '</span>';
					}
				}, {
					field: 'TrackingId',
					title: '追踪序号',
				}, {
					field: 'BalanceBefore',
					title: '原余额',
					width: 100,
				}, {
					field: 'AmountChanged',
					title: '变动金额',
					autoHide: false,
					width: 100,
					// callback function support for column rendering
					template: function (row) {
						var amountString = '';
						var stateColorClass = '';
						if (row.AmountChanged > 0) {
							stateColorClass = 'success';
						}
						else {
							stateColorClass = 'danger';
						}
						return '<span class="kt-font-bold kt-font-' + stateColorClass + '">' +
							row.AmountChanged + '</span>';
					}
				}, {
					field: 'BalanceAfter',
					title: '后余额',
					width: 100
				}/*, {
					field: "Actions",
					width: 80,
					title: "动作",
					sortable: false,
					autoHide: false,
					overflow: 'visible',
					template: function (row) {
						if (row.Type === '管理员冻结') {
							return '<button data-frozen-id="' + row.TrackingId + '" data-frozen-balance="' + -row.AmountChanged + '" data-toggle="show-modal-unfreeze" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air btn-success">' + '解冻' + '</button>';
						}
						else {
							return '<span class="btn btn-bold btn-sm btn-font-sm">' + '　' + '</span>';
						}
					},
				}*/
			],

		});
		bankbook_record_datatable.hide();

		frozen_balance_datatable = $('#datatable_frozen_balance').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'ShopAgent/FrozenRecord',
						params: {
							query: {
								userId: ''
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
				height: 600, // datatable's body's fixed height
				minHeight: 600,
				footer: false, // display/hide footer
				class: 'kt-datatable--brand'
			},

			toobar: {
				items: {
					pagination: {
						pageSizeSelect: [5, 10, 20, 30, 50, 100]
					}
				}
			},

			// column sorting
			sortable: true,

			pagination: true,

			search: {
				input: $('#modal_bankbook_records_list_datatable').find('#generalSearch'),
				delay: 400,
			},

			// columns definition
			columns: [
				{
					field: 'DateFroze',
					sortable: 'desc',
					selector: false,
					title: '时间',
				}, {
					field: 'ByAdminName',
					title: '管理员帐号',
				}, {
					field: 'AmountChanged',
					title: '冻结金额',
					autoHide: false,
					width: 100,
					// callback function support for column rendering
					template: function (row) {

						return '<span class="kt-font-bold kt-font-' + 'danger' + '">' +
							row.Amount + '</span>';
					}
				}, {
					field: 'Description',
					title: '备注'
				}, {
					field: "Actions",
					width: 80,
					title: "动作",
					sortable: false,
					autoHide: false,
					overflow: 'visible',
					template: function (row) {
						return '<button data-frozen-id="' + row.FrozenId + '" data-frozen-balance="' + row.Amount + '" data-toggle="show-modal-unfreeze" class="btn btn-sm btn-font-sm btn-elevate btn-elevate-air btn-success">' + '解冻' + '</button>';
					},
				}
			],

		});
		frozen_balance_datatable.hide();

		running_account_record_datatable = $('#datatable_running_account_record').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: '../Order/RunningAccountRecords',
						params: {
							query: {
								userId: ''
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
				autoHide: false,
				textAlign: 'center',
				template: function (row) {
					return row.Id;
				}
			}, {
				field: 'TrackingNumber',
				title: '订单号',
				width: 285,
				autoHide: false,
				template: function (row) {
					var output = '<div class="kt-user-card-v2">\
								<div class="kt-user-card-v2__details">\
									<span href="#" class="kt-user-card-v2__name ">\
									<a href="#" data-order-tracking-number="' + row.OrderTrackingNumber + '" data-toggle="show-info-order" class="kt-link kt-link--state kt-link--dark kt-font-boldest">' + row.OrderTrackingNumber + '</a></span>\
						            <span href="#" class="kt-user-card-v2__name " data-shop-order-id="' + row.ShopOrderId + '">商户订单号:' + row.ShopOrderId + '</span>\
								</div>\
							</div>';

					return output;
				}
			}, {
				field: 'ShopUserName',
				title: '商户',
				autoHide: false,
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
			},/* {
				field: "TraderUserName",
				title: "交易员",
				sortable: true,
				overflow: 'visible',
				width: 100,
				autoHide: false,
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
			}, */{
				field: 'Amount',
				title: '金额',
				width: 100,
				textAlign: 'center',
				autoHide: false,
				template: function (row) {
					var amount = (row.Amount).toFixed(0);
					return amount + '¥';
				}
			}, {
				field: 'DistributedAmount',
				title: '分红',
				width: 100,
				textAlign: 'center',
				autoHide: false,
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
				autoHide: false,
				template: function (row) {
					return convertUTCDateToLocalDate(new Date(row.DateCreated)).toLocaleString();
				}
			}, {
				field: "Status",
				title: "订单状态",
				textAlign: 'center',
				autoHide: false,
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
			}]
		});
		running_account_record_datatable.hide();

		$(document).on('click', '[data-toggle="show-modal_running_account_records_datatable"]', function () {
			var modal = $('#modal_running_account_record_datatable');
			var id = $(this).data('user-id');
			$('#current_running_account_record_user_id').val(id);
			modal.modal('show');
		});

		$(document).on('click', '[data-toggle="show-modal-change-balance"]', function () {
			var modal = $('#modal__change_balance');
			var id = $(this).data('user-id');
			var currentBalance = $(this).data('current-balance');
			console.log(id);
			console.log(currentBalance);
			modal.find('#userid').val(id);
			modal.find('#currentbalance').val(currentBalance);

			modal.modal('show');
		});

		$(document).on('click', '[data-toggle="show-modal_bankbook_records_list_datatable"]', function () {
			var modal = $('#modal_bankbook_records_list_datatable');
			var id = $(this).data('user-id');
			modalBalanceRecordsDatatable(id);
			modal.modal('show');
		});

		$(document).on('click', '[data-toggle="show-modal_frozen_balance_list_datatable"]', function () {
			var modal = $('#modal_frozen_balance_list_datatable');
			var id = $(this).data('user-id');
			modalfrozenBalanceDatatable(id);
			modal.modal('show');
		});

		$(document).on('click', '[data-toggle="show-modal-unfreeze"]', function () {
			//var modal = $('#modal_bankbook_records_list_datatable');
			var id = $(this).data('frozen-id');
			var amount = $(this).data('frozen-balance');
			modalUnfreeze(id, amount);
			//modal.modal('show');
		});
	}


	// user details
	var modalUserDetail = function (id) {
		var el = $('#modal_sub_datatable_ajax_source');
		$.ajax({
			url: "ShopAgent/Detail",
			type: "Get",
			contentType: "application/json",
			data: { shopAgentId: id },
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
				$('#quick_view_user_info_id').html(data.ShopAgentId);
				$('#quick_view_user_info_username').html(data.Username);
				$('#quick_view_user_info_password').html(data.Password);
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

	// change balance
	var modalChangeBalance = function () {
		//Validator
		changeBalanceFormValidator = changeBalanceFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				userid: {
					required: true
				},
				type: {
					required: true
				},

				amount: {
					required: true,
					min: 0
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

		$('#modal__change_balance').on('show.bs.modal', function (e) {


		}).on('hide.bs.modal', function (e) {
			$(this).find('#userid').val('');
			$(this).find('#currentbalance').val('');
		});

		$('#modal__change_balance').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();

			var btn = $('#modal__change_balance').find('[data-action="submit"]');


			if (changeBalanceFormValidator.form()) {
				// See: src\js\framework\base\app.js
				console.log(btn);
				KTApp.progress(btn);
				//KTApp.block(formEl);




				// See: http://malsup.com/jquery/form/#ajaxSubmit
				changeBalanceFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						//KTApp.unblock(formEl);
						var changeAmount = $('#modal__change_balance').find('#amount').val();
						var type = $('#modal__change_balance').find('#type').val();

						swal.fire({
							"title": "变帐完成",
							"text": "帐户余额已" + type + ": " + changeAmount + "¥",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								if (currentTableIndex == 1) {
									datatable.reload();
								}
								else if (currentTableIndex == 2) {
									downline_datatable.reload();
								}

								$('#modal__change_balance').modal('hide');
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
	};

	// balance records
	var modalBalanceRecordsDatatable = function (id) {

		var modal = $('#modal_bankbook_records_list_datatable');

		modal.find('#modal_bank_record_type').on('change', function () {
			bankbook_record_datatable.search($(this).val().toLowerCase());
		});

		modal.find('#kt_form_type').on('change', function () {
			bankbook_record_datatable.search($(this).val().toLowerCase());
		});

		modal.find('#kt_form_status,#kt_form_type').selectpicker();


		bankbook_record_datatable.hide();

		modal.on('shown.bs.modal', function () {
			var modalContent = $(this).find('.modal-content');
			bankbook_record_datatable.spinnerCallback(true, modalContent);
			//Set query string.
			var queryObejct = {
				userId: id,
				generalSearch: ''
			};
			bankbook_record_datatable.setDataSourceParam('query', queryObejct);
			bankbook_record_datatable.load();
			bankbook_record_datatable.search();





			bankbook_record_datatable.on('kt-datatable--on-layout-updated', function () {
				bankbook_record_datatable.show();
				bankbook_record_datatable.spinnerCallback(false, modalContent);
				bankbook_record_datatable.redraw();
			});

			//alreadyReloaded = true;


		}).on('hidden.bs.modal', function () {
			//datatable.KTDatatable('destroy');
		});
	};

	// frozen balances
	var modalfrozenBalanceDatatable = function (id) {

		var modal = $('#modal_frozen_balance_list_datatable');

		/*modal.find('#modal_bank_record_type').on('change', function () {
			bankbook_record_datatable.search($(this).val().toLowerCase());
		});

		modal.find('#kt_form_type').on('change', function () {
			bankbook_record_datatable.search($(this).val().toLowerCase());
		});*/

		modal.find('#kt_form_status,#kt_form_type').selectpicker();


		frozen_balance_datatable.hide();

		modal.on('shown.bs.modal', function () {
			var modalContent = $(this).find('.modal-content');
			bankbook_record_datatable.spinnerCallback(true, modalContent);

			//Set query string.
			var queryObejct = {
				userId: id,
				generalSearch: ''
			};
			frozen_balance_datatable.setDataSourceParam('query', queryObejct);
			frozen_balance_datatable.load();
			frozen_balance_datatable.search();

			frozen_balance_datatable.on('kt-datatable--on-layout-updated', function () {
				frozen_balance_datatable.show();
				frozen_balance_datatable.spinnerCallback(false, modalContent);
				frozen_balance_datatable.redraw();
			});

		}).on('hidden.bs.modal', function () {
			//datatable.KTDatatable('destroy');
		});
	};

	// running account records
	var modalRunningAccountRecordsDatatableInit = function (id) {
		var modal = $('#modal_running_account_record_datatable');

		modal.find('#kt_order_status_filter,#kt_order_payment_channel_filter,#kt_order_payment_scheme_filter').selectpicker();

		running_account_record_datatable.hide();

		modal.on('shown.bs.modal', function () {
			var tableUserId = $('#current_running_account_record_user_id').val();
			daterangepickerInit(tableUserId);
		}).on('hidden.bs.modal', function () {
			//datatable.KTDatatable('destroy');
		});
	};

	// unfreeze alert
	var modalUnfreeze = function (id, amount) {

		let accountFrozen = { FrozenId: id };
		var frozenId = id;
		swal.fire({
			buttonsStyling: false,

			text: "確定要取消此筆凍結嗎?  将归还 " + amount + "¥ 给商户代理。",
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
						url: "ShopAgent/Unfreeze",
						type: "POST",
						contentType: "application/json",
						data: JSON.stringify(frozenId),
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
					title: '解冻成功!',
					text: '',
					type: 'success',
					buttonsStyling: false,
					confirmButtonText: "OK",
					confirmButtonClass: "btn btn-sm btn-bold btn-brand",
				}).then((result) => {
					if (result.value) {
						frozen_balance_datatable.reload();

						if (currentTableIndex == 1) {
							datatable.reload();
						}
						else if (currentTableIndex == 2) {
							downline_datatable.reload();
						}
					}
				})
				// result.dismiss can be 'cancel', 'overlay',
				// 'close', and 'timer'
			} else if (result.dismiss === 'cancel') {
				swal.fire({
					title: '已取消解冻',
					text: '',
					type: 'error',
					buttonsStyling: false,
					confirmButtonText: "OK",
					confirmButtonClass: "btn btn-sm btn-bold btn-brand",
				});
			}
		});


		$('#modal__change_balance').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();

			var btn = $('#modal__change_balance').find('[data-action="submit"]');




			if (changeBalanceFormValidator.form()) {
				// See: src\js\framework\base\app.js
				console.log(btn);
				KTApp.progress(btn);
				//KTApp.block(formEl);




				// See: http://malsup.com/jquery/form/#ajaxSubmit
				changeBalanceFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						//KTApp.unblock(formEl);
						var changeAmount = $('#modal__change_balance').find('#amount').val();
						var type = $('#modal__change_balance').find('#type').val();

						swal.fire({
							"title": "变帐完成",
							"text": "帐户余额已" + type + ": " + changeAmount + "¥",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								window.location.href = "/ShopAgent";
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
	};


	// agent nav
	var agentNavHome = function () {
		$('#navs_agent_home').on('click', function () {
			//Clear hash value in url.
			history.pushState(null, null, window.location.href.split('#')[0]);
			$('#layer_user_id').val($(this).data(''));

			//Query the user's downlines.
			var queryObejct = {
				userId: '',
				downlineSearch: ''
			};

			downline_datatable.setDataSourceParam('query', queryObejct);
			downline_datatable.load();
			downline_datatable.search();
		});

		$('#subheader_agent_navs').on('click', '[data-record-id]', function () {
			console.log('clicked');
			var agentId = $(this).data('record-id');
			$('#layer_user_id').val($(this).data('record-id'));

			var queryObejct = {
				userId: agentId,
				downlineSearch: ''
			};
			downline_datatable.setDataSourceParam('query', queryObejct);
			downline_datatable.load();
			downline_datatable.search();
		});

	}

	// datatable switch
	var datatableSwitch = function () {
		// init form controls
		//$('#kt_form_status, #kt_form_type').selectpicker();

		// event handler on check and uncheck on records
		$("#switch_datatable_original").click(function () {
			//Hide all downline datatable elements.

			//De-selected any row.
			downline_datatable.setActiveAll(false);

			downline_datatable.hide();

			datatable.show();
			datatable.redraw();

			$('#downlineSearch').hide();
			$('#generalSearch').show();

			$('#subheader_agent_navs').hide();

			currentTableIndex = 1;

			$("#switch_datatable_navsagent").removeClass('btn-info');
			$("#switch_datatable_navsagent").addClass('btn-outline-hover-info');

			$(this).removeClass('btn-outline-hover-info');
			$(this).addClass('btn-info');
		});

		$("#switch_datatable_navsagent").click(function () {
			datatable.setActiveAll(false);
			datatable.hide();
			downline_datatable.show();
			downline_datatable.redraw();

			$('#generalSearch').hide();
			$('#downlineSearch').show();

			$('#subheader_agent_navs').show();

			currentTableIndex = 2;

			$("#switch_datatable_original").removeClass('btn-info');
			$("#switch_datatable_original").addClass('btn-outline-hover-info');

			$(this).removeClass('btn-outline-hover-info');
			$(this).addClass('btn-info');
		});
	}

	// Daterangepicker Init
	var daterangepickerInit = function (id) {
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
			updateDatatable(id);
			updateSumData(id);
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

		function updateDatatable(id) {
			var modal = $('#modal_running_account_record_datatable');
			var modalContent = modal.find('.modal-content');
			running_account_record_datatable.spinnerCallback(true, modalContent);

			//Set query string.
			var query = $('#generalSearch').val();
			var from = $('#order_search_from').val();
			var to = $('#order_search_to').val();

			//Parse to utc time.
			var from = new Date(from).toLocaleString('en-US', { timeZone: 'UTC' });
			var to = new Date(to).toLocaleString('en-US', { timeZone: 'UTC' });

			var queryObejct = {
				userId: id,
				generalSearch: query,
				DateFrom: from,
				DateTo: to,
			};

			running_account_record_datatable.setDataSourceParam('query', queryObejct);
			running_account_record_datatable.load();
			running_account_record_datatable.search();

			running_account_record_datatable.on('kt-datatable--on-layout-updated', function () {
				running_account_record_datatable.show();
				running_account_record_datatable.spinnerCallback(false, modalContent);
				running_account_record_datatable.redraw();
			});
		};
		function updateSumData(id) {
			var query = $('#generalSearch').val();
			var from = $('#order_search_from').val();
			var to = $('#order_search_to').val();

			//Parse to utc time.
			var from = new Date(from).toLocaleString('en-US', { timeZone: 'UTC' });
			var to = new Date(to).toLocaleString('en-US', { timeZone: 'UTC' });

			var dataUrl = '../Order/RunningAccountRecordSumData?userId=' + id + '&qurey=' + query + '&from=' + from + '&to=' + to;
			$.ajax({
				url: dataUrl,
				type: "Get",
				contentType: "application/json",
				success: function (response) {
					console.log(response);
					$('#sum_data_order_total_count').html(response.TotalCount);
					$('#sum_data_order_total_success_amount').html(response.TotalSuccessOrderAmount);
					$('#sum_data_order_total_commission').html(response.TotalCommissionAmount);
				},
				error: function (response) {
					console.log(response);
				}
			})
		};
	}

	// search
	var search = function () {
		$('#kt_form_status').on('change', function () {
			if (currentTableIndex == 1) {
				datatable.search($(this).val().toLowerCase(), 'Status');
			}
			else if (currentTableIndex == 2) {
				downline_datatable.search($(this).val().toLowerCase(), 'Status');
			}
		});
	}

	// selection
	var selection = function () {
		// init form controls
		//$('#kt_form_status, #kt_form_type').selectpicker();

		// event handler on check and uncheck on records

		datatable.on('kt-datatable--on-check kt-datatable--on-uncheck kt-datatable--on-layout-updated', function (e) {
			var checkedNodes;
			if (currentTableIndex == 1) {
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
			}

		});


		downline_datatable.on('kt-datatable--on-check kt-datatable--on-uncheck kt-datatable--on-layout-updated', function (e) {
			var checkedNodes;
			if (currentTableIndex == 2) {
				checkedNodes = downline_datatable.rows('.kt-datatable__row--active').nodes(); // get selected records
				var count = checkedNodes.length; // selected records count

				$('#kt_subheader_group_selected_rows').html(count);

				if (count > 0) {
					$('#kt_subheader_search').addClass('kt-hidden');
					$('#kt_subheader_group_actions').removeClass('kt-hidden');
				} else {
					$('#kt_subheader_search').removeClass('kt-hidden');
					$('#kt_subheader_group_actions').addClass('kt-hidden');
				}
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
			var ids;
			if (currentTableIndex == 1) {
				ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
					return $(chk).val();
				});
			}
			if (currentTableIndex == 2) {
				ids = downline_datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
					return $(chk).val();
				});
			}

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
			var ids;
			if (currentTableIndex == 1) {
				ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
					return $(chk).val();
				});
			}
			if (currentTableIndex == 2) {
				ids = downline_datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
					return $(chk).val();
				});
			}




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

					html: "确定要修改选取的 " + ids.length + " 個商户代理帳戶狀態為 " + status + " 吗?",
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
								url: "ShopAgent/UpdateAccountStatus",
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
							text: '你选取的商户代理帐户状态已经修改!',
							type: 'success',
							buttonsStyling: false,
							confirmButtonText: "OK",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						}).then((result) => {
							if (result.value) {
								if (currentTableIndex == 1) {
									datatable.reload();
								};
								if (currentTableIndex == 2) {
									downline_datatable.setActiveAll(false);

									var queryObejct = {
										userId: $('#layer_user_id').val(),
										downlineSearch: ''
									};

									downline_datatable.setDataSourceParam('query', queryObejct);
									downline_datatable.load();
									downline_datatable.search();
								};
							}
						});
						// result.dismiss can be 'cancel', 'overlay',
						// 'close', and 'timer'
					} else if (result.dismiss === 'cancel') {
						swal.fire({
							title: '已取消修改',
							text: '你选取的商户代理帐户状态尚未更改!',
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
			var ids;
			if (currentTableIndex == 1) {
				ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
					return $(chk).val();
				});
			}
			if (currentTableIndex == 2) {
				ids = downline_datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
					return $(chk).val();
				});
			}


			if (ids.length > 0) {

				//get ids and put in to a list.
				var idList = new Array();
				for (var i = 0; i < ids.length; i++) {
					idList[i] = ids[i];
				}

				// learn more: https://sweetalert2.github.io/
				swal.fire({
					buttonsStyling: false,

					text: "确定要删除选取的 " + ids.length + " 个商户代理帐户吗 ?",
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
								url: "ShopAgent/Delete",
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
							text: '你选取的商户代理帐户已删除!',
							type: 'success',
							buttonsStyling: false,
							confirmButtonText: "OK",
							confirmButtonClass: "btn btn-sm btn-bold btn-brand",
						}).then((result) => {
							if (result.value) {
								if (currentTableIndex == 1) {
									datatable.reload();
								};
								if (currentTableIndex == 2) {
									downline_datatable.setActiveAll(false);

									var queryObejct = {
										userId: $('#layer_user_id').val(),
										downlineSearch: ''
									};

									downline_datatable.setDataSourceParam('query', queryObejct);
									downline_datatable.load();
									downline_datatable.search();
								};
							}
						})
						// result.dismiss can be 'cancel', 'overlay',
						// 'close', and 'timer'
					} else if (result.dismiss === 'cancel') {
						swal.fire({
							title: '已取消',
							text: '你选取的商户代理帐户尚未删除!',
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

		downline_datatable.on('kt-datatable--on-layout-updated', function () {
			$('#kt_subheader_total').html('共 ' + datatable.getTotalRows() + ' 笔');
		});
	};

	return {
		// public functions
		init: function () {
			changeBalanceFormEl = $('#form_change_balance');

			init();
			modalChangeBalance();
			modalRunningAccountRecordsDatatableInit();
			datatableSwitch();
			agentNavHome();
			search();
			selection();
			selectedFetch();
			selectedStatusUpdate();
			selectedDelete();
			updateTotal();

			$("#switch_datatable_original").click();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});
