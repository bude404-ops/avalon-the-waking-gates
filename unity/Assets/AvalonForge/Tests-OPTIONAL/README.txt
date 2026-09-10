ForgeTests.cs.txt is DISABLED by default on purpose: it needs the NUnit / Test
Framework package. If your Unity project does NOT have com.unity.test-framework,
compiling it would throw red Console errors and kill the whole AvalonForge menu.

To enable (only if Test Framework is installed via Package Manager):
1. Rename this file to ../Editor/ForgeTests.cs
2. Wait for recompile — then Window > General > Test Runner shows the gates.

You do NOT need this file: Avalon > Forge > Validate Prefabs runs the exact same
gates with zero dependencies, and works headless in batch mode too.
