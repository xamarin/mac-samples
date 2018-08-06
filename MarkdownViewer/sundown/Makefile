OBJECTS = \
	autolink.o \
	buffer.o \
	houdini_href_e.o \
	houdini_html_e.o \
	html.o \
	html_smartypants.o \
	markdown.o \
	stack.o

CFLAGS = -fPIC -Wall -Werror -Wsign-compare -std=c99 -O3
LDFLAGS = -dynamiclib -undefined suppress -flat_namespace

all: libsundown.dylib

libsundown.dylib: $(OBJECTS)
	$(CC) -o $@ $+ $(LDFLAGS)

%.o: %.c
	$(CC) -c $< $(CFLAGS)

clean:
	rm -f $(OBJECTS)
	rm -f libsundown.dylib
