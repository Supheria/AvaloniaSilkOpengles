#version 300 es
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec3 aColor;
out vec4 vertexColor;

uniform mat4 projection;
uniform mat4 model;

void main()
{
    vertexColor = vec4(aColor.rgb, 1.0);
    gl_Position = projection * model * vec4(aPosition.xy, 0.0, 1.0);
//    gl_Position = vec4(aPosition.xy, 0.0, 1.0);
}