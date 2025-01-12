namespace Nevergreen;

using Godot;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IMainMenu : IControl { }

[Meta(typeof(IAutoNode))]
public partial class MainMenu : Control, IMainMenu {
  #region Exports
  #endregion

  #region Nodes
  [Node]
  private PanelContainer Background { get; set; }
  #endregion

  #region Provisions
  #endregion

  #region Dependencies
  #endregion

  #region State
  private MainMenuLogic Logic { get; set; } = default!;
  private MainMenuLogic.IBinding Binding { get; set; } = default!;
  #endregion

  #region Dependency Lifecycle
  public void Setup() => Logic = new();

  public void OnResolved() {
    Binding = Logic.Bind();

    // Bind functions to state outputs here


    Logic.Start();
  }
  #endregion

  #region Godot Lifecycle
  public override void _Notification(int what) => this.Notify(what);

  public void OnReady() {
    SetProcess(true);
    SetPhysicsProcess(true);
  }

  public void OnProcess(double delta) { }

  public void OnPhysicsProcess(double delta) { }

  public void OnExitTree() {
    Logic.Stop();
    Binding.Dispose();
  }
  #endregion

  #region Input Callbacks
  #endregion

  #region Output Callbacks
  #endregion
}

public interface IMainMenuLogic : ILogicBlock<MainMenuLogic.State>;

[Meta]
[LogicBlock(typeof(State), Diagram = true)]
public partial class MainMenuLogic
  : LogicBlock<MainMenuLogic.State>,
    IMainMenuLogic {
  public override Transition GetInitialState() => To<State>();

  public static class Input { }

  public static class Output { }

  public partial record State : StateLogic<State> {
    public State() {
      OnAttach(() => { });
      OnDetach(() => { });
    }
  }
}
