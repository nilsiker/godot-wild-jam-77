namespace Nevergreen;

using Godot;

public interface IEnemySettings {
  public int Health { get; }
  public int Speed { get; }
  public int Damage { get; }
  public bool Metamorphizes { get; }
  public bool Breeds { get; }
}

[Tool, GlobalClass]
public partial class EnemySettings : Resource, IEnemySettings {
  [Export] public int Health { get; set; }
  [Export] public int Speed { get; set; }
  [Export] public int Damage { get; set; }
  [Export] public bool Metamorphizes { get; set; }
  [Export] public bool Breeds { get; set; }
}
