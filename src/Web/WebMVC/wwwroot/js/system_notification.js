"use strict";

// Class definition
var VoiceBroadcaster = function () {

    //SignalR withdrawal broadcaster.
    var withdrawalBroadcasterInit = function () {
        var connection = new signalR.HubConnectionBuilder().withUrl("/withdrawalVoiceHub").build();

        connection.on("BroadcastVoice", function (message) {
            var newWithdrawals = message[0];
            var count = newWithdrawals.length;
            if (count > 0) {
                broadcastMessage('有' + count + '个用户申请提现');

                toastr.options = {
                    "closeButton": false,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": false,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "onclick": function () {
                        window.location.href = "/Withdrawal/PendingReview";
                    },
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "10000",
                    "extendedTimeOut": "10000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                };

                for (var i = 0; i < count; i++) {
                    var description = newWithdrawals[i].description;
                    var text = '<h6 class="flaticon-coins kt-m0">新提现申请:' + newWithdrawals[i].id + '</h6> </br>\
                    提现金额: '+ newWithdrawals[i].totalAmount + '</br>\
                    银行: '+ newWithdrawals[i].bankAccount.bankName + '</br>\
                    户名: '+ newWithdrawals[i].bankAccount.accountName + '</br>\
                    账号: '+ newWithdrawals[i].bankAccount.accountNumber + '</br>\
                    备注: '+ (description ? description : '-');
                    toastr.info(text);
                }
            }

        });
        connection.start().then(function () {
            console.log("Withdrawal Broadcaster Connected.");
        }).catch(function (err) {
            return console.error(err.toString());
        });
        /*$('#data_auto_refresh').on('change', function () {
            if ($('#data_auto_refresh:checked').val()) {
    
            }
            else {
                connection.stop();
            }
        });*/
    }

    //SignalR desposit broadcaster.
    var depositBroadcasterInit = function () {
        var connection = new signalR.HubConnectionBuilder().withUrl("/depositVoiceHub").build();

        connection.on("BroadcastVoice", function (message) {
            var newDiposits = message[0];
            var count = newDiposits.length;
            if (count > 0) {
                console.log(count);
                toastr.options = {
                    "closeButton": false,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": false,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": false,
                    "onclick": function () {
                        window.location.href = "/Deposit/PendingReview";
                    },
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "10000",
                    "extendedTimeOut": "10000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                };
                console.log(count);

                for (var i = 0; i < count; i++) {
                    console.log(i);

                    var description = newDiposits[i].description;
                    console.log('description:' + description);
                    console.log('id:' + newDiposits[i].id);
                    console.log('totalAmount:' + newDiposits[i].totalAmount);
                    console.log('bankName:' + newDiposits[i].bankAccount.bankName);
                    console.log('accountName:' + newDiposits[i].bankAccount.accountName);
                    console.log('accountNumber:' + newDiposits[i].bankAccount.accountNumber);
                    var text = '<h6 class="flaticon-coins kt-m0">新入金申请:' + newDiposits[i].id + '</h6> </br>\
                    存款金额: '+ newDiposits[i].totalAmount + '</br>\
                    收款银行: '+ newDiposits[i].bankAccount.bankName + '</br>\
                    收款户名: '+ newDiposits[i].bankAccount.accountName + '</br>\
                    收款账号: '+ newDiposits[i].bankAccount.accountNumber + '</br>\
                    备注: '+ (description ? description : '-');
                    toastr.info(text);
                }

                broadcastMessage('有' + count + '个用户申请存款');
            }

        });
        connection.start().then(function () {
            console.log("Deposit Broadcaster Connected.");
        }).catch(function (err) {
            return console.error(err.toString());
        });
        /*$('#data_auto_refresh').on('change', function () {
            if ($('#data_auto_refresh:checked').val()) {
    
            }
            else {
                connection.stop();
            }
        });*/
    }

    //SignalR member broadcaster.
    var memberBroadcasterInit = function () {
        var connection = new signalR.HubConnectionBuilder().withUrl("/memberVoiceHub").build();

        connection.on("BroadcastVoice", function (message) {
            var newMembers = message[0];
            var count = newMembers.length;
            if (count > 0) {
                for (var i = 0; i < count; i++) {
                    var baseRoleType = newMembers[i].baseRoleType;
                    var url = '';
                    var baseRoleName = '';
                    if (baseRoleType == 1) {
                        url = '/Trader/PendingReview';
                        baseRoleName = '交易员';
                    }
                    else if (baseRoleType == 2) {
                        url = '/TraderAgent/PendingReview';
                        baseRoleName = '交易员代理';
                    }
                    else if (baseRoleType == 3) {
                        url = '/Shop/PendingReview';
                        baseRoleName = '商户';
                    }
                    else if (baseRoleType == 4) {
                        url = '/ShopAgent/PendingReview';
                        baseRoleName = '商户代理';
                    }
                    var text = '<h6 class="flaticon-coins kt-m0">新用户申请:' + newMembers[i].id + '</h6> </br>\
                    账号: '+ newMembers[i].userName + '</br>\
                    全名: '+ newMembers[i].fullName + '</br>\
                    类型: '+ baseRoleName;


                    toastr.options = {
                        "closeButton": false,
                        "debug": false,
                        "newestOnTop": false,
                        "progressBar": false,
                        "positionClass": "toast-top-right",
                        "preventDuplicates": false,
                        "onclick": function () {
                            window.location.href = url;
                        },
                        "showDuration": "300",
                        "hideDuration": "1000",
                        "timeOut": "10000",
                        "extendedTimeOut": "10000",
                        "showEasing": "swing",
                        "hideEasing": "linear",
                        "showMethod": "fadeIn",
                        "hideMethod": "fadeOut"
                    };
                    toastr.warning(text);
                }
                broadcastMessage('有' + count + '个新用户待审核');
            }
        });
        connection.start().then(function () {
            console.log("Member Broadcaster Connected.");
        }).catch(function (err) {
            return console.error(err.toString());
        });
        /*$('#data_auto_refresh').on('change', function () {
            if ($('#data_auto_refresh:checked').val()) {
    
            }
            else {
                connection.stop();
            }
        });*/
    }

    //SignalR qr code broadcaster.
    var qrCodeBroadcasterInit = function () {
        var connection = new signalR.HubConnectionBuilder().withUrl("/qrcodeVoiceHub").build();

        connection.on("BroadcastVoice", function (message) {
            var newQrCodes = message[0];
            var count = newQrCodes.length;
            if (count > 0) {
                for (var i = 0; i < count; i++) {
                    var paymentChannel = newQrCodes[i].paymentChannel.name;
                    var paymentScheme = newQrCodes[i].paymentScheme.name;
                    var url = '/ManualCode/PendingReview';

                    var text = '<h6 class="flaticon-coins kt-m0">新二维码申请:' + newQrCodes[i].id + '</h6> </br>\
                    名称: '+ newQrCodes[i].fullName + '</br>\
                    类型: '+ paymentChannel + '</br>\
                    通道: '+ paymentScheme;


                    toastr.options = {
                        "closeButton": false,
                        "debug": false,
                        "newestOnTop": false,
                        "progressBar": false,
                        "positionClass": "toast-top-right",
                        "preventDuplicates": false,
                        "onclick": function () {
                            window.location.href = url;
                        },
                        "showDuration": "300",
                        "hideDuration": "1000",
                        "timeOut": "10000",
                        "extendedTimeOut": "10000",
                        "showEasing": "swing",
                        "hideEasing": "linear",
                        "showMethod": "fadeIn",
                        "hideMethod": "fadeOut"
                    };
                    toastr.error(text);
                }
                broadcastMessage('有' + count + '个二维码待审核');
            }
        });
        connection.start().then(function () {
            console.log("Qr Code Broadcaster Connected.");
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }


    var broadcastMessage = function (str) {
        //var url = "http://tts.baidu.com/text2audio?lan=zh&ie=UTF-8&text=+" + encodeURI(str); //baidu male
        var url = "http://tts.baidu.com/text2audio?cuid=baiduid&lan=zh&ctp=1&pdt=311&tex=+" + encodeURI(str); //baidu female

        var n = new Audio(url);

        n.src = url;

        n.play();
    }

    return {
        // Init demos
        init: function () {
            // init signalR withdrawal refresh.
            withdrawalBroadcasterInit();
            depositBroadcasterInit();
            memberBroadcasterInit();
            qrCodeBroadcasterInit();
        }
    };
}();

// Class initialization on page load
jQuery(document).ready(function () {
    VoiceBroadcaster.init();
});