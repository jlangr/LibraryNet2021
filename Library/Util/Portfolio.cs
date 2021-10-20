using System;
using System.Collections.Generic;
using System.Linq;
using LibraryNet2020.Util;

namespace LibraryTest.Util
{
    public class Portfolio
    {
        public bool IsEmpty => CountOfUniqueSymbols == 0;

        public int CountOfUniqueSymbols => SharesBySymbol.Count;

        private IDictionary<string, int> SharesBySymbol { get; } = new Dictionary<string, int>();
        // COMM => 2
        // IBM => 10;  COMM, IBM

        public decimal Value
        {
            get
            {
                if (IsEmpty) return 0;

                return SharesBySymbol.Keys
                    .Sum(symbol => StockService.Price(symbol) * Shares(symbol));
            }
        }

        public IStockService StockService { get; set; } = new NASDAQStockService();

                // if (IsEmpty) return 0m;
                // var enumerator = SharesBySymbol.Keys.GetEnumerator();
                // enumerator.MoveNext();
                // var soleSymbol = enumerator.Current;
        public int Shares(string symbol)
        {
            return !SharesBySymbol.ContainsKey(symbol) 
                ? 0 
                : SharesBySymbol[symbol];
        }

        public void Sell(string symbol, int numberOfShares)
        {
            ThrowOnOversell(symbol, numberOfShares);
            UpdateShares(symbol, -numberOfShares);
            RemoveSymbolIfNoSharesExist(symbol);
        }

        private void RemoveSymbolIfNoSharesExist(string symbol)
        {
            if (Shares(symbol) == 0)
                SharesBySymbol.Remove(symbol);
        }

        private void ThrowOnOversell(string symbol, int numberOfShares)
        {
            if (numberOfShares > Shares(symbol))
                throw new Exception();
        }

        public void Buy(string symbol, int numberOfShares)
        {
            UpdateShares(symbol, numberOfShares);
        }

        private void UpdateShares(string symbol, int numberOfShares)
        {
            SharesBySymbol[symbol] = Shares(symbol) + numberOfShares;
        }
    }
}