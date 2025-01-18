#version 300 es
layout (location = 0) in vec3 aPosition;
//layout (location = 1) in vec4 aColor;
//layout (location = 2) in vec2 aTexCoord;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in mat4 aTransform;

//out vec4 vertexColor;
out vec2 texCoord;
out vec3 normal;
out vec3 modelPos;

uniform mat4 projection;
uniform mat4 model;
uniform mat4 view;

void main()
{
//    vertexColor = vec4(aColor.rgba);
    normal = normalize((model * vec4(floor(aNormal), 0.0)).xyz);
    texCoord = aTexCoord;


    modelPos = vec3(model * vec4(aPosition, 1.0));
    gl_Position = projection * view * vec4(modelPos, 1.0);
//    gl_Position = projection * view * vec4(aPosition, 1.0);
//    gl_Position = projection * view * vec4(aPosition, 1.0);
//    gl_Position = vec4(aPosition.xy, 0.0, 1.0);
}