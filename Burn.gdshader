shader_type canvas_item;

uniform float noise_offset = 0.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

vec2 random(vec2 uv){
    uv = vec2( dot(uv, vec2(127.1,311.7) ),
               dot(uv, vec2(269.5,183.3) ) );
    return -1.0 + 2.0 * fract(sin(uv) * 43758.5453123);
}

float noise(vec2 uv) {
    vec2 uv_index = floor(uv);
    vec2 uv_fract = fract(uv);

    vec2 blur = smoothstep(0.0, 1.0, uv_fract);

    return mix( mix( dot( random(uv_index + vec2(0.0,0.0) ), uv_fract - vec2(0.0,0.0) ),
                     dot( random(uv_index + vec2(1.0,0.0) ), uv_fract - vec2(1.0,0.0) ), blur.x),
                mix( dot( random(uv_index + vec2(0.0,1.0) ), uv_fract - vec2(0.0,1.0) ),
                     dot( random(uv_index + vec2(1.0,1.0) ), uv_fract - vec2(1.0,1.0) ), blur.x), blur.y) + 0.5;
}

vec4 gradient_map(vec4 input_color, float x) {
	float transparent_part = 1.0;
	float orange_part = 1.2;
	if (x < transparent_part) {
		return vec4(0.0);
	}
	if (x < orange_part) {
		return vec4(1.0,0.5,0.1,input_color.a);
	}
	return input_color;
}

void fragment() {
	float noise_value = noise(UV / TEXTURE_PIXEL_SIZE / 16.0);
	
	COLOR = gradient_map(COLOR, noise_value + noise_offset);
}



//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
