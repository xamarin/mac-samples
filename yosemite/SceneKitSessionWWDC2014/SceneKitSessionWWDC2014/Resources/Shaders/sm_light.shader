uniform float lightIntensity = 1.0;

float diff = max(0.0, dot(_surface.normal, _light.direction));
_lightingContribution.diffuse = 0.1 * (diff * _light.intensity.rgb);

float back = lightIntensity * max(0.0, 0.18 + 0.29 * dot(_surface.normal, -_light.direction));
_lightingContribution.diffuse += (back * _light.intensity.rgb);

// rim/fresnel
float fn = lightIntensity * 1.5 * pow(1.0 - dot(_surface.normal, _surface.view), 1.3);
_lightingContribution.diffuse += (fn * _light.intensity.rgb);

