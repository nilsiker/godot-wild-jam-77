namespace Woodblight;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public partial record ClosingApplication : State, IGet<Input.FadeOutFinished> {
      public ClosingApplication() {
        this.OnEnter(() => Output(new Output.FadeOut()));
      }

      public Transition On(in Input.FadeOutFinished input) {
        Output(new Output.CloseApplication());
        return ToSelf();
      }
    }
  }
}
