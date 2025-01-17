namespace Nevergreen;

using System;
using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public partial record InMainMenu
      : State,
        IGet<Input.StartGame>,
        IGet<Input.QuitApp> {
      public InMainMenu() {
        OnAttach(() => Get<IAppRepo>().GameStartRequested += OnAppGameStartRequested);
        OnDetach(() => Get<IAppRepo>().GameStartRequested -= OnAppGameStartRequested);

        this.OnEnter(() => Output(new Output.ShowMainMenu()));
      }

      private void OnAppGameStartRequested() => Input(new Input.StartGame());

      Transition IGet<Input.StartGame>.On(in Input.StartGame input) => To<LeavingMenu>();

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input) => To<ClosingApplication>();
    }
  }
}
