/*
 * srver6.c     a simple ipv6 capable cloning echo server, that use clone(2) instead of fork(2)
 * Author	      he23[AT]area23.at
 * Open Source  (MIT Licenese)
 */
#define _GNU_SOURCE 

#include "socket6.h"
#include <err.h> 
#include <fcntl.h>
#include <sched.h>
#include <signal.h>
#include <stdint.h>
#include <sys/mman.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/utsname.h>
#include <sys/wait.h>
#include <unistd.h>
#include <arpa/inet.h>

/* defining global variables */
char *progname = NULL;          								// program name will later be set to argv[0]
char outmsg[MSGBUFLEN]; 
int c_sd = -1;                  								// c_sd client socket descriptor
int done = 0;

/* 
 * usemsg:
 *  msg ...     message to printf
 *  exitcode ...        exit code
 *
 * usemsg err and usage in one functions.
 * prints error message (if not NULL) to stderr in any case 
 * and exits program.
 */
void usemsg(char *msg, int exitcode)
{
	if (msg != NULL)
		(void) fprintf(stderr, "%s error:\t%s\n", progname, msg);
    else     
		fprintf(stderr, "usage: %s [port-number]\n", progname);
        
	exit(exitcode);
}


/* 
 * childfunc:
 *    arg ... void pointer to an array of any type
 *    returns childs exit code (exit status of child process / thread 
 *
 * childfunc needed by clone(2) processes connected client socket request
 */
static int childfunc(void *arg) 
{
	int exitcode = 0;
	long byteslen = 0;
	
	if (c_sd > 0) 
	{
		byteslen = handle_client(c_sd); 							// received bytes got from handle client socket 
        
		printf("%s:\tclient request handled after sending/receiving %ld bytes total.\n",
				progname, byteslen);
        
		close(c_sd); 											// close c_sd client socket descriptor 
		printf("%s\tclosed client socket descriptor %d now, exiting in 1 second", progname, c_sd);
	}
	else 
	{
		exitcode = -1;
		fprintf(stderr, "%s:\tclient socket c_sd returns -1; client socket is'nt availible anymore!\nclosing cloned child in 2 sec.", 
				progname);
	}

	sleep(1); 												// Keep child thread for 1s alive, before extting child thread.

	return exitcode;
}

/*
 * handle_client:
 *  csd ... client socket descriptor 
 *  returns total processed MSGBUFLEN (mostly 8192) bytes blocks
 * handles client socket connect and send & receive data *
 */
long handle_client(int csd)
{
	char buf[MSGBUFLEN];
	char *lastpos;
	int rsize;
	long bytestotal = 0;

	while (c_sd > 0 && bytestotal <= 64)
	{
		for (int ic = 0; ic < MSGBUFLEN; buf[ic++] = '\0');
		rsize = recv(c_sd, buf, MSGBUFLEN, 0); 
		if (rsize == 0) 
		{ 
			break; 
		} 
		send(c_sd, buf, strlen(buf), 0); 
		bytestotal += strlen(buf);
	}
        
	return bytestotal;
}


/*
 * main:
 *  argc ... argument counter
 *  argv ... array of arguments
 *  returns     program exit code
 * main entry point for server6_clone program
 */
int main(int argc, char *argv[])
{
	/* variables needed for ipv6 server sockets */                   
	int s_sd; 											// s_sd server socket descriptor
	struct sockaddr_in6 s_addr, c_addr;					// ipv6 server & client socket s_addr
	socklen_t c_addr_len = sizeof(c_addr);
	uint16_t tcp6port = TCPv6_PORT; 					// tcp v6 port default to TCPv6_PORT 7777 
  	char *serveraddr = "2600:1f18:7a3f:a700::6291";  // tcp v6 server address
	int reuseaddr = 1;    
	int wstatus, w;
	pid_t pid, wpid;									// pid needed for both fork() and clone(...)
	/* variables needed for stack and cloning */
	char *stack, *stacktop;                              
	off_t offset, pa_offset; 
	long pagesze_offet;
	struct utsname uts;
	
	progname = argv[0]; 
	printf("%s program started...\n", progname);

	offset = (off_t)MMAP_OFFSET;
	pagesze_offet = sysconf(_SC_PAGE_SIZE) - 1;
	pa_offset = offset & ~(sysconf(_SC_PAGE_SIZE) - 1);
	
	if (argc > 1) 
  	{ 
		serveraddr = argv[1];
		if (argc > 2) 
			if ((tcp6port = (uint16_t)atol(argv[2])) < 2) 
				tcp6port = TCPv6_PORT; 
	}

	printf("server trys listening on address %s port: %d\n", serveraddr, (int) tcp6port);	
	
	s_sd = socket(AF_INET6, SOCK_STREAM, IPPROTO_TCP); 	// create socket 
	if (s_sd == -1) 
		usemsg("Error on creating tcpv6 socket with socket()", EXIT_FAILURE);
	
	done = setsockopt(s_sd, SOL_SOCKET, SO_REUSEADDR, 	// set socket option  
				&reuseaddr, sizeof(reuseaddr));
	if (done == -1)
		usemsg("Error on setsockopt tcpv6 socket with socket()", EXIT_FAILURE);

	s_addr.sin6_family = AF_INET6; 
	s_addr.sin6_port = htons(tcp6port);
	// IPv6 string to sockaddr_in6.
	inet_pton(AF_INET6, serveraddr, &s_addr.sin6_addr);
	// s_addr.sin6_addr =  in6addr_any;
	
	done = bind(s_sd, (struct sockaddr *)&s_addr, 		// bind socket to s_addr  
		sizeof(s_addr));
	if (done == -1) 
	{
		close(s_sd);
		usemsg("Error on binding tcpv6 socket to sockadrwith socket()", EXIT_FAILURE);
	}
	
	done = listen(s_sd, 10);                   			// listen on socket, max connections 10 
	if (done == -1)	
	{
		close(s_sd);
		usemsg("listenning on socket failed!", EXIT_FAILURE);
	}		
	
	stack = mmap(NULL, STACKSZE, 						// memory map to stack, needed for clone(2) 
				PROT_READ | PROT_WRITE, MAP_PRIVATE | MAP_ANONYMOUS | MAP_STACK, -1, 0);
	if (stack == MAP_FAILED) 
	{               
		close(s_sd);  									// close server socket descriptor and exit with usemsg
		usemsg("mmap(NULL, STACKSZE, PROT_READ|PROT_WRITE, MAP_PRIVATE|MAP_ANONYMOUS|MAP_SHARED|MAP_STACK, -1, 0)  failed", 
			EXIT_FAILURE); 				  
	}        
	stacktop = stack + STACKSZE;    					// Assume stack grows downward
                
	while (1) 											// server runs forever
	{
	/*
	 * 	munmap(stack, STACKSZE);	 					// unmap stack 
	 */
                
		c_sd = accept(s_sd, 							// accept client connections to socket
				(struct sockaddr*)&c_addr, &c_addr_len);   
		if (c_sd == -1)
		{               
			close(s_sd);  								// close server socket descriptor and exit with usemsg
			usemsg("Errror when waiting on client connections by accept()", EXIT_FAILURE);
		}
                                        
	/*
	 * 	stack = mmap(NULL, STACKSZE,   					// Allocate stack again 
	 * 		PROT_READ | PROT_WRITE, MAP_PRIVATE | MAP_ANONYMOUS | MAP_STACK, -1, 0);                 
	 *
	 * 	if (stack == MAP_FAILED) 
	 * 	{
	 *   	sprintf(outmsg, "mmap(NULL, %d, PROT_READ|PROT_WRITE, MAP_PRIVATE|MAP_ANONYMOUS|MAP_SHARED|MAP_STACK, %d, %ld) failed",
	 *  					STACKSZE, -1, 0);
	 * 		close(s);                                               
	 *      close(c);                                                               
	 *      usemsg(outmsg, EXIT_FAILURE, 0);
	 * 	}
	 * 	else
	 *  	stacktop = stack + STACKSZE;                    
	 */
                 
		if ((pid = clone(childfunc, stacktop, 	 	// execute clone instead of pid=fork()
					CLONE_NEWUTS | SIGCHLD, argv[0])) == -1)
		{
			printf("%s: cannot clone(2), trying to fork(2)", progname);
			if ((pid = fork()) == -1)
			{
				close(s_sd);    					// close server socket descriptor
				close(c_sd);  						// close client socket descriptor 
				munmap(stack, STACKSZE); 			// unmap program stack
				usemsg("clone(2) and fork(2) failed and returned -1. Can neither clone(2) nor fork(2) process!",
						EXIT_FAILURE);
			}
			else if (pid == 0) 						// now in child process
			{
				 close(s_sd);						// close duplicated server socket descriptor in child
				 int exitchild = childfunc(argv[0]);
				 exit(exitchild);					// exit child process
			}
			else 									// now in parent process
			{
				close(c_sd);						// close client socket descriptor in parent
			}
		}

		printf("%s:\tsockets:\ts_sd=%d,\tc_sd=%d\nchild pid: %ld",
				progname, s_sd, c_sd, (intmax_t) pid);

		sleep(1);                    				// sleep 1s to not permanently occupy scheduler 

        /*
		for(done = 0; done == 0;) 
		{
			w = waitpid(pid, &wstatus, 				// Wait for child 
					WUNTRACED | WCONTINUED);  			
			if (w == -1) 	
			{
				sprintf(outmsg, "%s:\twaitpid(%ld, NULL, 0) returned -1.\nExiting -1", progname, (intmax_t) pid);
				close(s_sd);  						// close server socket descripto
				close(c_sd);          			    // close client socket descriptor
				munmap(stack, STACKSZE);			// unmap program stack                         
				usemsg(outmsg, EXIT_FAILURE);
			}
			
			if (WIFEXITED(wstatus)) 
				 printf("child pid %d exited, status=%d, done=%d\n", 
					pid, WEXITSTATUS(wstatus), ++done);
			else if (WIFSIGNALED(wstatus)) 
				printf("child pid %d terminated, signal=%d, done=%d\n", 
					pid, WTERMSIG(wstatus), ++done);
			else if (WIFSTOPPED(wstatus))
				printf("child pid %d stopped, signal=%d, done=%d\n", 
					pid, WTERMSIG(wstatus), done);
			else if (WIFCONTINUED(wstatus))
				printf("child pid %d continued..., done=%d\n", 
					pid, done);					
		}
        */
		
	}

}
