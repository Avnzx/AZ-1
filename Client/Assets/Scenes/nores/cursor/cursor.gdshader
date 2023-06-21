shader_type canvas_item;

uniform float transparency: hint_range(0.0, 1.0) = 0.4;

void fragment() {
	vec4 tex = texture(TEXTURE, UV);
	COLOR.rgb = vec3(1, 0.5, 0);
	COLOR.a = tex.a * transparency;
}
