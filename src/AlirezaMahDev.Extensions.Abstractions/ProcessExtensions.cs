using System.Diagnostics;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class ProcessExtensions
{
    extension(Process)
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public static async Task<string> InvokeAsync(string command)
        {
            var split = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 0)
            {
                throw new ArgumentException("Command cannot be empty");
            }

            var commandName = split[0];
            var commandArgumand = split.Length > 1 ? string.Join(" ", split[1..]) : null;
            ProcessStartInfo psi = new()
            {
                FileName = commandName,
                Arguments = commandArgumand,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new();
            process.StartInfo = psi;

            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await outputTask;

            return !string.IsNullOrEmpty(await errorTask)
                ? throw new($"Error: {await errorTask} {output}")
                : output.Trim();
        }
    }
}