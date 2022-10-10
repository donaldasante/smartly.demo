using SmartlyDemo.RiotSPA.Domain.Model.Salary;

namespace SmartlyDemo.RiotSPA.Domain.Model.Employee
{
    public class Employee
    {
        public virtual string FirstName { get; protected set; }
        public virtual string Surname { get; protected set; }
        public virtual SalaryDetails Salary { get; protected set; } = null;

        public Employee(
            string firstName,
            string surname)
        {
            FirstName = firstName;
            Surname = surname;
        }

        public void SetSalaryDetails(
            decimal annualGrossSalary,
            decimal superRatePercentage,
            string monthOfTheYear)
        {
            Salary = new SalaryDetails(annualGrossSalary, superRatePercentage, monthOfTheYear);
        }
    }
}
