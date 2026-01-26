using FluentValidation;
using OrderService.DataAccess.Postgres.Models;
using System.Text.RegularExpressions;

namespace OrderService.WebApi.Validators
{
    /// <summary>
    /// FluentValidation валидатор для проверки Order
    /// </summary>
    public class OrderValidator : AbstractValidator<Order>
    {
        private static int _phoneNumberMinSize = 1;
        private static int _phoneNumberMaxSize = 15;
        private static int _minEmailLength = 4;
        private readonly int _maxEmailLength;
        private readonly string _allowedEmailChars;

        public OrderValidator(IConfiguration configuration)
        {
            _maxEmailLength = int.Parse(configuration["InputLimits:MaxEmailLength"] ?? "254");
            _allowedEmailChars = configuration["InputLimits:AllowedEmailChars"] ?? "";

            RuleFor(o => o.ProductId).NotEmpty().WithMessage("ProductId должен быть заполнен");
            RuleFor(o => o.ProductId).GreaterThan(-1).WithMessage("ProductId не может быть отрицательным");

            RuleFor(o => o.EmailClient).NotEmpty().WithMessage("Email должен быть заполнен");
            RuleFor(o => o.EmailClient).Must(HaveOnlyAllowedChars).WithMessage("В почте клиента обнаружен недопустимый символ");
            RuleFor(o => o.EmailClient).Must(MatchEmailPattern).WithMessage("Почта клиента не действительна");

            RuleFor(o => o.Price).NotEmpty().WithMessage("Price должен быть заполнен");
            RuleFor(o => o.Price).GreaterThan(-1).WithMessage("Сумма заказа не может быть меньше нуля");

            RuleFor(o => o.PhoneNumber).NotEmpty().WithMessage("PhoneNumber должен быть заполнен");
            RuleFor(o => o.PhoneNumber).Must(MatchPhoneNumberPattern).WithMessage("Номер телефона клиента не действителен");
        }

        private bool HaveOnlyAllowedChars(string str)
        {
            if (_allowedEmailChars.Count() > 0)
            {
                foreach (char c in str)
                {
                    if (!_allowedEmailChars.Contains(c)) return false;
                }
            }

            return true;
        }

        private bool MatchEmailPattern(string email)
        {
            string pattern = @"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+";
            return Regex.IsMatch(email, pattern);
        }

        private bool MatchPhoneNumberPattern(string phoneNumber)
        {
            string pattern = @"^[\+]?[0-9][\s]??[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{3,5}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
    }
}
