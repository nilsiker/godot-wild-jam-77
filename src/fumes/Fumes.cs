namespace Woodblight;

using Godot;

public partial class Fumes : Sprite2D {
  // Called every frame. 'delta' is the elapsed time since the previous frame.

  [Export] private float _speed = 1f;
  private FastNoiseLite _noise = default!;
  public override void _Ready() => _noise = (FastNoiseLite)((NoiseTexture2D)Texture).Noise;

  // NOTE This has awful performance on low-end machines.
  // public override void _Process(double delta) {
  //   var offset = _noise.Offset;
  //   offset.Z += (float)delta * _speed;
  //   _noise.Offset = offset;
  // }
}
