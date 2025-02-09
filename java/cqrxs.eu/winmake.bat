
echo "building now CqrJd gui"
javac -classpath .:..\bcprov-jdk18on-1.79.jar -Xlint:deprecation CqrJDialog.java
javac -classpath .;..\bcprov-jdk18on-1.79.jar -Xlint:deprecation CqrJdFrame.java



