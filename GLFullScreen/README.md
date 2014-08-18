GLFullScreen
============

This sample code demonstrates `OpenGL` drawing to the entire screen.

Files Description
-----------------

* `MainWindowController.cs` - A controller object that handles full-screen/window modes switching and user interactions.
* `MyOpenGLView.m` - An `NSView` subclass that delegates to separate "scene" and "controller" objects for OpenGL rendering and input event handling.
* `Scene.cs` - A delegate object used by `MyOpenGLView` and `MainWindowController` to render a simple scene.
* `Texture.cs` - A helper class that loads an `OpenGL` texture from an image path.

Instructions
------------

* In the window mode, press the "Go FullScreen" button to switch to the full-screen mode.
* In the full-screen mode, press [ESC] to switch to the window mode.
* In both modes, press [space] to toggle rotation of the globe.
* Press [w]/[W] to toggle wireframe rendering.
* Hold and drag the mouse to change the roll angle and from which the light is coming.

Author
------ 

Copyright (C) 2014 Apple Inc. All rights reserved.  
Ported to Xamarin.Mac by Kenneth J. Pouncey.