namespace Nevergreen;

using Godot;

public partial class ExitArea : Area2D {
  [Export] private ERoom Room { get; set; }

  [Signal] public delegate void ExitAreaEnteredEventHandler(ERoom room);

  public override void _Ready() => BodyEntered += OnBodyEntered;

  private void OnBodyEntered(Node2D body) {
    if (body is IPlayer) {
      EmitSignal(SignalName.ExitAreaEntered, (int)Room);
    }
  }
}
