shader_type canvas_item;

// Background image to render.
uniform sampler2D fill_texture : filter_nearest, repeat_enable;

// Image scale.
uniform vec2 scale = vec2(1);

// Image offset.
uniform vec2 offset = vec2(0);

// Pixels below this alpha (transparency) level will not be rendered.
uniform float min_alpha : hint_range(0.0, 1.0) = 0.1;

// Color to replace.
uniform vec3 mask_color : source_color = vec3(1,0,0);

// How closely to match the selected color when performing the replacement.bool
uniform float color_match_threshold : hint_range(0.0, 1.0) = 0.8;

// World position of vertices for fragment rendering.
varying vec2 world_pos;

void vertex() {
	// Store world position of each vertex for fragment rendering.
    world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	// Don't render pixels above the minimum alpha level.
	if (COLOR.a < min_alpha)
	{
		// Render as transparent.
		COLOR = vec4(0.0);
	}
	// Don't replace pixels that are too different from the target color.
	else if (distance(COLOR.rgb, mask_color.rgb) < 1.0 - color_match_threshold)
	{
		// Scale the world position.
		vec2 scaled_pos = (world_pos - offset) / scale;
		// Set color to corresponding background image pixel.
		COLOR = texture(fill_texture, scaled_pos);
	}
}