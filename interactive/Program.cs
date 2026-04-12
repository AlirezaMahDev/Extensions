#!/usr/local/share/dotnet/dotnet run
#:sdk Microsoft.NET.Sdk
#:project ../src/AlirezaMahDev.Extensions/AlirezaMahDev.Extensions.csproj
#:project ../src/AlirezaMahDev.Extensions.Abstractions/AlirezaMahDev.Extensions.Abstractions.csproj

using AlirezaMahDev.Extensions.Abstractions;

using var rent = SmartMemoryPool<int>.Shared.Rent(100);
Console.WriteLine(rent.Memory.Length);
