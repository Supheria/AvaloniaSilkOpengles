#version 300 es

precision highp float;

// RGBA
out vec4 FragColor;

in vec3 currentPos;
in vec3 normal;
in vec3 color;

void main()
{
    FragColor = vec4(color, 1.0f);
}