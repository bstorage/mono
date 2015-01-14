﻿//
// MonoConnection.cs
//
// Author:
//       Martin Baulig <martin.baulig@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
extern alias MonoSecurity;
extern alias NewMonoSource;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using MonoSecurity::Mono.Security.Protocol.NewTls;
using MonoSslStreamFactory = NewMonoSource::Mono.Security.NewMonoSource.MonoSslStreamFactory;
using MonoSslStream = NewMonoSource::Mono.Security.NewMonoSource.MonoSslStream;

namespace Mono.Security.Instrumentation.Console
{
	using Framework;

	public abstract class MonoConnection : DotNetConnection, ICommonConnection
	{
		public MonoConnection (ConnectionFactory factory, IPEndPoint endpoint, IConnectionParameters parameters)
			: base (factory, endpoint, parameters)
		{
		}

		TlsSettings settings;
		MonoSslStream monoSslStream;

		public override TlsConnectionInfo GetConnectionInfo ()
		{
			return settings.ConnectionInfo;
		}

		protected abstract MonoSslStream Start (Socket socket, TlsSettings settings);

		protected abstract TlsSettings GetSettings ();

		protected sealed override Stream Start (Socket socket)
		{
			settings = GetSettings ();
			settings.EnableDebugging = Parameters.EnableDebugging;
			monoSslStream = Start (socket, settings);
			return monoSslStream;
		}

		protected override async Task<bool> TryCleanShutdown (bool waitForReply)
		{
			await monoSslStream.Shutdown (waitForReply);
			return true;
		}
	}
}

