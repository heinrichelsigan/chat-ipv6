# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23">**c#**</a>

# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/c">**c**</a>

# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/java">**java**</a>

# <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/doc">**doc**</a>

<hr />
<pre>
- all release downloads could be found 
  here: <a href="https://cqrxs.eu/download/" target="_blank">cqrxs.eu/download/</a>
  (<i>please take always the newest</i>i>.)
- last release (pre-alpha) deployed on 2025-04-10 is laterst here: 
  - <b>x86</b> <a href="https://cqrxs.eu/download/2025-08-02_SecureChat_WinClient_x86.7z" target="_blank">https://cqrxs.eu/download/2025-08-02_SecureChat_WinClient_x86.7z</a>
  - <b>x64</b> <a href="https://cqrxs.eu/download/2025-08-02_SecureChat_WinClient_x64.7z" target="_blank">https://cqrxs.eu/download/2025-08-02_SecureChat_WinClient_x64.7z</a>  
- <b><i><u>Features</u></i></b>:
  - <b>C#</b> full client as fat .exe 
    requires no framework 
    - server part <a href="https://srv.cqrxs.eu/v1.3/CqrService.asmx" target="_blank">https://srv.cqrxs.eu/v1.3/CqrService.asmx</a>
    - <b>Now <i>refactoring</i></b> tiny and fast crypt msg handling entities:
      <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23/Area23.At.Framework/Area23.At.Framework.Core/Cqr" target="_blank">C%23/Area23.At.Framework/Area23.At.Framework.Core/Cqr</a>
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

