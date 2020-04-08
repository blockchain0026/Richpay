"use strict";

// Class definition
var KTDashboard = function () {


    // Order Statistics.
    // Based on Chartjs plugin - http://www.chartjs.org/
    var orderStatistics = function (query, from, to) {
        $.ajax({
            url: "Order/Statistic?query=" + query + "&from=" + from + "&to=" + to,
            type: "Get",
            contentType: "application/json",
            success: function (response) {
                var orderStatistic = response;
                var getSuccessRateProgressBar = function (title, valueNow, text) {
                    var valueMax = 100;
                    var valueNowInPercent = ((valueNow / valueMax) * 100).toFixed(2);

                    console.log(valueNowInPercent);

                    var proressBarClass =
                        valueNowInPercent == 0 ? ' ' :
                            valueNowInPercent < 50 ? ' bg-danger' :
                                valueNowInPercent < 70 ? ' bg-warning' :
                                    valueNowInPercent <= 100 ? ' bg-success' : null;

                    var statsClass =
                        valueNowInPercent == 0 ? ' ' :
                            valueNowInPercent < 50 ? ' kt-font-danger' :
                                valueNowInPercent < 70 ? ' kt-font-warning' :
                                    valueNowInPercent <= 100 ? ' kt-font-success' : null;

                    var output = '';
                    output +=
                        '<div class="kt-widget24">\
                    <div class="kt-widget24__details">\
                        <div class="kt-widget24__info">\
                            <h4 class="kt-widget24__title">\
                                '+ title + '\
                            </h4>\
                            <span class="kt-widget24__desc">\
                                成功率 '+ text + '\
                            </span>\
                        </div>\
                        <span class="kt-widget24__stats '+ statsClass + '">\
                            '+ valueNowInPercent + '%\
                        </span>\
                    </div>\
                    <div class="progress progress--sm">\
                        <div class="progress-bar '+ proressBarClass + '" role="progressbar" style="width: ' + valueNowInPercent + '%;" aria-valuenow="' + valueNow + '" aria-valuemin="0" aria-valuemax="' + valueMax + '"></div>\
                    </div>\
                </div>';


                    return output;
                };

                var getNormalProgressBar = function (title, valueNow, valueMax) {
                    var valueNowInPercent = ((valueNow / valueMax) * 100).toFixed(2);

                    var proressBarClass =
                        valueNowInPercent == 0 ? ' ' :
                            valueNowInPercent < 50 ? ' bg-danger' :
                                valueNowInPercent < 70 ? ' bg-warning' :
                                    valueNowInPercent <= 100 ? ' bg-success' : null;

                    var statsClass =
                        valueNowInPercent == 0 ? ' ' :
                            valueNowInPercent < 50 ? ' kt-font-danger' :
                                valueNowInPercent < 70 ? ' kt-font-warning' :
                                    valueNowInPercent <= 100 ? ' kt-font-success' : null;


                    var output = '';
                    output += '\
                    <span class="kt-widget12__desc" >'+ title + '</span>\
                    <div class="kt-widget12__progress">\
                        <div class="progress kt-progress--sm">\
							 <div class="progress-bar '+ proressBarClass + '" role="progressbar" style="width: ' + valueNowInPercent + '%;" aria-valuenow="' + valueNow + '" aria-valuemin="0" aria-valuemax="' + valueMax + '"></div>\
                        </div>\
                        <span class="kt-widget12__stat'+ statsClass + '">\
                            '+ valueNowInPercent + '%\
                        </span>\
                    </div>';

                    return output;
                };

                var getShopRankDataOutput = function (shopRankData) {

                    var fullNames = Object.keys(shopRankData);
                    var amounts = Object.values(shopRankData);

                    var output = '';
                    output += '\
                    <div class="kt-widget4">';
                    for (var i = 0; i < fullNames.length; i++) {
                        output += '\
                    <div class="kt-widget4__item">\
                        <div class="kt-widget4__pic kt-widget4__pic--pic">\
                            <img src="assets/media/users/default.jpg" alt="">\
                        </div>\
                        <div class="kt-widget4__info">\
                            <a href="#" class="kt-widget4__username">\
                               '+ fullNames[i] + '\
                            </a>\
                            <p class="kt-widget4__text">\
                               测试商户\
                            </p>\
                        </div>\
                        <span class="kt-widget4__number kt-font-success">'+ amounts[i] + '¥</span>\
                   </div>\
                ';
                    }

                    output += '</div>';

                    return output;
                };

                var getTraderRankDataOutput = function (traderRankData) {

                    var fullNames = Object.keys(traderRankData);
                    var amounts = Object.values(traderRankData);

                    var output = '';
                    output += '\
                    <div class="kt-widget4">';
                    for (var i = 0; i < fullNames.length; i++) {
                        output += '\
                    <div class="kt-widget4__item">\
                        <div class="kt-widget4__pic kt-widget4__pic--pic">\
                            <img src="assets/media/users/default.jpg" alt="">\
                        </div>\
                        <div class="kt-widget4__info">\
                            <a href="#" class="kt-widget4__username">\
                               '+ fullNames[i] + '\
                            </a>\
                            <p class="kt-widget4__text">\
                               测试交易员\
                            </p>\
                        </div>\
                        <span class="kt-widget4__number kt-font-success">'+ amounts[i] + '¥</span>\
                   </div>\
                ';
                    }

                    output += '</div>';

                    return output;
                };


                $('#statistic_total_platform_profit').html(orderStatistic.TotalPlatformProfit + '¥');
                $('#statistic_total_shop_agent_profit').html(orderStatistic.TotalShopAgentProfit + '¥');
                $('#statistic_total_trade_user_profit').html(orderStatistic.TotalTradeUserProfit + '¥');
                $('#statistic_total_shop_return').html(orderStatistic.TotalShopReturn + '¥');

                $('#statistic_alipay_barcode_success_rate').html(
                    getSuccessRateProgressBar(
                        '支付宝个码',
                        orderStatistic.AlipayBarcodeSuccessRate * 100,
                        orderStatistic.AlipayBarcodeOrderSuccessCount + '/' + orderStatistic.AlipayBarcodeOrderCount
                    ));
                $('#statistic_alipay_merchant_success_rate').html(
                    getSuccessRateProgressBar(
                        '支付宝原生码',
                        orderStatistic.AlipayMerchantSuccessRate * 100,
                        orderStatistic.AlipayMerchantOrderSuccessCount + '/' + orderStatistic.AlipayMerchantOrderCount
                    ));
                $('#statistic_alipay_transaction_success_rate').html(
                    getSuccessRateProgressBar(
                        '支付宝转账',
                        orderStatistic.AlipayTransactionSuccessRate * 100,
                        orderStatistic.AlipayTransactionOrderSuccessCount + '/' + orderStatistic.AlipayTransactionOrderCount
                    ));
                $('#statistic_alipay_bank_success_rate').html(
                    getSuccessRateProgressBar(
                        '支付宝转银行',
                        orderStatistic.AlipayBankSuccessRate * 100,
                        orderStatistic.AlipayBankOrderSuccessCount + '/' + orderStatistic.AlipayBankOrderCount
                    ));
                $('#statistic_alipay_envelop_success_rate').html(
                    getSuccessRateProgressBar(
                        '支付宝红包',
                        orderStatistic.AlipayEnvelopSuccessRate * 100,
                        orderStatistic.AlipayEnvelopOrderSuccessCount + '/' + orderStatistic.AlipayEnvelopOrderCount
                    ));
                $('#statistic_wechat_barcode_success_rate').html(
                    getSuccessRateProgressBar(
                        '微信个码',
                        orderStatistic.WechatBarcodeSuccessRate * 100,
                        orderStatistic.WechatBarcodeOrderSuccessCount + '/' + orderStatistic.WechatBarcodeOrderCount
                    ));

                $('#statistic_order_total_amount').html(orderStatistic.OrderTotalAmount + '¥');
                $('#statistic_order_total_success_amount').html(orderStatistic.OrderTotalSuccessAmount + '¥');
                $('#statistic_order_daily_average_success_amount').html(orderStatistic.OrderDailyAverageSuccessAmount + '¥');
                $('#statistic_order_average_revenue_rate').html(
                    getNormalProgressBar('每单平均收益率', orderStatistic.OrderAverageRevenueRate * 100, 100));
                $('#statistic_order_success_rate').html(
                    getNormalProgressBar('成功率', orderStatistic.OrderSuccessRate * 100, 100));
                $('#statistic_shop_order_amount_rank_data_day').html(
                    getShopRankDataOutput(orderStatistic.ShopRankData));
                $('#statistic_shop_order_amount_rank_data_month').html(
                    getShopRankDataOutput(orderStatistic.ShopRankData));
                $('#statistic_trader_order_amount_rank_data_day').html(
                    getTraderRankDataOutput(orderStatistic.TraderRankData));
                $('#statistic_trader_order_amount_rank_data_month').html(
                    getTraderRankDataOutput(orderStatistic.TraderRankData));


                var container = KTUtil.getByID('kt_chart_order_statistics');
                if (!container) {
                    return;
                }

                //Clear inside elements.
                $('#kt_chart_order_statistics').html('');


                var HOURS = Object.keys(orderStatistic.OrderChartData);
                var DATA = Object.values(orderStatistic.OrderChartData);
                HOURS.forEach(function (part, index, theArray) {
                    theArray[index] = theArray[index] + "点";
                });
                /*DATA.forEach(function (part, index, theArray) {
                    theArray[index] = theArray[index] + "¥";
                });*/

                var maxParam = Math.max.apply(false, DATA);
                var stepSizeParam = Math.round(maxParam / 5);
                if (String(stepSizeParam).length > 1) {
                    var d = Math.pow(10, String(stepSizeParam).length - 1);
                    stepSizeParam = Math.ceil(stepSizeParam / d) * d;
                }
                console.log(stepSizeParam);
                var color = Chart.helpers.color;
                var barChartData = {
                    labels: HOURS,
                    datasets: [
                        {
                            fill: true,
                            //borderWidth: 0,
                            backgroundColor: color(KTApp.getStateColor('brand')).alpha(0.6).rgbString(),
                            borderColor: color(KTApp.getStateColor('brand')).alpha(0).rgbString(),

                            pointHoverRadius: 4,
                            pointHoverBorderWidth: 12,
                            pointBackgroundColor: Chart.helpers.color('#000000').alpha(0).rgbString(),
                            pointBorderColor: Chart.helpers.color('#000000').alpha(0).rgbString(),
                            pointHoverBackgroundColor: KTApp.getStateColor('brand'),
                            pointHoverBorderColor: Chart.helpers.color('#000000').alpha(0.1).rgbString(),

                            data: DATA
                        }
                    ]
                };
                var ctx = container.getContext('2d');
                var chart = new Chart(ctx, {
                    type: 'line',
                    data: barChartData,
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        legend: false,
                        scales: {
                            xAxes: [{
                                categoryPercentage: 0.35,
                                barPercentage: 0.70,
                                display: true,
                                scaleLabel: {
                                    display: false,
                                    labelString: 'Month'
                                },
                                gridLines: false,
                                ticks: {
                                    display: true,
                                    beginAtZero: true,
                                    fontColor: KTApp.getBaseColor('shape', 3),
                                    fontSize: 13,
                                    padding: 10
                                }
                            }],
                            yAxes: [{
                                categoryPercentage: 0.35,
                                barPercentage: 0.70,
                                display: true,
                                scaleLabel: {
                                    display: false,
                                    labelString: 'Value'
                                },
                                gridLines: {
                                    color: KTApp.getBaseColor('shape', 2),
                                    drawBorder: false,
                                    offsetGridLines: false,
                                    drawTicks: false,
                                    borderDash: [3, 4],
                                    zeroLineWidth: 1,
                                    zeroLineColor: KTApp.getBaseColor('shape', 2),
                                    zeroLineBorderDash: [3, 4]
                                },
                                ticks: {
                                    max: maxParam,
                                    stepSize: stepSizeParam,
                                    display: true,
                                    beginAtZero: true,
                                    fontColor: KTApp.getBaseColor('shape', 3),
                                    fontSize: 13,
                                    padding: 10
                                }
                            }]
                        },
                        title: {
                            display: false
                        },
                        hover: {
                            mode: 'index'
                        },
                        tooltips: {
                            enabled: true,
                            intersect: false,
                            mode: 'nearest',
                            bodySpacing: 5,
                            yPadding: 10,
                            xPadding: 10,
                            caretPadding: 0,
                            displayColors: false,
                            backgroundColor: KTApp.getStateColor('brand'),
                            titleFontColor: '#ffffff',
                            cornerRadius: 4,
                            footerSpacing: 0,
                            titleSpacing: 0
                        },
                        layout: {
                            padding: {
                                left: 0,
                                right: 0,
                                top: 5,
                                bottom: 5
                            }
                        }
                    }
                });
            },
            error: function (response) {
                console.log(response);
            }
        })
    }

    var refreshData = function (orderStatistic) {

        var getSuccessRateProgressBar = function (title, valueNow, text) {
            var valueMax = 100;
            var valueNowInPercent = (valueNow / valueMax).toFixed(2) * 100;

            var proressBarClass =
                valueNowInPercent == 0 ? ' ' :
                    valueNowInPercent < 50 ? ' bg-danger' :
                        valueNowInPercent < 70 ? ' bg-warning' :
                            valueNowInPercent <= 100 ? ' bg-success' : null;

            var statsClass =
                valueNowInPercent == 0 ? ' ' :
                    valueNowInPercent < 50 ? ' kt-font-danger' :
                        valueNowInPercent < 70 ? ' kt-font-warning' :
                            valueNowInPercent <= 100 ? ' kt-font-success' : null;

            var output = '';
            output +=
                '<div class="kt-widget24">\
                    <div class="kt-widget24__details">\
                        <div class="kt-widget24__info">\
                            <h4 class="kt-widget24__title">\
                                '+ title + '\
                            </h4>\
                            <span class="kt-widget24__desc">\
                                成功率 '+ text + '\
                            </span>\
                        </div>\
                        <span class="kt-widget24__stats '+ statsClass + '">\
                            '+ valueNowInPercent + '%\
                        </span>\
                    </div>\
                    <div class="progress progress--sm">\
                        <div class="progress-bar '+ proressBarClass + '" role="progressbar" style="width: ' + valueNowInPercent + '%;" aria-valuenow="' + valueNow + '" aria-valuemin="0" aria-valuemax="' + valueMax + '"></div>\
                    </div>\
                </div>';


            return output;
        };

        var getNormalProgressBar = function (title, valueNow, valueMax) {
            var valueNowInPercent = (valueNow / valueMax).toFixed(2) * 100;

            var proressBarClass =
                valueNowInPercent == 0 ? ' ' :
                    valueNowInPercent < 50 ? ' bg-danger' :
                        valueNowInPercent < 70 ? ' bg-warning' :
                            valueNowInPercent <= 100 ? ' bg-success' : null;

            var statsClass =
                valueNowInPercent == 0 ? ' ' :
                    valueNowInPercent < 50 ? ' kt-font-danger' :
                        valueNowInPercent < 70 ? ' kt-font-warning' :
                            valueNowInPercent <= 100 ? ' kt-font-success' : null;


            var output = '';
            output += '\
                    <span class="kt-widget12__desc" >'+ title + '</span>\
                    <div class="kt-widget12__progress">\
                        <div class="progress kt-progress--sm">\
							 <div class="progress-bar '+ proressBarClass + '" role="progressbar" style="width: ' + valueNowInPercent + '%;" aria-valuenow="' + valueNow + '" aria-valuemin="0" aria-valuemax="' + valueMax + '"></div>\
                        </div>\
                        <span class="kt-widget12__stat'+ statsClass + '">\
                            '+ valueNowInPercent + '%\
                        </span>\
                    </div>';

            return output;
        };

        var getShopRankDataOutput = function (shopRankData) {

            var fullNames = Object.keys(shopRankData);
            var amounts = Object.values(shopRankData);

            var output = '';
            output += '\
                    <div class="kt-widget4">';
            for (var i = 0; i < fullNames.length; i++) {
                output += '\
                    <div class="kt-widget4__item">\
                        <div class="kt-widget4__pic kt-widget4__pic--pic">\
                            <img src="assets/media/users/default.jpg" alt="">\
                        </div>\
                        <div class="kt-widget4__info">\
                            <a href="#" class="kt-widget4__username">\
                               '+ fullNames[i] + '\
                            </a>\
                            <p class="kt-widget4__text">\
                               测试商户\
                            </p>\
                        </div>\
                        <span class="kt-widget4__number kt-font-success">'+ amounts[i] + '¥</span>\
                   </div>\
                ';
            }

            output += '</div>';

            return output;
        };

        var getTraderRankDataOutput = function (traderRankData) {

            var fullNames = Object.keys(traderRankData);
            var amounts = Object.values(traderRankData);

            var output = '';
            output += '\
                    <div class="kt-widget4">';
            for (var i = 0; i < fullNames.length; i++) {
                output += '\
                    <div class="kt-widget4__item">\
                        <div class="kt-widget4__pic kt-widget4__pic--pic">\
                            <img src="assets/media/users/default.jpg" alt="">\
                        </div>\
                        <div class="kt-widget4__info">\
                            <a href="#" class="kt-widget4__username">\
                               '+ fullNames[i] + '\
                            </a>\
                            <p class="kt-widget4__text">\
                               测试交易员\
                            </p>\
                        </div>\
                        <span class="kt-widget4__number kt-font-success">'+ amounts[i] + '¥</span>\
                   </div>\
                ';
            }

            output += '</div>';

            return output;
        };

        $('#statistic_total_platform_profit').fadeOut('fast').html(orderStatistic.totalPlatformProfit + '¥').fadeIn('fast');
        $('#statistic_total_shop_agent_profit').fadeOut('fast').html(orderStatistic.totalShopAgentProfit + '¥').fadeIn('fast');
        $('#statistic_total_trade_user_profit').fadeOut('fast').html(orderStatistic.totalTradeUserProfit + '¥').fadeIn('fast');
        $('#statistic_total_shop_return').fadeOut('fast').html(orderStatistic.totalShopReturn + '¥').fadeIn('fast');

        $('#statistic_alipay_barcode_success_rate').fadeOut('fast').html(
            getSuccessRateProgressBar(
                '支付宝个码',
                orderStatistic.alipayBarcodeSuccessRate * 100,
                orderStatistic.alipayBarcodeOrderSuccessCount + '/' + orderStatistic.alipayBarcodeOrderCount
            )).fadeIn('fast');
        $('#statistic_alipay_merchant_success_rate').fadeOut('fast').html(
            getSuccessRateProgressBar(
                '支付宝原生码',
                orderStatistic.alipayMerchantSuccessRate * 100,
                orderStatistic.alipayMerchantOrderSuccessCount + '/' + orderStatistic.alipayMerchantOrderCount
            )).fadeIn('fast');
        $('#statistic_alipay_transaction_success_rate').fadeOut('fast').html(
            getSuccessRateProgressBar(
                '支付宝转账',
                orderStatistic.alipayTransactionSuccessRate * 100,
                orderStatistic.alipayTransactionOrderSuccessCount + '/' + orderStatistic.alipayTransactionOrderCount
            )).fadeIn('fast');
        $('#statistic_alipay_bank_success_rate').fadeOut('fast').html(
            getSuccessRateProgressBar(
                '支付宝转银行',
                orderStatistic.alipayBankSuccessRate * 100,
                orderStatistic.alipayBankOrderSuccessCount + '/' + orderStatistic.alipayBankOrderCount
            )).fadeIn('fast');
        $('#statistic_alipay_envelop_success_rate').fadeOut('fast').html(
            getSuccessRateProgressBar(
                '支付宝红包',
                orderStatistic.alipayEnvelopSuccessRate * 100,
                orderStatistic.alipayEnvelopOrderSuccessCount + '/' + orderStatistic.alipayEnvelopOrderCount
            )).fadeIn('fast');
        $('#statistic_wechat_barcode_success_rate').fadeOut('fast').html(
            getSuccessRateProgressBar(
                '微信个码',
                orderStatistic.wechatBarcodeSuccessRate * 100,
                orderStatistic.wechatBarcodeOrderSuccessCount + '/' + orderStatistic.wechatBarcodeOrderCount
            )).fadeIn('fast');

        $('#statistic_order_total_amount').fadeOut('fast').html(orderStatistic.orderTotalAmount + '¥').fadeIn('fast');
        $('#statistic_order_total_success_amount').fadeOut('fast').html(orderStatistic.orderTotalSuccessAmount + '¥').fadeIn('fast');
        $('#statistic_order_daily_average_success_amount').fadeOut('fast').html(orderStatistic.orderDailyAverageSuccessAmount + '¥').fadeIn('fast');
        $('#statistic_order_average_revenue_rate').fadeOut('fast').html(
            getNormalProgressBar('每单平均收益率', orderStatistic.orderAverageRevenueRate * 100, 100)).fadeIn('fast');
        $('#statistic_order_success_rate').fadeOut('fast').html(
            getNormalProgressBar('成功率', orderStatistic.orderSuccessRate * 100, 100)).fadeIn('fast');
        $('#statistic_shop_order_amount_rank_data_day').fadeOut('fast').html(
            getShopRankDataOutput(orderStatistic.shopRankData)).fadeIn('fast');
        $('#statistic_shop_order_amount_rank_data_month').fadeOut('fast').html(
            getShopRankDataOutput(orderStatistic.shopRankData)).fadeIn('fast');
        $('#statistic_trader_order_amount_rank_data_day').fadeOut('fast').html(
            getTraderRankDataOutput(orderStatistic.traderRankData)).fadeIn('fast');
        $('#statistic_trader_order_amount_rank_data_month').fadeOut('fast').html(
            getTraderRankDataOutput(orderStatistic.traderRankData)).fadeIn('fast');


        var container = KTUtil.getByID('kt_chart_order_statistics');

        if (!container) {
            return;
        }
        //Clear inside elements.
        $('#kt_chart_order_statistics').fadeOut('fast').html('').fadeIn('fast');

        var HOURS = Object.keys(orderStatistic.orderChartData);
        var DATA = Object.values(orderStatistic.orderChartData);
        HOURS.forEach(function (part, index, theArray) {
            theArray[index] = theArray[index] + "点";
        });
        /*DATA.forEach(function (part, index, theArray) {
            theArray[index] = theArray[index] + "¥";
        });*/


        var color = Chart.helpers.color;
        var barChartData = {
            labels: HOURS,
            datasets: [
                {
                    fill: true,
                    //borderWidth: 0,
                    backgroundColor: color(KTApp.getStateColor('brand')).alpha(0.6).rgbString(),
                    borderColor: color(KTApp.getStateColor('brand')).alpha(0).rgbString(),

                    pointHoverRadius: 4,
                    pointHoverBorderWidth: 12,
                    pointBackgroundColor: Chart.helpers.color('#000000').alpha(0).rgbString(),
                    pointBorderColor: Chart.helpers.color('#000000').alpha(0).rgbString(),
                    pointHoverBackgroundColor: KTApp.getStateColor('brand'),
                    pointHoverBorderColor: Chart.helpers.color('#000000').alpha(0.1).rgbString(),

                    data: DATA
                }
            ]
        };
        var ctx = container.getContext('2d');
        var chart = new Chart(ctx, {
            type: 'line',
            data: barChartData,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: false,
                scales: {
                    xAxes: [{
                        categoryPercentage: 0.35,
                        barPercentage: 0.70,
                        display: true,
                        scaleLabel: {
                            display: false,
                            labelString: 'Month'
                        },
                        gridLines: false,
                        ticks: {
                            display: true,
                            beginAtZero: true,
                            fontColor: KTApp.getBaseColor('shape', 3),
                            fontSize: 13,
                            padding: 10
                        }
                    }],
                    yAxes: [{
                        categoryPercentage: 0.35,
                        barPercentage: 0.70,
                        display: true,
                        scaleLabel: {
                            display: false,
                            labelString: 'Value'
                        },
                        gridLines: {
                            color: KTApp.getBaseColor('shape', 2),
                            drawBorder: false,
                            offsetGridLines: false,
                            drawTicks: false,
                            borderDash: [3, 4],
                            zeroLineWidth: 1,
                            zeroLineColor: KTApp.getBaseColor('shape', 2),
                            zeroLineBorderDash: [3, 4]
                        },
                        ticks: {
                            max: Math.max.apply(false, DATA),
                            stepSize: 10000,
                            display: true,
                            beginAtZero: true,
                            fontColor: KTApp.getBaseColor('shape', 3),
                            fontSize: 13,
                            padding: 10
                        }
                    }]
                },
                title: {
                    display: false
                },
                hover: {
                    mode: 'index'
                },
                tooltips: {
                    enabled: true,
                    intersect: false,
                    mode: 'nearest',
                    bodySpacing: 5,
                    yPadding: 10,
                    xPadding: 10,
                    caretPadding: 0,
                    displayColors: false,
                    backgroundColor: KTApp.getStateColor('brand'),
                    titleFontColor: '#ffffff',
                    cornerRadius: 4,
                    footerSpacing: 0,
                    titleSpacing: 0
                },
                layout: {
                    padding: {
                        left: 0,
                        right: 0,
                        top: 5,
                        bottom: 5
                    }
                }
            }
        });
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

                $('#statistic_search_from').val(start.format('MM/DD/YYYY'));
                $('#statistic_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
            } else if (label == '昨日') {
                title = '昨日:';
                range = start.format('MM/DD/YYYY');
                $('#statistic_search_from').val(start.format('MM/DD/YYYY'));
                $('#statistic_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
            } else {
                range = start.format('MM/DD/YYYY') + ' - ' + end.format('MM/DD/YYYY');
                $('#statistic_search_from').val(start.format('MM/DD/YYYY'));
                $('#statistic_search_to').val(end.add(24, 'hours').format('MM/DD/YYYY'));
            }
            $('#kt_dashboard_daterangepicker_date').html(range);
            updateData();
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


        function updateData() {
            var query = $('#statistic_search_query').val();
            var from = $('#statistic_search_from').val();
            var to = $('#statistic_search_to').val();

            //Parse to utc time.
            var from = new Date(from).toLocaleString('en-US', { timeZone: 'UTC' });
            var to = new Date(to).toLocaleString('en-US', { timeZone: 'UTC' });

            orderStatistics(query, from, to);
        }

        $(document).on('click', '[data-search-action="action-submit"]', function () {
            var btn = $(this);
            KTApp.progress(btn);

            var query = $('#statistic_search_query').val();
            var from = $('#statistic_search_from').val();
            var to = $('#statistic_search_to').val();

            //Parse to utc time.
            var from = new Date(from).toLocaleString('en-US', { timeZone: 'UTC' });
            var to = new Date(to).toLocaleString('en-US', { timeZone: 'UTC' });

            orderStatistics(query, from, to);

            KTApp.unprogress(btn);
        });


    }

    //SignalR data refresh.
    var dataRefreshInit = function () {
        var connection = new signalR.HubConnectionBuilder().withUrl("/orderStatisticHub").build();
        connection.on("RefreshData", function (message) {
            refreshData(message[0]);
            //broadcastMessage('有商户申请提单');
        });

        $('#data_auto_refresh').on('change', function () {
            if ($('#data_auto_refresh:checked').val()) {

                connection.start().then(function () {
                    console.log("SingalR Connected.");
                }).catch(function (err) {
                    return console.error(err.toString());
                });
            }
            else {
                connection.stop();
            }
        });

    }

    return {
        // Init demos
        init: function () {
            // init charts
            orderStatistics();

            // init daterangepicker
            daterangepickerInit();

            // init signalR dataRefresh
            dataRefreshInit();

            // demo loading
            var loading = new KTDialog({ 'type': 'loader', 'placement': 'top center', 'message': '载入中 ...' });
            loading.show();

            setTimeout(function () {
                loading.hide();
            }, 3000);
        }
    };
}();

// Class initialization on page load
jQuery(document).ready(function () {
    KTDashboard.init();
});