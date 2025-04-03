using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadMe : MonoBehaviour
{
    /*
    ******** Script Legend ********

    * B *

    Boss - Script controls the boss behavior, including movement, attack patterns, and triggering the next game state.
    Bullet - Controls SFX and particles of bullets.
    
    * C *

    Cockpit - Stores the values and visuals of the cockpit ship module.
    CreatePoepleInALine - Currently unused, but was created to generate a line of gameObjects with exponential scaling and offset to give 2D art a 3D depth look.

    * D *

    Damage - Defines a Damage class that holds a damage value.
    DashboardScoreText - Displays time, money, and obstacles destroyed in the dashboard UI.
    DestryParticles - Detroys the particle system after a countdown.
    DialogueManager - Controls the game story and dialogue, including tutorial style instructions.

    * E *

    EndConditionsUI - Displays the end screen and gives the player a choice to save their score.
    EndPlanet - Moves a planet visual when a level is completed.

    * G *

    GameCountdown - Displays a countdown and triggers game play.
    GameManger - Manages the game state, including starting and ending the game, and managing player stats.
    Goal - Blank. Used so that the gameObject can be found.

    * H *

    Health - Stores and changes a health value for an object.
    HostileShoot - Controls the shooting behavior of hostile objects in the game.
    Hull - Stores and changes the hull value for a space ship.

    * L *

    Loader - Switches between scenes in the game after loading is complete.
    LoaderCallback - Ensures a 1 frame delay for the loading scene to render before calling the LoaderCallback method.
    Loot - Spawns loot when called.

    * M *

    MaiMenuLevelScoreText - Displays saved scores for each level in the main menu.
    MiningMissile - Controls movement, particles, and SFX of mining missiles.
    MiningMissileLauncher - Spawns mining missiles.
    MusicManager - Manages the background music for different scenes in the game.

    * O *

    Obstacles - Controls attributes of obstacles such as appearance, visual cues, point value.
    ObstacleMovement - Controls the movement and movement triggered actions such as SFX, particles, and destruction.
    ObstacleSpawner - Control obstacles spawning.

    * P *

    PauseMenuUI - Controls the pause menu UI.
    PlanetEndPosition - Blank. Used so that the gameObject can be found.
    PlanetEndPosition2 - Blank. Used so that the gameObject can be found.
    PlantetRotate - Rotates a planet visual in a random direction and speed.
    PlayerMovement - Controls the player's movement within 5 lanes.
    PlayerStatsMaager - Centralizes and alters the player's stats.
    ProjectileSpawner - Spawns projectiles.
    ProgressBarUI - Updates the UI progress bars.

    * S *

    ScoreManager - Manages the player's collectable resources for a score output, and saving and loading.
    Scroller - Moves a repeating background.
    SFXManager - Manages sound effects (SFX) in the game.
    ShipUIManager - Controls the ship UI.
    StartPlanet - Moves a planet at the start of a level.

    */
}
