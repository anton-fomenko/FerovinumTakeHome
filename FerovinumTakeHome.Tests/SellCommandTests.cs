using FerovinumTakeHome.Commands;
using FerovinumTakeHome.Domain;

namespace FerovinumTakeHome.Tests
{
    public class SellCommandTests
    {
        [Fact]
        public void Execute_InvalidSku_ShouldNotCreateSellOrder()
        {
            // Arrange
            var context = new FerovinumContext();
            var cmd = new SellCommand("cheese", 500);

            // Act
            cmd.Execute(context);

            // Assert
            Assert.Empty(context.AllOrders);
            Assert.Empty(context.SellQueuesBySku);
        }

        [Fact]
        public void Execute_ShouldCreateSellOrder_AndEnqueueToFifo()
        {
            // Arrange
            var context = new FerovinumContext();
            var cmd = new SellCommand("wine", 1000);

            // Act
            cmd.Execute(context);

            // Assert
            Assert.Single(context.AllOrders);
            Assert.Contains("wine", context.AllOrders[0].SKU);
            Assert.Equal(1000, context.AllOrders[0].OriginalQuantity);
            Assert.False(context.AllOrders[0].IsClosed);

            Assert.Single(context.SellQueuesBySku);
            Assert.Equal(1, context.SellQueuesBySku["wine"].Count);

            Assert.Equal("sell wine 1000 remaining:1000", context.AllOrders[0].ToString());
        }
    }
}
