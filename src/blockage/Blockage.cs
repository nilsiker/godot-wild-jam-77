namespace Nevergreen;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IBlockage : IStaticBody2D { }

[Meta(typeof(IAutoNode))]
public partial class Blockage : StaticBody2D, IBlockage {
  #region Exports
  #endregion

  #region Nodes
  #endregion

  #region Dependencies
  [Dependency] private IRoomRepo RoomRepo => this.DependOn<IRoomRepo>();
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

  public void OnExitTree() { }
  #endregion

  #region Input Callbacks
  #endregion

  #region Output Callbacks
  #endregion
}
