namespace AlirezaMahDev.Extensions.Abstractions;

public static class ThreadPoolExtensions
{
    extension(Task task)
    {
        public async Task AsTaskRun()
        {
            if (task.IsCompleted)
            {
                await task;
            }
            else
            {
                await task.AsTaskRunCore();
            }
        }

        private async Task AsTaskRunCore()
        {
            await Task.Run(async () => await task);
        }
    }

    extension<T>(Task<T> task)
    {
        public async Task<T> AsTaskRun()
        {
            return task.IsCompleted ? await task : await task.AsTaskRunCore();
        }

        private async Task<T> AsTaskRunCore()
        {
            return await Task.Run(async () => await task);
        }
    }

    extension(ValueTask valueTask)
    {
        public async ValueTask AsTaskRun()
        {
            if (valueTask.IsCompleted)
            {
                await valueTask;
            }
            else
            {
                await valueTask.AsTask().AsTaskRunCore();
            }
        }
    }

    extension<T>(ValueTask<T> valueTask)
    {
        public async ValueTask<T> AsTaskRun()
        {
            return valueTask.IsCompleted ? await valueTask : await valueTask.AsTask().AsTaskRunCore();
        }
    }

    extension(ConfiguredTaskAwaitable awaitable)
    {
        public async ValueTask AsTaskRun()
        {
            if (awaitable.GetAwaiter().IsCompleted)
            {
                await awaitable;
            }

            await Task.Run(async () => await awaitable);
        }
    }

    extension<T>(ConfiguredTaskAwaitable<T> awaitable)
    {
        public async ValueTask<T> AsTaskRun()
        {
            return awaitable.GetAwaiter().IsCompleted ? await awaitable : await Task.Run(async () => await awaitable);
        }
    }

    extension(ConfiguredValueTaskAwaitable awaitable)
    {
        public async ValueTask AsTaskRun()
        {
            if (awaitable.GetAwaiter().IsCompleted)
            {
                await awaitable;
            }

            await Task.Run(async () => await awaitable);
        }
    }

    extension<T>(ConfiguredValueTaskAwaitable<T> awaitable)
    {
        public async ValueTask<T> AsTaskRun()
        {
            return awaitable.GetAwaiter().IsCompleted ? await awaitable : await Task.Run(async () => await awaitable);
        }
    }
}