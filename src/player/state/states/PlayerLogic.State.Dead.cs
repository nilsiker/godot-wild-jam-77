namespace Woodblight;

using Chickensoft.LogicBlocks;

public partial class PlayerLogic {
  public partial record State {
    public partial record Dead : State {
      public Dead() {
        this.OnEnter(() => Get<IGameRepo>().Lose());
      }
    }
  }
}
