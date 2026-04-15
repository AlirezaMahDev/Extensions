namespace AlirezaMahDev.Extensions.Abstractions;

public static class LockerStatusExtensions
{
    extension<TSelf>(TSelf self)
        where TSelf : ILockerStatus
    {
        public bool IsFree
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                var state = self.LockerState;
                return state.ThreadId == Environment.CurrentManagedThreadId || state is { ThreadId: 0, ReaderCount: 0, WriterCount: 0 };
            }
        }

        public int ThreadId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.LockerState.ThreadId;
            }
        }

        public int WriterCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.LockerState.WriterCount is var writerCount && writerCount < 0 ? -writerCount : writerCount;
            }
        }
        public bool HasWriter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.WriterCount > 0;
            }
        }

        public int ReaderCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.LockerState.ReaderCount;
            }
        }

        public bool HasReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return self.ReaderCount > 0;
            }
        }
    }
}