namespace Woodblight;

using System;
using Chickensoft.Collections;

public interface IAppRepo : IDisposable {
  public event Action? GameStartRequested;
  public event Action? GameStarted;
  public event Action? MainMenuRequested;
  public event Action? AppQuitRequested;
  public event Action? FadeOutRequested;
  public event Action? FadeOutFinished;
  public event Action? FadeInRequested;

  public IAutoProp<bool> UseDice { get; }
  public void SetUseDice(bool useDice);
  public void RequestMainMenu();
  public void RequestGameStart();
  public void OnGameStarted();
  public void RequestQuitApp();
  public void RequestFadeOut();
  public void OnFadeOutFinished();
  public void RequestFadeIn();
}

public partial class AppRepo() : IAppRepo {
  public event Action? MainMenuRequested;
  public event Action? AppQuitRequested;
  public event Action? GameStartRequested;
  public event Action? GameStarted;
  public event Action? FadeOutRequested;
  public event Action? FadeInRequested;
  public event Action? FadeOutFinished;

  public IAutoProp<bool> UseDice => _useDice;
  private readonly AutoProp<bool> _useDice = new(false);
  public void SetUseDice(bool useDice) => _useDice.OnNext(useDice);


  public void RequestMainMenu() => MainMenuRequested?.Invoke();
  public void RequestGameStart() => GameStartRequested?.Invoke();
  public void OnGameStarted() => GameStarted?.Invoke();
  public void RequestQuitApp() => AppQuitRequested?.Invoke();
  public void RequestFadeOut() => FadeOutRequested?.Invoke();
  public void RequestFadeIn() => FadeInRequested?.Invoke();
  public void OnFadeOutFinished() => FadeOutFinished?.Invoke();

  public void Dispose(bool disposing) {
    MainMenuRequested = null;
    AppQuitRequested = null;
    GameStartRequested = null;
    GameStarted = null;
    FadeOutRequested = null;
    FadeInRequested = null;
    FadeOutFinished = null;

    _useDice.OnCompleted();
    _useDice.Dispose();
  }

  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

}
