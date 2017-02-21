namespace SF.Serialization
{
    public interface IMessage
    {
        // message should return some static descriptor info so streams can know something about message if needed

        // Delegate to stream? We will use special calculate size stream for this
        // int CalculateSize();
        void WriteTo(IOutputStream stream);
        void ReadFrom(IInputStream stream);
    }
}