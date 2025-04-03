#
# Makefile for cqrjd java
# 

# GCCFLAGS compiler and linker optiuns passed to gcc 
# GCCFLAGS="-Wimplicit-function-declaration"

# default target is all
default: all

# all includes client6 server6_clone 
all: cqrjd bouncycastle

# cqrjd eu/cqrxs/gui/CqrJDialog.java eu/cqrxs/gui/CqrJdFrame.java eu/cqrxs/cqrframe/CqrFrame.java eu/cqrxs/cqrframe/CqrMenuBar.java eu/cqrxs/cqrframe/CqrFrame.java 
cqrjd: 
    /usr/bin/echo  "\n\ncompiling now with javac CqrJd: "
    /usr/bin/javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/cqrframe/.:./bcprov-jdk18on-1.79.jar" -Xlint:deprecation eu/cqrxs/gui/CqrJDialog.java eu/cqrxs/gui/CqrJdFrame.java
    /usr/bin/javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/cqrframe/.:./bcprov-jdk18on-1.79.jar"    -Xlint:deprecation eu/cqrxs/gui/ImageTest.java 
    /usr/bin/javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/cqrframe/.:./bcprov-jdk18on-1.79.jar"  -Xlint:deprecation eu/cqrxs/cqrframe/CqrFrame.java 
    /usr/bin/javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/cqrframe/.:./bcprov-jdk18on-1.79.jar"  -Xlint:deprecation eu/cqrxs/cqrframe/CqrMenuBar.java 
    /usr/bin/javac -classpath ".:./eu/cqrxs/.:./eu/cqrxs/gui/.:./eu/cqrxs/cqrframe/.:./bcprov-jdk18on-1.79.jar"  -Xlint:deprecation eu/cqrxs/JFrameApp.java
    /usr/bin/echo "build finished"

# bouncycastle
bouncycastle: 
        /usr/bin/echo "no bouncycastle code except jar: `ls -a *.jar`";

# clean cleans client6 server6_clone 
clean: 
    /usr/bin/echo "cleaning classes from last build in eu/cqrxs/ eu/cqrxs/cqrframe/ eu/cqrxs/gui/ "
    /usr/bin/echo -n "rm -f "
    for fc in $(/usr/bin/find -iname '*.class') ; do
        /usr/bin/echo -n "$fc " ;
        /usr/bin/rm -f $fc 
    done


