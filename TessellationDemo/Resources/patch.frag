#version 410 core
out vec4 FragColor;

uniform vec3 cameraPos;
uniform vec3 lightPos;

in vec3 position;
in vec3 tangent;
in vec3 bitangent;
in vec3 normal;

const vec3 color = vec3(0.5, 0.8, 0.2);

void main() {
    vec3 n = normalize(normal);
    
    vec3 lightDir = normalize(lightPos - position);
    float diff = max(dot(n, lightDir), 0.0);
    
    vec3 viewDir = normalize(cameraPos - position);
    vec3 reflectDir = reflect(-lightDir, n);
    float spec = 0.5 * pow(max(dot(viewDir, reflectDir), 0.0), 32);

    FragColor = vec4(min(color * diff + vec3(1, 1, 1) * spec, 1), 1);
}