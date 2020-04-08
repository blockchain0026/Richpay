"use strict";
// Class definition

var KTUserListDatatable = function () {

	// variables
	var datatable;


	var createBankOptionFormValidator;
	var createBankOptionFormEl;

	const aspController = 'Withdrawal';
	const aspActionDelete = 'Delete';
	const aspActionUpdate = 'Update';

	// init
	var init = function () {
		// init the datatables. Learn more: https://keenthemes.com/metronic/?page=docs&section=datatable
		datatable = $('#kt_apps_bank_option_list_datatable').KTDatatable({
			// datasource definition
			data: {
				type: 'remote',
				source: {
					read: {
						url: 'BankOption'
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
					noRecords: '查无提现银行',
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
				field: 'WithdrawalBankId',
				title: '#',
				sortable: false,
				width: 20,
				selector: {
					class: 'kt-checkbox--solid'
				},
				textAlign: 'center',
				template: function (row) {
					return row.WithdrawalBankId;
				}
			}, {
				field: 'BankName',
				title: '银行名称',
				template: function (row) {
					return row.BankName;
				}
			}, {
				field: 'DateCreated',
				title: '添加时间',
				textAlign: 'center',
				template: function (row) {
					return row.DateCreated;
				}
			}]
		});

		$(document).on('click', '[data-toggle="show-modal-create-bank-option"]', function () {
			var modal = $('#modal__create_bank_option');

			modal.modal('show');
		});
	}


	// create bank option modal
	var modalCreateBankOption = function () {
		//Validator
		createBankOptionFormValidator = createBankOptionFormEl.validate({
			// Validate only visible fields
			ignore: ":hidden",

			// Validation rules
			rules: {
				bankname: {
					required: true
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

		$('#modal__create_bank_option').on('show.bs.modal', function (e) {

		}).on('hide.bs.modal', function (e) {
			$(this).find('#bankName').val('');
		});

		$('#modal__create_bank_option').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();

			var btn = $('#modal__create_bank_option').find('[data-action="submit"]');

			if (createBankOptionFormValidator.form()) {
				// See: src\js\framework\base\app.js
				KTApp.progress(btn);
				KTApp.block(createBankOptionFormEl);


				// See: http://malsup.com/jquery/form/#ajaxSubmit
				createBankOptionFormEl.ajaxSubmit({
					success: function () {
						KTApp.unprogress(btn);
						KTApp.unblock(createBankOptionFormEl);

						swal.fire({
							"title": "添加成功",
							"text": "",
							"type": "success",
							"confirmButtonClass": "btn btn-secondary"
						}).then((result) => {
							if (result.value) {
								datatable.reload();

								$('#modal__create_bank_option').modal('hide');
							}
						});
					},
					error: function (data) {
						KTApp.unprogress(btn);
						KTApp.block(createBankOptionFormEl);

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


	// selection
	var selection = function () {
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

	// selected records delete
	var selectedDelete = function () {
		$('#kt_subheader_group_actions_delete_all').on('click', function () {
			// fetch selected IDs
			var ids = datatable.rows('.kt-datatable__row--active').nodes().find('.kt-checkbox--single > [type="checkbox"]').map(function (i, chk) {
				return $(chk).val();
			});

			if (ids.length > 0) {

				//get ids and put in to a list.
				var bankOptionIds = new Array();
				for (var i = 0; i < ids.length; i++) {
					bankOptionIds[i] = parseInt(ids[i]);
				}

				// learn more: https://sweetalert2.github.io/
				swal.fire({
					buttonsStyling: false,

					text: "确定要删除选取的 " + ids.length+" 个银行选项吗?",
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
								url: "DeleteBankOptions",
								type: "POST",
								contentType: "application/json",
								data: JSON.stringify(bankOptionIds),
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
							text: '你选取的提现银行选项已删除!',
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
			createBankOptionFormEl = $('#form_create_bank_option');

			init();
			modalCreateBankOption();
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
