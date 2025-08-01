#!/usr/bin/bash

# Path=%Path%;"C:\Program Files\Android\Android Studio\jbr\bin"
# set Path=%Path%;c:\Users\heinr\.jdks\graalvm-jdk-21.0.2\bin
# set Path=%Path%;c:\Users\heinr\.jdks\corretto-23.0.1\bin
# set Path=%Path%;c:\Users\heinr\.jdks\liberica-full-21.0.2\bin
# set Path=%Path%;c:\Users\heinr\.jdks\semeru-21.0.2\bin


echo "$0: cleaning classes from last build in eu/cqrxs/ eu/cqrxs/cqrframe/ eu/cqrxs/gui/ "
echo -n "$0: rm -f "
for fc in $(find -iname '*.class') ; do
    echo -n "$fc " ;
    rm -f $fc 
done

echo -n -e "\n\n$0: compiling now with javac CqrJd: "
for fj in $(find -iname '*.java') ; do
    echo -n "$fj ";
done 

echo -n -e "\n$0: javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar" -Xlint:deprecation eu/cqrxs/fw/net/NetworkAddresses.java"
javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar" -Xlint:deprecation eu/cqrxs/fw/net/NetworkAddresses.java

echo -n -e "\n$0: javac -classpath .:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/cqrframe/.:./bcprov-jdk18on-1.79.jar -Xlint:deprecation gui/*.java\n"
javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar" -Xlint:deprecation eu/cqrxs/gui/CqrJDialog.java eu/cqrxs/gui/CqrJdFrame.java
javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar"  -Xlint:deprecation eu/cqrxs/gui/ImageTest.java

echo -e -n "\n$0: javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/fw/.:./eu/cqrxs/fw/crypt/.:./eu/cqrxs/fw/crypt/endecoding/.:./bcprov-jdk18on-1.79.jar" -Xlint:deprecation eu/cqrxs/fw/crypt/endecoding/UuCoder.java eu/cqrxs/fw/crypt/endecoding/Base64Coder.java eu/cqrxs/fw/crypt/endecoding/EnDeCoder.java"
javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/fw/.:./eu/cqrxs/fw/crypt/.:./eu/cqrxs/fw/crypt/endecoding/.:./bcprov-jdk18on-1.79.jar" -Xlint:deprecation eu/cqrxs/fw/crypt/endecoding/UuCoder.java eu/cqrxs/fw/crypt/endecoding/Base64Coder.java eu/cqrxs/fw/crypt/endecoding/EnDeCoder.java

echo -e -n "\n$0: javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar"  -Xlint:deprecation eu/cqrxs/cqrframe/CqrFrame.java eu/cqrxs/cqrframe/CqrMenuBar.java"
javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar"  -Xlint:deprecation eu/cqrxs/cqrframe/CqrFrame.java 
javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar" -Xlint:deprecation eu/cqrxs/cqrframe/CqrMenuBar.java

javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/fw/net/.:./bcprov-jdk18on-1.79.jar"  -Xlint:deprecation eu/cqrxs/JFrameApp.java


echo "build finished"

