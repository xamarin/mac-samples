MDTOOL=/Applications/MonoDevelop.app/Contents/MacOS/mdtool

XDIRS = \
	AnimatingViews ButtonMadness DocumentSample 	\
	DrawerMadness NSTableViewBinding PlayFile 	\
	PopupBindings QTRecorder Rulers StillMotion	\
	SearchField DatePicker VillainTracker		\
	SkinnableApp RoundedTransparentWindow		\
	PredicateEditorSample

all:
	for i in $(XDIRS); do (cd $$i; $(MDTOOL) build); done

clean:
	for i in $(DIRS); do (cd $$i; make clean); done
	for i in $(XDIRS); do (cd $$i; rm -rf bin); done
