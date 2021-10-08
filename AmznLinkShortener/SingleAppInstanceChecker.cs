using System;
using System.Threading;

namespace AmznLinkShortener
{
    public static class SingleAppInstanceChecker
    {
        /// <summary>
        /// Arbitrary unique string
        /// </summary>
        private static Mutex _mutex = new Mutex(true, "0d12ad74-026f-40c3-bdae-e178ddee8602");

        public static bool IsNotRunning()
        {
            return _mutex.WaitOne(TimeSpan.Zero, true);
        }
    }
}
