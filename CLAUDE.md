# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

```bash
# Build the project
dotnet build

# Run the application
dotnet run --project Bork

# Clean build artifacts
dotnet clean
```

## Project Overview

Bork is a terminal emulator/UI application built with .NET 9 and SFML.Net for graphics rendering. It renders a grid-based terminal display using the Cascadia Code monospace font.

## Architecture

- **Bork/Program.cs** - Application entry point. Initializes font and Terminal, runs the main loop.
- **Bork/Output/Terminal.cs** - Core rendering class. Manages the SFML RenderWindow, handles events, and renders the terminal grid at 60 FPS. Supports auto-sizing: when `charSize = 0`, calculates optimal font size to fit desktop.
- **Bork/Output/TerminalCell.cs** - Struct representing a single terminal cell with pre-allocated SFML `Text` object.
- **Bork/Input/KeyboardInput.cs** - Keyboard input handling with key repeat support. Tracks state of all keys via `KeyPulse` (true on press + repeats), `KeyHeld` (true while held), and `KeyLastPressTime`. Uses 500ms initial delay and 100ms repeat interval.

## Key Dependencies

- **SFML.Net 3.0.0** - Graphics and window management library
- **.NET 9.0** - Target framework (configured in global.json with latestMinor rollforward)

## Assets

Font files in `Assets/Fonts/` are configured to copy to output directory on build.
