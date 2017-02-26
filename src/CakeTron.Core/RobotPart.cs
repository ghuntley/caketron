using CakeTron.Core.Contexts;

namespace CakeTron.Core
{
    public abstract class RobotPart
    {
        public abstract bool Handle(MessageContext context);
    }
}
