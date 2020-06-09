# UnityUIDrawable

A simple and performant drawing system for UGUI-space that makes use of rendertextures and shaders.

![gif](https://i.imgur.com/GAe83Ug.gif)
![gif](https://i.imgur.com/u9SjhPA.gif)

### Requirements

Unity 2019.2+ (might work with older ones, just not tested with)
OpenGLES3, DX11, Vulkan (OpenGLES2 does not work)


## Getting Started

Open up the project and ExampleDrawingScene

or

Create a screenspace-canvas and insert an UIDrawablePrefab into it.


## UIDrawable.cs
The whole drawing functionality resides in the UIDrawable.cs, but the UIDrawablePrefab provides a set of tools to make it use.

### Initialization Color
Sets the color of the rendertexture (the background being drawn on) on Awake and when erased.

### Render Multiplier
Multiplier for the underlying rendertexture's resolution.
1 being the width/height of the current RectTransform.

### Colors
A helper array of colors that can be used via SetColor.
- Note that one can just use SetColor(Color32)

## Acknowledgments

* Inspiration: https://stackoverflow.com/questions/52805523/distance-from-a-line-shadertoy

