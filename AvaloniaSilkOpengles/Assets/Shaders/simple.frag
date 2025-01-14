#version 300 es

precision highp float;

out vec4 FragColor;

in vec4 vertexColor;
in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    vec4 texColor = texture(texture0, texCoord) * 0.66;
    FragColor = vertexColor * 0.33 + texColor;
}