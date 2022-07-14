// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Diagnostics.NETCore.Client.WebSocketServer;

internal class WebSocketStreamAdapter : Stream, IWebSocketStreamAdapter
{
    private readonly WebSocket _webSocket;
    private readonly Action _onDispose;

    public WebSocket WebSocket { get => _webSocket; }
    public WebSocketStreamAdapter(WebSocket webSocket, Action onDispose)
    {
        _webSocket = webSocket;
        _onDispose = onDispose;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();

    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        Console.WriteLine("WebSocket stream adapter ReadAsync");
        var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, count), cancellationToken);
        Console.WriteLine("WebSocket stream adapter read {0} bytes", result.Count);
        return result.Count;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        Console.WriteLine("WebSocket stream adapter ReadAsync");
        var result = await _webSocket.ReceiveAsync(buffer, cancellationToken);
        Console.WriteLine("WebSocket stream adapter read {0} bytes", result.Count);
        return result.Count;
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        Console.WriteLine("WebSocket stream adapter WriteAsync {0} bytes", count);
        return _webSocket.SendAsync(new ArraySegment<byte>(buffer, offset, count), WebSocketMessageType.Binary, true, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken)
    {
        Console.WriteLine("WebSocket stream adapter WriteAsync {0} bytes", memory.Length);
        return _webSocket.SendAsync(memory, WebSocketMessageType.Binary, true, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Console.WriteLine("WebSocket stream adapter Dispose(true)");
            _onDispose();
            _webSocket.Dispose();
        }
    }

    bool IWebSocketStreamAdapter.IsConnected => _webSocket.State == WebSocketState.Open;

}
