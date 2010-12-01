DIRS = Hello 
XDIRS = \
	AnimatingViews ButtonMadness DocumentSample 	\
	DrawerMadness NSTableViewBinding PlayFile 	\
	PopupBindings QTRecorder Rulers StillMotion

all:
	for i in $(DIRS); do (cd $$i; make); done
	for i in $(XDIRS); do (cd $$i; xbuild); done
