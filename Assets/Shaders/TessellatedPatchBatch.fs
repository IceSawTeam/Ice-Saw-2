#version 330

// Input vertex attributes (from vertex shader)
in vec3 fragPosition;
in vec2 fragTexCoord;
in vec2 fragTexCoord2;
flat in int instanceId; // Used to index throught the uniforms below, 0 - 7

// Input uniform values
uniform sampler2D diffuseTextures[8];
uniform sampler2D lightmapTextures[8];
uniform int highlighted[8]; // 0 - false, 1 - true
uniform int lightmapsEnabled; // 0 - false, 1 - true

// Output fragment color
out vec4 finalColor;


void main() {
    vec4 textureColor;
    switch (instanceId) {
    case 0:
        textureColor = texture(diffuseTextures[0], fragTexCoord);
        break;
    case 1:
        textureColor = texture(diffuseTextures[1], fragTexCoord);
        break;
    case 2:
        textureColor = texture(diffuseTextures[2], fragTexCoord);
        break;
    case 3:
        textureColor = texture(diffuseTextures[3], fragTexCoord);
        break;
    case 4:
        textureColor = texture(diffuseTextures[4], fragTexCoord);
        break;
    case 5:
        textureColor = texture(diffuseTextures[5], fragTexCoord);
        break;
    case 6:
        textureColor = texture(diffuseTextures[6], fragTexCoord);
        break;
    case 7:
        textureColor = texture(diffuseTextures[7], fragTexCoord);
        break;
    }

    vec4 lightmapColor;
    switch (instanceId) {
    case 0:
        lightmapColor = texture(lightmapTextures[0], fragTexCoord2);
        break;
    case 1:
        lightmapColor = texture(lightmapTextures[1], fragTexCoord2);
        break;
    case 2:
        lightmapColor = texture(lightmapTextures[2], fragTexCoord2);
        break;
    case 3:
        lightmapColor = texture(lightmapTextures[3], fragTexCoord2);
        break;
    case 4:
        lightmapColor = texture(lightmapTextures[4], fragTexCoord2);
        break;
    case 5:
        lightmapColor = texture(lightmapTextures[5], fragTexCoord2);
        break;
    case 6:
        lightmapColor = texture(lightmapTextures[6], fragTexCoord2);
        break;
    case 7:
        lightmapColor = texture(lightmapTextures[7], fragTexCoord2);
        break;
    }

    const vec4 HIGHLIGHT_TINT = vec4(1, 1, 1, 1);
    vec4 texelColor = textureColor;
    if (lightmapsEnabled == 1) {
        texelColor *= lightmapColor;
    }
    if (highlighted[instanceId] == 1){
        texelColor *= HIGHLIGHT_TINT;
    }
    finalColor = texelColor;
}