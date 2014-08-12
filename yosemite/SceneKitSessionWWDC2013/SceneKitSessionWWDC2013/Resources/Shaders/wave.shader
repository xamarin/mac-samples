uniform float intensity = 0.0;

float length= length(_geometry.position.xy);
float angle = (length - u_time) * 10.0;
float mixValue = max(0.0, (7.0 - length) / 7.0);
float f = intensity * mix(0.0, 1.0, mixValue);

_geometry.position.z += f * (1.0 + sin(angle));
_geometry.normal.x = mix(0, sin(angle), f);
_geometry.normal.y = mix(1, cos(angle), f);
_geometry.normal = normalize(_geometry.normal);
