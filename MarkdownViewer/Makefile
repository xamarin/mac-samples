MONODEVELOP_APP := /Applications/MonoDevelop.app
MDTOOL ?= $(MONODEVELOP_APP)/Contents/MacOS/mdtool

CFLAGS = -m32
LDFLAGS = -m32
LIBSUNDOWN_DYLIB = Resources/libsundown.dylib

.PHONY: all dist clean

dist:: Markdown.app
	rm -rf $<
	cp -a bin/Debug/$< $<
	zip Markdown.zip -r9q $<

-include config.mk

all: Markdown.app

$(LIBSUNDOWN_DYLIB):
	$(MAKE) -f Makefile.sundown CFLAGS="$(CFLAGS)" LDFLAGS="$(LDFLAGS)" $@

Markdown.app: $(LIBSUNDOWN_DYLIB)
	"$(MDTOOL)" build Markdown.sln
	rm -rf $@

clean:
	$(MAKE) -f Makefile.sundown clean
	"$(MDTOOL)" build -t:Clean Markdown.sln
	rm -rf bin obj Markdown.{app,zip}
