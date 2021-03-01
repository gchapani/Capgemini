using Excel.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Excel.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly Business.Interfaces.IExcelBusiness _business;

        public static IWebHostEnvironment _environment;
        public ExcelController(IWebHostEnvironment environment, Business.Interfaces.IExcelBusiness business)
        {
            _environment = environment;
            _business = business;
        }

        [HttpGet("import")]
        public ActionResult GetAllImports()
        {
            List<GetAllImportsResult> oGetAllImportsResult = _business.GetAllImports();

            if (oGetAllImportsResult == null)
            {
                return BadRequest();
            }

            return Ok(oGetAllImportsResult);
        }

        [HttpGet("import/{id}")]
        public ActionResult GetImportById(int id)
        {
            List<GetImportByIdResult> oGetImportByIdResult = _business.GetImportById(id);

            if (oGetImportByIdResult == null)
            {
                return BadRequest();
            }

            return Ok(oGetImportByIdResult);
        }

        [HttpPost("insert")]
        public ActionResult Insert([FromForm] IFormFile arquivo)
        {
            InsertExcelResult _InsertExcelResult = _business.Insert(arquivo).Result;

            if (_InsertExcelResult == null)
            {
                return NotFound();
            }

            return Ok(_InsertExcelResult);
        }
    }
}