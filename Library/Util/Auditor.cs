using System;

namespace Library.Util
{
    public class StockAuditor
    {
        public virtual void Initialize() { }
        public virtual void Log(string message) { throw new Exception("LOGGER DOWN"); }

        public virtual void LogPurchase(string purchaseMade)
        {
            throw new Exception("you just interacted with the real object");
        }
    }
}