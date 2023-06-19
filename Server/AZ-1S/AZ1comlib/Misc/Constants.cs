using System;
using Godot;

public static class FrontierConstants {
    public const int chunkSize = 1073741824; // Metres = 1073741824 this is an ENTIRE side length
    // Amount to expand chunk borders, to overlap
    // To make them not flipflop between chunks
    public const double forgiveness = (chunkSize + 2d)/ (chunkSize*2d); // Metres

    public const int maxPlanetSize = 2000000; // diameter
}