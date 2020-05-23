using Models.DTO.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IAuthenticationDataService
    {
        Task<RegisterResultDTO> Register(RegisterRequestDTO item);
        Task<LoginResultDTO> Login(LoginRequestDTO item);
    }
}
