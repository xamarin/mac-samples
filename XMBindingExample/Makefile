XM_PATH = /Library/Frameworks/Xamarin.Mac.framework/Versions/Current

all:
	@# Create a dylib to test against
	mkdir -p bin tmp
	clang -dynamiclib -std=gnu99 SimpleClass.m  -current_version 1.0 -compatibility_version 1.0 -fvisibility=hidden -framework Cocoa -o bin/SimpleClass.dylib

	@# Create the C# Binding
	@# This terrible command syntax will be improved in XM 2.2
	MONO_PATH=$(XM_PATH)/lib/mono/Xamarin.Mac $(XM_PATH)/bin/bmac-mobile-mono $(XM_PATH)/lib/bmac/bmac-mobile.exe -baselib:$(XM_PATH)/lib/reference/mobile/Xamarin.Mac.dll --api=SimpleClass.cs -o:bin/SimpleClass.dll --tmpdir=tmp --ns=Simple

	@# Create a simple test project. If you create a Xamarin.Mac project in XS, this will be done for you when you build
	mcs /out:bin/SimpleTest.exe SimpleTest.cs /target:exe /nostdlib /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/System.dll /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/System.Core.dll /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll /reference:bin/SimpleClass.dll /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/mscorlib.dll
	$(XM_PATH)/bin/mmp /output:bin /name:SimpleTest /profile:Xamarin.Mac /arch:x86_64 /sgen /new-refcount /nolink /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/System.dll /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/System.Core.dll /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll /assembly:bin/SimpleClass.dll /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/mscorlib.dll bin/SimpleTest.exe

	@# Copy the dylib into your bundle. This will be a post-build step in XS, unless you are referencing a system installed dylib
	cp bin/SimpleClass.dylib bin/SimpleTest.app/Contents/MonoBundle/

	@# Show that everything is working
	./bin/SimpleTest.app/Contents/MacOS/SimpleTest

clean:
	rm -r ./bin
	rm -r ./tmp