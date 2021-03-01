using Excel.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Excel.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Import(IFormFile file)
        {

            using (var client = new HttpClient())
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.Add("X-FileName", fileName);
                fileContent.Headers.Add("X-ContentType", file.ContentType);

                using (var response = await client.PostAsync("https://localhost/insert", fileContent))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        
                    }
                    else
                    {
                        ViewBag.Message = $"Erro ao importar planilha: {response.RequestMessage}";
                    }
                }

                return View();
            }
        }

        public async Task<IActionResult> GetAllImports()
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://localhost/import"))
                {
                    if (response.IsSuccessStatusCode)
                    {

                    }
                    else
                    {
                        ViewBag.Message = response.RequestMessage;
                    }
                }

                return View();
            }
        }

        public async Task<IActionResult> GetImportById(int id)
        {

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://localhost/import/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {

                    }
                    else
                    {
                        ViewBag.Message = response.RequestMessage;
                    }
                }

                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
