using CakeTron.Core.Contexts;

namespace CakeTron.Core
{
    public abstract class RobotPart
    {
        public virtual void Initialize()
        {
            
        }

        public abstract bool Handle(MessageContext context);
    }
}
