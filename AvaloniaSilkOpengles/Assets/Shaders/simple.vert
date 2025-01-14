#version 300 es
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;
layout (location = 2) in vec2 aTexCoord;

out vec4 vertexColor;
out vec2 texCoord;

uniform mat4 projection;
uniform mat4 model;
uniform mat4 view;

void main()
{
    vertexColor = vec4(aColor.rgba);
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
//    gl_Position = vec4(aPosition.xy, 0.0, 1.0);
    texCoord = aTexCoord;
}