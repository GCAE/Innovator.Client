﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Innovator.Client.Connection
{
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  internal class MappedConnection : IRemoteConnection, IArasConnection
  {
    private Func<INetCredentials, string, bool, IPromise<ICredentials>> _authCallback;
    private IRemoteConnection _current;
    private IEnumerable<ServerMapping> _mappings;
    private ICredentials _lastCredentials;
    private Action<IHttpRequest> _settings;

    public ElementFactory AmlContext { get { return _current == null ? ElementFactory.Local : _current.AmlContext; } }
    public string Database { get { return _current?.Database; } }
    public Uri Url { get { return _current?.Url; } }
    public string UserId { get { return _current?.UserId; } }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay
    {
      get
      {
        var exp = _lastCredentials as ExplicitCredentials;
        if (exp != null)
          return string.Format("[Connection] {0} | {1} | {2}", exp.Username, Database, Url);
        return string.Format("[Connection] {0} | {1} | {2}", UserId, Database, Url);
      }
    }

    List<Action<IHttpRequest>> IArasConnection.DefaultSettings
    {
      get
      {
        return (_current as IArasConnection)?.DefaultSettings;
      }
    }

    public CompressionType Compression
    {
      get
      {
        return (_current as IArasConnection)?.Compression ?? CompressionType.none;
      }
    }

    public Version Version
    {
      get
      {
        var arasConn = _current as IArasConnection;
        if (arasConn == null)
          throw new NotSupportedException();
        return arasConn.Version;
      }
    }

    public MappedConnection(IEnumerable<ServerMapping> mappings
      , Func<INetCredentials, string, bool, IPromise<ICredentials>> authCallback)
    {
      _mappings = mappings;
      _authCallback = authCallback;
    }

    public UploadCommand CreateUploadCommand()
    {
      return _current.CreateUploadCommand();
    }

    public void DefaultSettings(Action<IHttpRequest> settings)
    {
      _current?.DefaultSettings(settings);
      _settings = settings;
    }

    public void Dispose()
    {
      _current?.Dispose();
    }

    public IEnumerable<string> GetDatabases()
    {
      return _mappings.SelectMany(s => s.Databases);
    }

    public void Login(ICredentials credentials)
    {
      Login(credentials, false).Wait();
    }

    public IPromise<string> Login(ICredentials credentials, bool async)
    {
      _lastCredentials = credentials;
      var mapping = _mappings.FirstOrDefault(m => m.Databases.Contains(credentials.Database));
      if (mapping == null)
        throw new InvalidOperationException($"The database '{credentials.Database}' could not be found.");
      var netCred = credentials as INetCredentials;
      IPromise<ICredentials> credPromise;

      var endpoint = credentials is WindowsCredentials
        ? mapping.Endpoints.AuthWin.Concat(mapping.Endpoints.Auth).FirstOrDefault()
        : mapping.Endpoints.Auth.FirstOrDefault();

      if (netCred != null && _authCallback != null && !string.IsNullOrEmpty(endpoint))
        credPromise = _authCallback(netCred, endpoint, async);
      else
        credPromise = Promises.Resolved(credentials);

      _current = mapping.Connection;
      if (_settings != null)
        _current.DefaultSettings(_settings);
      return credPromise.Continue(cred => _current.Login(cred, async));
    }

    public void Logout(bool unlockOnLogout)
    {
      _current?.Logout(unlockOnLogout);
      _current = null;
    }

    public void Logout(bool unlockOnLogout, bool async)
    {
      _current?.Logout(unlockOnLogout, async);
      _current = null;
    }

    public string MapClientUrl(string relativeUrl)
    {
      return _current.MapClientUrl(relativeUrl);
    }

    public Stream Process(Command request)
    {
      return _current.Process(request);
    }

    public IPromise<Stream> Process(Command request, bool async)
    {
      return _current.Process(request, async);
    }

    public IPromise<IRemoteConnection> Clone(bool async)
    {
      var newConn = new MappedConnection(_mappings, _authCallback);
      return newConn.Login(_lastCredentials, async)
        .Convert(u => (IRemoteConnection)newConn);
    }

    public void SetDefaultHeaders(Action<string, string> writer)
    {
      var arasConn = _current as IArasConnection;
      if (arasConn != null)
        arasConn.SetDefaultHeaders(writer);
    }
  }
}
