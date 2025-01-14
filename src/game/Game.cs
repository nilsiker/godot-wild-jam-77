namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IGame : INode2D, IProvide<IGameRepo>, IStateDebugInfo {
  public void StartGame();
  public void ChangeRoom(ERoom room);
}

[Meta(typeof(IAutoNode))]
public partial class Game : Node2D, IGame {
  #region Export
  [Export] private PackedScene _stumpScene = default!;
  [Export] private PackedScene _gladeScene = default!;
  [Export] private PackedScene _forestScene = default!;
  [Export] private PackedScene _tunnelScene = default!;
  [Export] private PackedScene _hiveScene = default!;
  #endregion

  #region State
  public IGameRepo GameRepo { get; set; } = default!;
  public IGameLogic Logic { get; set; } = default!;
  public GameLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Nodes
  [Node] private Node Room { get; set; } = default!;
  #endregion

  #region Provisions
  IGameRepo IProvide<IGameRepo>.Value() => GameRepo;
  #endregion

  #region Dependencies
  [Dependency]
  private IAppRepo AppRepo => this.DependOn<IAppRepo>();

  string IStateDebugInfo.Name => Name;

  public string State => Logic.Value.GetType().Name;
  #endregion



  #region Dependency Lifecycle
  public void Setup() => Logic = new GameLogic();

  public void OnResolved() {
    GameRepo = new GameRepo();
    Logic.Set(GameRepo);
    Logic.Set(AppRepo);
    Logic.Set(new GameLogic.Data() { Room = ERoom.Stump });

    Binding = Logic.Bind();
    Binding
      .Handle((in GameLogic.Output.SetPauseMode output)
        => SetGamePaused(output.IsPaused))
      .Handle((in GameLogic.Output.RoomTransitionRequested output)
        => OnOutputRoomTransitionRequested(output.Room));


    this.Provide();
    Logic.Start();

    AddToGroup("state_debug");
  }
  #endregion


  public void ChangeRoom(ERoom room) {
    var scene = room switch {
      ERoom.Stump => _stumpScene,
      ERoom.Glade => _gladeScene,
      ERoom.Forest => _forestScene,
      ERoom.Tunnel => _tunnelScene,
      ERoom.Hive => _hiveScene,
      _ => throw new NotImplementedException(),
    };

    if (Room.GetChildCount() > 0) {
      Room.GetChild(0).QueueFree();
    }
    Room.AddChild(scene.Instantiate());
  }


  #region Input Callbacks
  public void StartGame() => Logic.Input(new GameLogic.Input.StartGame());
  #endregion

  #region Output Callbacks
  private void SetGamePaused(bool isPaused) => GetTree().Paused = isPaused;
  private void OnOutputRoomTransitionRequested(ERoom room) => ChangeRoom(room);

  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public override void _Input(InputEvent @event) {
    if (Input.IsActionJustPressed(Inputs.Esc)) {
      Logic.Input(new GameLogic.Input.PauseButtonPressed());
    }
  }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
    GameRepo.Dispose();
  }
  #endregion
}
