using AutoMapper;
using BankingSolution.Models;
using BankingSolution.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSolution.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        public static Account user = new Account();

        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;

        public AccountController(IMapper mapper, ITokenService tokenService, IAccountService accountService)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _accountService = accountService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto request)
        {
            if(await _accountService.IsExistAsync(x => x.Username == request.Username))
                return BadRequest("This name already exists!");

            var acc = new Account();

            acc.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            acc.Username = request.Username;
            acc.Email = request.Email;
            acc.Balance = 100;

            await _accountService.AddAsync(acc);
            await _accountService.SaveAsync();

            return Ok("Registration successful.");
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> LoginAsync([FromBody] LoginDto request)
        {
            if (!await _accountService.IsExistAsync(x => x.Username == request.Username))
                return Unauthorized("Username or password is invalid.");

            var acc = await _accountService.GetByAsync(x=>x.Username == request.Username);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, acc.PasswordHash))
                return Unauthorized("Username or password is invalid.");

            return Ok(new
            {
                Id = acc.Id,
                Username = acc.Username,
                Token = _tokenService.CreateToken(acc)
            });
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDto>> GetByIdAsync(int id)
        {
            if (!await _accountService.IsExistAsync(x => x.Id == id))
            {
                return NotFound("Account with such id not found.");
            }

            var acc = await _accountService.GetByAsync(x => x.Id == id);
            var accDto = _mapper.Map<AccountDto>(acc);

            return accDto;
        }

        [Authorize]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccountsAsync()
        {
            IEnumerable<Account> accs = await _accountService.GetAllByAsync();
            var accDtos = accs.Select(x => _mapper.Map<AccountDto>(x)).ToList();
           
            return accDtos;
        }
    }
}
