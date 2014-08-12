uniform float fragIntensity = 1.0;

float videoShadow = 1.0;
float y = gl_FragCoord.y + u_time * 60.0;
if (fract(y * 0.125) > 0.5) {
	videoShadow = 0.5 + 0.2 * sin(u_time * 20.0);
}

_output.color.rgb = mix(_output.color.rgb, videoShadow * _output.color.rgb, fragIntensity);
