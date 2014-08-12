uniform float ambientOcclusionFactor = 0.07;
uniform float ambientOcclusionYFactor = 0.0;
varying float geometryZ = 1.0;
                                       
#pragma body

if (_geometry.texcoords[0].x > 0) {
    float z = _geometry.position.z - (100.0 - abs(_geometry.position.y)) * ambientOcclusionYFactor;
    if (_geometry.normal.x > 0) {
        geometryZ = z * ambientOcclusionFactor;
        geometryZ = clamp(geometryZ + 1.0, 0.2, 1.0);
    }
    else {
        geometryZ = z * ambientOcclusionFactor;
        geometryZ = clamp(geometryZ + 1.0, 0.2, 1.0);
    }
}
