## Startup Timing

One important metric of application performance is the time it takes to start up. Measuring that accurately can be a challenge, especially in the world of managed code, where a significant amount of work is done before your managed entry point is invoked.

This sample uses the custom initialization feature in Xamarin.Mac to have a C function invoked very early in the startup process. That function in our case just prints the current number of milliseconds via `gettimeofday`.

We can then compare that to a call to `DateTimeOffset.Now.ToUnixTimeMilliseconds` inside `AppDelegate.DidFinishLaunching` and have a very reasonable idea how long our startup took.

There will be a portion not measured here, as the creation of a process through the custom initialization method is not measure, but for a vast majority of applications this is neglectable.

The application can be launched from the command line via `bin/Debug/StartupTiming.app/Contents/MacOS/StartupTiming` to easily test timing repeatedly. 

## Technical Notes

- This application linked to the static library `native/time.a` which is native/time.c compiled via `clang -c time.c -o time.o && libtool -static time.o -o time.a`.
- `--link_flags="-force_load native/time.a"` needs to be passed as an additional mmp argument to prevent the native linker from stripping `xamarin_app_initialize` from your final binary.
