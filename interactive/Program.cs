#!/usr/local/share/dotnet/dotnet run
#:sdk Microsoft.NET.Sdk
#:project ../src/AlirezaMahDev.Extensions/AlirezaMahDev.Extensions.csproj
#:project ../src/AlirezaMahDev.Extensions.Abstractions/AlirezaMahDev.Extensions.Abstractions.csproj

using AlirezaMahDev.Extensions.Abstractions;

for (int i = 0; i < 1000; i++)
{
    using var rent = SmartMemoryPool<int>.Shared.Rent(Random.Shared.Next(0, 1000000));
    Console.WriteLine(rent.Memory.Length);
}