namespace Nevergreen;

using Godot;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using System;
using System.Linq;


public enum ERoom {
  None,
  Stump,
  Glade,
  Forest,
  Tunnel,
  Hive
}

public interface IRoom : INode2D, IProvide<IRoomRepo>, IStateDebugInfo { }

[Meta(typeof(IAutoNode))]
public partial class Room : Node2D, IRoom {
  #region Exports
  [Export] public ERoom RoomIdentifier { get; private set; }
  #endregion

  #region Nodes
  #endregion

  #region Provisions
  public IRoomRepo Value() => RoomRepo;
  #endregion

  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region State
  public IRoomRepo RoomRepo { get; set; } = default!;
  private RoomLogic Logic { get; set; } = default!;
  private RoomLogic.IBinding Binding { get; set; } = default!;

  string IStateDebugInfo.Name => Name;

  public string State => Logic.Value.GetType().Name;
  #endregion


  #region Dependency Lifecycle
  public void Setup() {
    Logic = new();
    RoomRepo = new RoomRepo(GetChildren().OfType<Enemy>().Count());
  }

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding.Handle((in RoomLogic.Output.RemoveBlockage _) => OnOutputRemoveBlockage());

    Logic.Set(RoomRepo);

    this.Provide();
    Logic.Start();
    GameRepo.OnRoomResolved();  // TODO taking a shortcut

    AddToGroup(StateDebug.GROUP);
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
    GameRepo.RequestRoomTransition(room);  // Shortcut!
  #endregion


  #region Output Callbacks
  private void OnOutputRemoveBlockage() {
    if (RoomIdentifier == ERoom.Tunnel) {
      GameRepo.Win();
    }
    else {
      GetNode<Node2D>("Blockage").QueueFree();
    }
  }
  #endregion
}

public interface IRoomLogic : ILogicBlock<RoomLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class RoomLogic
  : LogicBlock<RoomLogic.State>,
    IRoomLogic {
  public override Transition GetInitialState() => To<State.Infested>();
  public static class Input {
    public record struct Clear;
  }

  public static class Output {
    public record struct EnemyKilled;
    public record struct RemoveBlockage;
  }

  public partial record State : StateLogic<State> {


    public State() {
      OnAttach(() => Get<IRoomRepo>().EnemyCount.Sync += OnEnemyCountSync);
      OnDetach(() => Get<IRoomRepo>().EnemyCount.Sync -= OnEnemyCountSync);
    }

    private void OnEnemyCountSync(int enemyCount) {
      if (enemyCount == 0) {
        Input(new Input.Clear());
      }
    }

    public partial record Infested : State, IGet<Input.Clear> {
      public Transition On(in Input.Clear input) => To<Cleared>();
    }

    public partial record Cleared : State {
      public Cleared() {
        this.OnEnter(() => Output(new Output.RemoveBlockage()));
      }
    }
  }
}
