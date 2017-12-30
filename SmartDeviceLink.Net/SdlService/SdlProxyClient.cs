using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.SdlService
{
    /// <summary>
    /// Based on sdl_Android Sdl Proxy Base
    /// </summary>
    public class SdlProxyClient : IDisposable
    {
        private bool isDisposed = false;
        private ConcurrentQueue<RpcRequest> _rpcQueue = new ConcurrentQueue<RpcRequest>();
        public SdlProxyClient(string appName, string appId ,bool isMediaApp)//, LanguageType language, LanguageType )
        {
            
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                
            }
        }


        public void Send(RpcRequest request)
        {
            // convert rpcRequest to json byte array ( JsonConvert.Serialize(request).ToByteArray())
            _rpcQueue.Enqueue(request);
        }

        public Task SendAsync(RpcRequest request)
        {
            return Task.FromResult(0);
        }
    }
}
