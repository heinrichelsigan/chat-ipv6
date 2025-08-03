@echo off

REM 
REM Path=%Path%;"C:\Program Files\Android\Android Studio\jbr\bin"
REM SET CLASSPATH = %CLASSPATH%;"C:\Program Files\Android\Android Studio\jbr\lib"

echo Setting Path and CLASSPATH
SET Path=%Path%;C:\Users\heinr\.jdks\openjdk-23.0.1\bin
SET CLASSPATH = %CLASSPATH%;C:\Users\heinr\.jdks\openjdk-23.0.1\lib
set MYCLASSPATH=%CLASSPATH%;.\;.\bcprov-jdk18on-1.79.jar;.\eu\cqrxs\;.\eu\cqrxs\gui\;.\eu\cqrxs\fw\net\;.\eu\cqrxs\fw\crypt\;.\eu\cqrxs\fw\crypt\endecoding\;

echo "cleaning classes from last build in eu/cqrxs/ eu/cqrxs/gui/ "
echo "del /s /f /q *.class"
del /s /f /q *.class

echo "compiling now with javac CqrXs.Eu "
    

echo "javac.exe -classpath %MYCLASSPATH% -Xlint:deprecation eu\cqrxs\fw\net\NetworkAddresses.java"
javac.exe -classpath %MYCLASSPATH% -Xlint:unchecked -Xlint:deprecation  eu\cqrxs\fw\net\NetworkAddresses.java

echo "javac.exe -classpath %MYCLASSPATH% -Xlint:unchecked -Xlint:deprecation  eu\cqrxs\gui\CqrJDialog.java eu\cqrxs\gui\CqrJdFrame.java"
javac.exe -classpath %MYCLASSPATH% -Xlint:unchecked -Xlint:deprecation  eu\cqrxs\gui\CqrJDialog.java eu\cqrxs\gui\CqrJdFrame.java
javac.exe -classpath %MYCLASSPATH% -Xlint:unchecked -Xlint:deprecation  eu\cqrxs\gui\ImageTest.java


echo "javac.exe -classpath %MYCLASSPATH% -Xlint:unchecked -Xlint:deprecation  eu\cqrxs\fw\crypt\endecoding\EnDeCoder.java  eu\cqrxs\fw\crypt\endecoding\Base64Coder.java  eu\cqrxs\fw\crypt\endecoding\UuCoder.java eu\cqrxs\fw\crypt\endecoding\Base16Coder.java"
javac.exe -classpath %MYCLASSPATH% -Xlint:unchecked -Xlint:deprecation  eu\cqrxs\fw\crypt\endecoding\EnDeCoder.java  eu\cqrxs\fw\crypt\endecoding\Base64Coder.java  eu\cqrxs\fw\crypt\endecoding\UuCoder.java

REM  eu\cqrxs\fw\crypt\endecoding\Base16Coder.java

echo "build finished"

pause