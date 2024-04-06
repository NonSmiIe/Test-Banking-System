using AutoMapper;
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
    public class AccountControllerTests
    {
        private readonly AccountController accountController;
        private readonly Mock<IAccountService> mockAccountService = new Mock<IAccountService>();
        private readonly Mock<ITokenService> mockTokenService = new Mock<ITokenService>();
        private readonly Mock<IMapper> mockMapper = new Mock<IMapper>();
        public AccountControllerTests()
        {
            accountController = new AccountController(mockMapper.Object, mockTokenService.Object, mockAccountService.Object);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            accountController.Dispose();
        }

        [Test]
        public async Task RegisterAsync_ShouldReturn400_IfUsernameExists()
        {
            var request = new RegisterDto
            {
                Username = "test",
                Email = "test@test",
                Password = "password"
            };
            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);

            var result = await accountController.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.That(400, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task RegisterAsync_ShouldReturn200()
        {
            var request = new RegisterDto
            {
                Username = "test",
                Email = "test@test",
                Password = "password"
            };
            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(false);

            var result = await accountController.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.That(200, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task LoginAsync_ShouldReturn401_IfUserNameInvalid()
        {
            var request = new LoginDto
            {
                Username = "test",
                Password = "password"
            };
            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(false);

            var result = await accountController.LoginAsync(request);

            Assert.NotNull(result);
            Assert.That(401, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task LoginAsync_ShouldReturn401_IfPasswordInvalid()
        {
            var request = new LoginDto
            {
                Username = "test",
                Password = "password"
            };

            var acc = new Account
            {
                Username = "test",
                PasswordHash = "$2a$11$NAaLQL5jdutEM1qXqyG2P.2NzreUz9Lx2.us8Q/p0SZTAaGFNZXQmWRONG"
            };
            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(acc);
            var result = await accountController.LoginAsync(request);

            Assert.NotNull(result);
            Assert.That(401, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task LoginAsync_ShouldReturn200()
        {
            var request = new LoginDto
            {
                Username = "test",
                Password = "password"
            };

            var acc = new Account
            {
                Username = "test",
                PasswordHash = "$2a$11$NAaLQL5jdutEM1qXqyG2P.2NzreUz9Lx2.us8Q/p0SZTAaGFNZXQm"
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(acc);
            mockTokenService.Setup(x => x.CreateToken(It.IsAny<Account>())).Returns("token");

            var result = await accountController.LoginAsync(request);

            Assert.NotNull(result);
            Assert.That(200, Is.EqualTo(((IStatusCodeActionResult)result).StatusCode));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturn200()
        {
            var acc = new Account
            {
                Id = 1,
                Username = "test",
                
            };

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(true);
            mockAccountService.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(acc);
            mockMapper.Setup(x => x.Map<AccountDto>(It.IsAny<Account>())).Returns(new AccountDto { Id = acc.Id, Username = acc.Username});

            var result = await accountController.GetByIdAsync(1);

            Assert.NotNull(result.Value);
            Assert.That(result.Value.Id, Is.EqualTo(acc.Id));
            Assert.That(result.Value.Username, Is.EqualTo(acc.Username));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturn404_IfUsernotFound()
        {

            mockAccountService.Setup(x => x.IsExistAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(false);

            var result = await accountController.GetByIdAsync(1);

            Assert.NotNull(result.Result);
            Assert.That(404, Is.EqualTo(((IStatusCodeActionResult)result.Result).StatusCode));
        }

        [Test]
        public async Task GetAllAccountsAsync_ShouldReturn200()
        {
            var accs = new List<Account>
            {
                new Account { Id = 1, Username = "test"},
                new Account { Id = 2, Username = "test2"},
                new Account { Id = 3, Username = "test3"},
            };

            mockAccountService.Setup(x => x.GetAllByAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(accs);
            mockMapper.Setup(x => x.Map<AccountDto>(It.IsAny<Account>())).Returns(new AccountDto());

            var result = await accountController.GetAllAccountsAsync();

            Assert.NotNull(result.Value);
            Assert.That(result.Value.Count, Is.EqualTo(accs.Count));
        }
    }
}
