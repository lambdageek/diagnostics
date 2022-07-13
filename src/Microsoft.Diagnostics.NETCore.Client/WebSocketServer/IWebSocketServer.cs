// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Microsoft.Diagnostics.NETCore.Client.WebSocketServer;

public interface IWebSocketServer
{
    public Task StartServer(Uri uri, CancellationToken cancellationToken);

    public Task StopServer(CancellationToken cancellationToken);

    public Task<Stream> AcceptConnection(CancellationToken cancellationToken);
}