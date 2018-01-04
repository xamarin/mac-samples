QTRecorder
==========

This applications shows how to use various QuickTime features: 

* Capture video.
* Display a live video feed.
* Play the newly created video in the UI.
* Apply CoreImage effects to the video source.
* Use Cocoa drawers.
* Use Cocoa Key/Value bindings to the UI and a document-based interface.


---
###### Attention


The partial static registrar isn't compatible with QTKit.

The partial static registrar assumes everything in Xamarin.Mac.dll has been registered properly, but we skip QTKit-based types, because we don't have headers for those.

So you should add `<MonoBundlingExtraArgs>--registrar=dynamic</MonoBundlingExtraArgs>` to all configurations.