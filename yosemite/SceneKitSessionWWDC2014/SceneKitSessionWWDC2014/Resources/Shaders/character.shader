#pragma transparent

uniform float ghostFactor = 0.0;

float fresnel = 1.0 - abs(dot(_surface.view, _surface.normal));
float fresnelFactor = 0.9;
fresnel = (1.0 - fresnelFactor) + fresnelFactor * fresnel;

vec4 colorGhost = _output.color.rgba * fresnel;
_output.color.rgba = mix(_output.color, colorGhost, vec4(ghostFactor));
