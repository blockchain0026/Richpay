$(document).on('click', '[data-toggle="show-dialog_generate_api_key"]', function () {
	//var modal = $('#modal_bankbook_records_list_datatable');
	var id = $(this).data('user-id');
	modalGenerateApiKey(id);
	//modal.modal('show');
});


// generate api key.
var modalGenerateApiKey = function (shopId) {
	var newApiKey = '';

	swal.fire({
		buttonsStyling: false,

		text: "生成新的 API KEY 将会使原有的 API KEY 失效，确认生成吗?",
		type: "danger",

		confirmButtonText: "确认",
		confirmButtonClass: "btn btn-sm btn-bold btn-success",

		showCancelButton: true,
		cancelButtonText: "取消",
		cancelButtonClass: "btn btn-sm btn-bold btn-secondary",

		showLoaderOnConfirm: true,

		preConfirm: () => {
			return new Promise(function (resolve, reject) {
				$.ajax({
					url: "../Shop/GenerateApiKey",
					type: "POST",
					contentType: "application/json",
					data: JSON.stringify(shopId),
					success: function (response) {
						//Set api key variable for later display.
						newApiKey = response.apiKey;

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
				title: newApiKey,
				text: '生成完毕，关闭视窗后将无法再次查阅 API KEY，请妥善保管。',
				type: 'success',
				buttonsStyling: false,
				confirmButtonText: "OK",
				confirmButtonClass: "btn btn-sm btn-bold btn-brand",
			}).then((result) => {
				if (result.value) {
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
};
