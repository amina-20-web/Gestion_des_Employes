using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using DESKTOPJN6V6MTSQLEXPRESS.Data;

namespace DESKTOPJN6V6MTSQLEXPRESS
{
    public partial class FinalDBService
    {
        FinalDBContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly FinalDBContext context;
        private readonly NavigationManager navigationManager;

        public FinalDBService(FinalDBContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportAbsencesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/absences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/absences/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAbsencesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/absences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/absences/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAbsencesRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence>> GetAbsences(Query query = null)
        {
            var items = Context.Absences.AsQueryable();

            items = items.Include(i => i.employe);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAbsencesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAbsenceGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item);
        partial void OnGetAbsenceByAbsenceId(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> GetAbsenceByAbsenceId(int absenceid)
        {
            var items = Context.Absences
                              .AsNoTracking()
                              .Where(i => i.absence_id == absenceid);

            items = items.Include(i => i.employe);
 
            OnGetAbsenceByAbsenceId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAbsenceGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAbsenceCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item);
        partial void OnAfterAbsenceCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> CreateAbsence(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence absence)
        {
            OnAbsenceCreated(absence);

            var existingItem = Context.Absences
                              .Where(i => i.absence_id == absence.absence_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Absences.Add(absence);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(absence).State = EntityState.Detached;
                throw;
            }

            OnAfterAbsenceCreated(absence);

            return absence;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> CancelAbsenceChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAbsenceUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item);
        partial void OnAfterAbsenceUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> UpdateAbsence(int absenceid, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence absence)
        {
            OnAbsenceUpdated(absence);

            var itemToUpdate = Context.Absences
                              .Where(i => i.absence_id == absence.absence_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(absence);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAbsenceUpdated(absence);

            return absence;
        }

        partial void OnAbsenceDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item);
        partial void OnAfterAbsenceDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> DeleteAbsence(int absenceid)
        {
            var itemToDelete = Context.Absences
                              .Where(i => i.absence_id == absenceid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAbsenceDeleted(itemToDelete);


            Context.Absences.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAbsenceDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportDepartementsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/departements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/departements/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDepartementsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/departements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/departements/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDepartementsRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement>> GetDepartements(Query query = null)
        {
            var items = Context.Departements.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnDepartementsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDepartementGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item);
        partial void OnGetDepartementByDepartementId(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> GetDepartementByDepartementId(int departementid)
        {
            var items = Context.Departements
                              .AsNoTracking()
                              .Where(i => i.Departement_id == departementid);

 
            OnGetDepartementByDepartementId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDepartementGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDepartementCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item);
        partial void OnAfterDepartementCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> CreateDepartement(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement departement)
        {
            OnDepartementCreated(departement);

            var existingItem = Context.Departements
                              .Where(i => i.Departement_id == departement.Departement_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Departements.Add(departement);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(departement).State = EntityState.Detached;
                throw;
            }

            OnAfterDepartementCreated(departement);

            return departement;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> CancelDepartementChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDepartementUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item);
        partial void OnAfterDepartementUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> UpdateDepartement(int departementid, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement departement)
        {
            OnDepartementUpdated(departement);

            var itemToUpdate = Context.Departements
                              .Where(i => i.Departement_id == departement.Departement_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(departement);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDepartementUpdated(departement);

            return departement;
        }

        partial void OnDepartementDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item);
        partial void OnAfterDepartementDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> DeleteDepartement(int departementid)
        {
            var itemToDelete = Context.Departements
                              .Where(i => i.Departement_id == departementid)
                              .Include(i => i.Employes)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnDepartementDeleted(itemToDelete);


            Context.Departements.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDepartementDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportEmployesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/employes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/employes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportEmployesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/employes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/employes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnEmployesRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe>> GetEmployes(Query query = null)
        {
            var items = Context.Employes.AsQueryable();

            items = items.Include(i => i.Departement);
            items = items.Include(i => i.Manager);
            items = items.Include(i => i.Post);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnEmployesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnEmployeGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item);
        partial void OnGetEmployeByEmployeId(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> GetEmployeByEmployeId(int employeid)
        {
            var items = Context.Employes
                              .AsNoTracking()
                              .Where(i => i.Employe_id == employeid);

            items = items.Include(i => i.Departement);
            items = items.Include(i => i.Manager);
            items = items.Include(i => i.Post);
 
            OnGetEmployeByEmployeId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnEmployeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnEmployeCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item);
        partial void OnAfterEmployeCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> CreateEmploye(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe employe)
        {
            OnEmployeCreated(employe);

            var existingItem = Context.Employes
                              .Where(i => i.Employe_id == employe.Employe_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Employes.Add(employe);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(employe).State = EntityState.Detached;
                throw;
            }

            OnAfterEmployeCreated(employe);

            return employe;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> CancelEmployeChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnEmployeUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item);
        partial void OnAfterEmployeUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> UpdateEmploye(int employeid, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe employe)
        {
            OnEmployeUpdated(employe);

            var itemToUpdate = Context.Employes
                              .Where(i => i.Employe_id == employe.Employe_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(employe);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterEmployeUpdated(employe);

            return employe;
        }

        partial void OnEmployeDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item);
        partial void OnAfterEmployeDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> DeleteEmploye(int employeid)
        {
            var itemToDelete = Context.Employes
                              .Where(i => i.Employe_id == employeid)
                              .Include(i => i.Absences)
                              .Include(i => i.InverseManager)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnEmployeDeleted(itemToDelete);


            Context.Employes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterEmployeDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPostesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/postes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/postes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPostesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/postes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/postes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPostesRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste>> GetPostes(Query query = null)
        {
            var items = Context.Postes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPostesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPosteGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item);
        partial void OnGetPosteByPostId(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> GetPosteByPostId(int postid)
        {
            var items = Context.Postes
                              .AsNoTracking()
                              .Where(i => i.Post_id == postid);

 
            OnGetPosteByPostId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPosteGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPosteCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item);
        partial void OnAfterPosteCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> CreatePoste(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste poste)
        {
            OnPosteCreated(poste);

            var existingItem = Context.Postes
                              .Where(i => i.Post_id == poste.Post_id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Postes.Add(poste);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(poste).State = EntityState.Detached;
                throw;
            }

            OnAfterPosteCreated(poste);

            return poste;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> CancelPosteChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPosteUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item);
        partial void OnAfterPosteUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> UpdatePoste(int postid, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste poste)
        {
            OnPosteUpdated(poste);

            var itemToUpdate = Context.Postes
                              .Where(i => i.Post_id == poste.Post_id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(poste);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPosteUpdated(poste);

            return poste;
        }

        partial void OnPosteDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item);
        partial void OnAfterPosteDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> DeletePoste(int postid)
        {
            var itemToDelete = Context.Postes
                              .Where(i => i.Post_id == postid)
                              .Include(i => i.Employes)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPosteDeleted(itemToDelete);


            Context.Postes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPosteDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetRoleClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetroleclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetRoleClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetroleclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetRoleClaimsRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim>> GetAspNetRoleClaims(Query query = null)
        {
            var items = Context.AspNetRoleClaims.AsQueryable();

            items = items.Include(i => i.Role);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetRoleClaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetRoleClaimGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item);
        partial void OnGetAspNetRoleClaimById(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> GetAspNetRoleClaimById(int id)
        {
            var items = Context.AspNetRoleClaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Role);
 
            OnGetAspNetRoleClaimById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetRoleClaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetRoleClaimCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> CreateAspNetRoleClaim(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim aspnetroleclaim)
        {
            OnAspNetRoleClaimCreated(aspnetroleclaim);

            var existingItem = Context.AspNetRoleClaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetRoleClaims.Add(aspnetroleclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetroleclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetRoleClaimCreated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> CancelAspNetRoleClaimChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetRoleClaimUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> UpdateAspNetRoleClaim(int id, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim aspnetroleclaim)
        {
            OnAspNetRoleClaimUpdated(aspnetroleclaim);

            var itemToUpdate = Context.AspNetRoleClaims
                              .Where(i => i.Id == aspnetroleclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetroleclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetRoleClaimUpdated(aspnetroleclaim);

            return aspnetroleclaim;
        }

        partial void OnAspNetRoleClaimDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item);
        partial void OnAfterAspNetRoleClaimDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> DeleteAspNetRoleClaim(int id)
        {
            var itemToDelete = Context.AspNetRoleClaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetRoleClaimDeleted(itemToDelete);


            Context.AspNetRoleClaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetRoleClaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetRolesRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole>> GetAspNetRoles(Query query = null)
        {
            var items = Context.AspNetRoles.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetRolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetRoleGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item);
        partial void OnGetAspNetRoleById(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> GetAspNetRoleById(string id)
        {
            var items = Context.AspNetRoles
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAspNetRoleById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetRoleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetRoleCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item);
        partial void OnAfterAspNetRoleCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> CreateAspNetRole(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole aspnetrole)
        {
            OnAspNetRoleCreated(aspnetrole);

            var existingItem = Context.AspNetRoles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetRoles.Add(aspnetrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetRoleCreated(aspnetrole);

            return aspnetrole;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> CancelAspNetRoleChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetRoleUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item);
        partial void OnAfterAspNetRoleUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> UpdateAspNetRole(string id, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole aspnetrole)
        {
            OnAspNetRoleUpdated(aspnetrole);

            var itemToUpdate = Context.AspNetRoles
                              .Where(i => i.Id == aspnetrole.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetRoleUpdated(aspnetrole);

            return aspnetrole;
        }

        partial void OnAspNetRoleDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item);
        partial void OnAfterAspNetRoleDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> DeleteAspNetRole(string id)
        {
            var itemToDelete = Context.AspNetRoles
                              .Where(i => i.Id == id)
                              .Include(i => i.AspNetRoleClaims)
                              .Include(i => i.AspNetUserRoles)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetRoleDeleted(itemToDelete);


            Context.AspNetRoles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetRoleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserClaimsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetuserclaims/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserClaimsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetuserclaims/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserClaimsRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim>> GetAspNetUserClaims(Query query = null)
        {
            var items = Context.AspNetUserClaims.AsQueryable();

            items = items.Include(i => i.User);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserClaimsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserClaimGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item);
        partial void OnGetAspNetUserClaimById(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> GetAspNetUserClaimById(int id)
        {
            var items = Context.AspNetUserClaims
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.User);
 
            OnGetAspNetUserClaimById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserClaimGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserClaimCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> CreateAspNetUserClaim(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim aspnetuserclaim)
        {
            OnAspNetUserClaimCreated(aspnetuserclaim);

            var existingItem = Context.AspNetUserClaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserClaims.Add(aspnetuserclaim);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserclaim).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserClaimCreated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> CancelAspNetUserClaimChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserClaimUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> UpdateAspNetUserClaim(int id, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim aspnetuserclaim)
        {
            OnAspNetUserClaimUpdated(aspnetuserclaim);

            var itemToUpdate = Context.AspNetUserClaims
                              .Where(i => i.Id == aspnetuserclaim.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserclaim);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserClaimUpdated(aspnetuserclaim);

            return aspnetuserclaim;
        }

        partial void OnAspNetUserClaimDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item);
        partial void OnAfterAspNetUserClaimDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> DeleteAspNetUserClaim(int id)
        {
            var itemToDelete = Context.AspNetUserClaims
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserClaimDeleted(itemToDelete);


            Context.AspNetUserClaims.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserClaimDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserLoginsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetuserlogins/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserLoginsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetuserlogins/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserLoginsRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin>> GetAspNetUserLogins(Query query = null)
        {
            var items = Context.AspNetUserLogins.AsQueryable();

            items = items.Include(i => i.User);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserLoginsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserLoginGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item);
        partial void OnGetAspNetUserLoginByLoginProviderAndProviderKey(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> GetAspNetUserLoginByLoginProviderAndProviderKey(string loginprovider, string providerkey)
        {
            var items = Context.AspNetUserLogins
                              .AsNoTracking()
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey);

            items = items.Include(i => i.User);
 
            OnGetAspNetUserLoginByLoginProviderAndProviderKey(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserLoginGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserLoginCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> CreateAspNetUserLogin(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin aspnetuserlogin)
        {
            OnAspNetUserLoginCreated(aspnetuserlogin);

            var existingItem = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserLogins.Add(aspnetuserlogin);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserlogin).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserLoginCreated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> CancelAspNetUserLoginChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserLoginUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> UpdateAspNetUserLogin(string loginprovider, string providerkey, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin aspnetuserlogin)
        {
            OnAspNetUserLoginUpdated(aspnetuserlogin);

            var itemToUpdate = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == aspnetuserlogin.LoginProvider && i.ProviderKey == aspnetuserlogin.ProviderKey)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserlogin);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserLoginUpdated(aspnetuserlogin);

            return aspnetuserlogin;
        }

        partial void OnAspNetUserLoginDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item);
        partial void OnAfterAspNetUserLoginDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> DeleteAspNetUserLogin(string loginprovider, string providerkey)
        {
            var itemToDelete = Context.AspNetUserLogins
                              .Where(i => i.LoginProvider == loginprovider && i.ProviderKey == providerkey)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserLoginDeleted(itemToDelete);


            Context.AspNetUserLogins.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserLoginDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserRolesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetuserroles/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserRolesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetuserroles/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserRolesRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole>> GetAspNetUserRoles(Query query = null)
        {
            var items = Context.AspNetUserRoles.AsQueryable();

            items = items.Include(i => i.AspNetRole);
            items = items.Include(i => i.AspNetUser);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserRolesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserRoleGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item);
        partial void OnGetAspNetUserRoleByUserIdAndRoleId(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> GetAspNetUserRoleByUserIdAndRoleId(string userid, string roleid)
        {
            var items = Context.AspNetUserRoles
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.RoleId == roleid);

            items = items.Include(i => i.AspNetRole);
            items = items.Include(i => i.AspNetUser);
 
            OnGetAspNetUserRoleByUserIdAndRoleId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserRoleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserRoleCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> CreateAspNetUserRole(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole aspnetuserrole)
        {
            OnAspNetUserRoleCreated(aspnetuserrole);

            var existingItem = Context.AspNetUserRoles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserRoles.Add(aspnetuserrole);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuserrole).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserRoleCreated(aspnetuserrole);

            return aspnetuserrole;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> CancelAspNetUserRoleChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserRoleUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> UpdateAspNetUserRole(string userid, string roleid, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole aspnetuserrole)
        {
            OnAspNetUserRoleUpdated(aspnetuserrole);

            var itemToUpdate = Context.AspNetUserRoles
                              .Where(i => i.UserId == aspnetuserrole.UserId && i.RoleId == aspnetuserrole.RoleId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuserrole);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserRoleUpdated(aspnetuserrole);

            return aspnetuserrole;
        }

        partial void OnAspNetUserRoleDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item);
        partial void OnAfterAspNetUserRoleDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> DeleteAspNetUserRole(string userid, string roleid)
        {
            var itemToDelete = Context.AspNetUserRoles
                              .Where(i => i.UserId == userid && i.RoleId == roleid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserRoleDeleted(itemToDelete);


            Context.AspNetUserRoles.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserRoleDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUsersRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser>> GetAspNetUsers(Query query = null)
        {
            var items = Context.AspNetUsers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item);
        partial void OnGetAspNetUserById(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> GetAspNetUserById(string id)
        {
            var items = Context.AspNetUsers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAspNetUserById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item);
        partial void OnAfterAspNetUserCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> CreateAspNetUser(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser aspnetuser)
        {
            OnAspNetUserCreated(aspnetuser);

            var existingItem = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUsers.Add(aspnetuser);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetuser).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserCreated(aspnetuser);

            return aspnetuser;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> CancelAspNetUserChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item);
        partial void OnAfterAspNetUserUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> UpdateAspNetUser(string id, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser aspnetuser)
        {
            OnAspNetUserUpdated(aspnetuser);

            var itemToUpdate = Context.AspNetUsers
                              .Where(i => i.Id == aspnetuser.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetuser);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserUpdated(aspnetuser);

            return aspnetuser;
        }

        partial void OnAspNetUserDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item);
        partial void OnAfterAspNetUserDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> DeleteAspNetUser(string id)
        {
            var itemToDelete = Context.AspNetUsers
                              .Where(i => i.Id == id)
                              .Include(i => i.AspNetUserClaims)
                              .Include(i => i.AspNetUserLogins)
                              .Include(i => i.AspNetUserRoles)
                              .Include(i => i.AspNetUserTokens)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserDeleted(itemToDelete);


            Context.AspNetUsers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAspNetUserTokensToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetusertokens/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAspNetUserTokensToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/finaldb/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/finaldb/aspnetusertokens/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAspNetUserTokensRead(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> items);

        public async Task<IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken>> GetAspNetUserTokens(Query query = null)
        {
            var items = Context.AspNetUserTokens.AsQueryable();

            items = items.Include(i => i.User);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAspNetUserTokensRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAspNetUserTokenGet(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item);
        partial void OnGetAspNetUserTokenByUserIdAndLoginProviderAndName(ref IQueryable<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> items);


        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> GetAspNetUserTokenByUserIdAndLoginProviderAndName(string userid, string loginprovider, string name)
        {
            var items = Context.AspNetUserTokens
                              .AsNoTracking()
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name);

            items = items.Include(i => i.User);
 
            OnGetAspNetUserTokenByUserIdAndLoginProviderAndName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAspNetUserTokenGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAspNetUserTokenCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenCreated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> CreateAspNetUserToken(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken aspnetusertoken)
        {
            OnAspNetUserTokenCreated(aspnetusertoken);

            var existingItem = Context.AspNetUserTokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AspNetUserTokens.Add(aspnetusertoken);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(aspnetusertoken).State = EntityState.Detached;
                throw;
            }

            OnAfterAspNetUserTokenCreated(aspnetusertoken);

            return aspnetusertoken;
        }

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> CancelAspNetUserTokenChanges(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAspNetUserTokenUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenUpdated(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> UpdateAspNetUserToken(string userid, string loginprovider, string name, DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken aspnetusertoken)
        {
            OnAspNetUserTokenUpdated(aspnetusertoken);

            var itemToUpdate = Context.AspNetUserTokens
                              .Where(i => i.UserId == aspnetusertoken.UserId && i.LoginProvider == aspnetusertoken.LoginProvider && i.Name == aspnetusertoken.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(aspnetusertoken);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAspNetUserTokenUpdated(aspnetusertoken);

            return aspnetusertoken;
        }

        partial void OnAspNetUserTokenDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item);
        partial void OnAfterAspNetUserTokenDeleted(DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken item);

        public async Task<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> DeleteAspNetUserToken(string userid, string loginprovider, string name)
        {
            var itemToDelete = Context.AspNetUserTokens
                              .Where(i => i.UserId == userid && i.LoginProvider == loginprovider && i.Name == name)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAspNetUserTokenDeleted(itemToDelete);


            Context.AspNetUserTokens.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAspNetUserTokenDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}