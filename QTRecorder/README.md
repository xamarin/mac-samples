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


Apple has [deprecated](https://developer.apple.com/library/content/technotes/tn2300/_index.html#//apple_ref/doc/uid/DTS40012852-CH1-XCODE) QTKit and no longer ships header files needed by the static registrar.

This can be worked around by adding:

`<MonoBundlingExtraArgs>--registrar=dynamic</MonoBundlingExtraArgs>`

to the MMP arguments of all projects still using QTKit.

More information about the registrar can be found [here](https://developer.xamarin.com/guides/mac/under-the-hood/registrar/).