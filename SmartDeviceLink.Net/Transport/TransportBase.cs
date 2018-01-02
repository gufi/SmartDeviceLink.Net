using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Transport
{
    public abstract class TransportBase : ITransport
    {
        private readonly CancellationTokenSource _cTok;
        private readonly Task _recieveTask;
        private readonly Action<TransportPacket> _onRecievedPacket;
        protected IPacketParser Parser { get; }

        protected bool IsDisposed { get; private set; } = false;

        public TransportBase(Action<TransportPacket> onRecievedPacket)
        {
            _onRecievedPacket = onRecievedPacket;
            _cTok = new CancellationTokenSource();
            Parser = new ProtocolPacketParser(_onRecievedPacket);
            _recieveTask = Task.Run((Action)RecieveBytes, _cTok.Token); 
        }

        

        public abstract void RecieveBytes();
        public abstract Task SendAsync(TransportPacket packet);
        public abstract bool IsConnected { get; }
        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                _cTok.Cancel();
            }
            IsDisposed = true;
        }
    }
}
