# <a aref="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23">CqrJd C#</a>

Download with Visual Studio 2022 (or later) <a href="https://visualstudio.microsoft.com/de/downloads/" target="_blank">free Community Edition</a> 


## rich client [ .Net Core C# 9.0 ]

Open solution <a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/C%23/EU.CqrXs.WinForm.SecureChat.sln" target="_blank"><pre>🚧 C#/EU.CqrXs.WinForm.SecureChat.sln</pre></a>

<a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23/EU.CqrXs.WinForm.SecureChat"><pre>📁 C#/EU.CqrXs.WinForm.SecureChat</pre></a>

<a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/C%23/EU.CqrXs.WinForm.SecureChat/EU.CqrXs.WinForm.SecureChat.csproj"><pre>🚧 C#/EU.CqrXs.WinForm.SecureChat/EU.CqrXs.WinForm.SecureChat.csproj</pre></a>

## minimalistic server [ .Net 4.6.2 apache2 mod_mono ]
server keeps message in some kind of application state process

Open solution <a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/C%23/EU.CqrXs.CqrSrv.sln" target="_blank"><pre>🚧 C#/EU.CqrXs.CqrSrv.sln</pre></a>

<a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C%23/EU.CqrXs.CqrSrv.CqrJd"><pre>📁 C#/EU.CqrXs.CqrSrv.CqrJd</pre></a>

<a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/C%23/EU.CqrXs.CqrSrv.CqrJd/EU.CqrXs.CqrSrv.CqrJd.csproj"><pre>🚧 C#/EU.CqrXs.CqrSrv.CqrJd/EU.CqrXs.CqrSrv.CqrJd.csproj</pre></a>

<pre>EU.CqrXs/EU.CqrXs.WinForm.SecureChat.sln</pre>

u can use Amazon elastic cache instead:
<a href="https://github.com/heinrichelsigan/chat-ipv6/blob/main/doc/2025-03-01_AWS_Secure_Chat_With_Elastic_Cache.gif" target="_blank"><img src="https://raw.githubusercontent.com/heinrichelsigan/chat-ipv6/refs/heads/main/doc/2025-03-01_AWS_Secure_Chat_With_Elastic_Cache.gif"  border="0" /></a> 


## <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/C">CqrJd C</a>


# **C** *a cloning ipv6 socket server & client*

is same as a forking ipv6 server, but using clone(2) for posix threads instead of fork (copying entire process image).

<b>currently only availible for linux / unix and gcc</b>.

## build c sources with gcc under linux

client6.s and server6_clone.s will be compiled and linked with **gcc** and ***GNU make utility***.

- change directory to c sources containing directory c-server6 and then make clean:
  <pre>cd c/server6/
  <b>make clean</b>
  rm -f client6 client6.o server6_clone server6_clone.o
  </pre>


## <a href="https://github.com/heinrichelsigan/chat-ipv6/tree/main/java">CqrJd Java</a> 

## **Java** *ipv4/ipv6 EchoServer & EchoClient*

compile java sources with any javac compiler

### Windows 

- winmake
  <pre>winmake.bat
  > echo "building now EchoServer classes..."
  "building now EchoServer classes..."
  > javac EchoServer.java
  > echo "building now EchoClient class."
  "building now EchoClient class."
  > javac EchoClient.java
  > dir *.class
  Directory of C:\Users\heinr\source\chat-ipv6\java-server6
  10/12/2024  03:40             3.188 EchoClient.class
  10/12/2024  03:40             5.368 EchoServer.class
             2 File(s)          8.556 bytes</pre>

### Linux

- make all
  <pre>zen@virginia:~/prog/chat-ipv6/java-server6$ make all
  rm -f *.class
  javac EchoServer.java
  javac EchoClient.java
  zen@virginia:~/prog/chat-ipv6/java-server6$ ls -al *.class
  -rw-r--r-- 1 zen mailers 3188 Dec 10 03:44 EchoClient.class
  -rw-r--r-- 1 zen mailers 5368 Dec 10 03:44 EchoServer.class
  zen@virginia:~/prog/chat-ipv6/java-server6$</pre>

### starting java socket server with no args for both ipv4/6

- java EchoServer "172.31.20.156" "2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5" 7777
<pre>zen@paris:~/prog/chat-ipv6/java-server6$ java EchoServer "172.31.20.156" "2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5" 7777
server address set to 172.31.20.156
server address6 set to 2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5
Simple TCP Echo Server started ...
Simple TCP Echo Server started ...
EchoServer is listening on address 2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5 port 7777.
EchoServer is listening on address 172.31.20.156 port 7777.
Accepted connection to 2600:1f18:7a3f:a700:0:0:0:6291 (2600:1f18:7a3f:a700:0:0:0:6291) on port 58318.
Receiving from socket:
Finished, now sending back to socket:
2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5 =>      2600:1f18:7a3f:a700:0:0:0:6291   uname -a : Linux ip-172-31-57-91 6.8.0-1019-aws #21-Ubuntu SMP Wed Nov  6 21:21:49 UTC 2024 x86_64 x86_64 x86_64 GNU/Linux
client socket close()
Accepted connection to ec2-100-26-162-115.compute-1.amazonaws.com (100.26.162.115) on port 48304.
Receiving from socket:
Finished, now sending back to socket:
172.31.20.156  =>      ec2-100-26-162-115.compute-1.amazonaws.com/100.26.162.115       uname -a : Linux ip-172-31-57-91 6.8.0-1019-aws #21-Ubuntu SMP Wed Nov  6 21:21:49 UTC 2024 x86_64 x86_64 x86_64 GNU/Linux
client socket close()</pre>

### starting java echo client

- java EchoClient "2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5"  7777 "uname -a : `uname -a` "
<pre>zen@virginia:~/prog/chat-ipv6/java-server6$ java EchoClient "2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5"  7777 "uname -a : `uname -a` "
Receiving:
2a05:d012:209:ee00:a8ba:d6c4:bd60:bab5 =>      2600:1f18:7a3f:a700:0:0:0:6291   uname -a : Linux ip-172-31-57-91 6.8.0-1019-aws #21-Ubuntu SMP Wed Nov  6 21:21:49 UTC 2024 x86_64 x86_64 x86_64 GNU/Linux
finished
zen@virginia:~/prog/chat-ipv6/java-server6$ java EchoClient "<b>13.38.136.165</b>"  <b>7777</b> "uname -a : <i>`uname -a`</i> "
Receiving:
172.31.20.156  =>      ec2-100-26-162-115.compute-1.amazonaws.com/100.26.162.115       uname -a : Linux ip-172-31-57-91 6.8.0-1019-aws #21-Ubuntu SMP Wed Nov  6 21:21:49 UTC 2024 x86_64 x86_64 x86_64 GNU/Linux
finished
^C</pre>


<hr />

<a href="LinuxTerm.png" target="_blank"><img src="LinuxTerm.png" border="0" /></a>

<hr />