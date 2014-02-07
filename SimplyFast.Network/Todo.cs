/*
 TODO
 * Do we need interfaces for Socket and SocketServer?
 * - We can use TCP, UDP, ZMQ and other sockets
 * Do we need to expose underlying Socket?
 * - ZMQ and other network libs may not expose their sockets 
 * IDuplexStream wrapper for sockets
 * Something with Task<SomethingElse> AcceptAsync() for server-socket
 * Something with Task DisconnectAsync() and IDuplexStream for client-socket and incomming server socket
 * Something higher-level with messaging support, on interfaces ofc
 * Factory for creating sockets? 
 */