namespace Nevergreen;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IApp : INode, IProvide<IAppRepo>;

[Meta(typeof(IAutoNode))]
public partial class App : Node, IApp {
  #region Save
  #endregion

  #region Exports
  [Export]
  private PackedScene _gameScene = default!;
  #endregion

  #region State
  private IAppRepo AppRepo { get; set; } = default!;
  private AppLogic Logic { get; set; } = default!;
  private AppLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Provisions
  IAppRepo IProvide<IAppRepo>.Value() => AppRepo;

  #endregion

  #region Nodes
  [Node]
  private Node2D GameContainer { get; set; } = default!;

  [Node]
  private Control MainMenu { get; set; } = default!;

  [Node]
  private AnimationPlayer AnimationPlayer { get; set; } = default!;

  private IGame Game { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() {
    AppRepo = new AppRepo();

    Logic = new();
    Logic.Set(AppRepo);
  }

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here
    Binding
      .Handle((in AppLogic.Output.CloseApplication _) => OnOutputQuitApp())
      .Handle((in AppLogic.Output.SetupGame _) => OnOutputSetupGame())
      .Handle((in AppLogic.Output.HideGame _) => GameContainer.Visible = false)
      .Handle((in AppLogic.Output.RemoveGame _) => OnOutputRemoveGame())
      .Handle((in AppLogic.Output.ShowMainMenu _) => CallDeferred(nameof(OnOutputShowMainMenu)))
      .Handle((in AppLogic.Output.HideMainMenu _) => MainMenu.Visible = false)
      .Handle((in AppLogic.Output.ShowGame _) => CallDeferred(nameof(OnOutputShowGame)))
      .Handle((in AppLogic.Output.FadeIn _) => CallDeferred(nameof(OnOutputFadeIn)))
      .Handle((in AppLogic.Output.FadeOut _) => OnOutputFadeOut())
      .When<AppLogic.State>(state => GD.Print(state.GetType().Name));

    Logic.Start();
    this.Provide();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(false);
    SetPhysicsProcess(false);

    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public void OnExitTree() {
    AnimationPlayer.AnimationFinished -= OnAnimationFinished;

    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  private void OnAnimationFinished(StringName animName) {
    if (animName == "fade_out") {
      Logic.Input(new AppLogic.Input.FadeOutFinished());
    }
  }

  private void NewGame() => Logic.Input(new AppLogic.Input.StartGame());

  private void QuitApp() => Logic.Input(new AppLogic.Input.QuitApp());
  #endregion

  #region Output Callbacks
  private void OnOutputSetupGame() {
    Game = _gameScene.Instantiate<IGame>();
    GameContainer.AddChildEx(Game);
  }

  private void OnOutputRemoveGame() {
    if (GameContainer.GetChildCount() == 0) {
      return;
    }

    var game = GameContainer.GetChild(0);
    GameContainer.RemoveChildEx(game);
    game.QueueFree();
  }

  private void OnOutputShowGame() => GameContainer.Visible = true;

  private void OnOutputShowMainMenu() => MainMenu.Visible = true;

  private void OnOutputFadeOut() => AnimationPlayer.Play("fade_out");

  private void OnOutputFadeIn() => AnimationPlayer.Play("fade_in");

  private void OnOutputQuitApp() => GetTree().Quit();
  #endregion
}
