#version 300 es

precision highp float;

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

uniform mat4 v, p;
uniform mat4 model;
uniform mat4 translation;
uniform mat4 rotation;
uniform mat4 scale;
uniform float outlining;

void main()
{
    vec3 currentPos = vec3(model * translation * rotation * scale * vec4(aPos + aNormal * outlining, 1.0f));
    gl_Position = p * v * vec4(currentPos, 1.0f);
}