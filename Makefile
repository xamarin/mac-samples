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
	WhereIsMyMac				\
	VillainTracker

.PHONY: all clean git-reset-all-csproj migrate-all-to-xammac copy-apps qa-bundle

all:
	for i in $(XDIRS); do (cd $$i && "$(MDTOOL)" build) || exit $$?; done
	$(MAKE) -C MicroSamples

clean:
	-for i in $(XDIRS); do (cd $$i && rm -rf bin) || exit $$?; done
	$(MAKE) -C MicroSamples clean

git-reset-all-csproj:
	find . -name \*.csproj -exec git co {} \;

migrate-all-to-xammac:
	find . -name \*.csproj -exec ./migrate-to-xammac {} \;

copy-apps:
	rm -rf apps
	mkdir apps
	find . -type d -name \*.app -exec cp -a {} apps \;

qa-bundle: clean all copy-apps
	rm -rf qa-bundle
	mkdir qa-bundle
	mv apps "qa-bundle/MonoMac_From_$$(echo $$(/usr/libexec/PlistBuddy -c "Print :CFBundleName" -c "Print :CFBundleShortVersionString" "$(MDROOT)/Contents/Info.plist") | tr ' ' '_')"
	$(MAKE) clean
	$(MAKE) migrate-all-to-xammac
	for i in $(XDIRS); do (cd $$i && "$(MDTOOL)" build) || { echo "$$i failed to build for XamMac"; true; }; done;
	$(MAKE) copy-apps
	mv apps "qa-bundle/Xamarin.Mac_$$(cat /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/Version)"
