using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headstone.Service;
using Headstone.Models.Events.Order;

namespace Headstone.Tests.Commands
{
    [TestClass]
    public class TransactionCommands
    {
        private TransactionService transactionService = new TransactionService();

        [TestMethod]
        public void CreateTransaction()
        {
            var transactionCommand = new TransactionCreated()
            {
                UserID = 1010,
                FirstName = "Damla",
                LastName = "Koksal",
                OrderID = 40,
                CardNumber = "1234123412341234",
                TotalPrice = 50
            };

            var response = transactionService.CreateTransaction(transactionCommand);
            Assert.AreEqual(response.Type, Common.ServiceResponseTypes.Success);
        }
    }
}
