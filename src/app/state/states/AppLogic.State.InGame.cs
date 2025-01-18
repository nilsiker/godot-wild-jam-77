namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public partial record InGame : State, IGet<Input.BackToMainMenu>, IGet<Input.QuitApp> {
      public InGame() {
        OnAttach(() => Get<IAppRepo>().MainMenuRequested += OnMainMenuRequested);
        OnDetach(() => Get<IAppRepo>().MainMenuRequested -= OnMainMenuRequested);

        this.OnEnter(() => {
          Output(new Output.ShowGame());
          Output(new Output.FadeIn());
        });
      }

      private void OnMainMenuRequested() => Input(new Input.BackToMainMenu());

      Transition IGet<Input.BackToMainMenu>.On(in Input.BackToMainMenu input) => To<LeavingGame>();

      Transition IGet<Input.QuitApp>.On(in Input.QuitApp input) => To<ClosingApplication>();
    }
  }
}
