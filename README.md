# Proximity
This solution consists of the following projects:
1. Domain
2. Tests
3. App

Domain - is a folder with core classes (Game, Cell, some custom exception etc.) that represent the logic of this game.
Domain is not dependent on any other parts, instead other parts depend on domain.

Domain's core logic is covered with tests (they are in the Tests folder).

App - it contains a starting point of an application - Program class. It also contains a View class - in which the UI (pseudo-graphics in Windows console) is implemented.
