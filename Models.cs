using CounterStrikeSharp.API.Core;
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
