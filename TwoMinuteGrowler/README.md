TwoMinuteGrowler
================

Shows the use of using Growl in your applications.  It is simple Two-Minute countdown timer that
notifies the user by Growl notifications of specific time events.

Author:  Kenneth J. Pouncey 2011/01/23 

Requirements:
=============
Growl application installed
Growl SDK to be linked in
And of course the MonoMac plugin for MonoDevelop 

Custom Command for build:
=========================
This sample uses a custom command for After Build to link in the Growl framework.

mkdir -p ${TargetDir}/${SolutionName}.app/Contents/Frameworks; cp –r <LINK_TO_YOUR_GROWL_FRAMEWORK>/Frameworks/Growl.framework ${TargetDir}/${SolutionName}.app/Contents/Frameworks

Make sure you replace the <LINK_TO_YOUR_GROWL_FRAMEWORK> with the directory location where you copied the SDK frameworks.


For a full description of the process of linking the Growl framework can be found here:

http://cocoa-mono.org/?p=254
