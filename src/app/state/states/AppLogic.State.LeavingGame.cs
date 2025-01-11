namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public partial record LeavingGame : State, IGet<Input.FadeOutFinished> {
      public bool QuitToDesktop { get; set; }

      public LeavingGame() {
        this.OnEnter(() => Output(new Output.FadeOut()));
        this.OnExit(() => {
          Output(new Output.RemoveGame());
          Output(new Output.FadeIn());
        });
      }

      public Transition On(in Input.FadeOutFinished input) =>
        QuitToDesktop ? To<ClosingApplication>() : To<InMainMenu>();
    }
  }
}
