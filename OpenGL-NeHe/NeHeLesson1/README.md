NeHe Lesson 1
=============

Converted to Mono and C# by Kenneth J. Pouncey 2011/03/06 
from NeHe Productions: OpenGL Lessons
http://nehe.gamedev.net/data/lessons/lesson.asp?lesson=01

===========================================================================
DESCRIPTION:

This is the first lesson from the site that shows only the setup of the
Base Program.

MainWindowController.cs
A controller object that handles full-screen/window modes switching and user interactions.

MyOpenGLView.m
An NSView subclass that delegates to separate "scene" and "controller" objects for OpenGL rendering and input event handling.

Scene.cs
A delegate object used by MyOpenGLView and MainWindowController to render a simple scene.

Menu options:
View -> Toggle Full Screen or Alt-Cmd-F
===========================================================================
SAMPLE REQUIREMENTS

The supplied solution requires MonoMac bindings from the Mono Project.

===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

n/a
