using System;
using System.Runtime.InteropServices;

namespace TaskEngine.Helpers
{
    public class GTimer
    {
        private readonly double countDownTime;

        private long long0;
        private long long1;

        [DllImport("kernel32.dll")]
        private static extern int QueryPerformanceCounter(ref long l);

        [DllImport("kernel32.dll")]
        private static extern int QueryPerformanceFrequency(ref long l);

        public GTimer()
        {
            this.countDownTime = 0.0;
            this.method_3(GTimer.smethod_1());
            this.Reset();
        }

        public GTimer(double countDowntime)
        {
            this.countDownTime = countDowntime;
            this.method_3(GTimer.smethod_1());
            this.Reset();
        }

        private long method_0()
        {
            return this.long0;
        }

        private void method_1(long long_2)
        {
            this.long0 = long_2;
        }

        private long method_2()
        {
            return this.long1;
        }

        private void method_3(long long_2)
        {
            this.long1 = long_2;
        }

        public bool IsReady()
        {
            try
            {
                if (this.Peek() > this.countDownTime)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public void Reset()
        {
            this.method_1(GTimer.smethod_0());
        }

        public long Peek()
        {
            return (long)((double)(GTimer.smethod_0() - this.method_0()) / (double)this.method_2() * 10000.0);
        }

        private static long smethod_0()
        {
            long result = 0L;
            if (GTimer.QueryPerformanceCounter(ref result) == 0)
            {
                throw new NotSupportedException("Error while querying the high-resolution performance counter.");
            }
            return result;
        }

        private static long smethod_1()
        {
            long result = 0L;
            if (GTimer.QueryPerformanceFrequency(ref result) == 0)
            {
                throw new NotSupportedException("Error while querying the performance counter frequency.");
            }
            return result;
        }
    }
}