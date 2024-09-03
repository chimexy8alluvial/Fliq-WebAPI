using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Contracts.Payments
{
    public record PaymentResponse(
    bool Success,
    string? Message
);
}