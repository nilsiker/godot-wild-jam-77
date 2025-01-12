namespace Nevergreen;
using Chickensoft.LogicBlocks;


public partial class PlayerLogic {
  public partial record State {
    public record Attacking : State, IGet<Input.AnimationFinished> {
      private const string ATTACK = "attack";
      public Attacking() {
        OnAttach(() => { });
        OnDetach(() => { });

        this.OnEnter(() => {
          var data = Get<Data>();
          Output(new Output.AnimationUpdated(ATTACK));
          Output(new Output.Attacked(data.AttackDirection));
          Output(new Output.ForceApplied(data.AttackDirection * 1000f, true));
          if (data.AttackDirection.X < 0) {
            Output(new Output.FlipSprite(true));
          }
          else if (data.AttackDirection.X > 0) {
            Output(new Output.FlipSprite(false));
          }
        });
      }

      public Transition On(in Input.AnimationFinished input) =>
        input.Animation == ATTACK ? To<Idle>() : ToSelf();
    }
  }
}
