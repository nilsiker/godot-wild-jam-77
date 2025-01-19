namespace Woodblight;

using System;

public partial class PlayerLogic {
  public partial record State {
    public partial record Alive : State, IGet<Input.Attack>, IGet<Input.Damage>, IGet<Input.Die> {

      public Alive() {
        OnAttach(() => Get<IPlayerRepo>().Health.Sync += OnPlayerHealthSync);
        OnDetach(() => Get<IPlayerRepo>().Health.Sync -= OnPlayerHealthSync);
      }

      private void OnPlayerHealthSync(int health) {
        if (health < 1) {
          Input(new Input.Die());
        }
      }

      public Transition On(in Input.Attack input) {
        Get<Data>().AttackDirection = input.Direction;
        return To<Attacking>();
      }

      public Transition On(in Input.Damage input) {
        Get<IPlayerRepo>().Damage(input.Amount);
        Output(new Output.Damaged(input.Amount, -input.Direction));

        return ToSelf();
      }

      public Transition On(in Input.Die input) => To<Dead>();
    }
  }
}
