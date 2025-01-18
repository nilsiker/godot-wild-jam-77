namespace Nevergreen;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public partial record State {
    public partial record Alive {
      public partial record Idle : Alive, IGet<Input.Move> {
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
}
