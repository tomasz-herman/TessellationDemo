#version 410 core
out vec4 FragColor;

in vec3 tangent;
in vec3 bitangent;
in vec3 normal;

void main() {
    FragColor = vec4(normal, 1.0f);
}