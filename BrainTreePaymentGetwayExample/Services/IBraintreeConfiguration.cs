using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainTreePaymentGetwayExample.Services
{
    public interface IBraintreeConfiguration
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
