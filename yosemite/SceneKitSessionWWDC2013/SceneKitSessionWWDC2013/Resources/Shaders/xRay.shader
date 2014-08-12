#pragma transparent

float fresnelPower = 1.5;
float fresnelFactor = 0.75;

float fresnel = pow(1.0 - dot(_surface.view, _surface.normal), fresnelPower);
fresnel = (1.0 - fresnelFactor) + fresnelFactor * fresnel;

vec3 xRayColor = vec3(0.0, 0.745, 1.0);
_output.color.rgb = 1.5 * fresnel * xRayColor * _output.color.a;
_output.color.rgba *= fresnel;
