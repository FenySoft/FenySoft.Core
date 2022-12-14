namespace FenySoft.Core.Threading
{
    public class TCountdown
    {
        private long count; 
        
        public void Increment()
        {
            Interlocked.Increment(ref count);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref count);
        }

        public void Wait()
        {
            SpinWait wait = new SpinWait();

            wait.SpinOnce();

            while (Count > 0)
                Thread.Sleep(1);
        }

        public long Count
        {
            get { return Interlocked.Read(ref count); }
        }
    }
}
