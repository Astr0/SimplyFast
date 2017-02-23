using NetMQ;

namespace SF.Net.Sockets
{
    public class ZmqSubSocket : ZmqSocket
    {
        internal ZmqSubSocket(ZmqSocketFactory factory, NetMQSocket socket) : base(factory, socket)
        {
        }

        // ReSharper disable CSharpWarnings::CS0618
        public void Subscribe(string topic)
        {
            Socket.Subscribe(topic);
        }

        public void Subscribe(byte[] topic)
        {
            Socket.Subscribe(topic);
        }

        public void Unsubscribe(string topic)
        {
            Socket.Unsubscribe(topic);
        }

        public void Unsubscribe(byte[] topic)
        {
            Socket.Unsubscribe(topic);
        }

        // ReSharper restore CSharpWarnings::CS0618
    }
}