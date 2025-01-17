namespace Nevergreen;

using System;
using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State : StateLogic<State>, IGet<Input.FadeOutFinished> {
    public State() {
      OnAttach(() => {
        var appRepo = Get<IAppRepo>();
        appRepo.AppQuitRequested += OnAppQuit;
        appRepo.FadeOutRequested += OnAppFadeOutRequested;
        appRepo.FadeInRequested += OnAppFadeInRequested;

      });
      OnDetach(() => {
        var appRepo = Get<IAppRepo>();
        appRepo.AppQuitRequested -= OnAppQuit;
        appRepo.FadeOutRequested -= OnAppFadeOutRequested;
        appRepo.FadeInRequested -= OnAppFadeInRequested;
      });
    }

    public Transition On(in Input.FadeOutFinished input) {
      Get<IAppRepo>().OnFadeOutFinished();
      return ToSelf();
    }
    private void OnAppFadeInRequested() => Output(new Output.FadeIn());
    private void OnAppFadeOutRequested() => Output(new Output.FadeOut());
    private void OnAppQuit() => Input(new Input.QuitApp());
  }
}
