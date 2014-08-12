float flakeSize = 0.2;
float flakeIntensity = 0.7;

vec3 paintColor0 = vec3(0.9, 0.4, 0.3);
vec3 paintColor1 = vec3(0.9, 0.75, 0.2);
vec3 flakeColor = vec3(flakeIntensity, flakeIntensity, flakeIntensity);

vec3 rnd =  texture2D(u_diffuseTexture, _surface.diffuseTexcoord * vec2(1.0 / flakeSize)).rgb;
rnd = normalize(2 * rnd - 1.0);

vec3 nrm1 = normalize(0.05 * rnd + 0.95 * _surface.normal);
vec3 nrm2 = normalize(0.3 * rnd + 0.4 * _surface.normal);

float fresnel1 = clamp(dot(nrm1, _surface.view), 0.0, 1.0);
float fresnel2 = clamp(dot(nrm2, _surface.view), 0.0, 1.0);

vec3 col = mix(paintColor0, paintColor1, fresnel1);
col += pow(fresnel2, 106.0) * flakeColor;

_surface.normal = nrm1;
_surface.diffuse = vec4(col, 1.0);
_surface.emission = (_surface.reflective * _surface.reflective) * 2.0;
_surface.reflective = vec4(0.0);
