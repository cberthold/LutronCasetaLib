using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Example.Core
{
    public class AsyncSynchronizationContext : SynchronizationContext
    {
        public override void Send(SendOrPostCallback callback, object state)
        {
            try
            {
                callback(state);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            try
            {
                callback(state);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }

}
