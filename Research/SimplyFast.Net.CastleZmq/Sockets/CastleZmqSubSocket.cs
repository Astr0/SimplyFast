using Castle.Zmq;

namespace SF.Net.Sockets
{
    public class CastleZmqSubSocket: CastleZmqSocket
    {
        internal CastleZmqSubSocket(CastleZmqSocketFactory factory, IZmqSocket socket) : base(factory, socket)
        {
        }

        public void Subscribe(string topic)
        {
            Socket.Subscribe(topic);
        }

        public void Unsubscribe(string topic)
        {
            Socket.Unsubscribe(topic);
        }
    }
}