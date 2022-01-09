#version 450

layout (location = 0) in vec3 iPosition;
layout (location = 1) in vec3 iNormal;

uniform mat4 mvp;
uniform vec3 p[4][4][4];

out vec3 position;
out vec3 normal;

vec3 evaluate(vec3 uvw) {
    float u = uvw.y;
    float v = uvw.x;
    float w = uvw.z;
    float u0 = (1.-u) * (1.-u) * (1.-u);
    float u1 = 3. * u * (1.-u) * (1.-u);
    float u2 = 3. * u * u * (1.-u);
    float u3 = u * u * u;
    float v0 = (1.-v) * (1.-v) * (1.-v);
    float v1 = 3. * v * (1.-v) * (1.-v);
    float v2 = 3. * v * v * (1.-v);
    float v3 = v * v * v;
    float w0 = (1.-w) * (1.-w) * (1.-w);
    float w1 = 3. * w * (1.-w) * (1.-w);
    float w2 = 3. * w * w * (1.-w);
    float w3 = w * w * w;

    return
    w0 *
    ( u0 * ( v0*p[0][0][0] + v1*p[0][0][1] + v2*p[0][0][2] + v3*p[0][0][3] )
    + u1 * ( v0*p[0][1][0] + v1*p[0][1][1] + v2*p[0][1][2] + v3*p[0][1][3] )
    + u2 * ( v0*p[0][2][0] + v1*p[0][2][1] + v2*p[0][2][2] + v3*p[0][2][3] )
    + u3 * ( v0*p[0][3][0] + v1*p[0][3][1] + v2*p[0][3][2] + v3*p[0][3][3] )) + 
    w1 *
    ( u0 * ( v0*p[1][0][0] + v1*p[1][0][1] + v2*p[1][0][2] + v3*p[1][0][3] )
    + u1 * ( v0*p[1][1][0] + v1*p[1][1][1] + v2*p[1][1][2] + v3*p[1][1][3] )
    + u2 * ( v0*p[1][2][0] + v1*p[1][2][1] + v2*p[1][2][2] + v3*p[1][2][3] )
    + u3 * ( v0*p[1][3][0] + v1*p[1][3][1] + v2*p[1][3][2] + v3*p[1][3][3] )) +
    w2 *
    ( u0 * ( v0*p[2][0][0] + v1*p[2][0][1] + v2*p[2][0][2] + v3*p[2][0][3] )
    + u1 * ( v0*p[2][1][0] + v1*p[2][1][1] + v2*p[2][1][2] + v3*p[2][1][3] )
    + u2 * ( v0*p[2][2][0] + v1*p[2][2][1] + v2*p[2][2][2] + v3*p[2][2][3] )
    + u3 * ( v0*p[2][3][0] + v1*p[2][3][1] + v2*p[2][3][2] + v3*p[2][3][3] )) +
    w3 *
    ( u0 * ( v0*p[3][0][0] + v1*p[3][0][1] + v2*p[3][0][2] + v3*p[3][0][3] )
    + u1 * ( v0*p[3][1][0] + v1*p[3][1][1] + v2*p[3][1][2] + v3*p[3][1][3] )
    + u2 * ( v0*p[3][2][0] + v1*p[3][2][1] + v2*p[3][2][2] + v3*p[3][2][3] )
    + u3 * ( v0*p[3][3][0] + v1*p[3][3][1] + v2*p[3][3][2] + v3*p[3][3][3] ));
}

void main() {
    vec3 pos = evaluate(iPosition);
    vec3 pos2 = evaluate(iPosition + 0.01 * iNormal);
    vec3 norm = normalize(pos2 - pos);
    gl_Position = mvp * vec4(pos, 1.0);
    position = pos;
    normal = norm;
}
