namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State {
    public partial record Intro : State, IGet<Input.CutsceneFinished> {
      public Intro() {
        this.OnEnter(() => {
          Output(new Output.SetPauseMode(true));
          Output(new Output.StartIntro());
        });
      }

      public Transition On(in Input.CutsceneFinished input) {
        Get<IAppRepo>().RequestFadeOut();
        return To<ChangingRoom>();
      }
    }
  }
}
