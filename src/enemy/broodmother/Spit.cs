namespace Woodblight;

using Godot;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;

public interface ISpit : INode2D { }

[Meta(typeof(IAutoNode))]
public partial class Spit : Node2D, ISpit {
  #region Nodes
  [Node] private Area2D Area { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  #endregion

  #region State
  #endregion

  #region Dependency Lifecycle
  public void Setup() { }

  public void OnResolved() {
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
  }
  #endregion
}
