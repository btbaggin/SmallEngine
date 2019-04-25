using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public interface IMessageReceiver
    {
        void ReceiveMessage(IMessage pMessage);
    }
}
