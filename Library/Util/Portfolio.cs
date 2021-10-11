namespace LibraryTest.Util
{
    public class Portfolio
    {
        public bool IsEmpty => Count == 0;

        public int Count { get; private set; }

        public void Purchase(string symbol, int numberOfShares)
        {
            Count++;
        }
    }
}