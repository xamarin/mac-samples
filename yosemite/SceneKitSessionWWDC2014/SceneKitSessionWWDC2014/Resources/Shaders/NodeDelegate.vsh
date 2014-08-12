attribute vec4 a_position;
attribute vec2 a_angleAndLife;

uniform mat4 u_mvp;

varying vec2 v_angleAndLife;

void main() {
	gl_Position = u_mvp * a_position;
	v_angleAndLife = a_angleAndLife;
}
