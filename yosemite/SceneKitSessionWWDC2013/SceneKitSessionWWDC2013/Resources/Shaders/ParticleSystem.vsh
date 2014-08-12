#version 120

attribute vec4 a_position;
attribute vec3 a_velocity;
attribute vec2 a_angleAndLife;

uniform mat4 u_mv;

varying vec3 v_params; // angle, scale, life
varying vec4 v_position;
varying vec3 v_velocity;

void main()
{
	gl_Position = u_mv * vec4(a_position.xyz, 1.0);
    v_params = vec3(a_angleAndLife.x, a_position.w, a_angleAndLife.y);
    v_velocity = mat3(u_mv) * a_velocity;
}
