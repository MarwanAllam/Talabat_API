using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Services.Contract
{
    public interface IPaymentService
    {
        //Fun To Create Or Update Payment Intent
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
    }
}
