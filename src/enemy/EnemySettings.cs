namespace Nevergreen;

using Godot;

public interface IEnemyData {
  public int Health { get; }
  public int Speed { get; }
  public int Damage { get; }
}

[Tool, GlobalClass]
public partial class EnemySettings : Resource, IEnemyData {
  [Export] public int Health { get; set; }
  [Export] public int Speed { get; set; }
  [Export] public int Damage { get; set; }
}
