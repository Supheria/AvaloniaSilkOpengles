#version 300 es

precision highp float;

out vec4 FragColor;

in vec4 color;

uniform vec4 lightColor;

void main()
{
    FragColor = lightColor * color;
}