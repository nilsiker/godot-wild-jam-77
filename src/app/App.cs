namespace Nevergreen;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IApp : INode, IProvide<IAppRepo>, IStateDebugInfo;

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
  private IGame Game { get; set; } = default!;

  [Node] private IMainMenu MainMenu { get; set; } = default!;

  [Node] private AnimationPlayer AnimationPlayer { get; set; } = default!;

  #region IStateDebugInfo
  string IStateDebugInfo.Name => Name;
  public string State => Logic.Value.GetType().Name;

  #endregion

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
      .Handle((in AppLogic.Output.HideGame _) => Game.Visible = false)
      .Handle((in AppLogic.Output.RemoveGame _) => OnOutputRemoveGame())
      .Handle((in AppLogic.Output.ShowMainMenu _) => CallDeferred(nameof(OnOutputShowMainMenu)))
      .Handle((in AppLogic.Output.HideMainMenu _) => MainMenu.Visible = false)
      .Handle((in AppLogic.Output.ShowGame _) => CallDeferred(nameof(OnOutputShowGame)))
      .Handle((in AppLogic.Output.FadeIn _) => CallDeferred(nameof(OnOutputFadeIn)))
      .Handle((in AppLogic.Output.FadeOut _) => OnOutputFadeOut());

    this.Provide();
    Logic.Start();

    AddToGroup(StateDebug.GROUP);
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
    var gameNode = _gameScene.Instantiate<Game>();
    Game = gameNode;
    AddChild(gameNode);
  }

  private void OnOutputRemoveGame() => Game?.QueueFree();

  private void OnOutputShowGame() => Game.Visible = true;

  private void OnOutputShowMainMenu() => MainMenu.Open();

  private void OnOutputFadeOut() {
    // FIXME this is a hack, could be solved if Fadeout was its own repo where we keep track of the state.
    if (GetNode<ColorRect>("UI/Fadeout").Color.A < 0.1) {
      AnimationPlayer.Play("fade_out");
    }
  }

  private void OnOutputFadeIn() => AnimationPlayer.Play("fade_in");

  private void OnOutputQuitApp() => GetTree().Quit();
  #endregion
}
