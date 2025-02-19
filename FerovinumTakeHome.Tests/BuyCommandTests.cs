using FerovinumTakeHome.Commands;
using FerovinumTakeHome.Domain;

namespace FerovinumTakeHome.Tests
{
    public class BuyCommandTests
    {
        [Fact]
        public void Execute_NoSells_ShouldNotCreateBuyOrder()
        {
            // Arrange
            var context = new FerovinumContext();
            var cmd = new BuyCommand("wine", 500);

            // Act
            cmd.Execute(context);

            // Assert
            Assert.Empty(context.AllOrders);
            Assert.Empty(context.SellQueuesBySku);
        }

        [Fact]
        public void Execute_MoreThanEnoughStock_ShouldConsumeFromSameFirstSell()
        {
            // Arrange
            var context = new FerovinumContext();
            var sellCmd1 = new SellCommand("wine", 1000);
            sellCmd1.Execute(context);

            var sellCmd2 = new SellCommand("wine", 700);
            sellCmd2.Execute(context);

            // Act
            var buyCmd1 = new BuyCommand("wine", 500);
            buyCmd1.Execute(context);

            var buyCmd2 = new BuyCommand("wine", 500);
            buyCmd2.Execute(context);

            // Assert
            // We expect: 
            //  - Both buy orders should be fulfilled from first sell. 
            //  - Second sell remains untouched with 700 remaining.

            Assert.Equal(4, context.AllOrders.Count);

            Assert.Equal("sell wine 1000 closed", context.AllOrders[0].ToString());
            Assert.Equal("sell wine 700 remaining:700", context.AllOrders[1].ToString());
            Assert.Equal("buy wine 500 closed", context.AllOrders[2].ToString());
            Assert.Equal("buy wine 500 closed", context.AllOrders[3].ToString());
        }

        [Fact]
        public void Execute_FullFulfillment_ShouldCloseSell()
        {
            // Arrange
            var context = new FerovinumContext();
            var sellCmd = new SellCommand("wine", 1000);
            sellCmd.Execute(context);

            // Act
            var buyCmd = new BuyCommand("wine", 1000);
            buyCmd.Execute(context);

            // Assert
            // Sell is closed, and a buy order is added
            Assert.Equal(2, context.AllOrders.Count);

            var sellOrder = context.AllOrders[0] as SellOrder;
            Assert.True(sellOrder.IsClosed);
            Assert.Equal(0, sellOrder.Remaining);
            Assert.Equal("sell wine 1000 closed", context.AllOrders[0].ToString());

            var buyOrder = context.AllOrders[1] as BuyOrder;
            Assert.True(buyOrder.IsClosed);
            Assert.Equal(1000, buyOrder.FilledQuantity);
            Assert.Equal("buy wine 1000 closed", context.AllOrders[1].ToString());
        }

        [Fact]
        public void Execute_SomeButNotEnoughStock_ShouldPartiallyFill()
        {
            // Arrange
            var context = new FerovinumContext();
            // Sell only 300 wine
            var sellCmd = new SellCommand("wine", 300);
            sellCmd.Execute(context);

            // Act: attempt to buy 500
            var buyCmd = new BuyCommand("wine", 500);
            buyCmd.Execute(context);

            // Assert
            // We expect the buy order is 300, fully closed, 
            // but the request for 200 more is unfilled => no error, just partial fill
            Assert.Equal(2, context.AllOrders.Count);

            var sellOrder = context.AllOrders[0] as SellOrder;
            Assert.True(sellOrder.IsClosed);
            Assert.Equal(0, sellOrder.Remaining);
            Assert.Equal("sell wine 300 closed", context.AllOrders[0].ToString());

            var buyOrder = context.AllOrders[1] as BuyOrder;
            Assert.Equal(300, buyOrder.FilledQuantity);
            Assert.True(buyOrder.IsClosed);
            Assert.Equal("buy wine 300 closed", context.AllOrders[1].ToString());
        }
    }
}
