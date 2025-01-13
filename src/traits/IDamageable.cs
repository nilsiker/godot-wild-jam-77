
namespace Nevergreen.Traits;

using Godot;

public interface IDamageable {
  public void Damage(int amount, Vector2 direction);
}
