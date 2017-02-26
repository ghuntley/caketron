namespace CakeTron.Core
{
    public interface IAdapter
    {
        string FriendlyName { get; }
        IBroker Broker { get; }
    }
}
