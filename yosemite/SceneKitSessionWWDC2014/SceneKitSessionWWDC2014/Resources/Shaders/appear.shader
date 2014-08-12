#pragma transparent

uniform float appearProgress = 0.0;

vec4 p = vec4(_surface.position, 1.0) * u_inverseModelViewProjectionTransform;

if(appearProgress * 100.0 < p.y){
    _output.color.rgba = vec4(0.0);
    discard;
}

