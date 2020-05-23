using Interfaces;
using Microsoft.AspNetCore.Components;
using Models.DTO.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPathUI.Pages.Authentication
{
    public class RegisterBase : ComponentBase
    {
        [Inject]
        public IAuthenticationDataService AuthenticationDataService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public RegisterRequestDTO RequestDTO { get; set; }
        public bool Confirmvisible { get; set; }

        public bool FailedRegister { get; set; } = false;
        public List<string> FailedRegisterMessages { get; set; }
        public RegisterBase()
        {
            this.RequestDTO = new RegisterRequestDTO();
        }
        public async Task RegisterUser()
        {
            var result = await AuthenticationDataService.Register(RequestDTO);
            if(result.Successful)
                NavigationManager.NavigateTo("/Login");
            else
            {
                FailedRegister = true;
                FailedRegisterMessages = result.Errors.ToList();
            }
        }
    }
}
