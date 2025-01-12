namespace Nevergreen;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public partial record State {
    public partial record Moving : State, IGet<Input.Move> {
      public Moving() {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => Output(new Output.AnimationUpdated("run")));
        this.OnExit(() => { });
      }

      public Transition On(in Input.Move input) {
        var data = Get<Data>();
        var velocity = input.Direction * data.Speed;

        if (data.CurrentVelocity != velocity) {
          data.CurrentVelocity = velocity;
          Output(new Output.VelocityUpdated(data.CurrentVelocity));

          if (data.CurrentVelocity.X < 0) {
            Output(new Output.FlipSprite(true));
          }
          else if (data.CurrentVelocity.X > 0) {
            Output(new Output.FlipSprite(false));
          }
        }

        return velocity.IsZeroApprox()
          ? To<Idle>()
          : ToSelf();
      }
    }
  }
}
