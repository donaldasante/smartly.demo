using SmartlyDemo.RiotSPA.Domain.Model.Salary;

namespace SmartlyDemo.RiotSPA.Domain.Interface
{
    public interface ITaxService
    {
        void CalculateMonthlyPayslipForEmployee(SalaryDetails salaryDetails);
    }
}
