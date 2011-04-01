NeHe Lesson 13
==============

Converted to Mono and C# by Kenneth J. Pouncey 2011/03/21 
from NeHe Productions: OpenGL Lessons
http://nehe.gamedev.net/data/lessons/lesson.asp?lesson=13

===========================================================================
DESCRIPTION:

I think the question I get asked most often in email is "how can I display 
text on the screen using OpenGL?". You could always texture map text onto 
your screen. Of course you have very little control over the text, and 
unless you're good at blending, the text usually ends up mixing with the 
images on the screen. If you'd like an easy way to write the text you want 
anywhere you want on the screen in any color you want, using any of your 
computers built in fonts, then this tutorial is definitely for you. Bitmaps 
font's are 2D scalable fonts, they can not be rotated. They always face forward. 

Welcome to yet another Tutorial. This time on I'll be teaching you how to 
use Bitmap Fonts. You may be saying to yourself "what's so hard about 
putting text onto the screen". If you've ever tried it, it's not that easy!

Sure you can load up an art program, write text onto an image, load the image 
into your OpenGL program, turn on blending then map the text onto the screen. 
But this is time consuming, the final result usually looks blurry or blocky 
depending on the type of filtering you use, and unless your image has an 
alpha channel your text will end up transparent (blended with the objects 
on the screen) once it's mapped to the screen.

If you've ever used Wordpad, Microsoft Word or some other Word Processor, 
you may have noticed all the different types of Font's available. This tutorial 
will teach you how to use the exact same fonts in your own OpenGL programs. As 
a matter of fact... Any font you install on your computer can be used in your demos.

Not only do Bitmap Fonts looks 100 times better than graphical fonts (textures). 
You can change the text on the fly. No need to make textures for each word 
or letter you want to write to the screen. Just position the text, and use 
my handy new gl command to display the text on the screen.

I tried to make the command as simple as possible. All you do is type 
glPrint("Hello"). It's that easy. Anyways. You can tell by the long intro 
that I'm pretty happy with this tutorial. It took me roughly 1 1/2 hours to 
create the program. Why so long? Because there is literally no information 
available on using Bitmap Fonts, unless of course you enjoy MFC code. In order 
to keep the code simple I decided it would be nice if I wrote it all in 
simple to understand C code :)

A small note, this code is Windows specific. It uses the wgl functions of 
Windows to build the font. Apparently Apple has agl support that should 
do the same thing, and X has glx. Unfortunately I can't guarantee this 
code is portable. If anyone has platform independant code to draw fonts to 
the screen, send it my way and I'll write another font tutorial.

We start off with the typical code from lesson 1. We'll be adding the stdio.h 
header file for standard input/output operations; the stdarg.h header file to 
parse the text and convert variables to text, and finally the math.h header 
file so we can move the text around the screen using SIN and COS.

===========================================================================
SAMPLE REQUIREMENTS

The supplied solution requires MonoMac bindings from the Mono Project.

===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

n/a
