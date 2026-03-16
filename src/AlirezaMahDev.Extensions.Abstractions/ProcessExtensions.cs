using System.Diagnostics;

namespace AlirezaMahDev.Extensions.Abstractions;

public static class ProcessExtensions
{
    extension(Process)
    {
        public static async Task<string> InvokeAsync(string command)
        {
            string[] split = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 0)
            {
                throw new ArgumentException("Command cannot be empty");
            }

            string commandName = split[0];
            string? commandArgumand = split.Length > 1 ? string.Join(" ", split[1..]) : null;
            ProcessStartInfo psi = new()
            {
                FileName = commandName,
                Arguments = commandArgumand,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new() { StartInfo = psi };

            process.Start();

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string output = await outputTask;

            return !string.IsNullOrEmpty(await errorTask)
                ? throw new($"Error: {await errorTask} {output}")
                : output.Trim();
        }
    }
}