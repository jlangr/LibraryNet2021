using System;
using LibraryNet2020.Util;

namespace LibraryTest.Util
{
    public class NASDAQStockService : IStockService
    {
        public decimal Price(string symbol)
        {
            throw new Exception("the service is down right now!");
        }
    }
}