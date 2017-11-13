#include <stdio.h>
#include <sys/time.h>

// clang -c time.c -o time.o && libtool -static time.o -o time.a
void xamarin_app_initialize (void* p)
{
	struct timeval te; 
	gettimeofday (&te, NULL);
	long long milliseconds = te.tv_sec * 1000LL + te.tv_usec / 1000;
	printf ("%lld\n", milliseconds);
	fflush (stdout);
}
