uniform float geomIntensity = 1.0;

float freq = 1.8;
float power = 4.0;
float factor = 1.0;

float eval(vec3 p) {
	return pow(0.5 + 0.5 * cos(freq * p.x + 1.5 * u_time) *
						   sin(freq * p.y + 2.5 * u_time) *
						   sin(freq * p.z + 1.0 * u_time), power);
}

vec3 computeNormal(vec3 p, vec3 n) {
	vec3 e = vec3(0.1, 0.0, 0.0);
	return normalize(n - geomIntensity * vec3(eval(p + e.xyy) - eval(p - e.xyy),
						  		              eval(p + e.yxy) - eval(p - e.yxy),
                                              eval(p + e.yyx) - eval(p - e.yyx)));
}

#pragma body

vec3 p = _geometry.position.xyz;
float disp = factor * geomIntensity * eval(p);
_geometry.position.xyz += _geometry.normal * disp;
_geometry.normal.xyz = computeNormal(p, _geometry.normal);
