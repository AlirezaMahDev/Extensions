using System.Text.Json.Serialization;

namespace Parto.Extensions.Abstractions;

[method: JsonConstructor]
public record TaskStatusState(bool Success, string Title, string Message) : StatusState(Title, Message);

//
// public async Task UpdateChart(CancellationToken cancellationToken = default)
// {
//     try
//     {
//         await foreach (var ohlc in Chart(cancellationToken))
//         {
//             ChartCollection.AddOrUpdate(x => x.OpenTimestamp == ohlc.OpenTimestamp, ohlc);
//
//             if (ChartCollection.Count - 180 is var cleanCount and > 60)
//             {
//                 for (var i = 0; i < cleanCount; i++)
//                 {
//                     ChartCollection.RemoveAt(0);
//                 }
//             }
//         }
//     }
//     catch (Exception e)
//     {
//         Console.WriteLine(e);
//         throw;
//     }
// }
//
// public async Task UpdateLog(CancellationToken cancellationToken = default)
// {
//     try
//     {
//         await foreach (var log in Log(cancellationToken))
//         {
//             LogCollection.Insert(0, log);
//             if (LogCollection.Count - 200 > 100)
//             {
//                 foreach (var traderLog in LogCollection.TakeLast(100).ToArray())
//                 {
//                     LogCollection.Remove(traderLog);
//                 }
//             }
//         }
//     }
//     catch (Exception e)
//     {
//         Console.WriteLine(e);
//         throw;
//     }
// }