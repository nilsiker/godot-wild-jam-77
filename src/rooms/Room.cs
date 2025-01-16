namespace Nevergreen;

using Godot;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public enum ERoom {
  None,
  Stump,
  Glade,
  Forest,
  Tunnel,
  Hive
}

public interface IRoom : INode2D { }

[Meta(typeof(IAutoNode))]
public partial class Room : Node2D, IRoom {
  #region Exports
  [Export] public ERoom RoomIdentifier { get; private set; }
  #endregion

  #region Nodes
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region State
  private RoomLogic Logic { get; set; } = default!;
  private RoomLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Logic.Start();
    GameRepo.OnRoomResolved();  // TODO taking a shortcut as Room does not have a state now
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
  public void OnExitAreaEntered(ERoom room) =>
    GameRepo.RequestRoomTransition(room);
  #endregion

  #region Output Callbacks
  #endregion
}

public interface IRoomLogic : ILogicBlock<RoomLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class RoomLogic
  : LogicBlock<RoomLogic.State>,
    IRoomLogic {
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
