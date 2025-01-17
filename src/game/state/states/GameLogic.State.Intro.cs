namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State {
    public partial record Intro : State, IGet<Input.CutsceneFinished> {
      public Intro() {
        this.OnEnter(() => {
          Output(new Output.SetPauseMode(false));
          Output(new Output.StartIntro());
        });
      }

      public Transition On(in Input.CutsceneFinished input) => To<ChangingRoom>();
    }
  }
}
