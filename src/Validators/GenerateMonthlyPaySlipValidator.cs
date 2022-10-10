using FluentValidation;
using SmartlyDemo.RiotSPA.Contracts.Requests;
using SmartlyDemo.RiotSPA.ExtensionMethods;


namespace SmartlyDemo.RiotSPA.Validators
{
    public class GenerateMonthlyPaySlipValidator :
        Validator<GenerateMonthlyPaySlipReq>
    {
        public GenerateMonthlyPaySlipValidator()
        {
            RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Please Enter a FirstName")
            .MinimumLength(1)
            .WithMessage("Your FirstName is too short")
            .MaximumLength(100)
            .WithMessage("Your FirstName is too long");

            RuleFor(x => x.Surname)
            .NotNull()
            .NotEmpty()
            .WithMessage("Please Enter a Surname")
            .MinimumLength(1)
            .WithMessage("Your Surname is too short")
            .MaximumLength(100)
            .WithMessage("Your Surname is too long");

            RuleFor(x => x.AnnualGrossSalary)
            .GreaterThanOrEqualTo(100)
            .WithMessage("You probably need a new job ;-)");

            RuleFor(x => x.SuperRatePercentage)
            .NotEmpty()
            .WithMessage("Please enter your super rate as a percentage")
            .InclusiveBetween(0M, 0.5M)
            .WithMessage("Super Rate needs to be between 0 and 50%.");

            RuleFor(x => x.MonthOfTheYear)
            .NotNull()
            .NotEmpty()
            .WithMessage("Please Enter a Month of the year")
            .Must(StringExtensionMethods.IsConvertibleToMonth)
            .WithMessage("Please enter a valid month e.g. January, February");
        }
    }
}
