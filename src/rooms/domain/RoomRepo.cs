namespace Nevergreen;

using System;
using Chickensoft.Collections;
using Godot;

public interface IRoomRepo {
  public event Action? EnemyKilled;
  public event Action? EnemySpawned;

  public IAutoProp<int> EnemyCount { get; }

  public void OnEnemyKilled();
  public void OnEnemySpawned();
}

public class RoomRepo(int enemyCount) : IRoomRepo, IDisposable {
  public event Action? EnemyKilled;
  public event Action? EnemySpawned;

  public IAutoProp<int> EnemyCount => _enemyCount;
  private readonly AutoProp<int> _enemyCount = new(enemyCount);

  public void OnEnemyKilled() {
    _enemyCount.OnNext(_enemyCount.Value - 1);
    EnemyKilled?.Invoke();
  }

  public void OnEnemySpawned() {
    GD.Print(_enemyCount.Value + 1);
    _enemyCount.OnNext(_enemyCount.Value + 1);
    EnemySpawned?.Invoke();
  }

  public void Dispose() {
    EnemyKilled = null;
    EnemySpawned = null;

    _enemyCount.OnCompleted();
    _enemyCount.Dispose();

    GC.SuppressFinalize(this);
  }
}
