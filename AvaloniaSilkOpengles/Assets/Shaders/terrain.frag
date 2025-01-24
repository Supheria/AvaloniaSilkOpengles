#version 300 es

precision highp float;

// RGBA
out vec4 FragColor;

in vec3 currentPos;
in vec3 normal;
in vec3 color;
in vec2 uv;

uniform sampler2D diffuse0;
uniform sampler2D specular0;

void main()
{
//    vec4 texColor = texture(diffuse0, uv);
//    FragColor = texColor;
    FragColor = vec4(color, 1.0f);
//    FragColor = normalize(vec4(color * texColor.rgb, 1.0f));
}