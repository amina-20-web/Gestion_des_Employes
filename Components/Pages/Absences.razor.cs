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
    public partial class Absences
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

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> absences;

        protected RadzenDataGrid<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            absences = await FinalDBService.GetAbsences(new Query
            {
                Filter = $@"i => i.type_absence.Contains(@0) || i.statut.Contains(@0)",
                FilterParameters = new object[] { search },
                Expand = "employe"
            });
        }

        protected override async Task OnInitializedAsync()
        {
            absences = await FinalDBService.GetAbsences(new Query
            {
                Filter = $@"i => i.type_absence.Contains(@0) || i.statut.Contains(@0)",
                FilterParameters = new object[] { search },
                Expand = "employe"
            });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAbsence>("Add Absence", null);
            if (result != null)
            {
                await grid0.Reload();
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Succès",
                    Detail = "Absence ajoutée"
                });
            }
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> args)
        {
            await DialogService.OpenAsync<EditAbsence>("Edit Absence", new Dictionary<string, object> { { "absence_id", args.Data.absence_id } });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence absence)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await FinalDBService.DeleteAbsence(absence.absence_id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Succès",
                            Detail = "Absence supprimée"
                        });
                    }
                }
            }
            catch (Exception)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Erreur",
                    Detail = "Impossible de supprimer l'absence"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await FinalDBService.ExportAbsencesToCSV(new Query
                {
                    Filter = string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter,
                    OrderBy = grid0.Query.OrderBy,
                    Expand = "employe",
                    Select = string.Join(",", grid0.ColumnsCollection
                        .Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property))
                        .Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Absences");
            }

            if (args == null || args.Value == "xlsx")
            {
                await FinalDBService.ExportAbsencesToExcel(new Query
                {
                    Filter = string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter,
                    OrderBy = grid0.Query.OrderBy,
                    Expand = "employe",
                    Select = string.Join(",", grid0.ColumnsCollection
                        .Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property))
                        .Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Absences");
            }
        }
    }
}