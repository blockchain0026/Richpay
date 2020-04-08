using MediatR;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class QrCodeApprovedDomainEvent : INotification
    {
        public QrCodeApprovedDomainEvent(QrCode qrCode, bool isApproved, Admin admin)
        {
            QrCode = qrCode;
            IsApproved = isApproved;
            Admin = admin;
        }

        public QrCode QrCode { get; set; }
        public bool IsApproved { get; set; }
        public Admin Admin { get; set; }
    }
}
