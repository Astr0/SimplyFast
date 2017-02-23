namespace SF.Serialization.Tests.Protobuf.TestData
{
    public static class TestTools
    {
        public static FTestMessage ToMessage(this PTestMessage msg)
        {
            return new FTestMessage
            {
                Fbool = msg.Fbool,
                Fbytes = msg.Fbytes,
                Fdouble = msg.Fdouble,
                Fenum = msg.Fenum,
                Ffixed32 = msg.Ffixed32,
                Ffixed64 = msg.Ffixed64,
                Ffloat = msg.Ffloat,
                Finner = msg.Finner,
                Fint32 = msg.Fint32,
                Fint64 = msg.Fint64,
                Frep = msg.Frep,
                FrepEnum = msg.FrepEnum,
                FrepFixed32 = msg.FrepFixed32,
                FrepString = msg.FrepString,
                FrepUint32 = msg.FrepUint32,
                Fsfixed32 = msg.Fsfixed32,
                Fsfixed64 = msg.Fsfixed64,
                Fsint32 = msg.Fsint32,
                Fsint64 = msg.Fsint64,
                Fstring = msg.Fstring,
                Fuint32 = msg.Fuint32,
                Fuint64 = msg.Fuint64
            };
        }

        public static PTestMessage ToProtoNet(this FTestMessage msg)
        {
            return new PTestMessage
            {
                Fbool = msg.Fbool,
                Fbytes = msg.Fbytes,
                Fdouble = msg.Fdouble,
                Fenum = msg.Fenum,
                Ffixed32 = msg.Ffixed32,
                Ffixed64 = msg.Ffixed64,
                Ffloat = msg.Ffloat,
                Finner = msg.Finner,
                Fint32 = msg.Fint32,
                Fint64 = msg.Fint64,
                Frep = msg.Frep,
                FrepEnum = msg.FrepEnum,
                FrepFixed32 = msg.FrepFixed32,
                FrepString = msg.FrepString,
                FrepUint32 = msg.FrepUint32,
                Fsfixed32 = msg.Fsfixed32,
                Fsfixed64 = msg.Fsfixed64,
                Fsint32 = msg.Fsint32,
                Fsint64 = msg.Fsint64,
                Fstring = msg.Fstring,
                Fuint32 = msg.Fuint32,
                Fuint64 = msg.Fuint64
            };
        }
    }
}