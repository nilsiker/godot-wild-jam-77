namespace Nevergreen;

public partial class EnemyLogic {
  public partial record State {
    public partial record Alive : State, IGet<Input.Damage> {
      public Alive() { }

      public Transition On(in Input.Damage input) {
        var data = Get<Data>();
        data.Health -= input.Amount;

        Output(new Output.Damaged(input.Amount));
        Output(new Output.ForceApplied(input.Direction * 200f, true));

        return data.Health <= 0
          ? To<Dead>()
          : ToSelf();
      }
    }
  }
}
