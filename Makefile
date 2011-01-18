MDTOOL=/Applications/MonoDevelop.app/Contents/MacOS/mdtool

XDIRS = \
	AnimatingViews ButtonMadness DatePicker DocumentSample		\
	DrawerMadness GlossyClock macdoc NSTableViewBinding		\
	PopupBindings PredicateEditorSample QTRecorder			\
	RoundedTransparentWindow Rulers SearchField SkinnableApp	\
	StillMotion VillainTracker WhereIsMyMac

all:
	for i in $(XDIRS); do (cd $$i; $(MDTOOL) build); done

clean:
	for i in $(DIRS); do (cd $$i; make clean); done
	for i in $(XDIRS); do (cd $$i; rm -rf bin); done
