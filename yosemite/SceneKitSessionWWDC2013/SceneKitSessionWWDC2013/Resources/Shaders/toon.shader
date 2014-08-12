float dotProduct = max(0.0, dot(_surface.normal,_light.direction));

_lightingContribution.diffuse += (dotProduct * _light.intensity.rgb);
_lightingContribution.diffuse = floor(_lightingContribution.diffuse * 4.0) / 3.0;                                               

vec3 halfVector = normalize(_light.direction + _surface.view);

dotProduct = max(0.0, pow(max(0.0, dot(_surface.normal, halfVector)), _surface.shininess));
dotProduct = floor(dotProduct * 3.0) / 3.0;

_lightingContribution.specular += (dotProduct * _light.intensity.rgb);
