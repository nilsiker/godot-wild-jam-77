namespace Nevergreen;

using System;
using Chickensoft.Collections;
using Godot;

public enum EGameOverReason {
  Won,
  Lost
}

public interface IGameRepo : IDisposable {
  public event Action<EGameOverReason>? GameOver;

  public IAutoProp<Vector2> PlayerGlobalPosition { get; }
  public void UpdatePlayerGlobalPosition(Vector2 playerGlobalPosition);
}

public class GameRepo : IGameRepo {
  public event Action? GameReady;
  public event Action<EGameOverReason>? GameOver;
  public IAutoProp<Vector2> PlayerGlobalPosition => _playerGlobalPosition;
  private readonly AutoProp<Vector2> _playerGlobalPosition = new(Vector2.Zero);
  public void UpdatePlayerGlobalPosition(Vector2 playerGlobalPosition) =>
    _playerGlobalPosition.OnNext(playerGlobalPosition);

  public void Dispose() {
    _playerGlobalPosition.OnCompleted();
    _playerGlobalPosition.Dispose();
    GameReady = null;
    GameOver = null;
    GC.SuppressFinalize(this);
  }
}
