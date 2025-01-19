#version 300 es

precision highp float;

// RGBA
out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;
in vec3 currentPos;

uniform vec3 lightPos;
uniform vec3 camPos;
uniform vec4 lightColor;
uniform sampler2D tex0;
uniform sampler2D tex1;

vec4 point_light();
vec4 direct_light();
vec4 spot_light();

void main()
{
    FragColor = point_light();
//    FragColor = direct_light();
//    FragColor = spot_light();
}

vec4 point_light()
{
    vec3 lightVec = lightPos - currentPos;
    float dist = length(lightVec);
    //
    // short distance
//    float a = 3.0f;
//    float b = 0.7f;
    //
    // long distance
    float a = 0.05f;
    float b = 0.01f;
    float c = 1.0f;
    float inten = 1.0f / (a * dist * dist + b * dist + c);
    
    float ambient = 0.2f;

    vec3 lightDirection = normalize(lightVec);
    float diffuse = max(dot(normal, lightDirection), 0.0f);

    float specularLight = 0.5f;
    vec3 viewDirection = normalize(camPos - currentPos);
    vec3 reflectionDirection = reflect(-lightDirection, normal);
    float specAmount = pow(max(dot(viewDirection, reflectionDirection), 0.0f), 16.0f);
    float specular = specAmount * specularLight;

    vec4 texColor = texture(tex0, texCoord);
    vec4 specColor = texture(tex1, texCoord);
    vec4 fragment = (texColor * (diffuse * inten + ambient) + specColor.r * specular * inten) * lightColor;
    return vec4(fragment.rgb, 1.0f);
}

vec4 direct_light()
{
    float ambient = 0.2f;

    vec3 lightDirection = normalize(vec3(1.0f, 1.0f, 0.0f));
    float diffuse = max(dot(normal, lightDirection), 0.0f);

    float specularLight = 0.5f;
    vec3 viewDirection = normalize(camPos - currentPos);
    vec3 reflectionDirection = reflect(-lightDirection, normal);
    float specAmount = pow(max(dot(viewDirection, reflectionDirection), 0.0f), 16.0f);
    float specular = specAmount * specularLight;

    vec4 texColor = texture(tex0, texCoord);
    vec4 specColor = texture(tex1, texCoord);
    vec4 fragment = (texColor * (diffuse + ambient) + specColor.r * specular) * lightColor;
    return vec4(fragment.rgb, 1.0f);
}

vec4 spot_light()
{
    float outerCone = 0.90f;
    float innerCone = 0.95f;
    
    float ambient = 0.2f;

    vec3 lightDirection = normalize(lightPos - currentPos);
    float diffuse = max(dot(normal, lightDirection), 0.0f);

    float specularLight = 0.5f;
    vec3 viewDirection = normalize(camPos - currentPos);
    vec3 reflectionDirection = reflect(-lightDirection, normal);
    float specAmount = pow(max(dot(viewDirection, reflectionDirection), 0.0f), 16.0f);
    float specular = specAmount * specularLight;
    
    float angle = dot(vec3(0.0f, -1.0f, 0.0f), -lightDirection);
    float inten = clamp((angle - outerCone) / (innerCone - outerCone), 0.0f, 1.0f);

    vec4 texColor = texture(tex0, texCoord);
    vec4 specColor = texture(tex1, texCoord);
    vec4 fragment = (texColor * (diffuse * inten + ambient) + specColor.r * specular * inten) * lightColor;
    return vec4(fragment.rgb, 1.0f);
}