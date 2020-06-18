using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPathUI.Pages.Components
{
    public class DeleteRouteDialogBase : ComponentBase
    {
        [Parameter]//now we can cann it as parameter in razor file
        public EventCallback CloseEventCallBack { get; set; }
        public bool ShowDialog { get; set; }
        public void Show()
        {
            ShowDialog = true;
            StateHasChanged();
        }
        public async Task DeleteRoute()
        {
            ShowDialog = false;
            await CloseEventCallBack.InvokeAsync(null);
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }
    }
}
