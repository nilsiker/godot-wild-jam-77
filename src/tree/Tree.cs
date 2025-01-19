namespace Woodblight;

using Godot;

public partial class Tree : StaticBody2D {

  private Area2D _canopy = default!;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    _canopy = GetNode<Area2D>("Canopy");

    _canopy.BodyEntered += OnCanopyBodyEntered;
    _canopy.BodyExited += OnCanopyBodyExited;
  }

  private void OnCanopyBodyEntered(Node2D body) {
    if (body is IPlayer) {
      var mod = _canopy.Modulate;
      mod.A = 0.4f;
      _canopy.Modulate = mod;
    }
  }

  private void OnCanopyBodyExited(Node2D body) {
    if (body is IPlayer) {
      var mod = _canopy.Modulate;
      mod.A = 1;
      _canopy.Modulate = mod;
    }
  }
}
