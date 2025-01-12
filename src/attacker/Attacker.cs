namespace Nevergreen;

using Godot;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using System;

public interface IAttacker : IArea2D {
  public void Attack(Vector2 direction);
  public void SetActive(bool active);
}


[Meta(typeof(IAutoNode))]
public partial class Attacker : Area2D, IAttacker {
  #region Exports
  #endregion

  #region Nodes
  [Node] IAnimatedSprite2D FX { get; set; } = default!;
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
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }

  #endregion

  #region Input Callbacks
  public void Attack(Vector2 direction) {
    GlobalRotation = direction.Angle() + ((float)Math.PI / 2.0f);
    FX.Play("attack");
  }

  public void SetActive(bool active) => Monitoring = active;
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
