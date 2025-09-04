# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23">**c#**</a>

# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/c">**c**</a>

# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/java">**java**</a>

# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/doc">**doc**</a>

<hr />
<pre>
- all release downloads could be found 
  here: <a href="https://cqrxs.eu/download/" target="_blank">cqrxs.eu/download/</a>
  (<i>please take always the newest</i>.)
- last release (pre-alpha) deployed on 2025-08-07 can be downloaded here: 
  - <b>x86</b> <a href="https://cqrxs.eu/download/2025-08-19_SecureChat_Win_x86.7z" target="_blank">https://cqrxs.eu/download/2025-08-19_SeureChat_Win_x86.7z</a>
  - <b>x64</b> <a href="https://cqrxs.eu/download/2025-08-19_SeureChat_Win_x64.7z" target="_blank">https://cqrxs.eu/download/2025-08-19_SecureChat_Win_x64.7z</a>  
- <b><i><u>Features</u></i></b>:
  - <b>C#</b> full client as fat .exe, requires no framework;
     version v2.25.802 is a functional prototype.    
    - server part <a href="https://srv.cqrxs.eu/v1.0/CqrService.asmx" target="_blank">https://srv.cqrxs.eu/v1.0/CqrService.asmx</a>
    - <b>DONE</b> finished Swashbuckle project under Core. (Not all well tested, but fully implemented)
      <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23/EU.CqrXs.Srv.Svc.Swashbuckle" target="_blank">https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23/EU.CqrXs.Srv.Svc.Swashbuckle</a>
    - <b>TODO</b>: <i>refactoring</i> SVC WCF classic to SVC WCF Core and deploying it in a native .Net Core enviroment in linux.
      Currently a very old style asmx service runs at server to cache secure chats in session server mode, 
      a newer WCF Svc service and swashbuckle swagger service are included in the repository, but it doens't work properly in apache2 mod_mono.
      There is a way of updating Svc WCF classic to SVC WCF Core and running it under linux, 
      see: <a href="https://devblogs.microsoft.com/dotnet/net-core-and-systemd/" target="_blank">https://devblogs.microsoft.com/dotnet/net-core-and-systemd/</a>      
  - <b>java</b> is only skeleton (added uuencode/decode and base64 en/decode at end of july);
      you probably need to wait until automn or winter 2025 to get here something ready to use in real sceanarios.
      <b>Warning</b> a older version of BoundyCastle <a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/java/bcprov-jdk18on-1.79.jar" target="_blank">bcprov-jdk18on-1.79.jar</a> jar file is currently included;          
      please visit <a href="https://www.bouncycastle.org/download/" target="_blank">bouncycastle.org/download/</a> and <a href="https://www.bouncycastle.org/documentation/" target="_blank">bouncycastle.org/documentation/</a> to get newest version and documentation.
  - <b>c</b> even less then skeleton,
      but all you need to make a crypt pipeline by your own is in c folder of this repository,
      you have sadly to wait until next year 2026 for both Win32/Win64 and linux (maybe ARM64 too).      
</pre>

<hr />

# CqrJd Endpoint to Endpoint scenarios

In all *Endpoint to Endpoint* _peer-2-peer_ scenarios CqrJd doesn't need to proxy over cqrxs.eu
and can directly connect a secure tcp connection to another chat-client.

<a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/doc/2025-03-01_Peer_2_Peer.jpg" target="_blank"><img src="https://raw.githubusercontent.com/heinrichelsigan/chat-ipv6/refs/heads/main/doc/2025-03-01_Peer_2_Peer.jpg" border="0" /></a>

# CqrJd Session Server users connecting scenario

<a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/doc/2025-03-01_AWS_Secure_Chat_With_Elastic_Cache.png" target="_blank"><img src="https://raw.githubusercontent.com/heinrichelsigan/chat-ipv6/refs/heads/main/doc/2025-03-01_AWS_Secure_Chat_With_Elastic_Cache.png" border="0" /></a>

<hr />

