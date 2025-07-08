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
    public partial class AspNetUserRoles
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

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> aspNetUserRoles;

        protected RadzenDataGrid<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            aspNetUserRoles = await FinalDBService.GetAspNetUserRoles(new Query { Filter = $@"i => i.UserId.Contains(@0) || i.RoleId.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AspNetUser,AspNetRole" });
        }
        protected override async Task OnInitializedAsync()
        {
            aspNetUserRoles = await FinalDBService.GetAspNetUserRoles(new Query { Filter = $@"i => i.UserId.Contains(@0) || i.RoleId.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AspNetUser,AspNetRole" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddAspNetUserRole>("Add AspNetUserRole", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> args)
        {
            await DialogService.OpenAsync<EditAspNetUserRole>("Edit AspNetUserRole", new Dictionary<string, object> { {"UserId", args.Data.UserId}, {"RoleId", args.Data.RoleId} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole aspNetUserRole)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await FinalDBService.DeleteAspNetUserRole(aspNetUserRole.UserId, aspNetUserRole.RoleId);

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
                    Detail = $"Unable to delete AspNetUserRole"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await FinalDBService.ExportAspNetUserRolesToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "AspNetUser,AspNetRole",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "AspNetUserRoles");
            }

            if (args == null || args.Value == "xlsx")
            {
                await FinalDBService.ExportAspNetUserRolesToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "AspNetUser,AspNetRole",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "AspNetUserRoles");
            }
        }
    }
}