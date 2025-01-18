namespace Nevergreen;

using Godot;

public partial class Dice : AnimatedSprite2D {

  private Tween? _tween;
  public int Roll() {
    Visible = true;
    Frame = (int)GD.Randi() % 6;
    Modulate = Frame > 2 ? Colors.White : Colors.DarkRed;
    if (_tween is not null && _tween.IsRunning()) {
      _tween.Kill();
    }
    _tween = CreateTween();
    _tween.TweenProperty(this, "modulate:a", 0.0, 0.3).SetDelay(0.5);
    // return Frame + 1;
    return 6;
  }
}
