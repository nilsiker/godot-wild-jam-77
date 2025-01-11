namespace Nevergreen;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IAppLogic : ILogicBlock<AppLogic.State>;


[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class AppLogic
  : LogicBlock<AppLogic.State>,
    IAppLogic {
  public override Transition GetInitialState() => To<State.InMainMenu>();
  public static class Input {
    public record struct StartGame;

    public record struct BackToMainMenu;

    public record struct QuitGame;

    public record struct QuitApp;

    public record struct FadeOutFinished;

    public record struct GameReady;
  }

  public static class Output {
    public record struct SetupGame;

    public record struct ShowGame;

    public record struct HideGame;

    public record struct RemoveGame;

    public record struct ShowMainMenu;

    public record struct HideMainMenu;

    public record struct CloseApplication;

    public record struct FadeIn;

    public record struct FadeOut;
  }
}
