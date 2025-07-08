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
    public partial class EditAspNetRoleClaim
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
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            aspNetRoleClaim = await FinalDBService.GetAspNetRoleClaimById(Id);

            aspNetRolesForRoleId = await FinalDBService.GetAspNetRoles();
        }
        protected bool errorVisible;
        protected DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim aspNetRoleClaim;

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> aspNetRolesForRoleId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await FinalDBService.UpdateAspNetRoleClaim(Id, aspNetRoleClaim);
                DialogService.Close(aspNetRoleClaim);
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