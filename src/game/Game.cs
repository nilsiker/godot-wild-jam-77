namespace Nevergreen;

using System;
using System.Linq;
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
  [Node] private AnimationPlayer CutscenePlayer { get; set; } = default!;
  [Node] private PauseMenu PauseMenu { get; set; } = default!;
  private ERoom CurrentRoom { get; set; }
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
        => OnOutputRoomTransitionRequested(output.Room))
      .Handle((in GameLogic.Output.StartIntro _)
        => OnOutputStartIntro())
      .Handle((in GameLogic.Output.ShowPauseMenu _) => OnOutputShowPauseMenu())
       .Handle((in GameLogic.Output.StartOutro _) => OnOutputStartWinSequence());


    this.Provide();
    Logic.Start();

    AddToGroup(StateDebug.GROUP);
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

    var roomNode = scene.Instantiate<Room>();
    var entrance = roomNode.GetChildren()
      .OfType<ExitArea>()
      .FirstOrDefault(area => area.Room == CurrentRoom)?
      .PlayerEntryPosition;

    if (Room.GetChildCount() > 0) {
      Room.GetChild(0).QueueFree();
    }
    Room.AddChild(roomNode);
    CurrentRoom = room;

    if (entrance is not null) {
      Logic.Input(new GameLogic.Input.TeleportPlayerTo(entrance.GlobalPosition));
    }
  }

  #region Input Callbacks
  public void StartGame() => Logic.Input(new GameLogic.Input.StartGame());

  private void OnCutscenePlayerAnimationFinished(StringName animName) =>
    Logic.Input(new GameLogic.Input.CutsceneFinished());
  #endregion


  #region Output Callbacks
  private void SetGamePaused(bool isPaused) => GetTree().Paused = isPaused;
  private void OnOutputRoomTransitionRequested(ERoom room) =>
    CallDeferred(nameof(ChangeRoom), (int)room);

  private void OnOutputStartIntro() => CutscenePlayer.Play("intro");
  private void OnOutputShowPauseMenu() => PauseMenu.Open();
  private void OnOutputStartWinSequence() => CutscenePlayer.Play("outro");


  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public override void _Ready() =>
    CutscenePlayer.AnimationFinished += OnCutscenePlayerAnimationFinished;


  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(Inputs.Esc) && !PauseMenu.Visible) {
      Logic.Input(new GameLogic.Input.ClickPause());
    }
  }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
    GameRepo.Dispose();
  }
  #endregion
}
