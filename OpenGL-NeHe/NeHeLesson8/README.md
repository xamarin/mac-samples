NeHe Lesson 8
=============

Converted to Mono and C# by Kenneth J. Pouncey 2011/03/06 
from NeHe Productions: OpenGL Lessons
http://nehe.gamedev.net/data/lessons/lesson.asp?lesson=08

===========================================================================
DESCRIPTION:

Blending:
=========
There was a reason for the wait. A fellow programmer from the totally cool 
site Hypercosm, had asked if he could write a tutorial on blending. Lesson 
eight was going to be a blending tutorial anyways. So the timing was perfect! 
This tutorial expands on lesson seven. Blending is a very cool effect... 
I hope you all enjoy the tutorial. The author of this tutorial is Tom Stanis. 
He's put alot of effort into the tutorial, so let him know what you think. 
Blending is not an easy topic to cover. 


Simple Transparency 
===================

Most special effects in OpenGL rely on some type of blending. Blending is used 
to combine the color of a given pixel that is about to be drawn with the pixel 
that is already on the screen. How the colors are combined is based on the alpha 
value of the colors, and/or the blending function that is being used. Alpha is 
a 4th color component usually specified at the end. In the past you have used 
GL_RGB to specify color with 3 components. GL_RGBA can be used to specify alpha 
as well. In addition, we can use glColor4f() instead of glColor3f(). 

Most people think of Alpha as how opaque a material is. An alpha value of 0.0 
would mean that the material is completely transparent. A value of 1.0 would be 
totally opaque. 

The Blending Equation 
=====================

If you are uncomfortable with math, and just want to see how to do transparency, 
skip this section. If you want to understand how blending works, this section is 
for you. 

(Rs Sr + Rd Dr, Gs Sg + Gd Dg, Bs Sb + Bd Db, As Sa + Ad Da) 

OpenGL will calculate the result of blending two pixels based on the above equation. 
The s and d subscripts specify the source and destination pixels. The S and D 
components are the blend factors. These values indicate how you would like to blend 
the pixels. The most common values for S and D are (As, As, As, As) (AKA source alpha) 
for S and (1, 1, 1, 1) - (As, As, As, As) (AKA one minus src alpha) for D. 
This will yield a blending equation that looks like this: 

(Rs As + Rd (1 - As), Gs As + Gd (1 - As), Bs As + Bd (1 - As), As As + Ad (1 - As)) 

This equation will yield transparent/translucent style effects. 

Blending in OpenGL 
==================

We enable blending just like everything else. Then we set the equation, and turn 
off depth buffer writing when drawing transparent objects, since we still want 
objects behind the translucent shapes to be drawn. This isn't the proper way to 
blend, but most the time in simple projects it will work fine. 

Rui Martins Adds: The correct way is to draw all the transparent (with alpha < 1.0) 
polys after you have drawn the entire scene, and to draw them in reverse depth 
order (farthest first). This is due to the fact that blending two polygons (1 and 2) 
in different order gives different results, i.e. (assuming poly 1 is nearest to 
the viewer, the correct way would be to draw poly 2 first and then poly 1. If you 
look at it, like in reality, all the light comming from behind these two polys 
(which are transparent) has to pass poly 2 first and then poly 1 before it reaches 
the eye of the viewer. You should SORT THE TRANSPARENT POLYGONS BY DEPTH and draw 
them AFTER THE ENTIRE SCENE HAS BEEN DRAWN, with the DEPTH BUFFER ENABLED, or you 
will get incorrect results. I know this sometimes is a pain, but this is the 
correct way to do it. 

===========================================================================
SAMPLE REQUIREMENTS

The supplied solution requires MonoMac bindings from the Mono Project.

===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

n/a
