#version 300 es

precision highp float;

layout(location = 0) in vec3 aPos;
layout(location = 2) in vec3 aColor;

out vec4 color;

//uniform mat4 projection;
//uniform mat4 view;
uniform mat4 camMatrix;
uniform mat4 model;

void main()
{
    color = vec4(aColor, 1.0f);
//    gl_Position = projection * view * model * vec4(aPos, 1.0f);
    gl_Position = camMatrix * model * vec4(aPos, 1.0f);
}