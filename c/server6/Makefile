# 
# Makefile for ipv6 cloning socket server
# 

# GCCFLAGS compiler and linker optiuns passed to gcc 
GCCFLAGS="-Wimplicit-function-declaration"

# default target is all
default: all

# all includes client6 server6_clone 
all: client6 server6 server66

# client6
client6: client6.o
	gcc ${GCCFLAGS} -o client6 client6.o

client6.o: client6.c
	gcc ${GCCFLAGS} -o client6.o -c  client6.c


# server6
server6: server6.o
	gcc ${GCCFLAGS}  -o server6 server6.o

server6.o: server6.c
	gcc  ${GCCFLAGS} -o server6.o -c server6.c

server66: server66.o
	gcc ${GCCFLAGS}  -o server66 server66.o

server66.o: server66.c
	gcc  ${GCCFLAGS} -o server66.o -c server66.c


# clean cleans client6 server6_clone 
clean:
	rm -f client6 client6.o server6 server6.o  server66 server66.o


