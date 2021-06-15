using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeCensus4.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int DepartamentId { get; set; }

        public Departament Departament { get; set; }

        public int LanguageId { get; set; }

        public ProgrammingLanguage ProgrammingLanguage { get; set; }

        public Employee()
        {
            DateOfBirth = DateTime.Now;
        }
    }
}
