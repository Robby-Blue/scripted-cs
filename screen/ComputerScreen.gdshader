shader_type canvas_item;

vec2 distort(vec2 pos) {
	vec2 center = vec2(0.5, 0.5);
	vec2 dir_to_center = pos - center;
	pos += dir_to_center * pow(length(dir_to_center), 2.0) * 0.2;
	return pos;
}

void fragment() {
	COLOR = texture(TEXTURE, UV);
	vec2 pos = distort(UV);
	vec2 delta = vec2(0.001, -0.001);

	vec2 bleed_left = distort(UV-delta);
	vec2 bleed_right = distort(UV+delta);

	vec4 c = vec4(texture(TEXTURE, bleed_left)[0], texture(TEXTURE, pos)[1], texture(TEXTURE, bleed_right)[2], 255);

	bool isOutOfBounds = pos.x < 0.0 || pos.x > 1.0 || pos.y < 0.0 || pos.y > 1.0;

	if(isOutOfBounds){
		COLOR = vec4(0);
		discard;
	}

	int pixel_y = int(pos.y * 1000.0);
	int timer_offset = int(TIME * 100.0);
	if((pixel_y + timer_offset) % 10 <= 2) {
		c.a = 0.9;
	}

	COLOR = c;
}