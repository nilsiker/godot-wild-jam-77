namespace Nevergreen;

using Chickensoft.LogicBlocks;

public partial class EnemyLogic {
  public partial record State {
    public partial record Alive : State, IGet<Input.Damage> {
      public Alive() { }

      public Transition On(in Input.Damage input) {
        var data = Get<Data>();
        data.Health -= input.Amount;
        data.KnockbackDirection = input.Direction;

        Output(new Output.Damaged(input.Amount));
        Output(new Output.ForceApplied(input.Direction * 200f, true));

        return data.Health <= 0
          ? To<Dead>()
          : To<KnockedBack>();
      }

      public record KnockedBack : Alive, IGet<Input.Age> {
        private float _time;

        public KnockedBack() {
          this.OnEnter(() => _time = 0f);
        }

        public Transition On(in Input.Age input) {
          Output(new Output.ForceApplied(Get<Data>().KnockbackDirection * (0.5f - _time) * 40, false));
          _time += input.Time;
          return _time > 0.5f ? To<Aggroed>() : ToSelf();
        }
      }
    }
  }
}
