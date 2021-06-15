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
    public class DepartamentController : Controller
    {
        private readonly ILogger<Departament> _logger;

        private readonly IConfiguration _config;

        public DepartamentController(ILogger<Departament> logger, IConfiguration config)
        {
            _logger = logger;

            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public IActionResult Index()
        {
            var model = GetDepartaments();
            ViewBag.message = TempData["Message"];
            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Departament departament)
        {
            var sql = "INSERT INTO Departaments (departamentName) VALUES (@departamentName)";

            using (IDbConnection db = Connection)
            {
                db.Execute(sql, new { departament.DepartamentName });

                return RedirectToAction("Index");
            }
        }


        public ActionResult Delete(int id)
        {
            TempData["Message"] = false; 

            try
            {
                var sql = "DELETE FROM Departaments WHERE id = @id";

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

        [NonAction]
        private IEnumerable<Departament> GetDepartaments()
        {
            var sql = "SELECT * FROM departaments";
            using (IDbConnection db = Connection)
            {
                var departaments = db.Query<Departament>(sql);

                return departaments;
            }
        }
    }
}
