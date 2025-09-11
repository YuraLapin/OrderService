namespace OrderServiceMain.Utility
{
    public class InputChecker
    {
        private readonly int _maxClientName;
        private readonly string _allowedNameChars;

        public InputChecker(IConfiguration configuration)
        {
            _maxClientName = int.Parse(configuration["InputLimits:MaxClientLength"] ?? "50");
            _allowedNameChars = configuration["InputLimis:AllowedNameChars"] ?? "";
        }

        public string? CheckOrderId(int id)
        {
            if (id < 0) return "Id заказа не может быть меньше нуля";
            return null;
        }

        public string? CheckOrder(int sum, string clientName)
        {
            string? sumError = CheckSum(sum);
            if (sumError != null) return sumError;

            string? nameError = CheckClientName(clientName);
            if (nameError != null) return nameError;

            return null;
        }
        
        private string? CheckSum(int sum)
        {
            if (sum < 0) return "Сумма заказа не может быть отрицательным числом";
            return null;
        }

        private string? CheckClientName(string clientName)
        {
            if (clientName.Count() > _maxClientName) return "Имя клиента слишком длинное";
            if (_allowedNameChars.Count() > 0)
            {
                foreach (char c in clientName)
                {
                    if (!_allowedNameChars.Contains(c)) return "недопустимый символ в имени клиента";
                }
            }
            return null;
        }
    }
}
