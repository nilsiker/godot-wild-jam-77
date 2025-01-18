namespace Nevergreen;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Nevergreen.Traits;


public interface IEnemy : IArea2D, IDamageable, IStateDebugInfo { }

[Meta(typeof(IAutoNode))]
public partial class Enemy : Area2D, IEnemy {
  #region Exports
  [Export] private EnemySettings _settings = default!;
  #endregion
  private static PackedScene BroodmotherScene => GD.Load<PackedScene>("res://src/enemy/broodmother/Broodmother.tscn");
  private static PackedScene LarvaScene => GD.Load<PackedScene>("res://src/enemy/larva/Larva.tscn");

  #region Nodes
  [Node] private Sprite2D Sprite { get; set; } = default!;
  [Node] private AnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node] private CpuParticles2D DamageSplash { get; set; } = default!;
  [Node] private Area2D AggroArea { get; set; } = default!;
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  [Dependency] private IGameRepo GameRepo => this.DependOn<IGameRepo>();
  [Dependency] private IRoomRepo RoomRepo => this.DependOn<IRoomRepo>();
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
    )
    .Handle(
      (in EnemyLogic.Output.ReplaceWithBroodmother _) => OnOutputReplaceWithBroodmother()
    )
    .Handle(
      (in EnemyLogic.Output.SpawnLarva _) => OnOutputSpawnLarva()
    )
    .Handle(
      (in EnemyLogic.Output.FlipSprite output) => OnOutputFlipSprite(output.Flip)
    );

    Logic.Set(GameRepo);
    Logic.Set(RoomRepo);

    Logic.Set(new EnemyLogic.Data { // NOTE is this reasonable?
      Health = _settings.Health,
      Speed = _settings.Speed,
      Damage = _settings.Damage
    });

    Logic.Set(_settings as IEnemySettings);

    AddToGroup(StateDebug.GROUP);
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

  public void OnProcess(double delta) =>
    Logic?.Input(new EnemyLogic.Input.Age((float)delta));


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
      player.Damage(_settings.Damage, ((INode2D)player).GlobalPosition.DirectionTo(GlobalPosition));
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

  private void OnOutputForceApplied(Vector2 force, bool isImpulse) =>
    GlobalPosition += force * (float)GetPhysicsProcessDeltaTime();
  private void OnOutputAnimationUpdated(StringName animationName) => AnimationPlayer.Play(animationName);

  private void OnOutputDied() => QueueFree();

  private void OnOutputReplaceWithBroodmother() {
    var parent = GetParent();
    var broodmother = BroodmotherScene.Instantiate<Enemy>();
    broodmother.GlobalPosition = GlobalPosition;

    parent.AddChild(broodmother);
    QueueFree();
  }

  private void OnOutputSpawnLarva() {
    var larva = LarvaScene.Instantiate<Enemy>();
    larva.GlobalPosition = GlobalPosition + (GlobalPosition.DirectionTo(PlayerPosition) * 10);
    GetParent().AddChild(larva);
  }

  private void OnOutputFlipSprite(bool flip) => Sprite.FlipH = flip;
  #endregion


  #region IDamageable
  public void Damage(int amount, Vector2 direction) => Logic.Input(new EnemyLogic.Input.Damage(amount, direction));
  #endregion
}
