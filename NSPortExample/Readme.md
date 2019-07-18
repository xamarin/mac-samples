---
name: Xamarin.Mac - NSPortExample
description: The following is an example showing use of NSMessagePort & CFMessagePort to send information cross process, between applications. In the case of...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-mac
urlFragment: nsportexample
---
# NSPortExample

The following is an example showing use of `NSMessagePort` & `CFMessagePort` to send information cross process, between applications. In the case of this example, from a Xamarin.Mac C# program to a running Objective-C program.

## The example is based off the [Inter-Process Communication](http://nshipster.com/inter-process-communication/) article by Mattt Thompson.

## **Note:** There are a few binding that are used in the article that are not currently available in Xamarin.Mac. These are being tracked by this [Bugzilla](https://bugzilla.xamarin.com/show_bug.cgi?id=30815) defect.

## Running this Example

Follow these instructions to run this sample: 

1. Build and launch MessageReceiver first (via XCode)
2. Build and launch MessageSender (via Xamarin Studio)
3. Type a string in the MessageSender and hit the Send button
4. The message will appear in the MessageReceiver application
