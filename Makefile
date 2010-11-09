DIRS = Hello 
XDIRS = ButtonMadness DrawerMadness

all:
	for i in $(DIRS); do (cd $$i; make); done
	for i in $(XDIRS); do (cd $$i; xbuild); done
