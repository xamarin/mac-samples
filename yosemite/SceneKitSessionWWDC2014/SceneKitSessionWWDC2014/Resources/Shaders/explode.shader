uniform float explodeValue = 0.0;

#pragma body

 _geometry.position.xyz += _geometry.normal * explodeValue;
