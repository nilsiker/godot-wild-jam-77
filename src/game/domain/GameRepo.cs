namespace Nevergreen;

using System;


public enum EGameOverReason {
  Won,
  Lost
}

public interface IGameRepo : IDisposable {
  public event Action<EGameOverReason>? GameOver;
}

public class GameRepo : IGameRepo {
  public event Action? GameReady;
  public event Action<EGameOverReason>? GameOver;

  public void Dispose() {
    GameReady = null;
    GameOver = null;
    GC.SuppressFinalize(this);
  }
}
