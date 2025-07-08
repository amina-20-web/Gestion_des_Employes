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
    public partial class AddEmploye
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

        protected override async Task OnInitializedAsync()
        {
            employe = new DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe();

            employesForManagerId = await FinalDBService.GetEmployes();

            postesForPostId = await FinalDBService.GetPostes();

            departementsForDepartementId = await FinalDBService.GetDepartements();
        }
        protected bool errorVisible;
        protected DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe employe;

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> employesForManagerId;

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> postesForPostId;

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> departementsForDepartementId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await FinalDBService.CreateEmploye(employe);
                DialogService.Close(employe);
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