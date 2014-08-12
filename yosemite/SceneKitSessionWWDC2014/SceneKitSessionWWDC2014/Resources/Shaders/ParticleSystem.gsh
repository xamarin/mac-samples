#extension GL_EXT_geometry_shader4 : enable

uniform mat4 u_p;
uniform float u_trailFactor;

varying in vec3 v_params[]; // angle, scale, life
varying in vec3 v_velocity[];
varying out vec3 v_uv;

void main(void) {
    float sn = sin(v_params[0].x);
    float cs = cos(v_params[0].x);
    
    if (u_trailFactor > 0.) {
        vec2 v = vec2(v_velocity[0].x, v_velocity[0].y);
        float vlen = length(v);
        v /= vlen;
        vec2 t = vec2(-v.y, v.x);
        v *= v_params[0].y;
        t *= v_params[0].y;
        
        v_uv = vec3(-1.,  1., v_params[0].z);
        gl_Position = gl_PositionIn[0];
        gl_Position.xy += v;
        gl_Position = u_p * gl_Position;
        v_uv.xy = vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn) * 0.5 + 0.5;
        EmitVertex();
        
        v_uv = vec3( 1.,  1., v_params[0].z);
        gl_Position = gl_PositionIn[0];
        gl_Position.xy += t;
        gl_Position = u_p * gl_Position;
        v_uv.xy = vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn) * 0.5 + 0.5;
        EmitVertex();
        
        v_uv = vec3(-1., -1., v_params[0].z);
        gl_Position = gl_PositionIn[0];
        gl_Position.xy -= t;
        gl_Position = u_p * gl_Position;
        v_uv.xy = vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn) * 0.5 + 0.5;
        EmitVertex();
        
        v_uv = vec3( 1., -1., v_params[0].z);
        gl_Position = gl_PositionIn[0];
        gl_Position.xy -= v * (1. + vlen * u_trailFactor);
        gl_Position = u_p * gl_Position;
        v_uv.xy = vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn) * 0.5 + 0.5;
        EmitVertex();

        EndPrimitive();
        return;
    }
    
    sn *= v_params[0].y;
    cs *= v_params[0].y;
    
    v_uv = vec3(-1.,  1., v_params[0].z);
    gl_Position = gl_PositionIn[0];
    gl_Position.xy += vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn);
    gl_Position = u_p * gl_Position;
    v_uv.xy = v_uv.xy * 0.5 + 0.5;
    EmitVertex();

    v_uv = vec3(-1., -1., v_params[0].z);
    gl_Position = gl_PositionIn[0];
    gl_Position.xy += vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn);
    gl_Position = u_p * gl_Position;
    v_uv.xy = v_uv.xy * 0.5 + 0.5;
    EmitVertex();

    v_uv = vec3( 1.,  1., v_params[0].z);
    gl_Position = gl_PositionIn[0];
    gl_Position.xy += vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn);
    gl_Position = u_p * gl_Position;
    v_uv.xy = v_uv.xy * 0.5 + 0.5;
    EmitVertex();

    v_uv = vec3( 1., -1., v_params[0].z);
    gl_Position = gl_PositionIn[0];
    gl_Position.xy += vec2( v_uv.x * cs - v_uv.y * sn,  v_uv.y * cs + v_uv.x * sn);
    gl_Position = u_p * gl_Position;
    v_uv.xy = v_uv.xy * 0.5 + 0.5;
    EmitVertex();

    EndPrimitive();
}
