namespace Nevergreen;

using System;

public interface IRoomRepo {
  public event Action? EnemyKilled;
  public void OnEnemyKilled();
}

public class RoomRepo : IRoomRepo {
  public event Action? EnemyKilled;
  public void OnEnemyKilled() => EnemyKilled?.Invoke();

}
