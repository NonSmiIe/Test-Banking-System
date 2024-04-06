using AutoMapper;
using BankingSolution.Models;

namespace BankingSolution.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}
