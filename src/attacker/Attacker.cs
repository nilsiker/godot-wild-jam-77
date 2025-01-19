namespace Woodblight;

using Godot;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using System;
using Woodblight.Traits;


public interface IAttacker : INode2D {
  public void Attack(Vector2 direction);
  public void SetActive(bool active);
  public void HandleNodeEntered(Node node);
}


[Meta(typeof(IAutoNode))]
public partial class Attacker : Node2D, IAttacker {
  #region Exports
  #endregion

  #region Nodes
  [Node] private IAnimatedSprite2D FX { get; set; } = default!;
  [Node] private Dice Dice { get; set; } = default!;
  [Node] private Area2D Swing { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  private int _damage = 1;

  #region Dependencies
  [Dependency] private IAppRepo AppRepo => this.DependOn<IAppRepo>();
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

    Swing.BodyEntered += OnBodyEntered;
    Swing.AreaEntered += OnAreaEntered;
    Swing.Monitoring = false;
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
    if (AppRepo.UseDice.Value) {
      var roll = Dice.Roll();
      _damage = roll > 1 ? roll : 0;
    }
    else {
      _damage = 1;
    }
    var targetAngle = direction.Angle() + ((float)Math.PI / 2.0f);
    // NOTE roundabout way to fix the FX anim. It works, but is really messy.
    // If this turns into a problem, give this a proper solution
    if (targetAngle is < 0 or > (float)Math.PI) {
      targetAngle += (float)Math.PI;
      var scale = Swing.GlobalScale;
      scale.X = -1;
      Swing.GlobalScale = scale;
    }
    else {
      var scale = Swing.GlobalScale;
      scale.X = 1;
      Swing.GlobalScale = scale;
    }
    Swing.GlobalRotation = targetAngle;
    FX.Play("attack");
  }

  public void SetActive(bool active) => Swing.Monitoring = active;

  public void HandleNodeEntered(Node node) {
    if (_damage > 0 && node is IDamageable damageable) {
      damageable.Damage(_damage, GlobalPosition.DirectionTo(damageable.GlobalPosition)); // TODO make the damage amount configurable.
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
