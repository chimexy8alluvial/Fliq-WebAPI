using Fliq.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPaymentRepository
    {
        void Add(Payment payment);

        void Update(Payment payment);
    }
}