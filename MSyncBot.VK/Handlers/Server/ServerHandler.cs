using System.Net.Sockets;
using MLoggerService;
using NetCoreServer;

namespace MSyncBot.VK.Handlers.Server;

public class ServerHandler(string? address, int port, MLogger logger) : WsClient(address, port)
{
    private bool _stop;

    public void DisconnectAndStop()
    {
        _stop = true;
        CloseAsync(1000);
        while (IsConnected)
            Thread.Yield();
    }

    public override void OnWsConnecting(HttpRequest request)
    {
        request.SetBegin("GET", "/");
        request.SetHeader("Host", "localhost");
        request.SetHeader("Origin", "http://localhost");
        request.SetHeader("Upgrade", "websocket");
        request.SetHeader("Connection", "Upgrade");
        request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
        request.SetHeader("Sec-WebSocket-Protocol", "chat, superchat");
        request.SetHeader("Sec-WebSocket-Version", "13");
        request.SetBody();
    }

    public override void OnWsConnected(HttpResponse response) =>
        logger.LogSuccess($"Chat WebSocket client connected a new session with Id {Id}");

    public override void OnWsDisconnected() =>
        logger.LogError($"Chat WebSocket client disconnected a session with Id {Id}");

    public override void OnWsReceived(byte[] buffer, long offset, long size) =>
        new ReceivedMessageHandler().ReceiveMessage(buffer, offset, size, logger);

    protected override void OnDisconnected()
    {
        base.OnDisconnected();

        logger.LogError($"Chat WebSocket client disconnected a session with Id {Id}");

        Thread.Sleep(1000);

        if (!_stop)
            ConnectAsync();
    }

    protected override void OnError(SocketError error) =>
        logger.LogError($"Chat WebSocket client caught an error with code {error}");
}