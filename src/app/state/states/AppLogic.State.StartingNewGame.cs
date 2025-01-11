namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public abstract partial record State {
    public partial record StartingNewGame : State, IGet<Input.GameReady> {
      public StartingNewGame() {
        this.OnEnter(() => {
          Output(new Output.SetupGame());
          Input(new Input.GameReady());
        });
        OnDetach(() => { });
      }

      public Transition On(in Input.GameReady input) => To<InGame>();
    }
  }
}
