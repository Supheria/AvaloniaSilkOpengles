#version 300 es

precision highp float;

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTex;
// TODO
layout (location = 3) in mat4 aTransform;

out vec2 texCoord;
out vec3 normal;
out vec3 currentPos;

uniform mat4 projection;
uniform mat4 model;
uniform mat4 view;

void main()
{
    texCoord = aTex;
    normal = normalize((model * vec4(floor(aNormal), 0.0f)).xyz);
    currentPos = vec3(model * vec4(aPos, 1.0f));
    
    gl_Position = projection * view * vec4(currentPos, 1.0f);
}