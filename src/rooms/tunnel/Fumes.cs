namespace Nevergreen;

using Godot;
using System;

public partial class Fumes : Sprite2D {
  // Called every frame. 'delta' is the elapsed time since the previous frame.

  FastNoiseLite _noise;
  public override void _Ready() => _noise = (FastNoiseLite)((NoiseTexture2D)Texture).Noise;

  public override void _Process(double delta) {
    var offset = _noise.Offset;
    offset.Z += (float)delta;
    _noise.Offset = offset;
    GD.Print(_noise.Offset);
  }
}
