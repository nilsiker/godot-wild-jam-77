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
  public event Action<ERoom>? RoomTransitionRequested;
  public event Action? RoomResolved;
  public event Action<Vector2>? PlayerTeleportationRequested;

  public IAutoProp<Vector2> PlayerGlobalPosition { get; }
  public void UpdatePlayerGlobalPosition(Vector2 playerGlobalPosition);
  public void RequestRoomTransition(ERoom room);
  public void OnRoomResolved();
  public void RequestPlayerTeleportation(Vector2 globalPosition);
}

public class GameRepo : IGameRepo {
  public event Action? GameReady;
  public event Action<EGameOverReason>? GameOver;
  public event Action<ERoom>? RoomTransitionRequested;
  public event Action? RoomResolved;
  public event Action<Vector2>? PlayerTeleportationRequested;


  public IAutoProp<Vector2> PlayerGlobalPosition => _playerGlobalPosition;
  private readonly AutoProp<Vector2> _playerGlobalPosition = new(Vector2.Zero);
  public void UpdatePlayerGlobalPosition(Vector2 playerGlobalPosition) =>
    _playerGlobalPosition.OnNext(playerGlobalPosition);

  public void RequestRoomTransition(ERoom room) =>
    RoomTransitionRequested?.Invoke(room);

  public void OnRoomResolved() =>
    RoomResolved?.Invoke();

  public void RequestPlayerTeleportation(Vector2 globalPosition) =>
    PlayerTeleportationRequested?.Invoke(globalPosition);

  public void Dispose() {
    _playerGlobalPosition.OnCompleted();
    _playerGlobalPosition.Dispose();
    GameReady = null;
    GameOver = null;
    RoomTransitionRequested = null;
    RoomResolved = null;
    PlayerTeleportationRequested = null;

    GC.SuppressFinalize(this);
  }

}
