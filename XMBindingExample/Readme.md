XMBindingExample
================

This is a quick example, showing how to invoke `bmac` to create a binding for Xamarin.Mac Unified. In this example, a simple Objective-C library (`SimpleClass.h` and `SimpleClass.m`) will be bound to C# so it can be used in Xamarin.Mac. This example assumes that we will be creating a 64-bit library.

Binding Objective-C for Xamarin.Mac is very similar to binding for Xamarin.iOS, so we highly suggest that you read our iOS [Binding Objective-C](http://developer.xamarin.com/guides/ios/advanced_topics/binding_objective-c/) documentation before working through this sample.

## The Objective-C Library

The library that we are going to bind simply outputs **Hello, World!** when its `DoIt` method is invoked. And is defined as follows:

### SimpleClass.h

	#import <Cocoa/Cocoa.h>
	
	@interface SimpleClass : NSObject
	
	- (int) doIt;
	
	@end

### SimpleClass.m

	#import "SimpleClass.h"
	
	@implementation SimpleClass
	
	- (int)doIt {
		    NSLog(@"Hello, World!");
	    	    return 42;
	}
	@end

This class will be compiled into a _Dylib_ before we bind it to C# for use. 

## Binding Definition

The `SimpleClass.cs` file includes the instructions used to bind the Objective-C library to C# as follows:

	using Foundation;
	
	namespace Simple {
	  [BaseType (typeof (NSObject))]
	  interface SimpleClass {
	    [Export ("doIt")]
	    int DoIt ();
  	  }
	}

It tells `bmac` that we are binding a class `SimpleClass` of type `NSObject` and that it has a method of `DoIt` that returns an `int` value when the method is invoked.

For more information on Binding Definitions, please see our iOS [Binding Types Reference Guide](http://developer.xamarin.com/guides/ios/advanced_topics/binding_objective-c/binding_types_reference_guide/). 

## The Test Application

The `SimpleTest.cs` file includes a test Xamarin.Mac application that will be compiled and run again the new library after binding has successfully completed as follows:

	using System;

	using AppKit;
	using Simple;
	using System.Reflection;
	using System.IO;

	namespace SimpleTest
	{
		static class MainClass
		{
			// http://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
			public static string GetCurrentExecutingDirectory()
			{
				string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
				return Path.GetDirectoryName(filePath);
			}

			static void Main (string[] args)
			{
				var v = ObjCRuntime.Dlfcn.dlopen (GetCurrentExecutingDirectory () + "/SimpleClass.dylib", 0);

				NSApplication.Init ();
				SimpleClass c = new SimpleClass ();
				Console.WriteLine (c.DoIt());
			}
		}
	}

This test shows how to invoke the resulting library in Xamarin.Mac.

## The Makefile

We have included a `Makefile` that can be used to automate all of the step required to build the `Dylib`, bind it to C# and run the test application to confirm that binding was successful:

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

While the `Makefile` has been commented to make it easier to understand, we'll cover a few of the sections in detail below.

### Creating the Dylib

The first step is to compile the Objective-C class into a reusable library called `SimpleClass.dylib`:

	@# Create a dylib to test against
	mkdir -p bin tmp
	clang -dynamiclib -std=gnu99 SimpleClass.m  -current_version 1.0 -compatibility_version 1.0 -fvisibility=hidden -	framework Cocoa -o bin/SimpleClass.dylib

We'll need to include this library along with every Xamarin.Mac application that consumes it through binding.

## Creating the Binding

Next, the `Dylib` is bound to C# using the Binding Definition in the `SimpleClass.cs` file:

	@# Create the C# Binding
	@# This terrible command syntax will be improved in XM 2.2
	MONO_PATH=$(XM_PATH)/lib/mono/Xamarin.Mac $(XM_PATH)/bin/bmac-mobile-mono $(XM_PATH)/lib/bmac/bmac-mobile.exe -baselib:$(XM_PATH)/lib/reference/mobile/Xamarin.Mac.dll --api=SimpleClass.cs -o:bin/SimpleClass.dll --tmpdir=tmp --ns=Simple

A couple of things to note here:

* `$(XM_PATH)/lib/bmac/bmac-mobile.exe -baselib:$(XM_PATH)/lib/reference/mobile/Xamarin.Mac.dll` <br/>Binds against the Unified 64-bit version of Xamarin.Mac.
* `--api=SimpleClass.cs` <br/>Tells `mac` which Binding Definition file to use.
* `-o:bin/SimpleClass.dll` <br/>Defines the name of our output `.dll` library.
* `--ns=Simple` <br/>Defines the **Namespace** for our binding.

## Compiling the Test Application

Typically, you'll be creating a Xamarin.Mac Solution/Project File in Xamarin Studio to consume the bound library, but for the sake of easy testing, we are creating one in the `Makefile`:


	@# Create a simple test project. If you create a Xamarin.Mac project in XS, this will be done for you when you build
	mcs /out:bin/SimpleTest.exe SimpleTest.cs /target:exe /nostdlib /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/System.dll /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/System.Core.dll /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll /reference:bin/SimpleClass.dll /reference:$(XM_PATH)/lib/mono/Xamarin.Mac/mscorlib.dll
	$(XM_PATH)/bin/mmp /output:bin /name:SimpleTest /profile:Xamarin.Mac /arch:x86_64 /sgen /new-refcount /nolink /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/System.dll /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/System.Core.dll /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll /assembly:bin/SimpleClass.dll /assembly:$(XM_PATH)/lib/mono/Xamarin.Mac/mscorlib.dll bin/SimpleTest.exe
	
	@# Copy the dylib into your bundle. This will be a post-build step in XS, unless you are referencing a system installed dylib
	cp bin/SimpleClass.dylib bin/SimpleTest.app/Contents/MonoBundle/


The last line moves the `Dylib` into the Xamarin.Mac application's bundle. This would typically be done by Xamarin Studio automatically when you include the `SimpleClass.dll` in a project.

## Testing the Binding

Finally, we execute the test application to see if the binding was successful:

	@# Show that everything is working
	./bin/SimpleTest.app/Contents/MacOS/SimpleTest

# Executing the Makefile

To execute the `Makefile`, create the `Dylib`, bind it to C#, build the test app and run it, we'll do the following:

1. Start the **Terminal** application.
2. Move to the directory where the `Makefile` is.
3. Use the `make` command to execute the `Makefile`.

For example:

	Europa:~ kmullins$ cd /Users/kmullins/Downloads/XMBindingExample 
	Europa:XMBindingExample kmullins$ make

Would report the following example output while executing:

	mkdir -p bin tmp
	clang -dynamiclib -std=gnu99 SimpleClass.m  -current_version 1.0 -compatibility_version 1.0 -fvisibility=hidden -framework Cocoa -o bin/SimpleClass.dylib
		MONO_PATH=/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/bmac-mobile-mono /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/bmac/bmac-mobile.exe -baselib:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/reference/mobile/Xamarin.Mac.dll --api=SimpleClass.cs -o:bin/SimpleClass.dll --tmpdir=tmp --ns=Simple
		mcs /out:bin/SimpleTest.exe SimpleTest.cs /target:exe /nostdlib /reference:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/System.dll /reference:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/System.Core.dll /reference:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll /reference:bin/SimpleClass.dll /reference:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/mscorlib.dll
		SimpleTest.cs(21,8): warning CS0219: The variable `v' is assigned but its value is never used
		Compilation succeeded - 1 warning(s)
		/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/mmp /output:bin /name:SimpleTest /profile:Xamarin.Mac /arch:x86_64 /sgen /new-refcount /nolink /assembly:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/System.dll /assembly:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/System.Core.dll /assembly:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll /assembly:bin/SimpleClass.dll /assembly:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/mscorlib.dll bin/SimpleTest.exe
		Xamarin.Mac 2.0.1 Business Edition
		warning MM2006: Native library 'liboleaut32.dylib' was referenced but could not be found.
		bundling complete
		cp bin/SimpleClass.dylib bin/SimpleTest.app/Contents/MonoBundle/
		./bin/SimpleTest.app/Contents/MacOS/SimpleTest
		Xamarin.Mac: Could not load machine.config: /Users/kmullins/Downloads/XMBindingExample/bin/SimpleTest.app/Contents/MonoBundle/machine.config
		2015-06-04 09:04:39.801 SimpleTest[8503:6917825] Hello, World!
	42

If everything worked as expected, you'll see `Hello, World! 42` at the end of the results.

# Using the Binding in Another App

To use this binding in another Xamarin.Mac application, make a Reference to the `SimpleClass.dll` file that was created in the `bin` directory. Again, you'll need to moves the `Dylib` into the Xamarin.Mac application's bundle. This would typically be done by Xamarin Studio automatically when you include the `SimpleClass.dll` in a project.