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

Bork is an experimental text game (imagine Zork meeting Nethack meeting Doom) played in a terminal emulator/UI, built with .NET 9 and SFML.Net for rendering. It renders a grid-based terminal display using the Cascadia Code monospace font.

## Architecture

- **Bork/Program.cs** - Application entry point. Initializes font and Terminal, runs the main loop.
- **Bork/Output/Terminal.cs** - Core rendering class. Manages the SFML RenderWindow, handles events, and renders the terminal grid at 60 FPS. Supports auto-sizing: when `charSize = 0`, calculates optimal font size to fit desktop. Provides text output API:
  - `SetColor(Color)` - sets foreground color for subsequent operations
  - `Locate(col, row)` - sets cursor position (clamped to bounds)
  - `Clear()` - fills terminal with spaces using current color, resets cursor to (0,0)
  - `Print(string)` - prints text at cursor with `\n` and auto-wrap support
  - `Cols`, `Rows` - public properties for terminal dimensions
  - `CursorVisible` - show/hide blinking cursor (default: true)
  - `CursorSize` - cursor height as fraction of cell (default: 0.2)
  - Cursor blinks at 500ms intervals, always white color
- **Bork/Output/TerminalCell.cs** - Struct representing a single terminal cell with pre-allocated SFML `Text` object. Provides `Set(char, Color)`, `SetCharacter(char)`, and `SetColor(Color)` methods for modifying cell content.
- **Bork/Input/KeyboardInput.cs** - Keyboard input handling with key repeat support. Tracks state of all keys via `KeyPulse` (true on press + repeats), `KeyHeld` (true while held), and `KeyLastPressTime`. Uses 500ms initial delay and 100ms repeat interval.
- **Bork/Input/Prompt.cs** - Command input line rendered in the last terminal row. Features:
  - Green ">>" prefix, white text, blinking cursor
  - Full text editing: Left/Right arrows, Home/End, Backspace/Delete, insert mode
  - History navigation: Up/Down arrows cycle through previous commands
  - Hints system (autocomplete):
    - `AddHint(string)` - register a hint (other classes call this)
    - `ClearHints()` - called automatically on Enter
    - Matching hint completion shown in dark gray after cursor
    - TAB accepts the current hint
    - Hints stored in `SortedSet<string>` (alphabetically sorted, case-insensitive)
  - `OnSubmit` event fired when Enter pressed with non-empty text
  - Text input limited to terminal width minus prefix length
- **Bork/Core/Game.cs** - Main game class. Holds Terminal, KeyboardInput, Prompt, and World. Game loop calls `Update()` then `Render()`. Routes commands to World, manages Progress timing (1s ticks, paused 5s after input), displays text output buffer.

## World System (Component Tree)

The game world is a tree of components. Everything is a component - areas, player, items, exits. Position in tree defines relationships (siblings = same location, children = contained within).

- **Bork/World/Component.cs** - Abstract base class for all game objects:
  - Tree structure: `Parent`, `Children`, `AddChild()`, `RemoveChild()`, `MoveTo()`
  - Identity: `Name`, `Description`
  - Event handlers: `OnCommand(string) -> string?`, `OnProgress(int) -> string?`
  - Traversal helpers: `Siblings`, `FindChild<T>()`, `FindChildren<T>()`, `FindDescendant<T>()`, `Root`
- **Bork/World/World.cs** - Root component, orchestrates command/progress propagation:
  - `ProcessCommand(string)` - broadcasts command to all components, collects responses
  - `ProcessProgress(int)` - broadcasts turn tick to all components
  - `GetCommandHints()` - returns dynamic hints based on current game state
- **Bork/World/Area.cs** - A location containing other components:
  - `GetFullDescription()` - formats name, description, visible items, exits
- **Bork/World/Player.cs** - The player component:
  - `CurrentArea` - parent Area
  - Handles: `look`, `l`, `look at <x>`, `examine <x>`
- **Bork/World/Exit.cs** - Connection between areas:
  - `Direction` (north, south, etc.), `Target` Area
  - Handles: `go <dir>`, `<dir>`, direction aliases (n/s/e/w)
  - Moves Player to Target Area when activated

### Command Flow
```
User input -> Prompt.OnSubmit -> Game.OnPromptSubmit -> World.ProcessCommand
  -> recursive OnCommand on all components -> responses collected -> displayed
```

### Two Triggers
1. **Command** - when player submits text, all components receive it via `OnCommand()`
2. **Progress** - every 1 second (paused 5s after typing), all components receive turn via `OnProgress()`

## Key Dependencies

- **SFML.Net 3.0.0** - Graphics and window management library
- **.NET 9.0** - Target framework (configured in global.json with latestMinor rollforward)

## Assets

Font files in `Assets/Fonts/` are configured to copy to output directory on build.
