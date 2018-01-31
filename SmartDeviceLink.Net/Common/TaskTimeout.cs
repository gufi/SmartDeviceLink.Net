using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceLink.Net.Common
{
    public static class TaskTimeout
    {
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, int ms)
        {
            if (task == await Task.WhenAny(task, Task.Delay(ms)))
                return await task;
            else
                throw new TimeoutException();
        }

        public static async Task TimeoutAfter(this Task task, int ms)
        {
            if (task == await Task.WhenAny(task, Task.Delay(ms)))
                    await task;
            else
                throw new TimeoutException();
        }
    }
}
