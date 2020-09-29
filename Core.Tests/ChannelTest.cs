using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Core.Tests
{
    public class ChannelTest
    {
        private readonly ITestOutputHelper _output;

        public ChannelTest(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Theory]
        [InlineData(100000, 10)]
        public async Task TestSPSC_Performance(int numItems, int numIterations)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            for (var i = 0; i < numIterations; ++i)
            {
                var channel = Channel.New<int>(100);
                var writer = Task.Factory.StartNew(() =>
                {
                    foreach (var num in Enumerable.Range(1, numItems))
                    {
                        channel.Send(num);
                    }

                    channel.Close();
                });
                
                var reader = Task.Factory.StartNew(() =>
                {
                    var res = new List<int>(numItems);
                    res.AddRange(channel.Range());

                    return res;
                });
                
                await Task.WhenAll(reader, writer);
            }

            stopWatch.Stop();

            var elapsedMs = stopWatch.Elapsed.TotalMilliseconds;
            
            _output.WriteLine("SPSC N = {0}: {1:.00}ms/iteration, {2:.00}ns/item (tx+rx)", numItems,
                elapsedMs / numIterations, elapsedMs * 1000.0 / numItems / numIterations);
        }
    }
}