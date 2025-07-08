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
    public partial class AspNetUserClaims
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

        protected IEnumerable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> aspNetUserClaims;

        protected RadzenDataGrid<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            aspNetUserClaims = await FinalDBService.GetAspNetUserClaims(new Query { Filter = $@"i => i.ClaimType.Contains(@0) || i.ClaimValue.Contains(@0) || i.UserId.Contains(@0)", FilterParameters = new object[] { search }, Expand = "User" });
        }
        protected override async Task OnInitializedAsync()
        {
            aspNetUserClaims = await FinalDBService.GetAspNetUserClaims(new Query { Filter = $@"i => i.ClaimType.Contains(@0) || i.ClaimValue.Contains(@0) || i.UserId.Contains(@0)", FilterParameters = new object[] { search }, Expand = "User" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddAspNetUserClaim>("Add AspNetUserClaim", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> args)
        {
            await DialogService.OpenAsync<EditAspNetUserClaim>("Edit AspNetUserClaim", new Dictionary<string, object> { {"Id", args.Data.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim aspNetUserClaim)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await FinalDBService.DeleteAspNetUserClaim(aspNetUserClaim.Id);

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
                    Detail = $"Unable to delete AspNetUserClaim"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await FinalDBService.ExportAspNetUserClaimsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "User",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "AspNetUserClaims");
            }

            if (args == null || args.Value == "xlsx")
            {
                await FinalDBService.ExportAspNetUserClaimsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "User",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "AspNetUserClaims");
            }
        }
    }
}