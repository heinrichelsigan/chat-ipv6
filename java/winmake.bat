
set Path=%Path%;"C:\Program Files\Android\Android Studio\jbr\bin"
REM set Path=%Path%;c:\Users\heinr\.jdks\graalvm-jdk-21.0.2\bin
REM set Path=%Path%;c:\Users\heinr\.jdks\corretto-23.0.1\bin
REM set Path=%Path%;c:\Users\heinr\.jdks\liberica-full-21.0.2\bin
REM set Path=%Path%;c:\Users\heinr\.jdks\semeru-21.0.2\bin


echo "building now CqrJd gui"
javac -classpath .:.\eu\cqrxs\.:.\eu\cqrxs\gui\.:.\eu\cqrxs\cqrframe\.:.\bcprov-jdk18on-1.79.jar -Xlint:deprecation eu\cqrxs\gui\CqrJDialog.java
javac -classpath .:.\eu\cqrxs\.:.\eu\cqrxs\gui\.:.\eu\cqrxs\cqrframe\.:.\bcprov-jdk18on-1.79.jar -Xlint:deprecation eu\cqrxs\gui\CqrJdFrame.java


pause
