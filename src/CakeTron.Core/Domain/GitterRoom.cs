using System.Diagnostics;

namespace CakeTron.Core.Domain
{
    [DebuggerDisplay("{Name,nq} {Id,nq}")]
    public sealed class GitterRoom
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool OneToOne { get; set; }
    }
}
