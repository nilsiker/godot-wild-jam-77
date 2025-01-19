
namespace Nevergreen;

using System;
using Chickensoft.Collections;

public interface IPlayerRepo : IDisposable {
  public IAutoProp<int> Health { get; }

  public int Damage(int amount);
  public void SetHealth(int health);
}

public class PlayerRepo(int health) : IPlayerRepo {

  public IAutoProp<int> Health => _health;
  private readonly AutoProp<int> _health = new(health);

  public int Damage(int amount) {
    var newHealth = Health.Value - amount;
    _health.OnNext(newHealth);
    return newHealth;
  }

  public void Dispose() {
    _health.OnCompleted();
    _health.Dispose();

    GC.SuppressFinalize(this);
  }

  public void SetHealth(int health) => _health.OnNext(health);
}
