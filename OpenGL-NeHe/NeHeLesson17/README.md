NeHe Lesson 17
==============

Converted to Mono and C# by Kenneth J. Pouncey 2011/03/12 
from NeHe Productions: OpenGL Lessons
http://nehe.gamedev.net/data/lessons/lesson.asp?lesson=17

===========================================================================
DESCRIPTION:

2D Texture Font:
================

The original version of this tutorial code was written by Giuseppe D'Agata. 
In this tutorial you will learn how to write any character or phrase you want 
to the screen using texture mapped quads. You will learn how to read one of 
256 different characters from a 256x256 texture map, and finally I will show 
you how to place each character on the screen using pixels rather than units. 
Even if you're not interested in drawing 2D texture mapped characters to the 
screen, there is lots to learn from this tutorial. Definitely worth reading! 


This tutorial brought to you by NeHe & Giuseppe D'Agata... 

I know everyones probably sick of fonts. The text tutorials I've done so far 
not only display text, they display 3D text, texture mapped text, and can 
handle variables. But what happens if you're porting your project to a machine 
that doesn't support Bitmap or Outline fonts? 

Thanks to Giuseppe D'Agata we have yet another font tutorial. What could 
possibly be left you ask!? If you remember in the first Font tutorial I 
mentioned using textures to draw letters to the screen. Usually when you use 
textures to draw text to the screen you load up your favorite art program, 
select a font, then type the letters or phase you want to display. You then 
save the bitmap and load it into your program as a texture. Not very efficient 
for a program that require alot of text, or text that continually changes! 

This program uses just ONE texture to display any of 256 different characters 
on the screen. Keep in mind your average character is just 16 pixels wide and 
roughly 16 pixels tall. If you take your standard 256x256 texture it's easy to 
see that you can fit 16 letters across, and you can have a total of 16 rows up
and down. If you need a more detailed explanation: The texture is 256 pixels 
wide, a character is 16 pixels wide. 256 divided by 16 is 16 :) 

So... Lets create a 2D textured font demo! This program expands on the code 
from lesson 1. In the first section of the program, we include the math and 
stdio libraries. We need the math library to move our letters around the screen 
using SIN and COS, and we need the stdio library to make sure the bitmaps we 
want to use actually exist before we try to make textures out of them.
===========================================================================
SAMPLE REQUIREMENTS

The supplied solution requires MonoMac bindings from the Mono Project.

===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

n/a
