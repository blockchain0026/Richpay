"use strict";
// Class definition

var KTUserListDatatable = function () {
	// variables
	var datatable;
	var createRoleFormValidator;
	var createRoleFormEl;
	var updateRoleFormValidator;
	var updateRoleFormEl;
	var avatar;

	// init
	var init = function () {
		datatable = $('#kt_apps_role_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'Role/Search'
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
					noRecords: '查无角色',
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
				field: 'RoleId',
				title: '#',
				sortable: false,
				selector: {
					class: 'kt-checkbox--solid'
				},
				width: 50,
				textAlign: 'center',
				template: function (row) {
					return row.Id;
				}
			}, {
				field: 'Name',
				title: '角色名称',
				width: 250,
				autoHide: false,
				template: function (row) {
					return row.Name;
				}
			}, {
				field: "Actions",
				width: 70,
				title: "动作",
				textAlign: 'center',
				sortable: false,
				autoHide: false,
				overflow: 'visible',
				template: function (row) {

					return '\
							<div class="dropdown">\
								<a href="javascript:;" class="btn btn-sm btn-clean btn-icon btn-icon-md" data-toggle="dropdown">\
									<i class="flaticon-more-1"></i>\
								</a>\
								<div id="row_action_dropdown_menu" class="dropdown-menu dropdown-menu-right">\
									<ul class="kt-nav">\
										<li class="kt-nav__item">\
											<a href="#" data-role-id="' + row.Id + '" data-role-name="' + row.Name + '" class="kt-nav__link" data-toggle="show_modal_update_role" >\
												<i class="kt-nav__link-icon flaticon2-expand"></i>\
												<span class="kt-nav__link-text">更新角色</span>\
											</a>\
										</li>\
									</ul>\
								</div>\
							</div>\
						';
				}
			}]
		});

		$(document).on('click', '[data-toggle="show_modal_create_role"]', function () {
			var modal = $('#modal__create_role');
			modal.modal('show');
		});
		$(document).on('click', '[data-toggle="show_modal_update_role"]', function () {
			var modal = $('#modal__update_role');

			var roleId = $(this).data('role-id');
			$.ajax({
				url: "Role/GetRolePermissions",
				type: "Get",
				contentType: "application/json",
				data: { id: roleId },
				success: function (response) {
					console.log("Get Permissions:" + response);
					console.log($('#update_role_tree_permissions'));

					$("#update_role_tree_permissions").jstree().deselect_all(true);
					for (var i = 0; i < response.length; i++) {
						//$('#update_role_permissions_list').jstree(true).select_node('Permissions.Dashboards.View');
						$('#update_role_tree_permissions').jstree(true).select_node(response[i]);
					}
					$('#update_role_tree_permissions').jstree('close_all');
					//$('#update_role_tree_permissions').jstree(true).select_node(response);
				},
				error: function (response) {
					console.log(response);
				}
			})

			var roleName = $(this).data('role-name');
			$('#update_role_input_role_name').val(roleName);

			modal.modal('show');
		});
	}

	var initPermissionsTree = function () {
		$('#kt_tree_3, #update_role_tree_permissions').jstree({
			'plugins': ["checkbox", "types"],
			'core': {
				"themes": {
					"responsive": true
				},
				'data': [{
					"text": "仪表板",
					"children": [{
						"id": "Permissions.Dashboards.View",
						"text": "检视",
						"state": {
							"selected": false,
							"opened": true
						},
						"icon": "fa fa-eye kt-font-primary "
					}]
				}, {
					"text": "管理员管理",
					"children": [{
						"text": "账户管理",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.Administration.Managers.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Administration.Managers.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.Administration.Managers.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.Administration.Managers.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						},]
					}, {
						"text": "角色管理",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.Administration.Roles.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Administration.Roles.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.Administration.Roles.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.Administration.Roles.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						},]
					}]
				}, {
					"text": "系统设置",
					"children": [{
						"text": "基本信息",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.SystemConfiguration.SiteBaseInfo.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.SystemConfiguration.SiteBaseInfo.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					}, {
						"text": "支付",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.SystemConfiguration.Payment.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.SystemConfiguration.Payment.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					}, {
						"text": "提示音",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.SystemConfiguration.SystemNotificationSound.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.SystemConfiguration.SystemNotificationSound.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					}, {
						"text": "用户通知",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.SystemConfiguration.UserNotification.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.SystemConfiguration.UserNotification.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					}, {
						"text": "提现",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.SystemConfiguration.WithdrawalAndDeposit.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.SystemConfiguration.WithdrawalAndDeposit.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					}, {
						"text": "通道",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.SystemConfiguration.PaymentChannel.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.SystemConfiguration.PaymentChannel.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					},]
				}, {
					"text": "码商管理",
					"children": [{
						"text": "代理列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.Organization.TraderAgents.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Organization.TraderAgents.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.Organization.TraderAgents.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.Organization.TraderAgents.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						}, {
							"id": "Permissions.Organization.TraderAgents.BankBook.View",
							"text": "账变记录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Organization.TraderAgents.FrozenRecord.View",
							"text": "冻结纪录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Organization.TraderAgents.ChangeBalance.Create",
							"text": "变账",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"text": "层级浏览",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
							"children": [{
								"id": "Permissions.Organization.TraderAgents.Downlines.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.Organization.TraderAgents.Downlines.Create",
								"text": "添加",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-plus kt-font-success ",
							}, {
								"id": "Permissions.Organization.TraderAgents.Downlines.Edit",
								"text": "修改",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}, {
								"id": "Permissions.Organization.TraderAgents.Downlines.Delete",
								"text": "删除",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-recycle kt-font-danger ",
							},]
						},]
					}, {
						"text": "交易员列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.Organization.Traders.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Organization.Traders.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.Organization.Traders.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.Organization.Traders.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						}, {
							"id": "Permissions.Organization.Traders.BankBook.View",
							"text": "账变记录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Organization.Traders.FrozenRecord.View",
							"text": "冻结纪录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.Organization.Traders.ChangeBalance.Create",
							"text": "变账",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						},]
					}, {
						"text": "待审核信息",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"text": "代理",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
							"children": [{
								"id": "Permissions.Organization.TraderAgents.PendingReview.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.Organization.TraderAgents.PendingReview.Review",
								"text": "审核",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}]
						}, {
							"text": "交易员",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
							"children": [{
								"id": "Permissions.Organization.Traders.PendingReview.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.Organization.Traders.PendingReview.Review",
								"text": "审核",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}]
						}]
					},]
				}, {
					"text": "商户管理",
					"children": [{
						"text": "代理列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.ShopManagement.ShopAgents.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.ShopManagement.ShopAgents.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.ShopManagement.ShopAgents.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.ShopManagement.ShopAgents.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						}, {
							"id": "Permissions.ShopManagement.ShopAgents.BankBook.View",
							"text": "账变记录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.ShopManagement.ShopAgents.FrozenRecord.View",
							"text": "冻结纪录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.ShopManagement.ShopAgents.ChangeBalance.Create",
							"text": "变账",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"text": "层级浏览",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
							"children": [{
								"id": "Permissions.ShopManagement.ShopAgents.Downlines.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.ShopManagement.ShopAgents.Downlines.Create",
								"text": "添加",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-plus kt-font-success ",
							}, {
								"id": "Permissions.ShopManagement.ShopAgents.Downlines.Edit",
								"text": "修改",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}, {
								"id": "Permissions.ShopManagement.ShopAgents.Downlines.Delete",
								"text": "删除",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-recycle kt-font-danger ",
							},]
						},]
					}, {
						"text": "商户列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.ShopManagement.Shops.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.ShopManagement.Shops.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.ShopManagement.Shops.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.ShopManagement.Shops.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						}, {
							"id": "Permissions.ShopManagement.Shops.BankBook.View",
							"text": "账变记录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.ShopManagement.Shops.FrozenRecord.View",
							"text": "冻结纪录",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.ShopManagement.Shops.ChangeBalance.Create",
							"text": "变账",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.ShopManagement.Shops.ApiKey.Create",
							"text": "API KEY",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-danger ",
						},]
					}, {
						"text": "待审核信息",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"text": "代理",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
							"children": [{
								"id": "Permissions.ShopManagement.ShopAgents.PendingReview.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.ShopManagement.ShopAgents.PendingReview.Review",
								"text": "审核",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}]
						}, {
							"text": "商户",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
							"children": [{
								"id": "Permissions.ShopManagement.Shops.PendingReview.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.ShopManagement.Shops.PendingReview.Review",
								"text": "审核",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}]
						}]
					},]
				}, {
					"text": "提现管理",
					"children": [{
						"text": "提现列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.WithdrawalManagement.Withdrawals.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.WithdrawalManagement.Withdrawals.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.WithdrawalManagement.Withdrawals.SearchUser",
							"text": "搜索用户",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}]
					}, {
						"text": "待处理列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.WithdrawalManagement.PendingReview.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.WithdrawalManagement.PendingReview.Approve",
							"text": "审核",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.WithdrawalManagement.PendingReview.ForceSuccess",
							"text": "强制成功",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.WithdrawalManagement.PendingReview.ApproveCancellation",
							"text": "审核取消申请",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						},]
					}, {
						"text": "提现银行选项",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.WithdrawalManagement.BankOptions.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.WithdrawalManagement.BankOptions.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.WithdrawalManagement.BankOptions.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						}]
					},]
				}, {
					"text": "入金管理",
					"children": [{
						"text": "入金列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.DepositManagement.Deposits.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.DepositManagement.Deposits.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.DepositManagement.Deposits.SearchUser",
							"text": "搜索用户",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}]
					}, {
						"text": "待处理列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.DepositManagement.PendingReview.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.DepositManagement.PendingReview.Verify",
							"text": "审核",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					}, {
						"text": "收款账户列表",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.DepositManagement.DepositBankAccounts.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.DepositManagement.DepositBankAccounts.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.DepositManagement.DepositBankAccounts.Delete",
							"text": "删除",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-recycle kt-font-danger ",
						}]
					},]
				}, {
					"text": "二维码管理",
					"children": [{
						"text": "手动",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.QrCodeManagement.Manual.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.Edit",
							"text": "修改",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.SearchTrader",
							"text": "搜索交易员",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.SearchShop",
							"text": "搜索商户",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.Enable",
							"text": "启用",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.Disable",
							"text": "停用",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.StartPairing",
							"text": "开始配对/轮巡",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.StopPairing",
							"text": "停止配对/轮巡",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"id": "Permissions.QrCodeManagement.Manual.ResetRiskControlData",
							"text": "重置数据",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}, {
							"text": "待审核",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
							"children": [{
								"id": "Permissions.QrCodeManagement.Manual.PendingReview.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.QrCodeManagement.Manual.PendingReview.Approve",
								"text": "审核",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}, {
								"id": "Permissions.QrCodeManagement.Manual.PendingReview.Reject",
								"text": "拒绝",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-recycle kt-font-danger ",
							}]
						}, {
							"text": "二维码数据",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
							"children": [{
								"id": "Permissions.QrCodeManagement.Manual.CodeData.View",
								"text": "检视",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-eye kt-font-primary ",
							}, {
								"id": "Permissions.QrCodeManagement.Manual.CodeData.Edit",
								"text": "修改",
								"state": {
									"selected": false,
									"opened": true
								},
								"icon": "fa fa-edit kt-font-warning ",
							}]
						}]
					}]
				}, {
					"text": "订单管理",
					"children": [{
						"text": "订单总览",
						"state": {
							"selected": false,
							"opened": true
						},
						"children": [{
							"id": "Permissions.OrderManagement.PlatformOrders.View",
							"text": "检视",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-eye kt-font-primary ",
						}, {
							"id": "Permissions.OrderManagement.PlatformOrders.Create",
							"text": "添加",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-plus kt-font-success ",
						}, {
							"id": "Permissions.OrderManagement.PlatformOrders.ConfirmPayment",
							"text": "确认收款",
							"state": {
								"selected": false,
								"opened": true
							},
							"icon": "fa fa-edit kt-font-warning ",
						}]
					}]
				},
				],
				expand_selected_onload: false
			},
			"types": {
				"default": {
					"icon": "fa fa-folder kt-font-warning"
				},
				"file": {
					"icon": "fa fa-file  kt-font-warning"
				}
			},
		});

		// after the tree is loaded
		$("#update_role_tree_permissions").on("loaded.jstree", function () {
			// don't use "#" for ID
		});
	};

	// create role modal
	var modalCreateRole = function () {
		//Validator
		createRoleFormValidator = createRoleFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				rolename: {
					required: true
				}
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份参数有误，请修正",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		$('#modal__create_role').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();

			var btn = $('#modal__create_role').find('[data-action="submit"]');

			if (createRoleFormValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createRoleFormEl);

				//Get all selected permissions's id.
				var permissions = new Array();
				var checkedPermissions = $("#kt_tree_3").jstree('get_checked');
				for (var i = 0; i < checkedPermissions.length; i++) {
					if (!checkedPermissions[i].startsWith("j1")) {
						permissions.push(checkedPermissions[i]);
					}
				}

				//Create input for each permissions.
				var permissionsListEl = $('#create_role_permissions_list');
				permissionsListEl.html('');
				for (var i = 0; i < permissions.length; i++) {
					$('<input name="permissions[' + i + ']" value="' + permissions[i] + '" hidden />').appendTo(permissionsListEl);
				}

				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createRoleFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createRoleFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_role').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(createRoleFormEl);
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

	// update role modal
	var modalUpdateRole = function () {
		//Validator
		updateRoleFormValidator = updateRoleFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				// Step 1
				rolename: {
					required: true
				}
			},

			// Display error
			invalidHandler: function (event, validator) {
				KTUtil.scrollTop();

				swal.fire({
					"title": "",
					"text": "部份参数有误，请修正",
					"type": "error",
					"buttonStyling": false,
					"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
				});
			},

			// Submit valid form
			submitHandler: function (form) {

			}
		});

		$('#modal__update_role').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();

			var btn = $('#modal__update_role').find('[data-action="submit"]');

			if (updateRoleFormValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(updateRoleFormEl);

				//Get all selected permissions's id.
				var permissions = new Array();
				var checkedPermissions = $("#update_role_tree_permissions").jstree('get_checked');
				for (var i = 0; i < checkedPermissions.length; i++) {
					if (!checkedPermissions[i].startsWith("j2")) {
						permissions.push(checkedPermissions[i]);
					}
				}
				console.log(checkedPermissions);
				//Create input for each permissions.
				var permissionsListEl = $('#update_role_permissions_list');
				permissionsListEl.html('');
				for (var i = 0; i < permissions.length; i++) {
					$('<input name="permissions[' + i + ']" value="' + permissions[i] + '" hidden />').appendTo(permissionsListEl);
				}

				// See: http://malsup.com/jquery/form/#ajaxSubmit
				updateRoleFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(updateRoleFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__update_role').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.unblock(updateRoleFormEl);
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


	// search
	var search = function () {
		/*$('#kt_order_type_filter').on('change', function () {
			datatable.search($(this).val(), 'OrderType');
		});*/
		$('#kt_order_status_filter').on('change', function () {
			datatable.search($(this).val(), 'OrderStatus');
		});
		$('#kt_order_payment_channel_filter').on('change', function () {
			datatable.search($(this).val(), 'OrderPaymentChannel');
		});
		$('#kt_order_payment_scheme_filter').on('change', function () {
			datatable.search($(this).val(), 'OrderPaymentScheme');
		});
	}

	// selection
	var selection = function () {
		// init form controls
		$('#kt_order_status_filter, #kt_order_payment_channel_filter, #kt_order_payment_scheme_filter').selectpicker();

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

	// selected records delete
	var selectedDelete = function () {
		$('#kt_subheader_group_actions_delete_all').on('click', function () {
			// fetch selected IDs
			var ids;
			ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
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

					text: "确定要删除选取的 " + ids.length + " 个角色吗 ?",
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
								url: "Role/Delete",
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
							text: '你选取的角色已删除',
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
							text: '你选取的交易员帐户尚未删除!',
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

			createRoleFormEl = $('#form_create_role');
			updateRoleFormEl = $('#form_update_role');

			initPermissionsTree();
			modalCreateRole();
			modalUpdateRole();

			search();
			selection();
			selectedDelete();
			updateTotal();
		},
	};
}();

// On document ready
KTUtil.ready(function () {
	KTUserListDatatable.init();
});