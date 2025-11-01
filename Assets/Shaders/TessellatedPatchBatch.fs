#version 330

// Input vertex attributes (from vertex shader)
in vec3 fragPosition;
in vec2 fragTexCoord;
in vec2 fragTexCoord2;
flat in int instanceId; // Used to index throught the uniforms below, 0 - 7

// Input uniform values
uniform sampler2D deffuseTextures[8];
uniform sampler2D lightmapTextures[8];
uniform int highlighted[8]; // 0 - false, 1 - true
uniform int lightmapsEnabled; // 0 - false, 1 - true

// Output fragment color
out vec4 finalColor;


void main() {
    const vec4 HIGHLIGHT_TINT = vec4(1, 1, 1, 1);
    vec4 texelColor = texture(deffuseTextures[instanceId], fragTexCoord);
    texelColor += texelColor * texture(lightmapTextures[instanceId], fragTexCoord2) * lightmapsEnabled;
    texelColor += texelColor * HIGHLIGHT_TINT * highlighted[instanceId];
    finalColor = texelColor;
}