#version 300 es

precision highp float;

out vec4 FragColor;

//in vec4 vertexColor;
in vec2 texCoord;

uniform sampler2D atlasTexture;

void main()
{
//    vec4 texColor = texture(happyTexture, texCoord) * 0.66;
//    FragColor = vertexColor * 0.33 + texColor;
    FragColor = texture(atlasTexture, texCoord);
}