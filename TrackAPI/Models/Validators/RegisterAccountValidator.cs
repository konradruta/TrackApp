using FluentValidation;
using TrackAPI.Entities;
using TrackAPI.Models;

namespace VueApp1.Server.Models.Validators
{
    public class RegisterAccountValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterAccountValidator(TrackDbContext dbContext)
        {
            RuleFor(a => a.Password)
                .MinimumLength(8)
                .NotEmpty();

            RuleFor(a => a.ConfirmPassword)
                .MinimumLength(8)
                .Equal(p => p.Password).WithMessage("The password are diffrents");

            RuleFor(u => u.UserName)
                .Custom((value, context) =>
                {
                    var takenName = dbContext.Users.Any(u => u.UserName == value);

                    if (takenName)
                    {
                        context.AddFailure("That username is taken");
                    }
                });

            RuleFor(u => u.Email)
                .Custom((value, context) =>
                {
                    var takenEmail = dbContext.Users.Any(u => u.Email == value);

                    if (takenEmail)
                    {
                        context.AddFailure("That e-mail is taken");
                    }
                });
        }
    }
}
