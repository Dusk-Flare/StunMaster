Use this template on GitHub or just [download the code](https://github.com/alduris/TemplateMod/archive/refs/heads/master.zip), whichever is easiest.

Rename `src/TestMod.csproj`, then edit `mod/modinfo.json` and `src/Plugin.cs` to customize your mod.

See [the modding wiki](https://rainworldmodding.miraheze.org/wiki/Downpour_Reference/Mod_Directories) for `modinfo.json` documentation.

To update your mod to work in future updates, replace `PUBLIC-Assembly-CSharp.dll` and `HOOKS-Assembly-CSharp.dll` with the equivalents found in `Rain World/BepInEx/utils` and `Rain World/BepInEx/plugins` as well as `Assembly-CSharp-firstpass.dll` found in `Rain World/RainWorld_Data/Managed`.