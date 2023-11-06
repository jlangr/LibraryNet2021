using System;

namespace LibraryNet2020.Util
{
    public class NASDAQStockService: IStockService
    {
        public decimal Price(string symbol)
        {
            throw new Exception("uh oh the darn system is down again");
        }
    }
}