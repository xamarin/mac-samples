uniform sampler2D floorMap;
uniform vec3 floorImageNamePosition = vec3(1000.0, 1000.0, 1000.0);
uniform float floorImageNameScale = 10.0;

#pragma body

vec4 floorFragPos = u_inverseViewTransform * vec4(_surface.position, 1.0);

float inradius = 8.0;
float outradius = 12.0;

float dist = distance(floorImageNamePosition.xz, floorFragPos.xz);
float mixValue = clamp((outradius - (dist - inradius)) / outradius, 0.0, 1.0);
mixValue = mixValue + mixValue - (mixValue * mixValue);

vec4 color = texture2D(floorMap, _surface.diffuseTexcoord * floorImageNameScale);
_surface.diffuse = mix(_surface.diffuse, color, mixValue);

