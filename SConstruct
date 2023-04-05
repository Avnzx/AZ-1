#!/usr/bin/env python

EnsureSConsVersion(3, 0, 0)
EnsurePythonVersion(3, 6)


env = Environment()
SConscript('Documentation/SConscript')
SConscript('Client/SConscript')
SConscript('Server/SConscript')


env.Alias('docs', 'Documentation/SConscript')