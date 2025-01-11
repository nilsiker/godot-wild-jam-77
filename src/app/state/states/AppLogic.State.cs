namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State : StateLogic<State> {
    public State() {
      OnAttach(() => Get<IAppRepo>().AppQuitRequested += OnAppQuit);
      OnDetach(() => Get<IAppRepo>().AppQuitRequested -= OnAppQuit);
    }

    private void OnAppQuit() => Input(new Input.QuitApp());
  }
}
