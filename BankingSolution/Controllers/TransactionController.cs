using BankingSolution.Models;
using BankingSolution.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSolution.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IAccountService _accountService;
        public TransactionController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("deposit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DepositAsync(Transaction transaction)
        {
            if (!await _accountService.IsExistAsync(x => x.Id == transaction.SenderId))
                return NotFound();

            var acc = await _accountService.GetByAsync(x => x.Id == transaction.SenderId);
            acc.Balance += transaction.Amount;

            _accountService.Update(acc);
            await _accountService.SaveAsync();

            return Ok("Deposit Successful");
        }

        [HttpPost("withdraw")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> WithdrawAsync(Transaction transaction)
        {
            if (!await _accountService.IsExistAsync(x => x.Id == transaction.SenderId))
                return NotFound();

            var acc = await _accountService.GetByAsync(x => x.Id == transaction.SenderId);

            if (acc.Balance < transaction.Amount)
                return BadRequest("Insufficient funds");

            acc.Balance -= transaction.Amount;

            _accountService.Update(acc);
            await _accountService.SaveAsync();

            return Ok("Withdrawal Successful");
        }

        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> TransferAsync(Transaction transaction)
        {
            if (transaction.SenderId == transaction.RecipientId) return BadRequest();

            if (!await _accountService.IsExistAsync(x => x.Id == transaction.SenderId) || !await _accountService.IsExistAsync(x => x.Id == transaction.RecipientId))
                return NotFound();

            var senderAcc = await _accountService.GetByAsync(x => x.Id == transaction.SenderId);
            var recipientAcc = await _accountService.GetByAsync(x => x.Id == transaction.RecipientId);

            if (senderAcc.Balance < transaction.Amount)
                return BadRequest("Insufficient funds");

            senderAcc.Balance -= transaction.Amount;
            recipientAcc.Balance += transaction.Amount;

            _accountService.UpdateRange([senderAcc, recipientAcc]);
            await _accountService.SaveAsync();

            return Ok("Transfer Successful");
        }


    }
}
