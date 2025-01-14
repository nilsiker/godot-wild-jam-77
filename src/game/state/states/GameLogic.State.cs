namespace Nevergreen;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public abstract partial record State : StateLogic<State> {
  }
}
