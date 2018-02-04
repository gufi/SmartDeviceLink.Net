using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Models;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Transport
{
    public abstract class TransportBase : ITransport
    {
        private readonly CancellationTokenSource _cTok;
        private readonly Task _recieveTask;
        protected IPacketParser Parser { get; }

        protected bool IsDisposed { get; private set; } = false;

        public TransportBase()
        {
            _cTok = new CancellationTokenSource();
            Parser = new ProtocolPacketParser(PacketRecieved);
            _recieveTask = Task.Run((Action)RecieveBytes, _cTok.Token); 
        }

        private void PacketRecieved(TransportPacket packet)
        {
            OnRecievedPacket?.Invoke(packet);
        }

        public abstract void RecieveBytes();
        public abstract Task SendAsync(TransportPacket packet);
        public abstract bool IsConnected { get; }
        public Action<TransportPacket> OnRecievedPacket { get; set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                _cTok.Cancel();
                OnRecievedPacket = null;
            }
            IsDisposed = true;
        }
    }
}
