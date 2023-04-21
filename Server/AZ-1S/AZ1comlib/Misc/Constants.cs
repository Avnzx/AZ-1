using System;
using Godot;

public static class FrontierConstants {
    public const int chunkSize = 1048576; // Metres
    // Amount to expand chunk borders, to overlap
    // To make them not flipflop between chunks
    public const double forgiveness = (chunkSize + 2d)/ (chunkSize*2d); // Metres
}