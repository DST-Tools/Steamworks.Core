using System;
using System.Collections.Generic;
using System.Text;

namespace Steamworks.Core
{
    static class CallbackFactory
    {
        unsafe internal delegate void InternalHandler(void* A_0, bool A_1);
#if IA32
        [System.Runtime.InteropServices.DllImport("APICallbackFactoryx86")]
#elif AMD64
        [System.Runtime.InteropServices.DllImport("APICallbackFactoryx64")]
#endif
        internal static extern IntPtr MakeCallResult(int callbackID, CallResultFlags flags, ulong apiCallHandle, int size, InternalHandler handler);

    }
    /// <summary>
    /// Flags
    /// </summary>
    public enum CallResultFlags : byte
    {
        /// <summary>No other options.</summary>
        None = 0,
        ///<summary>Steamworks internal.</summary>
        k_ECallbackFlagsRegistered = 1,
        /// <summary>Make this callresult as game server call.</summary>
        k_ECallbackFlagsGameServer = 2
    }
    /// <summary>
    /// Provide access to Callresult System.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CallResult<T> where T : unmanaged
    {
        /// <summary>
        /// Callresult handler
        /// </summary>
        /// <param name="result"></param>
        /// <param name="ioFail"></param>
        public delegate void CallResultHandler(ref T result, bool ioFail);

        /// <summary>
        /// This will be invoked when result is received.
        /// </summary>
        public event CallResultHandler OnReceivedResult;

        /// <summary>
        /// Create A Call result listener
        /// </summary>
        /// <param name="APICallHandle"></param>
        /// <param name="k_iCallback"></param>
        /// <param name="flags"></param>
        public unsafe CallResult(ulong APICallHandle, int k_iCallback, CallResultFlags flags = CallResultFlags.None)
        {
            CallbackBase = CallbackFactory.MakeCallResult(k_iCallback, flags, APICallHandle, sizeof(T), InternalCallresultHandler);
        }

        private IntPtr CallbackBase = IntPtr.Zero;
        private unsafe void InternalCallresultHandler(void* param, bool iofail)
        {
            T result = *(T*)param;
            OnReceivedResult(ref result, iofail);
        }
    }
}
