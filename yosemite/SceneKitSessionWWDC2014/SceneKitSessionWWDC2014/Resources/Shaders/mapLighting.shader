float dotProduct = max(0.0, dot(_surface.normal, _light.direction));

_lightingContribution.diffuse += vec3(0.7) + (dotProduct * _light.intensity.rgb);
_lightingContribution.diffuse = clamp(_lightingContribution.diffuse, 0.0, 1.0);
