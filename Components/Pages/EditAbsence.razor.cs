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
    public partial class EditAbsence
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
        public int absence_id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            absence = await FinalDBService.GetAbsenceByAbsenceId(absence_id);

            employesForemployeId = await FinalDBService.GetEmployes();
        }
        protected bool errorVisible;
        protected DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence absence;

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> employesForemployeId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await FinalDBService.UpdateAbsence(absence_id, absence);
                DialogService.Close(absence);
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