#!/usr/local/share/dotnet/dotnet run
#:sdk Microsoft.NET.Sdk

var s = 0;
for(var i = 18; i >= 0; i--)
{
    s += 1 << i;
}
Console.WriteLine($"{s} < {1 << 19}");
