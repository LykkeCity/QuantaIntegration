namespace Core
{
    public class Constants
    {
        public const string EmailNotifierQueue = "emailsqueue";
        public const string SlackNotifierQueue = "slack-notifications";

        public const string UserContractQueue = "user-contracts";
        public const string TransactionMonitoringQueue = "transaction-monitoring";

        public const int GasForUserContractDeploy = 500000;
        public const int GasForTransfer = 200000;
        public const int GasForQuantaContractCreation = 500000;

        public const string IssueNotifyQueue = "quanta-in";
        public const string CashoutQueue = "quanta-out";

        public const int QuantaCoinDecimals = 0;
    }
}