namespace SimplyFast.IoC.Tests.TestData
{
    public class SomeClass3
    {
        public readonly int Value;

        public SomeClass3(): this(11)
        {
        }

        public SomeClass3(int value)
        {
            Value = value;
        }
    }
}