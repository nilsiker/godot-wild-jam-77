namespace Nevergreen;

using System;

public interface IRoomRepo {
  public event Action? EnemyKilled;
  public event Action? EnemySpawned;

  public void OnEnemyKilled();
  public void OnEnemySpawned();
}

public class RoomRepo : IRoomRepo {
  public event Action? EnemyKilled;
  public event Action? EnemySpawned;
  public void OnEnemyKilled() => EnemyKilled?.Invoke();
  public void OnEnemySpawned() => EnemySpawned?.Invoke();
}
