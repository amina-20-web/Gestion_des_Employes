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
    public partial class Employes
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

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> employes;

        protected RadzenDataGrid<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            employes = await FinalDBService.GetEmployes(new Query { Filter = $@"i => i.Employe_Prenom.Contains(@0) || i.Employe_nom.Contains(@0) || i.Sexe.Contains(@0) || i.Email.Contains(@0) || i.Telephone.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Manager,Post,Departement" });
        }
        protected override async Task OnInitializedAsync()
        {
            employes = await FinalDBService.GetEmployes(new Query { Filter = $@"i => i.Employe_Prenom.Contains(@0) || i.Employe_nom.Contains(@0) || i.Sexe.Contains(@0) || i.Email.Contains(@0) || i.Telephone.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Manager,Post,Departement" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddEmploye>("Add Employe", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> args)
        {
            await DialogService.OpenAsync<EditEmploye>("Edit Employe", new Dictionary<string, object> { {"Employe_id", args.Data.Employe_id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe employe)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await FinalDBService.DeleteEmploye(employe.Employe_id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Employe"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await FinalDBService.ExportEmployesToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Manager,Post,Departement",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Employes");
            }

            if (args == null || args.Value == "xlsx")
            {
                await FinalDBService.ExportEmployesToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Manager,Post,Departement",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Employes");
            }
        }
    }
}