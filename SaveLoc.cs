using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SaveLoc;

public class DimensionVector(float x, float y, float z)
{
  public float X { get; set; } = x;
  public float Y { get; set; } = y;
  public float Z { get; set; } = z;

  public Vector ToVector()
  {
    return new Vector(X, Y, Z);
  }

  public QAngle ToQAngle()
  {
    return new QAngle(X, Y, Z);
  }

  public override string ToString()
  {
    return $"{X} {Y} {Z}";
  }
}

public class SaveLocPlayer(CCSPlayerController client)
{
  public CCSPlayerController GetPlayer() => Player;

  public CCSPlayerController Player { get; init; } = client;

  private DimensionVector? origin = null;
  private DimensionVector? angle = null;
  private DimensionVector? velocity = null;

  public void SetLocation(DimensionVector origin, DimensionVector angle, DimensionVector velocity)
  {
    this.origin = origin;
    this.angle = angle;
    this.velocity = velocity;
  }

  public (DimensionVector?, DimensionVector?, DimensionVector?) GetLocation()
  {
    return (origin, angle, velocity);
  }
}

public class SaveLocPlugin : BasePlugin
{
  public override string ModuleName => "Save and Teleport location Plugin";
  public override string ModuleVersion => "0.0.1";
  public override string ModuleAuthor => "injurka";

  private static SaveLocPlugin _instance;
  public static SaveLocPlugin Instance => _instance;

  private Dictionary<IntPtr, SaveLocPlayer> SaveLocPlayers = new();
  public List<SaveLocPlayer> Players => SaveLocPlayers.Values.ToList();

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

  public override void Load(bool hotReload)
  {
    base.Load(hotReload);

    _instance ??= this;

    RegisterListener<Listeners.OnClientConnect>(OnClientConnectHandler);
    RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnectHandler);
  }

  [ConsoleCommand("saveloc", "Save current location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnSaveLocation(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null) return;

    var origin = new DimensionVector(
      pawn.AbsOrigin!.X,
      pawn.AbsOrigin!.Y,
      pawn.AbsOrigin!.Z
    );

    var angle = new DimensionVector(
      pawn.EyeAngles!.X,
      pawn.EyeAngles!.Y,
      pawn.EyeAngles!.Z
    );

    var velocity = new DimensionVector(
      pawn.AbsVelocity!.X,
      pawn.AbsVelocity!.Y,
      pawn.AbsVelocity!.Z
    );

    client.PrintToChat($" {ChatColors.Purple} Saved location");
    Instance.GetPlayer(client).SetLocation(origin, angle, velocity);
  }

  [ConsoleCommand("tploc", "Teleport to location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnTeleportLoc(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null) return;

    var (position, rotation, velocity) = Instance.GetPlayer(client).GetLocation();

    if (position != null && rotation != null && velocity != null)
    {
      pawn.Teleport(position.ToVector(), rotation.ToQAngle(), velocity.ToVector());
    }
  }

  private void OnClientDisconnectHandler(int slot)
  {
    var client = new CCSPlayerController(NativeAPI.GetEntityFromIndex(slot + 1));

    if (!client.IsValid || client.IsBot) return;

    SetPlayer(client, null);
  }

  private void OnClientConnectHandler(int slot, string name, string ipAddress)
  {
    var client = new CCSPlayerController(NativeAPI.GetEntityFromIndex(slot + 1));

    if (!client.IsValid || client.IsBot) return;

    var newPlayer = new SaveLocPlayer(client);

    Instance.SetPlayer(client, newPlayer);
  }
}
