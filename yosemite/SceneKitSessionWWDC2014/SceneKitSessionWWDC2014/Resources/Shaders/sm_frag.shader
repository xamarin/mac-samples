#pragma transparent

uniform float fragIntensity = 1.0;

float videoShadow = 1.0;
float y = gl_FragCoord.y + u_time * 100.0;
if (fract(y * 0.06) > 0.5) {
	videoShadow = 0.0;//0.3 + 0.2 * sin(u_time * 0.0);
}

_output.color.b += 0.3;// + sin(u_time*500.0) * 0.3;

_output.color.rgba = mix(_output.color.rgba, videoShadow * _output.color.rgba, fragIntensity);
