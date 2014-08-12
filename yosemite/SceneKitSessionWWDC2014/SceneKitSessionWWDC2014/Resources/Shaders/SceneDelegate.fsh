////uniform vec2 resolution;
//uniform float time;
//uniform float factor;
//
//const float lineCount = 10.;
//const float bias = 0.5;
//const float sharpness = 4.0;
//const float smoothness = 0.05;
//
//float sharpen(float v, float dist) {
//    float d = smoothness + pow(abs(1. - dist),4.) * 0.5;
//    return smoothstep(0.5 - d,.5 + d, v);
//}
//
//void main(void) {
//	vec2 position = (( gl_FragCoord.xy / resolution.xy ) - vec2(0.5));
//	vec4 color = vec4(1.0);
//	vec2 abspos = abs(position * 2.);
//	float depth = clamp(sqrt(abspos.y*abspos.y + abspos.x*abspos.x), 0., 2.);
//	float lc = mix(lineCount*2., lineCount, depth);
//	vec2 pos = mod((abspos*.5-.75) * lc - time*2.0, 1.);
//    
//	float mx = 0.5;
//	float ao = 1.0;
//	
//	if (abs(position.x) > abs(position.y)) {
//        mx = abs(1.-sharpen(abs(pos.x*2. - 1.), depth) - sharpen(abs(mod(abspos.y / abspos.x * lineCount * 0.5, 2.)-1.), abspos.x));
//        ao = 1. - abspos.y / abspos.x;
//        ao *= clamp(abspos.x*abspos.x, 0., 1.);
//	} else {
//        mx = abs(1.-sharpen(abs(pos.y*2. - 1.), depth) - sharpen(abs(mod(abspos.x / abspos.y * lineCount * 0.5, 2.)-1.), abspos.y));
//        ao = 1. - abspos.x / abspos.y;
//        ao *= clamp(abspos.y*abspos.y, 0., 1.);
//	}
//	color.rgb = mix(vec3(0.0, 0.0, 0.0), vec3( 1.0, 1.0, 1.0 ), mx);
//	color.rgb *= 0.5 * depth * factor * pow(ao, 0.35);
//	gl_FragColor = color;
//}

uniform float time;
uniform vec2 resolution;
uniform float factor;

vec4 t(vec2 uv)
{
    float j = sin(uv.y*3.14+time*5.0);
    float i = sin(uv.x*15.0-uv.y*2.0*3.14+time*3.0);
    float n = -clamp(i,-0.2,0.0)-0.0*clamp(j,-0.2,0.0);
    
    return 3.5*(vec4(0.2,0.5,1.0,1.0)*n);
    
}

void main(void)
{
    vec2 p = -1.0 + 2.0 * gl_FragCoord.xy / resolution.xy;
    vec2 uv;
    
    float r = sqrt(dot(p,p));
    float a = atan(p.y*(0.3+0.1*cos(time*2.0+p.y)),p.x*(0.3+0.1*sin(time+p.x)))+time;
    
    
    uv.x = time + 1.0/( r + .01);
    uv.y = 4.0*a/3.1416;
    
    gl_FragColor = mix(vec4(0.0), t(uv)*r*r*2.0, factor);
}