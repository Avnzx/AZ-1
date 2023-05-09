# AZ-1
A Small space sim game

## Server
The server **must** use a custom version of the godot engine, with Mono and F64 enabled. [This](https://github.com/godotengine/godot/blob/a4131b61b10cb3a7fe0e1e76ed11dfebfa55e7e6/modules/mono/README.md) contains more info. TLDR: pass `precision=double` and `module_mono_enabled = "yes"` in your `custom.py`

- The server will be compiled with a custom compiled version of godot, prefix ARGUMENTS with `gds_` to apply them **to the custom godot version**, these will **not** be added to the SERVER, CLIENT, or DOCS
[]
# Brain Damage (for future development)
- Shared Client/Server code is in the SERVER, symlink is in the CLIENT (target SERVER) to AZ1comlib (AZ-1 common library)


# Remember:

- [Node *owner* != Node *parent*](https://www.reddit.com/r/godot/comments/rgti99/node_parents_vs_owner/)


# Design decisions

- To allow for screens like remote TV's (cameras with remote TV's) each "camera" does not need to have  