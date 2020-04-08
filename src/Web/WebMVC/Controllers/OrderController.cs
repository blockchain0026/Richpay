using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using Util.Tools.QrCode;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.RunningAccounts;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels.Json;
using WebMVC.ViewModels.OrderViewModels;

namespace WebMVC.Controllers
{
    [Authorize(Policy = Permissions.Additional.Reviewed)]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderQueries _orderQueries;
        private readonly IRunningAccountQueries _runningAccountQueries;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Util.Tools.QrCode.IQrCodeService _qrCodeService;

        public OrderController(IOrderService orderService, IOrderQueries orderQueries, IRunningAccountQueries runningAccountQueries, UserManager<ApplicationUser> userManager, Util.Tools.QrCode.IQrCodeService qrCodeService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
        }

        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.View)]
        public IActionResult Index()
        {
            string userBaseRole = string.Empty;

            if (User.IsInRole(Roles.Manager))
            {
                userBaseRole = Roles.Manager;
            }
            else if (User.IsInRole(Roles.TraderAgent))
            {
                userBaseRole = Roles.TraderAgent;
            }
            else if (User.IsInRole(Roles.Trader))
            {
                userBaseRole = Roles.Trader;
            }


            var vm = new IndexViewModel
            {
                UserId = User.GetUserId<string>(),
                UserBaseRole = userBaseRole
            };

            return View(vm);
        }


        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.View)]
        [HttpPost]
        public async Task<IActionResult> Search([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "QrCode");
            }
            var contextUserId = User.GetUserId<string>();

            if (!DateTime.TryParse(query.DateFrom, out DateTime dateFrom))
            {
                //dateFrom = DateTime.Now.AddDays(-1);
                dateFrom = DateTime.Now;
            }
            if (!DateTime.TryParse(query.DateTo, out DateTime dateTo))
            {
                //dateTo = DateTime.Now;
                dateTo = DateTime.Now.AddDays(1);
            }

            var sumData = await _orderService.GetPlatformOrderEntrysTotalSumData(
                contextUserId,
                query.generalSearch ?? string.Empty,
                dateFrom,
                dateTo,
                query.OrderStatus ?? null,
                query.OrderPaymentChannel ?? null,
                query.OrderPaymentScheme ?? null
                );

            var orderEntries = await _orderService.GetPlatformOrderEntrys(
                pagination.page - 1,
                pagination.perpage,
                contextUserId,
                query.generalSearch ?? string.Empty,
                sort.field,
                dateFrom,
                dateTo,
                query.OrderStatus ?? null,
                query.OrderPaymentChannel ?? null,
                query.OrderPaymentScheme ?? null,
                sort.sort);

            var jsonResult = new KTDatatableResult<OrderEntry>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)sumData.TotalCount / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = sumData.TotalCount,
                    sort = "desc",
                    field = "OrderId"
                },
                data = orderEntries
            };

            return Json(jsonResult);
        }


        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.View)]
        [HttpGet]
        public async Task<IActionResult> OrderSumData(string query, string from, string to,
            string orderStatus, string orderPaymentChannel, string orderPaymentScheme)
        {
            if (query?.Length > 256 || from?.Length > 50 || to?.Length > 50
                || orderStatus?.Length > 50 || orderPaymentChannel?.Length > 50 || orderPaymentScheme?.Length > 50)
            {
                throw new InvalidOperationException("无效参数");
            }
            if (!DateTime.TryParse(from, out DateTime dateFrom))
            {
                //dateFrom = DateTime.Now.AddDays(-1);
                dateFrom = DateTime.Now;
            }
            if (!DateTime.TryParse(to, out DateTime dateTo))
            {
                //dateTo = DateTime.Now;
                dateTo = DateTime.Now.AddDays(1);
            }

            var sumData = await _orderService.GetPlatformOrderEntrysTotalSumData(
                User.GetUserId<string>(),
                query ?? null,
                dateFrom,
                dateTo,
                orderStatus ?? null,
                orderPaymentChannel ?? null,
                orderPaymentScheme ?? null
                );

            return Json(sumData);
        }


        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.View)]
        [HttpGet]
        public async Task<IActionResult> Statistic(string query, string from, string to)
        {
            try
            {
                if (query?.Length > 256 || from?.Length > 50 || to?.Length > 50)
                {
                    throw new InvalidOperationException("无效参数");
                }
                if (!DateTime.TryParse(from, out DateTime dateFrom))
                {
                    //dateFrom = DateTime.Now.AddDays(-1);
                    dateFrom = DateTime.Now;
                }
                if (!DateTime.TryParse(to, out DateTime dateTo))
                {
                    //dateTo = DateTime.Now;
                    dateTo = DateTime.Now.AddDays(1);
                }
                if (User.IsInRole(Roles.Manager))
                {
                    var statistic = await _orderQueries.GetOrderEntriesStatisticAsync(
                        query,
                        dateFrom,
                        dateTo
                        );

                    return Json(statistic);
                }
                else if (User.IsInRole(Roles.Shop))
                {
                    var statistic = await _orderQueries.GetOrderEntriesStatisticByShopIdAsync(
                        User.GetUserId<string>(),
                        query,
                        dateFrom,
                        dateTo
                        );

                    return Json(statistic);
                }
                else if (User.IsInRole(Roles.Trader))
                {
                    var statistic = await _orderQueries.GetOrderEntriesStatisticByTraderIdAsync(
                        User.GetUserId<string>(),
                        query,
                        dateFrom,
                        dateTo
                        );

                    return Json(statistic);
                }
                else
                {
                    throw new UnauthorizedAccessException("用户没有读取权限");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = Permissions.OrderManagement.RunningAccountRecords.View)]
        [HttpGet]
        public IActionResult RunningAccountRecords()
        {
            string userBaseRole = string.Empty;

            if (User.IsInRole(Roles.Manager))
            {
                userBaseRole = Roles.Manager;
            }
            else if (User.IsInRole(Roles.TraderAgent))
            {
                userBaseRole = Roles.TraderAgent;
            }
            else if (User.IsInRole(Roles.Trader))
            {
                userBaseRole = Roles.Trader;
            }

            var vm = new IndexViewModel
            {
                UserId = User.GetUserId<string>(),
                UserBaseRole = userBaseRole
            };

            return View(vm);
        }


        [Authorize(Policy = Permissions.OrderManagement.RunningAccountRecords.View)]
        [HttpGet]
        public async Task<IActionResult> RunningAccountRecordSumData(string userId, string query, string from, string to, string orderStatus)
        {
            if (query?.Length > 256 || from?.Length > 50 || to?.Length > 50 || orderStatus?.Length > 50)
            {
                throw new InvalidOperationException("无效参数");
            }
            if (!DateTime.TryParse(from, out DateTime dateFrom))
            {
                //dateFrom = DateTime.Now.AddDays(-1);
                dateFrom = DateTime.Now;
            }
            if (!DateTime.TryParse(to, out DateTime dateTo))
            {
                //dateTo = DateTime.Now;
                dateTo = DateTime.Now.AddDays(1);
            }
            var sumData = await _orderService.GetRunningAccountRecordsTotalSumDataByUserIdAsync(
                User.GetUserId<string>(),
                userId,
                query ?? null,
                dateFrom,
                dateTo,
                orderStatus
                );

            return Json(sumData);
        }


        [Authorize(Policy = Permissions.OrderManagement.RunningAccountRecords.View)]
        [HttpPost]
        public async Task<IActionResult> RunningAccountRecords([FromForm] KTPagination pagination, [FromForm] KTQuery query, [FromForm] KTSort sort)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Order");
            }
            var contextUserId = User.GetUserId<string>();
            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            if (!string.IsNullOrWhiteSpace(query.DateFrom))
            {
                dateFrom = DateTime.Parse(query.DateFrom);
            }
            if (!string.IsNullOrWhiteSpace(query.DateTo))
            {
                dateTo = DateTime.Parse(query.DateTo);
            }

            var sumData = await _orderService.GetRunningAccountRecordsTotalSumDataByUserIdAsync(
                contextUserId,
                query.userId ?? string.Empty,
                query.generalSearch ?? string.Empty,
                dateFrom,
                dateTo,
                query.OrderStatus ?? null
                );

            var runningAccountRecords = await _orderService.GetRunningAccountRecordsByUserIdAsync(
                contextUserId,
                query.userId ?? string.Empty,
                pagination.page - 1,
                pagination.perpage,
                query.generalSearch ?? string.Empty,
                sort.field,
                dateFrom,
                dateTo,
                query.OrderStatus ?? null,
                sort.sort);

            var jsonResult = new KTDatatableResult<RunningAccountRecord>
            {
                meta = new KTPagination
                {
                    page = pagination.page,
                    pages = (int)Math.Ceiling(((decimal)sumData.TotalCount / pagination.perpage)),
                    perpage = pagination.perpage,
                    total = (int)sumData.TotalCount,
                    sort = "desc",
                    field = "RunningAccountRecordId"
                },
                data = runningAccountRecords
            };

            return Json(jsonResult);
        }



        [Authorize(Policy = Permissions.OrderManagement.RunningAccountRecords.View)]
        [HttpGet]
        public async Task<IActionResult> RunningAccountRecordStatistic(string query, string from, string to)
        {
            try
            {
                if (query?.Length > 256 || from?.Length > 50 || to?.Length > 50)
                {
                    throw new InvalidOperationException("无效参数");
                }
                if (!DateTime.TryParse(from, out DateTime dateFrom))
                {
                    dateFrom = DateTime.Now.AddDays(-1);
                }
                if (!DateTime.TryParse(to, out DateTime dateTo))
                {
                    dateTo = DateTime.Now;
                }

                var userRole = string.Empty;
                if (User.IsInRole(Roles.ShopAgent))
                {
                    userRole = Roles.ShopAgent;
                }
                else if (User.IsInRole(Roles.TraderAgent))
                {
                    userRole = Roles.TraderAgent;
                }

                var statistic = await _runningAccountQueries.GetRunningAccountRecordsStatisticAsync(
                    User.GetUserId<string>(),
                    userRole,
                    query,
                    dateFrom,
                    dateTo
                    );

                return Json(statistic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }








        // POST: Order/CreateOrderToPlatform
        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.Create)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrderToPlatform([FromForm]CreateOrderToPlatformViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }

                await _orderService.CreateOrderToPlatform(
                    User.GetUserId<string>(),
                    vm.ShopId,
                    vm.ShopOrderId,
                    vm.ShopOrderAmount,
                    vm.ShopReturnUrl,
                    vm.ShopOkReturnUrl,
                    vm.OrderGatewayType
                    );

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Order/CreateConcurrencyOrderToPlatform
        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateConcurrencyOrderToPlatform([FromBody]CreateConcurrencyOrderToPlatformViewModel vm)
        {
            try
            {
                /*if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Request.");
                }*/
                string shopId;

                if (!string.IsNullOrEmpty(vm.ShopId))
                {
                    shopId = vm.ShopId;
                }
                else
                {
                    var shop = await _userManager.Users
                        .Where(u => u.UserName == vm.ShopUserName)
                        .Select(u => new
                        {
                            u.Id
                        })
                        .FirstOrDefaultAsync();
                    shopId = shop.Id;
                }


                var orderId = await _orderService.CreateOrderToPlatform(
                    User.GetUserId<string>(),
                    shopId,
                    vm.ShopOrderId,
                    vm.ShopOrderAmount,
                    "https://www.richpay168.com/",
                    "https://www.richpay168.com/",
                    vm.OrderGatewayType
                    );

                return Ok(new { success = true, orderId = orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Order/CreateTestOrderToPlatform
        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateTestOrderToPlatform([FromBody]CreateTestOrderToPlatformViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("无效参数。");
                }

                var localUrl = await _orderService.CreateTestOrderToPlatform(
                    User.GetUserId<string>(),
                    vm.OrderAmount,
                    vm.QrCodeId
                    );

                return Ok(new { success = true, localUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Order/DeleteAllTestOrders
        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.Create)]
        [HttpPost]
        public async Task<IActionResult> DeleteAllTestOrders()
        {
            try
            {
                await _orderService.DeleteAllTestOrders(
                    User.GetUserId<string>());

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Order/ConfirmPayment
        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.ConfirmPayment)]
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment([FromBody]int orderId)
        {
            try
            {
                //await _orderService.ConfirmOrderByTrackingNumber(orderTrackingNumber, User.GetUserId<string>());
                await _orderService.ConfirmOrderById(orderId, User.GetUserId<string>());

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: Order/ConfirmPaymentByAdmin
        [Authorize(Policy = Permissions.OrderManagement.PlatformOrders.ConfirmPayment)]
        [HttpPost]
        public async Task<IActionResult> ConfirmPaymentByAdmin([FromBody]int orderId)
        {
            try
            {
                await _orderService.ConfirmOrderByAdmin(orderId);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // GET: Order
        public ActionResult TestIndex(string scheme)
        {
            /*var paymentUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            paymentUrl += "/Order/Pay";*/
            var h5PaymentPageUrl = @"alipays://platformapi/startapp?appId=60000012&url=";
            var paymentPageUrl = "http://114.38.108.172/Order/Pay";
            var paymentPageUrlEncode = HttpUtility.UrlEncode(paymentPageUrl);
            h5PaymentPageUrl += paymentPageUrlEncode;

            byte[] qrCode = this._qrCodeService.CreateQrCode(h5PaymentPageUrl);
            /*if (scheme == "scantransaction")
            {
                var paymentCommand123Qr = @"alipayqr://platformapi/startapp?appId=20000123&actionType=scan&biz_data={""s"":""money"",""u"":""2088432193306889"",""a"":""1"",""m"":""OID: 你好123456""}";

                qrCode = this._qrCodeService.CreateQrCode(paymentCommand123Qr);
            }
            if (scheme == "scanwait")
            {
            }
            if (scheme == "scanbarcode")
            {
                var paymentUrl = "alipays://platformapi/startapp?appId=20000067&url=https://qr.alipay.com/fkx19487gzoxav9jcwzh1c7?t=1578893421957";
                qrCode = this._qrCodeService.CreateQrCode(paymentUrl);
            }*/


            var vm = new IndexViewModel
            {
                QrCodeImageData = "data:image/png;base64," + Convert.ToBase64String(qrCode),
                H5PaymentPageUrl = h5PaymentPageUrl
            };

            /*using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                var qrCode = qrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    //ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            
                }
            }*/
            return View(vm);
        }


        // GET: Order/Pay2
        public ActionResult Pay2()
        {
            /*var paymentUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            paymentUrl += "/Order/Pay";*/
            var paymentCommand123Qr = @"alipays://platformapi/startapp?appId=20000123&actionType=scan&biz_data={""s"":""money"",""u"":""2088732318934891"",""a"":""1"",""m"":""OID: 你好123456""}";

            var h5PaymentPageUrl = @"alipays://platformapi/startapp?appId=60000012&url=";
            var paymentPageUrl = "https://www.richpay168.com/Order/Pay";
            var paymentPageUrlEncode = HttpUtility.UrlEncode(paymentPageUrl);
            h5PaymentPageUrl += paymentPageUrlEncode;

            byte[] qrCode = this._qrCodeService.CreateQrCode(paymentCommand123Qr);


            var vm = new PayViewModel
            {
                QrCodeImageData = "data:image/png;base64," + Convert.ToBase64String(qrCode),
                //H5PaymentPageUrl = h5PaymentPageUrl
            };
            return View(vm);
        }


        [HttpGet]
        public IActionResult Pay()
        {
            //var paymentCommand = @"alipays://platformapi/startapp?appId=20000123&actionType=scan&biz_data={""s"":""money"",""u"":""2088732318934891"",""a"":""1"",""m"":""OID: 旋487""}";
            //var paymentCommand = @"alipays://platformapi/startapp?appId=20000123&actionType=scan&biz_data={""s"":""money"",""u"":""2088922432281861"",""a"":""1"",""m"":""OID: 你好123456""}";
            //var paymentCommand123s = @"alipays://platformapi/startapp?appId=20000123&actionType=scan&biz_data={""s"":""money"",""u"":""2088432193306889"",""a"":""1"",""m"":""OID: 你好123456""}";
            var paymentCommand123Qr = @"alipayqr://platformapi/startapp?appId=20000123&actionType=scan&biz_data={""s"":""money"",""u"":""2088432193306889"",""a"":""1"",""m"":""OID: 你好123456""}";
            var paymentCommand988s = @"alipays://platformapi/startapp?appId=09999988&actionType=toAccount&goBack=NO&amount=1&userId=2088432193306889&memo=你好123456";
            var envelopCommand = @"alipays://platformapi/startapp?appId=88886666&appLaunchMode=3&canSearch=false&chatLoginId=1&chatUserId=" + "2088432193306889" + "&chatUserName=123&chatUserType=1&entryMode=personalStage&prevBiz=chat&schemaMode=portalInside&target=personal&money=" + 1 + "&amount=" + 1 + "&remark=" + "你好123456";
            //var paymentCommand56 = @"alipays://platformapi/startapp?appId=20000056&amount=1&userId=2088432193306889&memo=你好123456";
            //var paymentCommand116 = @"alipay://platformapi/startapp?appId=20000116&amount=1&userId=2088432193306889&memo=你好123456";



            var vm = new PayViewModel
            {
                //H5PaymentCommand20000123S = paymentCommand123s,
                H5PaymentCommand20000123QR = paymentCommand123Qr,
                H5PaymentCommand09999988S = paymentCommand988s,

                H5EnvelopCommand = envelopCommand
            };

            return View(vm);
        }


    }
}