namespace SimplyFast.Reflection.Tests.TestData
{
    public class TestClass3 : TestClass2
    {
        private int _field = 1;

        public TestClass3()
        {
            Priv = 0;
            Priv2 = 0;
        }

        public int this[int index]
        {
            get { return index + 1; }
            set { _field = index*value; }
        }

        public int CanGet
        {
            get { return _field; }
        }

        public int CanSet
        {
            set { _field = value; }
        }

        private int Priv { get; set; }
        private static int Priv2 { get; set; }

        public int GetPrivSum()
        {
            return Priv + Priv2;
        }

        public new int GetF1()
        {
            return base.GetF1() + 1;
        }

        public override void SetP2P3(string p2, string p3)
        {
            P2 = p2 + "_";
            P3 = p3 + "_";
        }
    }
}