namespace Woodblight;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[Meta(typeof(IAutoNode))]
public partial class ExitArea : Area2D {
  public override void _Notification(int what) => this.Notify(what);

  [Signal] public delegate void ExitAreaEnteredEventHandler(ERoom room);
  [Export] public ERoom Room { get; private set; }
  public Node2D PlayerEntryPosition => GetNode<Node2D>("PlayerEntryPosition");

  public override void _Ready() => BodyEntered += OnBodyEntered;


  private void OnBodyEntered(Node2D body) {
    if (body is IPlayer) {
      EmitSignal(SignalName.ExitAreaEntered, (int)Room);
    }
  }
}
