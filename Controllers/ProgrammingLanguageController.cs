using EmployeeCensus4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using Dapper;

namespace EmployeeCensus4.Controllers
{
    public class ProgrammingLanguageController : Controller
    {
        private readonly ILogger<Departament> _logger;

        private readonly IConfiguration _config;

        public ProgrammingLanguageController(ILogger<Departament> logger, IConfiguration config)
        {
            _logger = logger;

            _config = config;
        }
        public IActionResult Index()
        {
            var model = GetLanguages();
            ViewBag.message = TempData["Message"];
            return View(model);
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            TempData["Message"] = false; 
            try
            {
                var sql = "DELETE FROM ProgrammingLanguages WHERE id = @id";
                using (IDbConnection db = Connection)
                {
                    db.Execute(sql, new { id });

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["Message"] = true;
                return RedirectToAction("Index");
            }
            

        }

        [HttpPost]
        public IActionResult Create(ProgrammingLanguage language)
        {
            var sql = "INSERT INTO ProgrammingLanguages (languageName) VALUES (@LanguageName)";

            using (IDbConnection db = Connection)
            {
                db.Execute(sql, new { language.LanguageName });

                return RedirectToAction("Index");
            }
        }

        [NonAction]
        private IEnumerable<ProgrammingLanguage> GetLanguages()
        {
            var sql = "SELECT * FROM ProgrammingLanguages";
            using (IDbConnection db = Connection)
            {
                var languages = db.Query<ProgrammingLanguage>(sql);

                return languages;
            }
        }
    }
}
