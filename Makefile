MDTOOL=/Applications/MonoDevelop.app/Contents/MacOS/mdtool

iDIRS = \
	NSAlert					\
	AnimatingViews 				\
	ButtonMadness 				\
	CAQuartzComposition 			\
	CoreAnimationBook/BackgroundFilteredView\
	CoreAnimationBook/CustomAnimationTiming \
	CoreAnimationBook/CustomizeAnimation 	\
	CoreAnimationBook/CustomizeAnimation2 	\
	CoreAnimationBook/FilteredView 		\
	CoreAnimationBook/GroupAnimation 	\
	CoreAnimationBook/KeyFrameMoveAView 	\
	CoreAnimationBook/LayerBackedControls 	\
	CoreAnimationBook/QCBackground 		\
	CoreAnimationBook/TimedAnimation 	\
	CoreAnimationBook/QCBackground		\
	CoreTextArcMonoMac			\
	CoreWLANWirelessManager			\

XDIRS = \
	PredicateEditorSample 			\
	PopupBindings 				\
	QTRecorder				\
	RoundedTransparentWindow		\
	Rulers 					\
	SearchField 				\
	SkinnableApp				\
	StillMotion				\
	TwoMinuteGrowler			\
	VillainTracker 				\
	WhereIsMyMac

all:
	for i in $(XDIRS); do (cd $$i; $(MDTOOL) build); done
	(cd MicroSamples; make)

clean:
	-for i in $(DIRS); do (cd $$i; make clean); done
	-for i in $(XDIRS); do (cd $$i; rm -rf bin); done
	-(cd MicroSamples; rm -rf *.exe *.mdb)
