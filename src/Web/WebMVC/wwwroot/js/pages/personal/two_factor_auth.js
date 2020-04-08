"use strict";

var KTTwoFactorAuth = function () {
	var enableTwoFactorAuthFormEl;
	var disableTwoFactorAuthFormEl;

	var init = function () {


		$(document).on('click', '[data-toggle="show_modal_enable_two_factor_auth"]', function () {
			var modal = $('#modal__enable_two_factor_auth');

			modal.modal('show');
		});
		$(document).on('click', '[data-toggle="show_modal_enable_two_factor_auth"]', function () {

		});
	}

	var modalEnableTwoFactorAuth = function () {
		$('#modal__enable_two_factor_auth').on('show.bs.modal', function (e) {
			$.ajax({
				url: "EnableTwoFactorAuth",
				type: "Get",
				contentType: "application/json",
				data: JSON.stringify(),
				success: function (response) {
					$('#two_factor_shared_key').html('');
					$('#qrcode_image').attr('src', '');
					if (response.success) {
						$('#two_factor_shared_key').html(response.sharedKey);
						$('#qrcode_image').attr('src', response.authQrCode);
						//$('#qrCodeData').data('url', response.authenticatorUri);
					}
					else {
						console.log(response);
					}
				},
				error: function (response) {
					console.log(response);
				}
			});
		}).on('hide.bs.modal', function (e) {
			$(this).find('#two_factor_shared_key').html('');
			$(this).find('#qrcode_image').attr('src', '');
		});

		//Submit handler
		$('#modal__enable_two_factor_auth').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#modal__enable_two_factor_auth').find('[data-action="submit"]');


			//$('#kt_create_barcode_order_input').val(orderData);

			KTApp.progress(btn);
			KTApp.block(enableTwoFactorAuthFormEl);


			enableTwoFactorAuthFormEl.ajaxSubmit({
				success: function () {
					KTApp.unprogress(btn);
					KTApp.unblock(enableTwoFactorAuthFormEl);

					swal.fire({
						"title": "验证成功",
						"text": "日后登录账户时需打开认证应用并输入认证码",
						"type": "success",
						"confirmButtonClass": "btn btn-secondary"
					}).then((result) => {
						if (result.value) {
							$('#modal__enable_two_factor_auth').modal('hide');
							location.reload();
						}
					});
				},
				error: function (data) {
					KTApp.unprogress(btn);
					KTApp.unblock(enableTwoFactorAuthFormEl);
					console.log(data);
					swal.fire({
						"title": "验证失败",
						"text": data.responseJSON.message,
						"type": "error",
						"buttonStyling": false,
						"confirmButtonClass": "btn btn-brand btn-sm btn-bold"
					});
				}
			});
		});
	};

	var disableTwoFactorAuth = function () {
		//Submit handler
		$('#disable_two_factor_auth_form').on('click', '[data-action="submit"]', function (e) {
			e.preventDefault();
			var btn = $('#disable_two_factor_auth_form').find('[data-action="submit"]');


			//$('#kt_create_barcode_order_input').val(orderData);

			KTApp.progress(btn);

			swal.fire({
				buttonsStyling: false,

				text: "确认停用GA认证吗?",
				type: "error",

				confirmButtonText: "确认",
				confirmButtonClass: "btn btn-sm btn-bold btn-danger",

				showCancelButton: true,
				cancelButtonText: "取消",
				cancelButtonClass: "btn btn-sm btn-bold btn-secondary",

				showLoaderOnConfirm: true,

				preConfirm: () => {
					return new Promise(function (resolve, reject) {
						disableTwoFactorAuthFormEl.ajaxSubmit({
							success: function (response) {
								KTApp.unprogress(btn);
								//KTApp.unblock(disableTwoFactorAuthFormEl);
								resolve(response);
							},
							error: function (data) {
								KTApp.unprogress(btn);
								//KTApp.unblock(disableTwoFactorAuthFormEl);
								console.log(data);
								reject(data.responseText)
							}
						});

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
						title: '已停用GA认证',
						text: '',
						type: 'success',
						buttonsStyling: false,
						confirmButtonText: "OK",
						confirmButtonClass: "btn btn-sm btn-bold btn-brand",
					}).then((result) => {
						KTApp.unprogress(btn);
						if (result.value) {
							location.reload();
						}
					})
					// result.dismiss can be 'cancel', 'overlay',
					// 'close', and 'timer'
				} else if (result.dismiss === 'cancel') {
					KTApp.unprogress(btn);
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
		});
	};


	return {
		// public functions
		init: function () {
			enableTwoFactorAuthFormEl = $('#enable_two_factor_auth_form_send_code');
			disableTwoFactorAuthFormEl = $('#disable_two_factor_auth_form');

			init();

			modalEnableTwoFactorAuth();
			disableTwoFactorAuth();
		},
	};
}();


// On document ready
KTUtil.ready(function () {
	KTTwoFactorAuth.init();
});