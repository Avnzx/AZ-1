#!/usr/bin/env python
from SCons.Script import EnsureSConsVersion,EnsurePythonVersion,Environment,SConscript


EnsureSConsVersion(3, 0, 0)
EnsurePythonVersion(3, 6)


env = Environment()
docs = SConscript('Documentation/SConscript')
# SConscript('Client/SConscript')
SConscript('Server/SConscript')

env.Alias('everything', ['docs','Server/SConscript'])
