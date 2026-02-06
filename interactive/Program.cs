#!/usr/local/share/dotnet/dotnet run
#:sdk Microsoft.NET.Sdk
#:project ../src/AlirezaMahDev.Extensions/AlirezaMahDev.Extensions.csproj
#:project ../src/AlirezaMahDev.Extensions.Abstractions/AlirezaMahDev.Extensions.Abstractions.csproj

using System.Runtime.InteropServices;

using AlirezaMahDev.Extensions.Abstractions;

//                          0, 1, 2, 3, 4, 5, 6, 7, 8, 9
Memory<double> memory = new([1, 1.1, 2, 2.1, 2.2, 3, 3.1, 3.2, 5.1, 5.2]);
var comparisonChain = ComparisonChain<double>
    .ChainOrderBy(x => (int)x)
    .ChainOrderBy(x => x % 1);

var near = memory.Near(2, comparisonChain, 1);

foreach (var item in near)
{
    Console.WriteLine(item);
}

Console.WriteLine();

