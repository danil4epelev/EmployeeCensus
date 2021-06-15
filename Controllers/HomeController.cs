using EmployeeCensus4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Threading.Tasks;

namespace EmployeeCensus4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
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
            var model = GetEmployees();
            return View(model);
        }

        public IActionResult Add()
        {
            GetEmployeesNames();

            var result = GetListLanguageAndDepartaments();
            var model = new ModelForEmployee { employee = new Employee(), @enum = result };

            return View(model);
        }

        public IActionResult Update(Employee employee)
        {
            int idEmpoyee = (int)TempData["IdEmpoyee"];

            var sql = "UPDATE Employees SET surname = @Surname,  name = @Name, dateOfBirth = @DateOfBirth, departamentId = @DepartamentId, languageId = @LanguageId" +
                " WHERE id = @IdEmpoyee";
            using (IDbConnection db = Connection)
            {
                db.Execute(sql, new { employee.Surname, employee.Name, employee.DateOfBirth, employee.DepartamentId, employee.LanguageId, idEmpoyee });

                return RedirectToAction("Index");
            }
        }

        public IActionResult Edit(int id)
        {
            var employee = GetEmployee(id);
            var data = GetListLanguageAndDepartaments();
            var model = new ModelForEmployee { employee = employee, @enum = data };
            TempData["IdEmpoyee"] = id;
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            var sql = "INSERT INTO Employees (surname, name, dateOfBirth, departamentId, " +
                    "languageId) VALUES(@Surname, @Name, @DateOfBirth, @DepartamentId, @LanguageId)";

            using (IDbConnection db = Connection)
            {
                employee.DepartamentId = employee.Departament.Id;
                employee.LanguageId = employee.ProgrammingLanguage.Id;
                db.Execute(sql, new { employee.Surname, employee.Name, employee.DateOfBirth, employee.DepartamentId, employee.LanguageId });

                return RedirectToAction("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Delete(int id)
        {
            var sql = "DELETE FROM Employees WHERE id = @id";
            using (IDbConnection db = Connection)
            {
                db.Execute(sql, new { id });

                return RedirectToAction("Index");
            }


        }

        [NonAction]
        private IEnumerable<Employee> GetEmployees()
        {
            var sql = @"
            SELECT * FROM Employees
            
            SELECT * FROM Departaments

            SELECT * FROM ProgrammingLanguages
            ";
            using (var multi = Connection.QueryMultiple(sql))
            {
                var employees = multi.Read<Employee>();
                var departaments = multi.Read<Departament>();
                var languages = multi.Read<ProgrammingLanguage>();

                return employees.Select(x => new Employee
                {
                    Id = x.Id,
                    Name = x.Name,
                    DepartamentId = x.DepartamentId,
                    Surname = x.Surname,
                    DateOfBirth = x.DateOfBirth,
                    Departament = departaments.FirstOrDefault(y => y.Id == x.DepartamentId),
                    ProgrammingLanguage = languages.FirstOrDefault(y => y.Id == x.LanguageId),
                });
            }
        }
        [NonAction]
        private Employee GetEmployee(int id)
        {
            var sql = "SELECT * FROM Employees WHERE id = @id";
            using (IDbConnection db = Connection)
            {
                var result = db.Query<Employee>(sql,new { id }).First();

                return result;
            }
        }
        [NonAction]
        private EnumDepartamentsAndLanguage GetListLanguageAndDepartaments()
        {
            var sql = @"
            SELECT * FROM Departaments

            SELECT * FROM ProgrammingLanguages
            ";
            using (var multi = Connection.QueryMultiple(sql))
            {
                EnumDepartamentsAndLanguage model = new EnumDepartamentsAndLanguage
                {
                    departaments = multi.Read<Departament>(),

                    programmingLanguages = multi.Read<ProgrammingLanguage>()
                };

                return model;
            }


        }
        [NonAction]
        private async void GetEmployeesNames()
        {
            var sql = "SELECT name FROM Employees";
            using (IDbConnection db = Connection)
            {
                var result  = await Task.Run(()=>db.Query<string>(sql));

                names = result.ToList();
            }
        }

        private static List<string> names;
        public IEnumerable<string> AutocompleteSearch(string term)
        {
            var list = names.Where(n => n.Contains(term));
            var model = list.Distinct();
            

            return model;
        }
    }
}
