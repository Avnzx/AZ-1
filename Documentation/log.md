# Journal
## 2022-10-21
  - Think about a game, pulling ideas from games like Space engineers (thematically and graphical dream) , Aurora 4x (for the depth of gameplay), From the depths (for the advanced physics simulations and freedoms)
    - Very little from Aurora, maybe also look into how we can make orbits fun
      - Orbital maneuvers? Like Kerbal Space Program?
  - Aircraft physics
  - In depth weapons systems
  - In depth propulsion systems
    - Fusion torches require power, resources, cooling
    - Possibly implement something like the Elite Frame Shift Drive
      - Exclude witchspace as it gives too many freedoms and would make the project too large
      - Include *supercruise* as it enables travel at speed but still leaves the player vulnerable to encounters and such
        - *Supercruise interceptor?, AI Pulling players out of supercruise? Players pulling players out of supercruise?*
  - Player is free to move like FtD and SE
    - Player does things to kickstart the major processes like SE
    - Maybe make production of things more factorio-like than SE like
    - **TODO**
      - Figure out how to do air sim (Airgear?)
      - Figure out how to do planet orbits *efficiently*
      - Decouple the camera from being a player so we can have TV Screens unlike SE
### Progress report
- Create project and begin a new godot scene
## 2022-10-24
- Should support N-body physics if time allows for such
  - Basic Kepler's laws if unable to do so
## 2022-12-06
- Begin pulling ideas for inventory management systems from SE, Mindustry, and Factorio
  - Find a developer's creation of factorio belts on [github](https://github.com/emeraldpowder/FactorioBelts)






## Solving Problems
  - Large distances from world origin **will** cause issues with FPPE (Float Point Precision Error)
    - Split the world into large clusters (a very small resemblance to MC chunking system but avoid FPPE)
      - [Implementation Details](https://blog.marekrosa.org/2014/12/)


# Progress
- General
  - [ ] Chunking system
  - [ ] Player Movement and Camera
    - [ ] Fall damage / General Velocity Damage
    - [ ] Player movement is limited
      - [ ] If grounded then do not rotate vertically when looking up and down
      - [ ] If not grounded freelook as long as there is ΔV left to consume
        - [ ] Else limit range of look to like ±30°
- [ ] Planets
  - [ ] Planet Gravity
  - [ ] Planet Orbits
    - [ ] Wikipedia [N-Body Simulation](https://en.wikipedia.org/wiki/N-body_simulation)
    - [ ] GitHub [Simulation projects](https://github.com/topics/orbital-simulation)
  - [ ] Voxel planets
- [ ] Graphics
  - [ ] Star cubemap as background
- [ ] Audio
- [ ] AI players
  - [ ] Can pilot ships
  - [ ] Can engage in foot combat w/guns
  - [ ] Can pilot rovers
- [ ] Save Reload System
  - [ ] Save and reload planets
- [ ] Resource Management
  - [ ] Basic transmission of items into and out of blocks

# Useful Resources
  - [nVidia GPU Gems (CUDA TARGET) - Physics Sim Chapter + N-Body](https://developer.nvidia.com/gpugems/gpugems3/part-v-physics-simulation/chapter-31-fast-n-body-simulation-cuda)
  - Simple implementation of [astrophys universe](https://github.com/notakamihe/Unity-Star-Systems-and-Galaxies)
  - Sebastian Lague's [series](https://github.com/SebLague/Procedural-Planets) on procedural planet generation
  
# Creation of documentation (delete after maybe?)
  - [CTAN Animation Package](https://gitlab.com/agrahn/animate)

# Documentation of submodules
## Pipeline (/conveyor) system
- Conveyor networks need to be analysed to find possible paths
  - Single conveyors can be grouped into transport groups, with 2 I/O ports
  - Do something like a layer 3 router to optimise paths? 
  - Time must be taken and the maximum throughput of a path must be determined


# Main ideas
- Make a game with the complexity and shipbuilding of space engineers and factorio
- Make a game with good AI that is legitimately a threat to the player
  - To the level where playing against AI is fun and a constant threat