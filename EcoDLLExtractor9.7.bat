@echo off
::Batch File extracts ECO DLL from the ECO Repo Folder

::Core File Locations, Don't change these
set core=\Server\Eco.Core\bin\Release\net6.0\Eco.Core.dll
set gameplay=\Server\Eco.Gameplay\bin\Release\net6.0-windows\Eco.Gameplay.dll
set modkit=\Server\Eco.Modkit\bin\Release\net6.0-windows\Eco.Modkit.dll
set mods=\Server\Eco.Mods\bin\Release\net6.0-windows\Eco.Mods.dll
set enet=\Server\Eco.Networking.Enet\bin\Release\net6.0\Eco.Networking.Enet.dll
set plugins=\Server\Eco.Plugins\bin\Release\net6.0-windows\Eco.Plugins.dll
set shared=\Server\Eco.Shared\bin\Release\net6.0\Eco.Shared.dll
set simulation=\Server\Eco.Simulation\bin\Release\net6.0-windows\Eco.Simulation.dll
set stats=\Server\Eco.Stats\bin\Release\net6.0\Eco.Stats.dll
set web=\Server\Eco.WebServer\bin\Release\net6.0-windows\Eco.WebServer.dll
set world=\Server\Eco.World\bin\Release\net6.0\Eco.World.dll
set worldgen=\Server\Eco.WorldGenerator\bin\Release\net6.0-windows\Eco.WorldGenerator.dll
set forms=\Server\Eco.WindowsForms\bin\Release\net6.0-windows\Eco.WindowsForms.dll
set graph=\Server\NodeGraphControl\bin\Release\net6.0-windows\NodeGraphControl.dll
set server=\Server\bin\Release\net6.0-windows\EcoServer.dll

::Replace this line with the location of the Eco Repo
set path=D:\Github\Eco.Develop

::Replace this line with the destination
set dest=%cd%\dlls

::Extract Files
@echo on
copy "%path%%core%" "%dest%" /y
copy "%path%%gameplay%" "%dest%" /y
copy "%path%%modkit%" "%dest%" /y
copy "%path%%mods%" "%dest%" /y
copy "%path%%enet%" "%dest%" /y
copy "%path%%plugins%" "%dest%" /y
copy "%path%%shared%" "%dest%" /y
copy "%path%%simulation%" "%dest%" /y
copy "%path%%stats%" "%dest%" /y
copy "%path%%web%" "%dest%" /y
copy "%path%%world%" "%dest%" /y
copy "%path%%worldgen%" "%dest%" /y
copy "%path%%forms%" "%dest%" /y
copy "%path%%graph%" "%dest%" /y
copy "%path%%server%" "%dest%" /y

pause
