using BankingSolution.Controllers;
using BankingSolution.Models;
using BankingSolution.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TransactionControllerTests
    {
        private readonly TransactionController transactionController;
        private readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        public TransactionControllerTests()
        {
            transactionController = new TransactionController(mockAccountService.Object);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            transactionController.Dispose();
        }

        [Test] 
        public async Task DepositAsync_ShouldReturn404_IfUserNotFound()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 1,
                RecipientId = 2,
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(false);

            var result = await transactionController.DepositAsync(transactions);

            Assert.NotNull(result);
            Assert.That(404, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task DepositAsync_ShouldReturn200()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 1,
                RecipientId = 2,
            };
            var acc = new Account
            {
                Id = 1,
                Balance = 1,
                Username = "test"
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(acc);

            var result = await transactionController.DepositAsync(transactions);

            Assert.NotNull(result);
            Assert.That(200, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task WithdrawAsync_ShouldReturn404_IfUserNotFound()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 1,
                RecipientId = 2,
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(false);

            var result = await transactionController.WithdrawAsync(transactions);

            Assert.NotNull(result);
            Assert.That(404, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task WithdrawAsync_ShouldReturn400_IfInsufficientFunds()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 100,
                RecipientId = 2,
            };
            var acc = new Account
            {
                Id = 1,
                Balance = 0,
                Username = "test"
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(acc);

            var result = await transactionController.WithdrawAsync(transactions);

            Assert.NotNull(result);
            Assert.That(400, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task WithdrawAsync_ShouldReturn200()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 1,
                RecipientId = 2,
            };
            var acc = new Account
            {
                Id = 1,
                Balance = 1,
                Username = "test"
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(acc);

            var result = await transactionController.WithdrawAsync(transactions);

            Assert.NotNull(result);
            Assert.That(200, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task TransferAsync_ShouldReturn404_IfUserNotFound()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 1,
                RecipientId = 2,
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(false);

            var result = await transactionController.TransferAsync(transactions);

            Assert.NotNull(result);
            Assert.That(404, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task TransferAsync_ShouldReturn400_IfSenderAndRecipientIdsSame()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 100,
                RecipientId = 1,
            };

            var result = await transactionController.TransferAsync(transactions);

            Assert.NotNull(result);
            Assert.That(400, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode)!);
        }

        [Test]
        public async Task TransferAsync_ShouldReturn400_IfInsufficientFunds()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 100,
                RecipientId = 2,
            };
            var senderAcc = new Account
            {
                Id = 1,
                Balance = 0,
                Username = "test"
            };
            var recipientAcc = new Account
            {
                Id = 2,
                Balance = 100,
                Username = "test2"
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(x=>x.Id== transactions.SenderId)).ReturnsAsync(senderAcc);
            mockAccountService.Setup(x => x.GetByAsync(x=>x.Id== transactions.RecipientId)).ReturnsAsync(recipientAcc);

            var result = await transactionController.TransferAsync(transactions);

            Assert.NotNull(result);
            Assert.That(400, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode)!);
        }

        [Test]
        public async Task TransferAsync_ShouldReturn200()
        {
            var transactions = new Transaction
            {
                SenderId = 1,
                Amount = 100,
                RecipientId = 2,
            };
            var senderAcc = new Account
            {
                Id = 1,
                Balance = 150,
                Username = "test"
            };
            var recipientAcc = new Account
            {
                Id = 2,
                Balance = 100,
                Username = "test2"
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(x => x.Id == transactions.SenderId)).ReturnsAsync(senderAcc);
            mockAccountService.Setup(x => x.GetByAsync(x => x.Id == transactions.RecipientId)).ReturnsAsync(recipientAcc);

            var result = await transactionController.TransferAsync(transactions);

            Assert.NotNull(result);
            Assert.That(200, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode)!);
        }
    }
}
