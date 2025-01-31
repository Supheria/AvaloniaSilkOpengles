#version 300 es

precision highp float;

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aColor;
layout (location = 3) in vec2 aUv;

out vec3 currentPos;
out vec3 normal;
out vec3 color;
out vec2 uv;

uniform mat4 v, p;
uniform mat4 model;
uniform mat4 translation;
uniform mat4 rotation;
uniform mat4 scale;

void main()
{
    mat4 transform = model * translation * rotation * scale;
    currentPos = vec3(transform * vec4(aPos, 1.0f));
    normal = normalize((transform * vec4(aNormal, 0.0f)).xyz);
//    normal = normalize(aNormal);
    color = aColor;
    uv = aUv;

    gl_Position = p * v * vec4(currentPos, 1.0f);
}
