using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace DESKTOPJN6V6MTSQLEXPRESS.Components.Pages
{
    public partial class EditAspNetUserToken
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public FinalDBService FinalDBService { get; set; }

        [Parameter]
        public string UserId { get; set; }

        [Parameter]
        public string LoginProvider { get; set; }

        [Parameter]
        public string Name { get; set; }

        protected override async Task OnInitializedAsync()
        {
            aspNetUserToken = await FinalDBService.GetAspNetUserTokenByUserIdAndLoginProviderAndName(UserId, LoginProvider, Name);

            aspNetUsersForUserId = await FinalDBService.GetAspNetUsers();
        }
        protected bool errorVisible;
        protected DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken aspNetUserToken;

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> aspNetUsersForUserId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await FinalDBService.UpdateAspNetUserToken(UserId, LoginProvider, Name, aspNetUserToken);
                DialogService.Close(aspNetUserToken);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}