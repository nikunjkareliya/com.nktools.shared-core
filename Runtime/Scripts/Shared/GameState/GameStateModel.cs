namespace Shared.Core
{
    public class GameStateModel
    {
        private GameState _currentState;
        public GameState CurrentState { get => _currentState; set => _currentState = value; }

        private GameState _previousState;
        public GameState PreviousState { get => _previousState; set => _previousState = value; }
    }
}