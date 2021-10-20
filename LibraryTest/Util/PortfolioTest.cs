using System;
using LibraryNet2020.Util;
using Xunit;

namespace LibraryTest.Util
{
    public class PortfolioTest
    {
        private Portfolio portfolio;
        private readonly int MINIMUM_VALUE = 100;

        public PortfolioTest()
        {
            portfolio = new Portfolio();
        }

        public class Value : PortfolioTest
        {
            private const decimal CURRENT_PRICE_COMMSCOPE = 10m;
            private const decimal CURRENT_PRICE_IBM = 100m;

            class StockServiceStub : IStockService
            {
                public decimal Price(string symbol)
                {
                    if (symbol == "IBM") return CURRENT_PRICE_IBM;
                    return CURRENT_PRICE_COMMSCOPE;
                }
            }

            [Fact]
            public void IsZeroWhenNoPurchasesMade()
            {
                Assert.Equal(0, portfolio.Value);
            }
            
            [Fact]
            public void IsCurrentPriceOfSymbolForSingleSharePurchase()
            {
                portfolio.StockService = new StockServiceStub();
                portfolio.Buy("COMM", 1);
                
                Assert.Equal(CURRENT_PRICE_COMMSCOPE, portfolio.Value);
            }
            
            [Fact]
            public void MultipliesStockPriceByNumberOfShares()
            {
                portfolio.StockService = new StockServiceStub();
                portfolio.Buy("COMM", 10);
                
                Assert.Equal(10 * CURRENT_PRICE_COMMSCOPE, portfolio.Value);
            }
            //
            [Fact]
            public void SumsTotalValuesForAllSymbols()
            {
                portfolio.StockService = new StockServiceStub();
                portfolio.Buy("COMM", 10);
                portfolio.Buy("IBM", 20);
                
                Assert.Equal(10 * CURRENT_PRICE_COMMSCOPE 
                    + 20 * CURRENT_PRICE_IBM,
                    portfolio.Value);
            }
        }
        
        [Fact]
        public void IsEmptyWhenNoPurchasesMade()
        {
            Assert.True(portfolio.IsEmpty);
        }
        


        [Fact]
        public void CountIsZeroWhenNoPurchasesMade()
        {
            Assert.Equal(0, portfolio.CountOfUniqueSymbols);
        }
        
        [Fact]
        public void CountIsOneWhenPurchaseMade()
        {
            portfolio.Buy("AAPL", 42);
            
            Assert.Equal(1, portfolio.CountOfUniqueSymbols);
        }
        
        [Fact]
        public void CountIncreasesWhenPurchaseOfDifferentSymbol()
        {
            portfolio.Buy("AAPL", 42);
            portfolio.Buy("COMM", 40);
            
            Assert.Equal(2, portfolio.CountOfUniqueSymbols);
        }
        
        [Fact]
        public void CountDoesNotIncreaseWhenPurchaseSameSymbol()
        {
            portfolio.Buy("AAPL", 42);
            portfolio.Buy("AAPL", 40);
            
            Assert.Equal(1, portfolio.CountOfUniqueSymbols);
        }
        
        [Fact]
        public void ReturnsZeroForSharesOfSymbolNotPurchased()
        {
            Assert.Equal(0, portfolio.Shares("COMM"));
        }
        
        [Fact]
        public void ReturnsNumberOfSharesPurchasedForSymbol()
        {
            portfolio.Buy("COMM", 42);
            
            Assert.Equal(42, portfolio.Shares("COMM"));
        }
        
        [Fact]
        public void SeparatesSharesBySymbol()
        {
            portfolio.Buy("COMM", 42);
            portfolio.Buy("AAPL", 40);
            
            Assert.Equal(42, portfolio.Shares("COMM"));
            Assert.Equal(40, portfolio.Shares("AAPL"));
        }
        
        [Fact]
        public void AccumulatesSharesOfSameSymbolPurchase()
        {
            portfolio.Buy("COMM", 42);
            portfolio.Buy("COMM", 50);
            
            Assert.Equal(92, portfolio.Shares("COMM"));
        }
        
        [Fact]
        public void ReducesShareCountOnSell()
        {
            portfolio.Buy("COMM", 42);
            portfolio.Sell("COMM", 12);

            Assert.Equal(30, portfolio.Shares("COMM"));
        }
        
        [Fact]
        public void ReducesCountOfUniqueSymbolsOnSellAllForSymbol()
        {
            portfolio.Buy("AAPL", 20);
            portfolio.Buy("COMM", 42);
            portfolio.Sell("COMM", 42);

            Assert.Equal(1, portfolio.CountOfUniqueSymbols);
        }

        [Fact]
        public void ThrowsExceptionOnOversell()
        {
            portfolio.Buy("AAPL", 20);

            Assert.Throws<Exception>(
                () => portfolio.Sell("AAPL", 20 + 1)
            );
        }
    }
}