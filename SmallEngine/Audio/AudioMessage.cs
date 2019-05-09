using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Audio
{
    struct AudioMessage : Messages.IMessage
    {
        public Messages.IMessageReceiver Recipient { get; private set; }

        public Messages.IMessageReceiver Sender { get; private set; }

        public string Type { get; private set; }

        public AudioResource Resource { get; private set; }

        public float Volume { get; private set; }

        public int Id { get; private set; }

        public AudioMessage(string pType, int pId)
        {
            Type = pType;
            Id = pId;
            Resource = null;
            Volume = 0f;
            Sender = null;
            Recipient = null;
        }

        public AudioMessage(string pType, AudioResource pResource, float pVolume, int pId)
        {
            Type = pType;
            Resource = pResource;
            Volume = pVolume;
            Id = pId;
            Sender = null;
            Recipient = null;
        }

        public T GetData<T>()
        {
            throw new NotSupportedException();
        }
    }
}
