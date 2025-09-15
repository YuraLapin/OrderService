using OrderService.DataAccess.Postgres.Models;
using System.Text.RegularExpressions;

namespace OrderService.WebApi.Utility
{
    public class InputChecker
    {
        private readonly int _maxEmailLength;
        private readonly string _allowedEmailChars;
        private static int _phoneNumberMaxSize = 15;

        public InputChecker(IConfiguration configuration)
        {
            _maxEmailLength = int.Parse(configuration["InputLimits:MaxEmailLength"] ?? "254");
            _allowedEmailChars = configuration["InputLimis:AllowedEmailChars"] ?? "";
        }

        public string? CheckId(long id)
        {
            if (id < 0) return "Id заказа не может быть меньше нуля";
            return null;
        }

        public string? CheckOrder(Order order)
        {
            string? idError = CheckId(order.ProductId);
            if (idError != null) return idError;

            string? emailError = CheckEmail(order.EmailClient);
            if (emailError != null) return emailError;

            string? priceError= CheckPrice(order.Price);
            if (priceError != null) return priceError;

            string? phoneError = CheckPhoneNumber(order.PhoneNumber);
            if (phoneError != null) return phoneError;

            return null;
        }

        private string? CheckEmail(string email)
        {
            if (email.Count() > _maxEmailLength) return "Почта клиента слишком длинная";

            if (_allowedEmailChars.Count() > 0)
            {
                foreach (char c in email)
                {
                    if (!_allowedEmailChars.Contains(c)) return "В почте клиента обнаружен недопустимый символ";
                }
            }

            string pattern = @"[^@ \t\r\n]+@[^@ \t\r\n]+\.[^@ \t\r\n]+";
            if (!Regex.IsMatch(email, pattern)) return "Почта клиента не действительна";

            return null;
        }

        private string? CheckPrice(decimal price)
        {
            if (price < 0) return "Сумма заказа не может быть отрицательным числом";
            return null;
        }

        private string? CheckPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.Count() > _phoneNumberMaxSize) return $"Номер телефона клиента не действителен 1 {phoneNumber}";

            string pattern = @"^[\+]?[0-9][\s]??[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{3,5}$";
            if (!Regex.IsMatch(phoneNumber, pattern)) return $"Номер телефона клиента не действителен 2 {phoneNumber}";

            return null;
        }
    }
}
