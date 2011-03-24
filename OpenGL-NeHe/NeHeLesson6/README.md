NeHe Lesson 6
=============

Converted to Mono and C# by Kenneth J. Pouncey 2011/03/06 
from NeHe Productions: OpenGL Lessons
http://nehe.gamedev.net/data/lessons/lesson.asp?lesson=06

===========================================================================
DESCRIPTION:

Texture Mapping:
================

You asked for it, so here it is... Texture Mapping!!! In this tutorial I will 
teach you how to map a bitmap image onto the six sides of a cube. We'll use 
the GL code from lesson one to create this project. It's easier to start with 
an empty GL window than to modify the last tutorial.  

You'll find the code from lesson one is extremely valuable when it comes to 
developing a project quickly. The code in lesson one sets everything up for 
you, all you have to do is concentrate on programming the effect(s). 

Learning how to texture map has many benefits. Lets say you wanted a missile 
to fly across the screen. Up until this tutorial we'd probably make the entire 
missile out of polygons, and fancy colors. With texture mapping, you can take 
a real picture of a missile and make the picture fly across the screen. Which 
do you think will look better? A photograph or an object made up of triangles 
and squares? By using texture mapping, not only will it look better, but your 
program will run faster. The texture mapped missile would only be one quad 
moving across the screen. A missile made out of polygons could be made up of 
hundreds or thousands of polygons. The single texture mapped quad will use alot 
less processing power. 

===========================================================================
SAMPLE REQUIREMENTS

The supplied solution requires MonoMac bindings from the Mono Project.

===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

n/a
