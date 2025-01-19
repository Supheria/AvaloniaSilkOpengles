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

//uniform mat4 projection;
uniform mat4 camMatrix;
uniform mat4 model;
//uniform mat4 view;

void main()
{
    currentPos = vec3(model * vec4(aPos, 1.0f));
    normal = normalize((model * vec4(floor(aNormal), 0.0f)).xyz);
    texCoord = aTex;
    
//    gl_Position = projection * view * vec4(currentPos, 1.0f);
    gl_Position = camMatrix * vec4(currentPos, 1.0f);
}