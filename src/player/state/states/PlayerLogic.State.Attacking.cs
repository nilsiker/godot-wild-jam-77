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
        });
      }

      public Transition On(in Input.AnimationFinished input) =>
        input.Animation == ATTACK ? To<Idle>() : ToSelf();
    }
  }
}
