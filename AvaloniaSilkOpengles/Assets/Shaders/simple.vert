#version 300 es

precision highp float;

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 3) in vec2 aTex;
// TODO
layout (location = 4) in mat4 aTransform;

out vec3 currentPos;
out vec3 normal;
out vec2 texCoord;

uniform mat4 v, p;
uniform mat4 model;
uniform mat4 translation;
uniform mat4 rotation;
uniform mat4 scale;

void main()
{
    mat4 trans = model * translation * rotation * scale;
    currentPos = vec3(trans * vec4(aPos, 1.0f));
//    normal = normalize((trans * vec4(floor(aNormal), 0.0f)).xyz);
    normal = normalize(aNormal);
    texCoord = aTex;
    
//    gl_Position = projection * view * vec4(currentPos, 1.0f);
    gl_Position = p * v * vec4(currentPos, 1.0f);
}