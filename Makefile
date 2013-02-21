include config.mk

XDIRS = \
	MarkdownViewer				\
	FSEvents				\
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
	DatePicker				\
	DockAppIcon				\
	DocumentSample				\
	DrawerMadness 				\
	Fire 				\
	GLFullScreen				\
	GLSLShader				\
	GlossyClock 				\
	ImageKitDemoStep1			\
	NSTableViewBinding			\
	OpenGLLayer 				\
	OpenGLViewSample 			\
	OpenGL-NeHe/NeHeLesson1			\
	OpenGL-NeHe/NeHeLesson2			\
	OpenGL-NeHe/NeHeLesson3			\
	OpenGL-NeHe/NeHeLesson4			\
	OpenGL-NeHe/NeHeLesson5			\
	OpenGL-NeHe/NeHeLesson6			\
	OpenGL-NeHe/NeHeLesson7			\
	OpenGL-NeHe/NeHeLesson8			\
	OpenGL-NeHe/NeHeLesson9			\
	PredicateEditorSample 			\
	PopupBindings 				\
	QTRecorder				\
	RoundedTransparentWindow		\
	Rulers 					\
	SearchField 				\
	SkinnableApp				\
	StillMotion				\
	TwoMinuteGrowler			\
	WhereIsMyMac				\
	VillainTracker

all:
	for i in $(XDIRS); do (cd $$i && "$(MDTOOL)" build) || exit $$?; done
	$(MAKE) -C MicroSamples

clean:
	-for i in $(XDIRS); do (cd $$i && rm -rf bin) || exit $$?; done
	$(MAKE) -C MicroSamples clean
