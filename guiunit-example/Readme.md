GUIUnit Example
===============

This is a quick example showing how to package up a `nunit` test that requires Xamarin.Mac into a bundle and invoke it using [GUIUnit](https://github.com/mono/guiunit).

## About GUIUnit

`GUIUnit` (based on `NUnitLite`) is a small-footprint implementation of much of the current `NUnit` framework. It is distributed in source form and is intended for use in situations where `NUnit` is too large or complex. In particular, it targets mobile and embedded environments as well as testing of applications that require "embedding" the framework in another piece of software, as when testing plugin architectures.

###USAGE

`NUnitLite` is not "installed" in your system. Instead, you should include `nunitlite.dll` in your project. Your test assembly should be an exe file and should reference the `nunitlite` assembly. If you place a call like this in your Main:

```
    new TextUI().Execute(args);
```

then `NUnitLite` will run all the tests in the test project, using the args provided. Use `-help` to see the available options.

### DOCUMENTATION

`NUnitLite` uses the `NUnit.Framework` namespace, which allows relatively easy portability between `NUnit` and `NUnitLite`. Currently, there is no separate set of documentation for `NUnitLite` so you should use the docs for `NUnit 2.6` or later in conjunction with the information in this file.

## Using this Example

To use this example, do the following:

1. In **Terminal**, navigate to the directory where the example is stored on your computer.
2. Type `make run` to create the package.
3. Type `make clear` when you are finished.
