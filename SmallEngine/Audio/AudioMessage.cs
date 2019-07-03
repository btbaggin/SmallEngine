using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Audio
{
    struct AudioMessage : Messages.IMessage
    {
        /// <inheritdoc/>
        public Messages.IMessageReceiver Recipient { get; private set; }

        /// <inheritdoc/>
        public Messages.IMessageReceiver Sender { get; private set; }

        /// <inheritdoc/>
        public string Type { get; private set; }

        /// <summary>
        /// The resource that will be played when this message is processed
        /// </summary>
        public AudioResource Resource { get; private set; }

        /// <summary>
        /// The volume at which the resource will play
        /// </summary>
        public float Volume { get; private set; }

        /// <summary>
        /// The ID of the sound playing instance
        /// </summary>
        public int ID { get; private set; }

        public AudioMessage(string pType, int pId)
        {
            Type = pType;
            ID = pId;
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
            ID = pId;
            Sender = null;
            Recipient = null;
        }

        /// <inheritdoc/>
        public T GetData<T>()
        {
            throw new NotSupportedException();
        }
    }
}
