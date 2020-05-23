using Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Models.DTO.Authentication;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPathUI.Pages.Authentication
{
    public class LoginBase : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public LoginRequestDTO LoginModel { get; set; }
        [Inject]
        public IAuthenticationDataService AuthenticationDataService { get; set; }
        [Inject]
        public ILocalStorageManagerService LocalStorageManagerService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public bool FailedLogin { get; set; } = false;
        public string FailedLoginMessage { get; set; }
        //[Inject]
        //public ISessionStorageDataService SessionStorage { get; set; }
        public LoginBase()
        {
            this.LoginModel = new LoginRequestDTO();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await LocalStorageManagerService.DeletePermanentItemAsync("Token");
        }
        public async Task LoginUser()
        {
            var result = await AuthenticationDataService.Login(LoginModel);
            if (result.Successful)
            {
                //await SessionStorage.SetItemAsync("Token", result.Token);
                NavigationManager.NavigateTo("/Map");
            }
            else
            {
                FailedLogin = true;
                FailedLoginMessage = result.Error;
            }
        }
    }
}
