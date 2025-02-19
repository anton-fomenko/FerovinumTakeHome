using FerovinumTakeHome.Services;

namespace FerovinumTakeHome.Tests;

public class IntegrationTests
{
    [Fact]
    public void FollowSampleIO()
    {
        var processor = new FerovinumProcessor();
        var outputs = new List<List<string>>();

        outputs.Add(processor.ProcessCommand("sell wine 1000"));   // #1
        outputs.Add(processor.ProcessCommand("sell whisky 100")); // #2
        outputs.Add(processor.ProcessCommand("buy wine 500"));    // #3
        outputs.Add(processor.ProcessCommand("buy wine 1000"));   // #4
        outputs.Add(processor.ProcessCommand("buy wine 500"));    // #5
        outputs.Add(processor.ProcessCommand("sell whisky 100")); // #6
        outputs.Add(processor.ProcessCommand("buy whisky 120"));  // #7

        // Assert each step with expected lines...
        // 1) After "sell wine 1000"
        var expected1 = new List<string>() {
            "sell wine 1000 remaining:1000"
        };
        Assert.Equal(expected1, outputs[0]);

        // 2) After "sell whisky 100"
        var expected2 = new List<string>() {
            "sell wine 1000 remaining:1000",
            "sell whisky 100 remaining:100"
        };
        Assert.Equal(expected2, outputs[1]);

        // 3) After "buy wine 500"
        var expected3 = new List<string>() {
            "sell wine 1000 remaining:500",
            "sell whisky 100 remaining:100",
            "buy wine 500 closed"
        };
        Assert.Equal(expected3, outputs[2]);

        // 4) After "buy wine 1000"
        var expected4 = new List<string>() {
            "sell wine 1000 closed",
            "sell whisky 100 remaining:100",
            "buy wine 500 closed",
            "buy wine 500 closed"
        };
        Assert.Equal(expected4, outputs[3]);

        // 5) After "buy wine 500"
        // same as #4 => no new order
        Assert.Equal(expected4, outputs[4]);

        // 6) After "sell whisky 100"
        var expected6 = new List<string>() {
            "sell wine 1000 closed",
            "sell whisky 100 remaining:100",
            "buy wine 500 closed",
            "buy wine 500 closed",
            "sell whisky 100 remaining:100"
        };
        Assert.Equal(expected6, outputs[5]);

        // 7) After "buy whisky 120"
        var expected7 = new List<string>() {
            "sell wine 1000 closed",
            // The FIRST whisky sell (from #2) is fully used => "closed"
            "sell whisky 100 closed",
            "buy wine 500 closed",
            "buy wine 500 closed",
            // The SECOND whisky sell (from #6) is partially used => "remaining:80"
            "sell whisky 100 remaining:80",
            "buy whisky 120 closed"
        };
        Assert.Equal(expected7, outputs[6]);
    }
}
