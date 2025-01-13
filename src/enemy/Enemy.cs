namespace Nevergreen;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Nevergreen.Traits;


public interface IEnemy : IRigidBody2D, IDamageable, IStateDebugInfo { }

[Meta(typeof(IAutoNode))]
public partial class Enemy : RigidBody2D, IEnemy {
  #region Exports
  [Export] private EnemySettings _settings = default!;
  #endregion

  #region Nodes
  [Node] private AnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node] private CpuParticles2D DamageSplash { get; set; } = default!;
  [Node] private Area2D AggroArea { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  #endregion

  #region State
  private EnemyLogic Logic { get; set; } = default!;
  private EnemyLogic.IBinding Binding { get; set; } = default!;
  private Vector2 PlayerPosition { get; set; } = Vector2.Zero;  // TODO move this into EnemyData or something

  string IStateDebugInfo.Name => Name;

  public string State => Logic.Value.GetType().Name;
  #endregion


  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding.Handle(
      (in EnemyLogic.Output.PlayerTrackedAt output) =>
        OnOutputPlayerTrackedAt(output.GlobalPosition)
    ).Handle(
      (in EnemyLogic.Output.Damaged output) =>
        OnOutputDamaged(output.Amount)
    ).Handle(
      (in EnemyLogic.Output.ForceApplied output) =>
        OnOutputForceApplied(output.Force, output.IsImpulse)
    ).Handle(
      (in EnemyLogic.Output.AnimationUpdated output) =>
        OnOutputAnimationUpdated(output.AnimationName)
    ).Handle(
      (in EnemyLogic.Output.Died _) => OnOutputDied()
    );

    Logic.Set(GameRepo);
    Logic.Set(new EnemyLogic.Data { // NOTE is this reasonable?
      Health = _settings.Health,
      Speed = _settings.Speed,
      Damage = _settings.Damage
    });

    AddToGroup("state_debug");
    Logic.Start();
  }
  #endregion




  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);

    BodyEntered += OnBodyEntered;
    AggroArea.BodyEntered += OnAggroAreaBodyEntered;
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) {
    var direction = GlobalPosition.DirectionTo(PlayerPosition);

    if (!direction.IsZeroApprox()) {
      Logic.Input(new EnemyLogic.Input.Move(direction));
    }
  }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  private void OnBodyEntered(Node body) {
    if (body is IPlayer player) {
      player.Damage(_settings.Damage, player.GlobalPosition.DirectionTo(GlobalPosition));
    }
  }

  private void OnAggroAreaBodyEntered(Node2D body) {
    if (body is IPlayer) {
      Logic.Input(new EnemyLogic.Input.PlayerInRange());
    }
  }

  private void OnAnimationFinished(StringName animName) => Logic.Input(new EnemyLogic.Input.AnimationFinished(animName));

  #endregion

  #region Output Callbacks
  private void OnOutputPlayerTrackedAt(Vector2 globalPosition) =>
    PlayerPosition = globalPosition;

  private void OnOutputDamaged(float amount) {
    DamageSplash.Restart();
    AnimationPlayer.Play("RESET");
    AnimationPlayer.Play("hit");
  }

  private void OnOutputForceApplied(Vector2 force, bool isImpulse) {
    if (isImpulse) {
      ApplyImpulse(force);
    }
    else {
      ApplyForce(force);
    }
  }
  private void OnOutputAnimationUpdated(StringName animationName) => AnimationPlayer.Play(animationName);
  private void OnOutputDied() => QueueFree();
  #endregion

  #region IDamageable
  public void Damage(int amount, Vector2 direction) => Logic.Input(new EnemyLogic.Input.Damage(amount, direction));
  #endregion
}
