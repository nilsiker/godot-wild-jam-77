namespace Nevergreen;

using System.Numerics;
using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public partial record State {
    public record Attacking : State, IGet<Input.AnimationFinished>, IGet<Input.UpdateHitting> {
      private const string ATTACK = "attack";
      public Attacking() {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => {
          var data = Get<Data>();
          Output(new Output.AnimationUpdated(ATTACK));
          Output(new Output.StartAttacking(data.AttackDirection));
          if (data.AttackDirection.X < 0) {
            Output(new Output.FlipSprite(true));
          }
          else if (data.AttackDirection.X > 0) {
            Output(new Output.FlipSprite(false));
          }
        });

        this.OnExit(() => Output(new Output.FinishedAttacking()));
      }

      public Transition On(in Input.AnimationFinished input) =>
        input.Animation == ATTACK ? To<Idle>() : ToSelf();
      public Transition On(in Input.UpdateHitting input) {
        if (input.IsHitting) {
          Output(new Output.ForceApplied(Get<Data>().AttackDirection * 100f, true));

        }
        Output(new Output.SetHitting(input.IsHitting));
        return ToSelf();
      }
    }
  }
}
