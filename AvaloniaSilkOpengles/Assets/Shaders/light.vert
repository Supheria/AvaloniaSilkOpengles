#version 300 es

precision highp float;

layout(location = 0) in vec3 aPos;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0);
}

//#version 300 es
//
//layout (location = 0) in vec3 aPos;
//
//uniform mat4 model;
//uniform mat4 camMatrix;
//
//void main()
//{
//    gl_Position = camMatrix * model * vec4(aPos, 1.0f);
//}