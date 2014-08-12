uniform float surfIntensity = 1.0;

float n = 1.0;
vec3 p = vec3(_surface.diffuseTexcoord, 1.0) * 8.0;
for (int i = 1; i < 7; i++) {
	float nx = p.x + cos(p.z + sin(float(i) * p.y + u_time + float(i) * 1341.5));
	float ny = p.y + sin(p.z + cos(float(i) * p.x + u_time + float(i) * 3341.5));
	p = vec3(nx, ny, 1.0 * p.z);
}

vec3 col = mix(vec3(1.0, 0.5, 0.0), vec3(3.4, 2.0, 0.5), p.x * 0.1);
col = mix(vec3(1.0), col, surfIntensity);

_surface.diffuse.rgb = col;
