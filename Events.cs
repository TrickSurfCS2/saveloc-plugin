using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;

namespace SaveLoc;

partial class SaveLocPlugin
{
  public override void Load(bool hotReload)
  {
    base.Load(hotReload);

    Instance = this;
  }

  [GameEventHandler]
  public HookResult OnPlayerFullConnect(EventPlayerConnectFull @event, GameEventInfo _)
  {
    CCSPlayerController? client = @event.Userid;

    if (client == null || !client.IsValid || client.IsBot || !client.UserId.HasValue)
      return HookResult.Continue;

    Instance.SetPlayer(client, new SaveLocPlayer(client));

    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnClientDisconnect(EventPlayerDisconnect @event, GameEventInfo _)
  {
    CCSPlayerController? client = @event.Userid;

    if (client == null || !client.IsValid || client.IsBot)
      return HookResult.Continue;

    SetPlayer(client, null);

    return HookResult.Continue;
  }
}
