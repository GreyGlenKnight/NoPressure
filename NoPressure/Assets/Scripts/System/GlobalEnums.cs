using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectType
{
    None,
    Damage,
    GivePressure,
}

public enum WorldSpaceUnit
{
    Tile, //Smallest unit == 1 Unity Unit
    Sector, // 10x10 Tiles 1/16 of a file
    MpaNode, // 1 File 40x40 Tiles
}

public enum FloorTileType
{
    Blank = 0,
    Broken = 1,
    Wired = 2,
    WiredAndBroken = 3,
}

public enum SpawnType
{
    None = 0,
    Wall = 1,

    Enemy = 3,
    Obstacle = 4,
    ForceField = 5,

    Pistol = 20,
    Rifle = 21,
    Carbine = 22,
    PlasmaThrower = 23,
    RocketLauncher = 24,

    Mine = 30,

    Shield = 40,

    MecanicalTools = 50,
    ElectricalTools = 51,

    PortableGenerator = 70,
    PortablePressure = 71,

    PowerCube = 96,
    PressureStation = 97,
    BrokenPressureStation = 98,


};