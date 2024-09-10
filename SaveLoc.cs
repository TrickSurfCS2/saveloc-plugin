using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SaveLoc;

public class Location
{
  public required DimensionVector origin { get; set; }
  public required DimensionVector angle { get; set; }
  public required DimensionVector velocity { get; set; }
}

public class DimensionVector
{
  public required float X { get; set; }
  public required float Y { get; set; }
  public required float Z { get; set; }

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
  public CCSPlayerController Player { get; init; } = client;
  public List<Location> SavedLocations { get; set; } = new();
  public int CurrentSavelocIndex { get; set; } = 0;

  public void SetLocation(Location location)
  {
    SavedLocations.Add(location);
    CurrentSavelocIndex = SavedLocations.Count - 1;
  }

  public void TeleportToSavedLocation()
  {
    if (SavedLocations.Count == 0)
    {
      Player.PrintToChat($" {ChatColors.White}No saveloc's");
      return;
    }

    var location = SavedLocations[CurrentSavelocIndex];

    if (location != null)
    {
      Player.PlayerPawn.Value!.Teleport(
        location.origin.ToVector(),
        location.angle.ToQAngle(),
        location.velocity.ToVector()
      );
    }
  }
}

partial class SaveLocPlugin : BasePlugin
{
  public override string ModuleName => "Save and Teleport location Plugin";
  public override string ModuleVersion => "0.0.1";
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

  [ConsoleCommand("saveloc", "Save current location")]
  [ConsoleCommand("sm_saveloc", "Save current location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnSaveLocation(CCSPlayerController client, CommandInfo _)
  {
    // var pawn = client.PlayerPawn.Value;
    // if (pawn == null)
    //   return;

    // var origin = new DimensionVector
    // {
    //   X = pawn.AbsOrigin!.X,
    //   Y = pawn.AbsOrigin!.Y,
    //   Z = pawn.AbsOrigin!.Z
    // };
    // var angle = new DimensionVector
    // {
    //   X = pawn.EyeAngles!.X,
    //   Y = pawn.EyeAngles!.Y,
    //   Z = pawn.EyeAngles!.Z
    // };
    // var velocity = new DimensionVector
    // {
    //   X = pawn.AbsVelocity!.X,
    //   Y = pawn.AbsVelocity!.Y,
    //   Z = pawn.AbsVelocity!.Z
    // };

    // var location = new Location
    // {
    //   origin = origin,
    //   angle = angle,
    //   velocity = velocity,
    // };

    // var player = Instance.GetPlayer(client);
    // player.SetLocation(location);
    // player.Player.PrintToChat($" {ChatColors.Purple} Saved location {ChatColors.Grey}#{player.SavedLocations.Count}");
  }
}
