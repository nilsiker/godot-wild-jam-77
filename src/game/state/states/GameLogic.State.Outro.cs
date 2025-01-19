namespace Woodblight;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State {
    public partial record Outro : State, IGet<Input.CutsceneFinished> {
      public Outro() {
        this.OnEnter(() => {
          Output(new Output.SetPauseMode(true));
          Output(new Output.StartOutro());
        });
      }

      public Transition On(in Input.CutsceneFinished input) {
        Get<IAppRepo>().RequestMainMenu();
        return ToSelf();
      }
    }
  }
}
