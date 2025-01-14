namespace Nevergreen;

using System;

public interface IAppRepo : IDisposable {
  public event Action? GameStartRequested;
  public event Action? GameStarted;
  public event Action? MainMenuRequested;
  public event Action? AppQuitRequested;
  public event Action? FadeOutRequested;
  public event Action? FadeInRequested;

  public void RequestMainMenu();
  public void RequestGameStart();
  public void OnGameStarted();
  public void RequestQuitApp();

  public void RequestFadeOut();
  public void RequestFadeIn();
}

public partial class AppRepo() : IAppRepo {
  public event Action? MainMenuRequested;
  public event Action? AppQuitRequested;
  public event Action? GameStartRequested;
  public event Action? GameStarted;
  public event Action? FadeOutRequested;
  public event Action? FadeInRequested;

  public void RequestMainMenu() => MainMenuRequested?.Invoke();

  public void RequestGameStart() => GameStartRequested?.Invoke();

  public void OnGameStarted() => GameStarted?.Invoke();

  public void RequestQuitApp() => AppQuitRequested?.Invoke();

  public void RequestFadeOut() => FadeOutRequested?.Invoke();

  public void RequestFadeIn() => FadeInRequested?.Invoke();

  public void Dispose(bool disposing) {
    MainMenuRequested = null;
    GameStartRequested = null;
    GameStarted = null;
    AppQuitRequested = null;
  }

  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}
