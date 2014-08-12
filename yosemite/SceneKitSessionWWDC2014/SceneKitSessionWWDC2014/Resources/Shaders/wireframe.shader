#pragma transparent

#pragma body

vec2 texCoord = _surface.diffuseTexcoord;

vec2 fw = fwidth(texCoord);
float lineWidth = 1.;
vec2 threshold = fw * lineWidth;
vec2 d = smoothstep(vec2(0.), threshold, abs(texCoord));
d *= smoothstep(vec2(1.), vec2(1.)-threshold, texCoord);
float alpha = 1. - d.x * d.y;
    
alpha = 1. - alpha;
_output.color.rgba *= alpha;