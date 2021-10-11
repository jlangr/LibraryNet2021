using Xunit;

namespace LibraryTest.Util
{
    public class PortfolioTest
    {
        private Portfolio portfolio;

        public PortfolioTest()
        {
            portfolio = new Portfolio();
        }
        
        [Fact]
        public void IsEmptyWhenNoPurchasesMade()
        {
            Assert.True(portfolio.IsEmpty);
        }
        
        [Fact]
        public void IsNotEmptyWhenPurchaseMade()
        {
            portfolio.Purchase("COMM", 1);

            Assert.False(portfolio.IsEmpty);
        }

        [Fact]
        public void CountIsZeroWhenNoPurchasesMade()
        {
            Assert.Equal(0, portfolio.Count);           
        }
        
        [Fact]
        public void CountIsOneWhenPurchaseMade()
        {
            portfolio.Purchase("AAPL", 1);
            
            Assert.Equal(1, portfolio.Count);           
        }
        
        [Fact]
        public void CountIncreasesWhenPurchase()
        {
            portfolio.Purchase("AAPL", 1);
            portfolio.Purchase("COMM", 1);
            
            Assert.Equal(2, portfolio.Count);           
        }
        
        [Fact]
        public void CountDoesNotIncreaseWhenPurchaseSameSymbol()
        {
            portfolio.Purchase("COMM", 1);
            portfolio.Purchase("COMM", 1);
            
            Assert.Equal(1, portfolio.Count);           
        }
        
    }
}