#version 300 es

precision highp float;

out vec4 FragColor;

//in vec4 vertexColor;
in vec2 texCoord;
in vec3 normal;
in vec3 modelPos;

uniform vec3 lightPos;
uniform vec3 camPos;
uniform vec4 lightColor;
//uniform sampler2D atlasTexture;
uniform sampler2D tex0;
uniform sampler2D tex1;

void main()
{
    //    vec4 texColor = texture(happyTexture, texCoord) * 0.66;
    //    FragColor = vertexColor * 0.33 + texColor;
//    FragColor = texture(atlasTexture, texCoord);

//    FragColor = vec4(normal, 1);
    
//    vec4 texColor = texture(atlasTexture, texCoord);
//    FragColor = vec4(texColor.rgb  * 0.5, 0.5);
    
//    float diffuse = max(dot(normal, lightDirect), 0.0);
//    float ambient = 0.3;
//    float lighting = max(diffuse, ambient);
//    vec4 tex_color = texture(atlasTexture, texCoord);
//    FragColor = tex_color * lighting;

//    vec3 normalized = normalize(normal);
    float ambient = 0.2;
    
    vec3 lightDirection = normalize(lightPos - modelPos);
    float diffuse = max(dot(normal, lightDirection), 0.0);
//    float diffuse = pow(max(dot(normal, lightDirection), 0.0), 2.0);
    
    float specularLight = 0.5f;
    vec3 viewDirection = normalize(camPos - modelPos);
    vec3 reflectionDirection = reflect(-lightDirection, normal);
    float specAmount = pow(max(dot(viewDirection, reflectionDirection), 0.0), 16.0);
    float specular = specAmount * specularLight;
    
    vec4 texColor = texture(tex0, texCoord);
    vec4 specColor = texture(tex1, texCoord);
    vec4 fragment = (texColor * (diffuse + ambient) + specColor.r * specular) * lightColor;
    vec4 fragment = (texColor * (diffuse + ambient) ) * lightColor;
    FragColor = vec4(fragment.rgb, 1.0);
//    FragColor = vec4(lightDirection, 1.0);
    
//        FragColor = vec4(normalized, 1);
}