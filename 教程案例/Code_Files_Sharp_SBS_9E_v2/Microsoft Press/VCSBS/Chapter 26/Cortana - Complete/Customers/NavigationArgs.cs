namespace Customers
{
    internal class NavigationArgs
    {
        internal string commandMode { get; }
        internal string customerName { get; }

        public NavigationArgs(string customerName, string commandMode)
        {
            this.customerName = customerName;
            this.commandMode = commandMode;
        }
    }
}