namespace Nevergreen;
using Chickensoft.LogicBlocks;
using Shouldly;

public partial class PlayerLogic {
  public partial record State {
    public partial record Idle : State, IGet<Input.Move> {
      public Idle() {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => Output(new Output.AnimationUpdated("idle")));
        this.OnExit(() => { });
      }

      public Transition On(in Input.Move input) =>
        input.Direction.IsZeroApprox()
          ? ToSelf()
          : To<Moving>();
    }
  }
}
