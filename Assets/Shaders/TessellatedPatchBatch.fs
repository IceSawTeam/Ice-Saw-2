#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec2 fragTexCoord2;
flat in int instanceId; // Used to index throught the uniforms below, 0 - 7

// Input uniform values
uniform sampler2D diffuseTexture;
uniform sampler2D lightmap;
uniform int highlighted[8]; // 0 - false, 1 - true
uniform int lightmapsEnabled; // 0 - false, 1 - true

// Output fragment color
out vec4 finalColor;


void main() {
    vec4 textureColor = texture(diffuseTexture, fragTexCoord);

    vec4 lightmapColor = texture(lightmap, fragTexCoord2);

    // Matches SelectionManager.HighlightColor (255, 170, 0) so patches look consistent with
    // every other highlighted object type in the editor.
    const vec3 HIGHLIGHT_TINT = vec3(1.0, 0.667, 0.0);
    const float HIGHLIGHT_STRENGTH = 0.55;

    vec4 texelColor = textureColor;
     if (lightmapsEnabled == 1) {
         texelColor = (textureColor - lightmapColor) * lightmapColor.w;
         texelColor.w = 1;
    }
    if (highlighted[instanceId] == 1){
        texelColor.rgb = mix(texelColor.rgb, HIGHLIGHT_TINT, HIGHLIGHT_STRENGTH);
    }
    finalColor = texelColor;
}