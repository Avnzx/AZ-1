# 3d Space Game
## Client
The client is gamers.

The system does not act to control any hardware.

Solve the problem mainly by creating a client server architecture (to combat fppe, as the server will store global transforms in 64 or 128 bit floats, and the client will use f32's only as GPU's cannot use f64's). This can be done natively via extensions like AVX512, or emulated in software through libraries like GCC's libquadmath. The architecture allows for a multiplayer game by nature. My goal is to implement basic ships that can destroy each other and planet orbital mechanics via N-body simulation, sped up by barnes-hut octrees. For using 64 bit floats godot can be recompiled. If time allows then implement a resource management system (factorio style), and if time allows further then shipbuilding (space engineers) and control systems (from the depths) 
## Approaches and ideas considered / rejected
Considered a style of traditional development, rejected in favor of agile.
Considered using godot engine extensions to be able to modify the physics engine to support f128 on the "server side"
Rejected making the client use higher precision floating point numbers, does not work due to modern hardware limitations. 
Rejected using floating point interval notation to control the amount of error. This is an overly complex solution that slows down major parts of the physics engine and is marginally worth it.

## Programming language
C# (Through Mono), GDScript, C++ (Through GDExtension for high performant code). Using the Godot engine compiled for at minimum f64.









# ESP8266 Power Controller
## Client
The client is people that have grid tie solar power systems, to limit the power draw of specific devices at their house to reduce energy cost by scheduling them when a cost predicition is reached.

Solving the problem is expected to be done by using publicly available API's to access the data from the solar system, as well as weather data to gauge a general measure of if keeping the device on is worth it. This is done on an ESP8266 which is an internet of things microcontroller that is WiFi enabled.
## Approaches and ideas considered / rejected

## Programming language
C++ With platformIO framework
