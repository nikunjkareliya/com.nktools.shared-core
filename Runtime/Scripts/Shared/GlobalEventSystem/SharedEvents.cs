namespace Shared.Core
{
    public static class SharedEvents
    {
        // GameState
        public static readonly GameEvent<GameState> OnGameStateChanged = new GameEvent<GameState>();
        public static readonly GameEvent<GameState> OnGameStateEntered = new GameEvent<GameState>();
        public static readonly GameEvent<GameState> OnGameStateExited = new GameEvent<GameState>();

        // Panels        
        public static readonly GameEvent<BasePanel> OnPanelRegistered = new GameEvent<BasePanel>();
        public static readonly GameEvent<GameState> OnPanelShow = new GameEvent<GameState>();
    }
}