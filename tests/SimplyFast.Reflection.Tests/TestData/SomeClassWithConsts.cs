namespace SimplyFast.Reflection.Tests.TestData
{
    public class SomeClassWithConsts
    {
        #region SomeEnumType enum

        public enum SomeEnumType : long
        {
            My1 = 34543523423423423,
            My2 = 34534543
        }

        #endregion

        public const byte TestB = 3;

        public const bool TestBool = true;

        public const char TestC = 'c';
        public const double TestD = 23.23;
        public const decimal TestDec = 323423123142342324;
        public const SomeEnumType TestEnum = SomeEnumType.My1;
        public const float TestF = 23.43f;
        public const int TestI = -34;

        public const long TestL = -2343242343432;
        public const short TestS = 3;
        public const sbyte TestSbyte = -21;
        public const string TestStr = "test";
        public const uint TestUInt = 3453423423;
        public const ulong TestULong = 2342342342342;
        public const ushort TestUShort = 21;

        public static object GetTest2(int arg)
        {
            switch (arg)
            {
                case 1:
                    return TestBool;
                case 2:
                    return TestC;
                case 3:
                    return TestStr;
                case 4:
                    return TestB;
                case 5:
                    return TestSbyte;
                case 6:
                    return TestS;
                case 7:
                    return TestUShort;
                case 8:
                    return TestUInt;
                case 9:
                    return TestI;
                case 10:
                    return TestL;
                case 11:
                    return TestULong;
                case 12:
                    return TestF;
                case 13:
                    return TestD;
                case 14:
                    return TestDec;
                case 15:
                    return TestEnum;
            }
            return TestI;
        }
    }
}