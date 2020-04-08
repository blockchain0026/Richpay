using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Model.Orders;
using Ordering.Infrastructure;
using Pairing.Infrastructure;
using WebMVC.Infrastructure.Services;
using WebMVC.ViewModels.PayViewModels;

namespace WebMVC.Controllers
{
    public class PayController : Controller
    {
        private readonly OrderingContext _orderingContext;
        private readonly PairingContext _pairingContext;
        private readonly Util.Tools.QrCode.IQrCodeService _qrCodeService;

        public PayController(OrderingContext orderingContext, PairingContext pairingContext, Util.Tools.QrCode.IQrCodeService qrCodeService)
        {
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            _pairingContext = pairingContext ?? throw new ArgumentNullException(nameof(pairingContext));
            _qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
        }

        // GET: Pay
        public async Task<IActionResult> Index(string orderTrackingNumber = null)
        {
            if (string.IsNullOrWhiteSpace(orderTrackingNumber))
            {
                return NotFound();
            }

            //Get Order that is not expired and is awaiting payment.
            var order = await _orderingContext.Orders
                .AsNoTracking()
                .Include(o => o.OrderStatus)
                .Include(o => o.PayeeInfo)
                .Where(o => o.OrderStatus.Id == OrderStatus.AwaitingPayment.Id
                && !o.IsExpired
                && o.TrackingNumber == orderTrackingNumber)
                .FirstOrDefaultAsync();


            if (order is null)
            {
                return NotFound();
            }

            //Get Qr Code.
            var qrCode = await _pairingContext.QrCodes
                .AsNoTracking()
                .Where(q => q.Id == order.PayeeInfo.QrCodeId)
                .FirstOrDefaultAsync();

            if (qrCode is null)
            {
                return NotFound();
            }

            //Get Payment Command.
            var paymentCommand = qrCode.GeneratePaymentCommand(order.Amount);

            //Generate QR Code.
            byte[] qrCodeData = this._qrCodeService.CreateQrCode(paymentCommand);
            var qrCodeImageData = "data:image/png;base64," + Convert.ToBase64String(qrCodeData);


            //Build View Model.
            var vm = new IndexViewModel
            {
                Amount = (int)order.Amount,
                TimeLeft = (int)order.ExpirationTimeInSeconds - (int)(DateTime.UtcNow - order.DateCreated).TotalSeconds,
                PaymentCommand = paymentCommand,
                OrderTrackingNumber = order.TrackingNumber,
                QrCodeImageData = qrCodeImageData
            };

            return View(vm);
        }

        // GET: Pay
        public async Task<IActionResult> CheckOrderStatus(string orderTrackingNumber)
        {
            if (string.IsNullOrWhiteSpace(orderTrackingNumber))
            {
                return NotFound();
            }

            //Get Order.
            var order = await _orderingContext.Orders
                .AsNoTracking()
                .Include(o => o.OrderStatus)
                .Include(o => o.PayeeInfo)
                .Include(o => o.ShopInfo)
                .Where(o => o.TrackingNumber == orderTrackingNumber)
                .FirstOrDefaultAsync();


            if (order is null)
            {
                return BadRequest("查无订单");
            }

            if (order.OrderStatus.Id == OrderStatus.Success.Id)
            {
                return Ok(new { success = true, status = 0, return_url = order.ShopInfo.ShopReturnUrl });
            }
            else
            {
                return Ok(new { success = true, status = 1 });
            }
        }

    }
}