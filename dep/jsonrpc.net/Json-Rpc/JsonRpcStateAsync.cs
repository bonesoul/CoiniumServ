using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AustinHarris.JsonRpc
{
    public class JsonRpcStateAsync : IAsyncResult
    {
        public JsonRpcStateAsync(AsyncCallback cb, Object extraData)
        {
            this.cb = cb;
            asyncState = extraData;
            isCompleted = false;
        }

        public string JsonRpc { get; set; }
        public string Result { get; set; }

        private AsyncCallback cb = null;
        private Object asyncState;
        public object AsyncState
        {
            get
            {
                return asyncState;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return false;
            }
        }

        // If this object was not being used solely with ASP.Net this
        // method would need an implementation. ASP.Net never uses the
        // event, so it is not implemented here.
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                // not supported
                return null;
            }
        }

        private Boolean isCompleted;
        public bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
        }

        internal void SetCompleted()
        {
            isCompleted = true;
            if (cb != null)
            {
                cb(this);
            }
        }
    }
}
