namespace Core.IssueNotifier
{
    public class IssueNotifyMessage
    {
        public string Contract { get; set; }

        public decimal Amount { get; set; }

        public string TransactionHash { get; set; }
    }
}
