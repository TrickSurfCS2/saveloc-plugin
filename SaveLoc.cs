using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace SaveLoc;

[MinimumApiVersion(260)]
public partial class SaveLocPlugin : BasePlugin
{
  public override string ModuleName => "Save and Teleport location Plugin";
  public override string ModuleVersion => "0.0.2";
  public override string ModuleAuthor => "injurka";

  public static SaveLocPlugin Instance { get; private set; } = new();

  private Dictionary<IntPtr, SaveLocPlayer> SaveLocPlayers = new();

  public SaveLocPlayer GetPlayer(CCSPlayerController client)
  {
    return SaveLocPlayers[client.Handle];
  }

  public void SetPlayer(CCSPlayerController client, SaveLocPlayer? player)
  {
    if (player != null)
    {
      SaveLocPlayers[client.Handle] = player;
    }
    else
    {
      SaveLocPlayers.Remove(client.Handle);
    }
  }
}
