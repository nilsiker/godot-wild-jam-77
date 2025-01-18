namespace Nevergreen;

public partial class PlayerLogic {
  public partial record State {
    public partial record Alive : State, IGet<Input.Attack>, IGet<Input.Damage> {

      public Transition On(in Input.Attack input) {
        Get<Data>().AttackDirection = input.Direction;
        return To<Attacking>();
      }

      public Transition On(in Input.Damage input) {
        var data = Get<Data>();
        data.Health -= 1;

        Output(new Output.Damaged(input.Amount, -input.Direction));

        return data.Health < 1
          ? To<Dead>()
          : ToSelf();
      }
    }
  }
}
