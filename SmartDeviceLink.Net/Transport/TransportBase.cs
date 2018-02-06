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
        private readonly Task _receiveTask;
        protected IPacketParser Parser { get; }

        protected bool IsDisposed { get; private set; } = false;

        public TransportBase()
        {
            _cTok = new CancellationTokenSource();
            Parser = new ProtocolPacketParser(PacketReceived);
            _receiveTask = Task.Factory.StartNew(ReceiveBytes, _cTok.Token,TaskCreationOptions.None);
            _receiveTask.ConfigureAwait(false);
        }

        private void PacketReceived(TransportPacket packet)
        {
            OnReceivedPacket?.Invoke(packet);
        }

        public abstract Task ReceiveBytes(object o);
        public abstract Task SendAsync(TransportPacket packet);
        public abstract bool IsConnected { get; }
        public Action<TransportPacket> OnReceivedPacket { get; set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                _cTok.Cancel();
                OnReceivedPacket = null;
            }
            IsDisposed = true;
        }
    }
}
