/*
 * socket6.h common header file for server6.c and client6.c
 * Author             he23[AT]area23.at
 * Open Source  (MIT Licenese)
 */
#define _GNU_SOURCE 

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>

#define TCPv6_PORT      7777            /* default tcp ipv6 port */
#define STACKSZE        (1024 * 1024)   /* Stack size for cloned child */
#define MMAP_OFFSET     1024            /* default memory map offset */
#define MSGBUFLEN       8192            /* send and receive message buffer size */

void usemsg(char *msg, int exitcode);	/* function handle_client declaration */
long handle_client(int csd);		/* function handle_client declaration */
static int childfunc(void *arg); 	/* function childfunc declaration */
