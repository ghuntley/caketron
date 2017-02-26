namespace CakeTron.Core
{
    public interface IStartup
    {
        string FriendlyName { get; }

        void Start();
    }
}
