
namespace Nevergreen;

using System;

public interface IPlayerRepo : IDisposable {

}

public class PlayerRepo : IPlayerRepo {
  public void Dispose() => GC.SuppressFinalize(this);
}
