#version 330

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec2 vertexTexCoord2;
in mat4 instanceTransform; // Always Identity

// Input uniform values
const int patchCount = 8;
uniform mat4 mvp;
uniform vec3 controlPoints[patchCount * 16];
uniform vec2 diffuseTextureUVs[patchCount * 4];
uniform vec2 lightmapTextureUVs[patchCount * 4];
uniform int highlight[patchCount]; // 0 - false, 1 - true

// Output vertex attributes (to fragment shader)
out vec3 fragPosition;
out vec2 fragTexCoord;
out vec2 fragTexCoord2;
flat out int fragHighlighted;
flat out int instanceId;


void main() {
    // Set texcoords for texture
    for (int i = 0; i < 4; i++) {
        int textureIndex = gl_InstanceID * 4 + i;
        fragTexCoord = diffuseTextureUVs[textureIndex];
    }

    // Set texcoords for lightmaps
    for (int i = 0; i < 4; i++) {
        int lightmapIndex = gl_InstanceID * 4 + i;
        fragTexCoord2 = lightmapTextureUVs[lightmapIndex];
    }

    // Set the vertex positions
    vec3 vertexControlPoints[16];
    for (int i = 0; i < 16; i++) {
        int cpIndex = gl_InstanceID * 16 + gl_VertexID;
        vertexControlPoints[i] = controlPoints[cpIndex];
    }

    // Send other vertex attributes to fragment shader,
    // and set the vertex position.
    fragHighlighted = highlight[gl_InstanceID];
    vec4 pos = mvp * instanceTransform * vec4(EvaluateBezierSurface(vertexControlPoints, vertexPosition.xy), 1.0);
    fragPosition = pos;
    gl_Position = pos;
}

vec3 EvaluateBezierSurface(in vec3 controlPoints[16], vec2 uv) {
    // Compute 4 control points along the u direction
    vec3 uPoints[4];
    for (int i = 0; i < 4; i++) {
        int row = i * 4;
        vec3 p0 = controlPoints[row];
        vec3 p1 = controlPoints[row + 1];
        vec3 p2 = controlPoints[row + 2];
        vec3 p3 = controlPoints[row + 3];
        uPoints[i] = BezierInterpolate(p0, p1, p2, p3, uv.x)
    }
    // Compute the final position on the surface using v
    return BezierInterpolate(uPoints[0], uPoints[1], uPoints[2], uPoints[3], uv.y);
}

vec3 BezierInterpolate(vec3 p0, vec3 p1, vec3 p2, vec3 p3, float t) {
    vec3 q0 = mix(p0, p1, t);
    vec3 q1 = mix(p1, p2, t);
    vec3 q2 = mix(p2, p3, t);
    vec3 a =  mix(q0, q1, t)
    vec3 b =  mix(q1, q2, t)
    return mix(a, b, t);
}

vec2 UVInterpolate(vec2 uv[4], vec2 blendPosition) {
    // Quadrilateral interpolation
    vec2 a = mix(uv[0], uv[2], blendPosition.x);
    vec2 b = mix(uv[1], uv[3], blendPosition.y);
    return mix(a, b, blendPosition.y);
}
