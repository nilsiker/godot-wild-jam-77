namespace Nevergreen;

using System;
using Chickensoft.Collections;

public enum EGameOverReason {
  Won,
  Lost
}

public interface IGameRepo : IDisposable {
  public event Action<EGameOverReason>? GameOver;
  public IAutoProp<int> Sludge { get; }
}

public class GameRepo : IGameRepo {
  public event Action? GameReady;
  public event Action<EGameOverReason>? GameOver;

  public IAutoProp<int> Sludge => _sludge;
  private readonly AutoProp<int> _sludge = new(0);

  public void Dispose() {
    GameReady = null;
    GameOver = null;
    GC.SuppressFinalize(this);
  }
}
