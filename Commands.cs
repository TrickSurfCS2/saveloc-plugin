using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SaveLoc;

partial class SaveLocPlugin
{
  [ConsoleCommand("saveloc", "Save current location")]
  [ConsoleCommand("sm_saveloc", "Save current location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnSaveLocation(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null)
      return;

    var origin = new DimensionVector
    {
      X = pawn.AbsOrigin!.X,
      Y = pawn.AbsOrigin!.Y,
      Z = pawn.AbsOrigin!.Z
    };
    var angle = new DimensionVector
    {
      X = pawn.EyeAngles!.X,
      Y = pawn.EyeAngles!.Y,
      Z = pawn.EyeAngles!.Z
    };
    var velocity = new DimensionVector
    {
      X = pawn.AbsVelocity!.X,
      Y = pawn.AbsVelocity!.Y,
      Z = pawn.AbsVelocity!.Z
    };

    var location = new Location
    {
      origin = origin,
      angle = angle,
      velocity = velocity,
    };

    var player = Instance.GetPlayer(client);
    player.SetLocation(location);
    player.Player.PrintToChat($" {ChatColors.Purple} Saved location {ChatColors.Grey}#{player.CurrentSavelocIndex}");
  }

  [ConsoleCommand("tploc", "Teleport to current saved location")]
  [ConsoleCommand("sm_tp", "Teleport to current saved location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnTeleportLoc(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null || !(client is { PawnIsAlive: true }))
      return;

    var player = Instance.GetPlayer(client);

    player.TeleportToSavedLocation();
  }

  [ConsoleCommand("prevloc", "Teleport to previous saved location and remove current")]
  [ConsoleCommand("sm_prevloc", "Teleport to previous saved saved location and remove current")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnPrevLocation(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null || !(client is { PawnIsAlive: true }))
      return;

    var player = Instance.GetPlayer(client);

    if (player.SavedLocations.Count <= 1)
    {
      client.PrintToChat($" {ChatColors.White}No saveloc's");
      return;
    }

    player.SavedLocations.RemoveAt(player.CurrentSavelocIndex);
    player.CurrentSavelocIndex--;

    if (player.CurrentSavelocIndex < 0)
      player.CurrentSavelocIndex = player.SavedLocations.Count - 1;

    player.TeleportToSavedLocation();
    player.Player.PrintToChat($" {ChatColors.Purple} Removed current location and switch to {ChatColors.Grey}#{player.CurrentSavelocIndex}");
  }

  [ConsoleCommand("backloc", "Teleport to previous saved location")]
  [ConsoleCommand("sm_backloc", "Teleport to previous saved location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnBackLocation(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null || !(client is { PawnIsAlive: true }))
      return;

    var player = Instance.GetPlayer(client);

    if (player.SavedLocations.Count == 0)
    {
      client.PrintToChat($" {ChatColors.White}No saveloc's");
      return;
    }

    player.CurrentSavelocIndex--;

    if (player.CurrentSavelocIndex < 0)
      player.CurrentSavelocIndex = player.SavedLocations.Count - 1;

    player.TeleportToSavedLocation();
    player.Player.PrintToChat($" {ChatColors.Purple} Changed location to {ChatColors.Grey}#{player.CurrentSavelocIndex}");
  }

  [ConsoleCommand("nextloc", "Teleport to next saved location")]
  [ConsoleCommand("sm_nextloc", "Teleport to next saved location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnNextLocation(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null || !(client is { PawnIsAlive: true }))
      return;

    var player = Instance.GetPlayer(client);

    if (player.SavedLocations.Count <= 1)
    {
      client.PrintToChat($" {ChatColors.White}No saveloc's");
      return;
    }

    player.CurrentSavelocIndex = (player.CurrentSavelocIndex + 1) % player.SavedLocations.Count;
    player.TeleportToSavedLocation();
    player.Player.PrintToChat($" {ChatColors.Purple} Changed location to {ChatColors.Grey}#{player.CurrentSavelocIndex}");
  }

  [ConsoleCommand("clearloc", "Clear all saved location")]
  [ConsoleCommand("sm_clearloc", "Clear all saved location")]
  [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnClearLoc(CCSPlayerController client, CommandInfo _)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null)
      return;

    var player = Instance.GetPlayer(client);

    player.CurrentSavelocIndex = 0;
    player.SavedLocations.Clear();
    player.Player.PrintToChat($" {ChatColors.Purple} All saved location removed");
  }

  [ConsoleCommand("toloc", "To index saved location")]
  [ConsoleCommand("sm_toloc", "To index saved location")]
  [CommandHelper(minArgs: 1, usage: "<toloc> 1", whoCanExecute: CommandUsage.CLIENT_ONLY)]
  public void OnToLoc(CCSPlayerController client, CommandInfo command)
  {
    var pawn = client.PlayerPawn.Value;
    if (pawn == null || command.ArgCount < 2)
      return;

    var player = Instance.GetPlayer(client);

    _ = int.TryParse(command.GetArg(1), out var index);

    if (index < 0 || index > player.SavedLocations.Count - 1)
    {
      client.PrintToChat($" {ChatColors.White}Saveloc not found");
      return;
    }


    var beforeIndex = player.CurrentSavelocIndex;
    player.CurrentSavelocIndex = index;
    player.TeleportToSavedLocation();
    player.Player.PrintToChat($" {ChatColors.Purple} Switch current location from {ChatColors.Grey}#{beforeIndex} to {ChatColors.Grey}#{index}");
  }
}
