﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Messages
{
    public interface IMessage
    {
        IMessageReceiver Recipient { get; }
        IMessageReceiver Sender { get; }
        string Type { get; }
        T GetData<T>();
    }
}
