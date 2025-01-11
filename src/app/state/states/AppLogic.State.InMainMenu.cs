namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public partial record InMainMenu
      : State,
        IGet<Input.StartGame>,
        IGet<Input.QuitApp> {
      public InMainMenu() {
        this.OnEnter(() => Output(new Output.ShowMainMenu()));
      }

      Transition IGet<Input.StartGame>.On(in Input.StartGame input) => To<LeavingMenu>();

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input) => To<ClosingApplication>();
    }
  }
}
