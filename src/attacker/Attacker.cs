namespace Nevergreen;

using Godot;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using System;
using Nevergreen.Traits;


public interface IAttacker : IArea2D {
  public void Attack(Vector2 direction);
  public void SetActive(bool active);
  public void HandleNodeEntered(Node node);
}


[Meta(typeof(IAutoNode))]
public partial class Attacker : Area2D, IAttacker {
  #region Exports
  #endregion

  #region Nodes
  [Node] private IAnimatedSprite2D FX { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  #endregion

  #region State
  private AttackerLogic Logic { get; set; } = default!;
  private AttackerLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here

    Logic.Start();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);

    BodyEntered += OnBodyEntered;
    AreaEntered += OnAreaEntered;
    Monitoring = false;
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }

  #endregion

  #region Signal Handlers
  private void OnBodyEntered(Node2D body) => HandleNodeEntered(body);
  private void OnAreaEntered(Area2D area) => HandleNodeEntered(area);
  #endregion

  #region IAttacker
  public void Attack(Vector2 direction) {
    var targetAngle = direction.Angle() + ((float)Math.PI / 2.0f);
    // NOTE roundabout way to fix the FX anim. It works, but is really messy.
    // If this turns into a problem, give this a proper solution
    if (targetAngle is < 0 or > (float)Math.PI) {
      targetAngle += (float)Math.PI;
      var scale = GlobalScale;
      scale.X = -1;
      GlobalScale = scale;
    }
    else {
      var scale = GlobalScale;
      scale.X = 1;
      GlobalScale = scale;
    }
    GlobalRotation = targetAngle;
    FX.Play("attack");
  }

  public void SetActive(bool active) => Monitoring = active;

  public void HandleNodeEntered(Node node) {
    if (node is IDamageable damageable) {
      damageable.Damage(1, GlobalPosition.DirectionTo(damageable.GlobalPosition)); // TODO make the damage amount configurable.
    }
  }
  #endregion

  #region Output Callbacks
  #endregion
}

public interface IAttackerLogic : ILogicBlock<AttackerLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class AttackerLogic
  : LogicBlock<AttackerLogic.State>,
    IAttackerLogic {
  public override Transition GetInitialState() => To<State>();

  public static class Input { }

  public static class Output { }

  public partial record State : StateLogic<State> {
    public State() {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}
