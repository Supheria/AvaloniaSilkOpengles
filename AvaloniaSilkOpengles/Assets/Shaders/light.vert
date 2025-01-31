#version 300 es

precision highp float;

layout(location = 0) in vec3 aPos;
layout(location = 2) in vec3 aColor;

out vec4 color;

//uniform mat4 projection;
//uniform mat4 view;
uniform mat4 v, p;
uniform mat4 model;
uniform mat4 translation;
uniform mat4 rotation;
uniform mat4 scale;

void main()
{
    color = vec4(aColor, 1.0f);
//    gl_Position = projection * view * model * vec4(aPos, 1.0f);
    gl_Position = p * v * model * translation * rotation * scale * vec4(aPos, 1.0f);
}