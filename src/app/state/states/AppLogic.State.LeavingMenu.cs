namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public partial record LeavingMenu : State, IGet<Input.FadeOutFinished> {
      public LeavingMenu() {
        this.OnEnter(() => Output(new Output.FadeOut()));
        this.OnExit(() => Output(new Output.HideMainMenu()));
      }

      public Transition On(in Input.FadeOutFinished input) {
        Output(new Output.FadeIn());
        return To<StartingNewGame>();
      }
    }
  }
}
