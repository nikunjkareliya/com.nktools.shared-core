namespace Shared.Core
{
    public static class SharedEvents
    {
        // GameState
        public static readonly GameEvent<GameState> GameStateChanged = new GameEvent<GameState>();
        public static readonly GameEvent<GameState> GameStateEntered = new GameEvent<GameState>();
        public static readonly GameEvent<GameState> GameStateExited = new GameEvent<GameState>();

        // Panels        
        public static readonly GameEvent<BasePanel> PanelRegistered = new GameEvent<BasePanel>();
        public static readonly GameEvent<GameState> PanelShow = new GameEvent<GameState>();
    }
}