namespace CakeTron.Core.Domain
{
    public sealed class Message
    {
        public User User { get; set; }
        public string Text { get; set; }
    }
}