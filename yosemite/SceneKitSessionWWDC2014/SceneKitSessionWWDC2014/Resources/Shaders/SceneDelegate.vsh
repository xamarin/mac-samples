attribute vec4 position;
attribute vec2 texcoord0;

varying vec2 v_uv;

void main(void) {
    gl_Position = position;
	v_uv = texcoord0;
}