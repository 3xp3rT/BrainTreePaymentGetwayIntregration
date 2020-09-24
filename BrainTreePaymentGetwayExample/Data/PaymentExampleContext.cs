using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BrainTreePaymentGetwayExample.Models;

    public class PaymentExampleContext : DbContext
    {
        public PaymentExampleContext (DbContextOptions<PaymentExampleContext> options)
            : base(options)
        {
        }

        public DbSet<BrainTreePaymentGetwayExample.Models.User> User { get; set; }

        public DbSet<BrainTreePaymentGetwayExample.Models.Checkout> Checkout { get; set; }
    }
