using Excel.Domain;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Excel.Business.Interfaces
{
    public interface IExcelBusiness
    {
        List<GetAllImportsResult> GetAllImports();
        List<GetImportByIdResult> GetImportById(int id);
        Task<InsertExcelResult> Insert(IFormFile arquivo);
    }
}