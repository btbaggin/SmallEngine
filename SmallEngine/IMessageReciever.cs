using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public interface IMessageReceiver
    {
        void ReceiveMessage(GameMessage pMessage);
    }
}
