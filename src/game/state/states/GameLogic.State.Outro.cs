namespace Nevergreen;

using Godot;

public partial class GameLogic {
  public abstract partial record State {
    public partial record Outro : State, IGet<Input.CutsceneFinished> {
      public Outro() { }

      public Transition On(in Input.CutsceneFinished input) {
        Get<IAppRepo>().RequestMainMenu();
        GD.Print("Should remove game");
        return ToSelf();
      }
    }
  }
}
