using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using BrainTreePaymentGetwayExample.Models;
using BrainTreePaymentGetwayExample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrainTreePaymentGetwayExample.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IBraintreeConfiguration _paymentConfig;
        private readonly PaymentExampleContext _context;
        public PaymentsController(IBraintreeConfiguration paymentConfig, PaymentExampleContext context)
        {
            _paymentConfig = paymentConfig;
            _context = context;

        }
           
           public static readonly TransactionStatus[] transactionSuccessStatuses =
                {
                TransactionStatus.AUTHORIZED,
                TransactionStatus.AUTHORIZING,
                TransactionStatus.SETTLED,
                TransactionStatus.SETTLING,
                TransactionStatus.SETTLEMENT_CONFIRMED,
                TransactionStatus.SETTLEMENT_PENDING,
                TransactionStatus.SUBMITTED_FOR_SETTLEMENT
            };

           

         public async Task< IActionResult> Index(int? id)
       {
            Checkout checkout = new Checkout();
            if (id == null)
            {
                return NotFound();
            }
            checkout.User = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (checkout.User == null)
            {
                return NotFound();
            }
            ViewBag.ClientToken = _paymentConfig.GetGateway().ClientToken.Generate();
            return View(checkout);
        }
       
           [ValidateAntiForgeryToken]
           [HttpPost]
            public async Task<IActionResult> Checkout(Checkout model)
            {
            if (ModelState.IsValid)
            {
                string paymentStatus = string.Empty;
                var gateway = _paymentConfig.GetGateway();

                var request = new TransactionRequest
                {
                    Amount =Convert.ToDecimal( model.Price),
                    PaymentMethodNonce = model.PaymentMethodNonce,
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                Result<Transaction> result = gateway.Transaction.Sale(request);
                if (result.IsSuccess())
                {
                    paymentStatus = "Succeded";
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AllPayments));
                }
                else
                {
                    string errorMessages = "";
                    foreach (ValidationError error in result.Errors.DeepAll())
                    {
                        errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                    }

                    paymentStatus = errorMessages;
                }
            }
           // TempData["msg"] = paymentStatus;
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> AllPayments()
        {
            var paymentExampleContext = _context.Checkout.Include(c => c.User);
            return View(await paymentExampleContext.ToListAsync());
        }
    }
}
