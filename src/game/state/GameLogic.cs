namespace Nevergreen;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IGameLogic : ILogicBlock<GameLogic.State>;

[Meta, LogicBlock(typeof(State), Diagram = true)]
public partial class GameLogic : LogicBlock<GameLogic.State>, IGameLogic {
  public class Data {
    public ERoom Room;
  }

  public override Transition GetInitialState() => To<State.InRoom>();

  public static class Input {
    public readonly record struct PauseButtonPressed;
    public readonly record struct StartGame;
    public readonly record struct TransitionRoom(ERoom Room);
    public readonly record struct RoomResolved;
  }

  public static class Output {
    public readonly record struct SetPauseMode(bool IsPaused);
    public readonly record struct RoomTransitionRequested(ERoom Room);
    public readonly record struct RoomTransitionFinished;

  }
}
