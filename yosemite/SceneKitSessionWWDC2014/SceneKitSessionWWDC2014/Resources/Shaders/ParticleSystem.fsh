uniform sampler2D u_tex;
uniform sampler2D u_ramp;

varying vec3 v_uv;

void main(void) {
    vec4 tex = texture2D(u_tex, v_uv.xy);
    vec4 col = texture2D(u_ramp, v_uv.zx);
	gl_FragColor = tex * col;
}
