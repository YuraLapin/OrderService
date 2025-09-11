namespace OrderServiceDataBase.Models
{
    public class CompletableOrder: Order
    {
        public bool IsComplete { get; set; }

        public CompletableOrder(Order order)
        {
            Id = order.Id;
            Sum = order.Sum;
            ClientName = order.ClientName;
            IsComplete = false;
        }

        public CompletableOrder(Order order, bool isComplete)
        {
            Id = order.Id;
            Sum = order.Sum;
            ClientName = order.ClientName;
            IsComplete = isComplete;
        }
    }
}
