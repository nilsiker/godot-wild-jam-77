namespace Woodblight;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IGameLogic : ILogicBlock<GameLogic.State>;

[Meta, LogicBlock(typeof(State), Diagram = true)]
public partial class GameLogic : LogicBlock<GameLogic.State>, IGameLogic {
  public class Data {
    public ERoom Room;
  }

  public override Transition GetInitialState() => To<State.Intro>();

  public static class Input {
    public readonly record struct StartGame;
    public readonly record struct TransitionRoom(ERoom Room);
    public readonly record struct RoomResolved;
    public readonly record struct TeleportPlayerTo(Vector2 GlobalPosition);
    public readonly record struct RequestOutro;
    public readonly record struct CutsceneFinished;
    public readonly record struct ClickPause;

  }

  public static class Output {
    public readonly record struct SetPauseMode(bool IsPaused);
    public readonly record struct RoomTransitionRequested(ERoom Room);
    public readonly record struct RoomTransitionFinished;
    public readonly record struct StartIntro;
    public readonly record struct StartOutro;
    public readonly record struct ShowPauseMenu;

  }
}
