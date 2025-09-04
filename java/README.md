# **Java** *ipv4/ipv6 EchoServer & EchoClient*

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