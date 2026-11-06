# Learn2Slither 🐍

**A Snake game that learns through reinforcement learning.**

Learn2Slither is a C#/.NET 8 implementation of Snake featuring a **Q-learning agent**. The agent trains over repeated game sessions, receives rewards from its environment, and learns which actions to choose. The project also includes a graphical interface, a human-play mode, and command-line options for training and inspecting the agent.

Developed as part of the **42 Learn2Slither** project.

## Features

- **Q-learning AI:** train an agent using a Q-table and an epsilon-greedy action policy.
- **Graphical game:** visualize sessions with SFML.Net, including audio effects.
- **Interactive launch menu:** configure a run without manually entering command-line options.
- **Human mode:** play Snake yourself.
- **Headless training:** run many sessions without rendering the game.
- **Save and load:** persist trained agent state and continue training or evaluate it.
- **Inspection tools:** step-by-step execution, compact state representation and snake-vision display.
- **Adjustable settings:** board size, session count, game speed, and epsilon decay.

## Tech stack

| Component | Technology |
| --- | --- |
| Language | C# |
| Runtime | .NET 8 |
| Graphics and audio | SFML.Net 2.6.0 |
| Learning approach | Tabular Q-learning |

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Platform-compatible native **CSFML/SFML 2.6** libraries required by SFML.Net (graphics, window, system, and audio components, plus their native dependencies).
- A desktop display environment for graphical or human mode. For training without a window, use `-visual off`.

> **Platform note:** Native SFML dependencies differ between Linux, macOS and Windows. The NuGet package restores the managed bindings, but native library loading can still require platform-specific setup. This repository includes `install-dotnet.sh`; inspect its contents before running it.

## Installation

Clone the repository:

```bash
git clone https://github.com/Goschad/Learn2slither.git
cd Learn2slither
```

Restore dependencies and build:

```bash
dotnet restore
dotnet build
```

Start the graphical menu:

```bash
dotnet run
```

Running with **no arguments** opens the graphical lobby. Passing command-line arguments configures the run directly.

## Usage

### Train an agent without graphics

```bash
dotnet run -- -sessions 1000 -visual off -save models/training.txt
```

### Watch the AI play

```bash
dotnet run -- -sessions 10 -visual on
```

### Load a trained model without learning

```bash
dotnet run -- -load models/training.txt -dontlearn -sessions 5 -visual on
```

### Inspect the agent one step at a time

```bash
dotnet run -- -sessions 1 -visual on -step-by-step
```

### Play in human mode

```bash
dotnet run -- -human -visual on
```

### Change the board size

```bash
dotnet run -- -sessions 100 -size 15 -visual off
```

### Command-line options

| Option | Purpose |
| --- | --- |
| `-sessions N` | Number of game sessions |
| `-size N` | Board width and height |
| `-visual on\|off` | Enable or disable the graphical window |
| `-save PATH` | Save the learned agent state |
| `-load PATH` | Load an existing agent state |
| `-dontlearn` | Run without updating the agent's learned values |
| `-step-by-step` | Advance the visualized agent session one move at a time |
| `-state-vision` | Display the agent's vision/state information |
| `-human` | Control the snake yourself |
| `-sans` | Enable the alternative presentation mode |
| `-speed VALUE` | Set the graphical game speed |

The launch menu also exposes **epsilon decay** as a configurable setting. The program defaults to a 10×10 board, one session, a 150 ms step interval, and an epsilon decay factor of 0.995.

**Important option restrictions:**
- Human mode requires graphics and is incompatible with agent-specific training/inspection flags and model loading or saving.
- Step-by-step mode requires graphics.
- `-dontlearn` cannot be combined with `-save`.
- Running without visualization is intended for faster batch training.

## How Q-learning works

At each move, the agent:

1. Observes the current board through a compact state representation.
2. Chooses an action, balancing **exploration** (trying an action) and **exploitation** (using previously learned values).
3. Applies the action to the Snake environment.
4. Receives a reward and observes the next state.
5. Updates the Q-value associated with the previous state and action.

The standard Q-learning update is:

$$
Q(s,a) \leftarrow Q(s,a) + \alpha\left[r + \gamma\max_{a'}Q(s',a') - Q(s,a)\right]
$$

Where:

- `s`, `a`: previous state and action
- `r`: reward after performing the action
- `s'`: new state
- `α` (alpha): learning rate
- `γ` (gamma): discount factor for future rewards

An **epsilon-greedy** policy allows the agent to explore at the beginning and gradually rely more on the Q-table as training progresses. The `-dontlearn` option disables Q-value updates for evaluation.

## Project layout

```text
Learn2slither/
├── assets/                 # Graphics and audio resources
├── src/                    # Game, agent, display and support code
├── Program.cs              # Entry point, run modes and session loop
├── Learn2Slither.csproj    # .NET configuration and SFML.Net dependency
├── install-dotnet.sh       # .NET installation helper script
├── .gitignore
└── README.md
```

In the source code, components are organized around the game board, the agent and its state representation, the SFML display, audio and argument/menu handling.

## Training notes

- Headless training reports the **average snake length over the last 100 sessions** at regular intervals.
- Training outcomes can vary between runs because action selection and game events involve randomness.
- A saved model is useful for separating the learning phase from later demonstrations.
- Make sure the supplied model path exists before using `-load`.

## Authors

Project repository: [Goschad/Learn2slither](https://github.com/Goschad/Learn2slither)

---

*Built for learning and experimenting with reinforcement learning through Snake.*
