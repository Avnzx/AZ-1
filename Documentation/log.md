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
  - [ ] Voxel planets
- [ ] Graphics
  - [ ] Star cubemap as background
- [ ] Audio 
