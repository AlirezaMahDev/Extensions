#!/usr/local/share/dotnet/dotnet run
#:sdk Microsoft.NET.Sdk
#:project ../src/AlirezaMahDev.Extensions/AlirezaMahDev.Extensions.csproj
#:project ../src/AlirezaMahDev.Extensions.Abstractions/AlirezaMahDev.Extensions.Abstractions.csproj

using AlirezaMahDev.Extensions.Abstractions;

for (int i = 0; i < 1000; i++)
{
    int size = Random.Shared.Next(0, 1000000);
    using var rent = SmartMemoryPool<int>.Shared.Rent(size);
    Console.WriteLine($"{rent.Memory.Length}=={size}");
}