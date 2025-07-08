using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using DESKTOPJN6V6MTSQLEXPRESS.Data;

namespace DESKTOPJN6V6MTSQLEXPRESS.Controllers
{
    public partial class ExportFinalDBController : ExportController
    {
        private readonly FinalDBContext context;
        private readonly FinalDBService service;

        public ExportFinalDBController(FinalDBContext context, FinalDBService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/FinalDB/absences/csv")]
        [HttpGet("/export/FinalDB/absences/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAbsencesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAbsences(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/absences/excel")]
        [HttpGet("/export/FinalDB/absences/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAbsencesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAbsences(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/departements/csv")]
        [HttpGet("/export/FinalDB/departements/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartementsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDepartements(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/departements/excel")]
        [HttpGet("/export/FinalDB/departements/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartementsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDepartements(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/employes/csv")]
        [HttpGet("/export/FinalDB/employes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmployesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetEmployes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/employes/excel")]
        [HttpGet("/export/FinalDB/employes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportEmployesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetEmployes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/postes/csv")]
        [HttpGet("/export/FinalDB/postes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPostes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/postes/excel")]
        [HttpGet("/export/FinalDB/postes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPostes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetroleclaims/csv")]
        [HttpGet("/export/FinalDB/aspnetroleclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRoleClaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetRoleClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetroleclaims/excel")]
        [HttpGet("/export/FinalDB/aspnetroleclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRoleClaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetRoleClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetroles/csv")]
        [HttpGet("/export/FinalDB/aspnetroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetroles/excel")]
        [HttpGet("/export/FinalDB/aspnetroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetRolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetuserclaims/csv")]
        [HttpGet("/export/FinalDB/aspnetuserclaims/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserClaimsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetuserclaims/excel")]
        [HttpGet("/export/FinalDB/aspnetuserclaims/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserClaimsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserClaims(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetuserlogins/csv")]
        [HttpGet("/export/FinalDB/aspnetuserlogins/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserLoginsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserLogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetuserlogins/excel")]
        [HttpGet("/export/FinalDB/aspnetuserlogins/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserLoginsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserLogins(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetuserroles/csv")]
        [HttpGet("/export/FinalDB/aspnetuserroles/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserRolesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetuserroles/excel")]
        [HttpGet("/export/FinalDB/aspnetuserroles/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserRolesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserRoles(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetusers/csv")]
        [HttpGet("/export/FinalDB/aspnetusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUsers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetusers/excel")]
        [HttpGet("/export/FinalDB/aspnetusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUsers(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetusertokens/csv")]
        [HttpGet("/export/FinalDB/aspnetusertokens/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserTokensToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAspNetUserTokens(), Request.Query, false), fileName);
        }

        [HttpGet("/export/FinalDB/aspnetusertokens/excel")]
        [HttpGet("/export/FinalDB/aspnetusertokens/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAspNetUserTokensToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAspNetUserTokens(), Request.Query, false), fileName);
        }
    }
}
