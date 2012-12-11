GLFullScreen
=============

Converted to Mono and C# by Kenneth J. Pouncey 2011/03/05 from Apple's
Sample project of the same name (Copyright (C) 2010 Apple Inc. All
rights reserved.).

===========================================================================
DESCRIPTION:

This sample code demonstrates OpenGL drawing to the entire screen. 

When in the window mode, 
	press the "Go FullScreen" button to switch to the full-screen mode;
When in the full-screen mode, 
	press [ESC] to switch to the window mode;
In both modes, 
	press [space] to toggle rotation of the globe;
	press [w]/[W] to toggle wireframe rendering;
	holding and dragging the mouse to change the roll angle and from which the light is coming.

===========================================================================
SAMPLE REQUIREMENTS

The supplied solution requires MonoMac bindings from the Mono Project.

===========================================================================
PACKAGING LIST:

MainWindowController.cs
A controller object that handles full-screen/window modes switching and user interactions.

MyOpenGLView.m
An NSView subclass that delegates to separate "scene" and "controller" objects for OpenGL rendering and input event handling.

Scene.cs
A delegate object used by MyOpenGLView and MainWindowController to render a simple scene.

Texture.cs
A helper class that loads an OpenGL texture from an image path.

===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

n/a
