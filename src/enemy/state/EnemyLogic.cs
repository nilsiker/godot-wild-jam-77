namespace Nevergreen;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

public interface IEnemyLogic : ILogicBlock<EnemyLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class EnemyLogic
  : LogicBlock<EnemyLogic.State>,
    IEnemyLogic {
  public override Transition GetInitialState() => To<State.Alive.Wandering>();
  protected override void HandleError(Exception e) => throw e;

  public class Data {
    public int Speed;
    public int Health;
    public int Damage;
  }

  public static class Input {
    public record struct AnimationFinished(StringName Animation);
    public record struct Move(Vector2 Direction);
    public record struct Damage(int Amount, Vector2 Direction);
    public record struct PlayerInRange;
  }

  public static class Output {
    public record struct AnimationUpdated(StringName AnimationName);
    public record struct PlayerTrackedAt(Vector2 GlobalPosition);
    public record struct ForceApplied(Vector2 Force, bool IsImpulse);
    public record struct Damaged(float Amount);
    public record struct Died;
  }
}
