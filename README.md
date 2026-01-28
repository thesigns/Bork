# Bork

A text adventure game engine inspired by Zork and MUDs, built with .NET 9 and SFML.Net.

## Features

- Terminal-based UI with Cascadia Code font
- Component tree architecture - the entire game world is a hierarchy of components
- Command parser with autocomplete hints
- Real-time progress system (turns tick every second when idle)

## Building and Running

```bash
dotnet build
dotnet run --project Bork
```

## Architecture

The game world is a tree of components where position defines relationships:
- Siblings are in the same location
- Children are contained within their parent
- Player moves by changing position in the tree

### Commands

- `look` / `l` - describe current location
- `go <direction>` / `<direction>` / `n/s/e/w` - move to another area
- `examine <object>` / `look at <object>` - inspect something

## Requirements

- .NET 9.0
- SFML.Net 3.0.0
