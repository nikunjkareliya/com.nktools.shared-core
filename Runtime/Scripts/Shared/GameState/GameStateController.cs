using System.Collections;
using UnityEngine;

namespace Shared.Core
{
    public class GameStateController : BaseController
    {
        [SerializeField] private GameState _initialState;

        [SerializeField] private GameState _currentState;
        public GameState CurrentState => _currentState;
        private GameState _previousState;

        private GameStateModel _gameStateModel;

        protected override void Init()
        {            
            _gameStateModel = ModelStore.Get<GameStateModel>();
        }

        protected override void Register()
        {            
            SharedEvents.OnGameStateChanged.Register(ChangeState);
        }

        protected override void Deregister()
        {            
            SharedEvents.OnGameStateChanged.Unregister(ChangeState);
        }
        
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            ChangeState(_initialState);
        }

        private void ChangeState(GameState gameState)
        {            
            // Catch previous state before changing to new state
            _previousState = _currentState;
            _gameStateModel.PreviousState = _currentState;

            SharedEvents.OnGameStateExited.Execute(gameState);                
            
            _currentState = gameState;
            _gameStateModel.CurrentState = gameState;

            SharedEvents.OnGameStateEntered.Execute(gameState);            
            SharedEvents.OnPanelShow.Execute(gameState);            
            Debug.Log($"<color=cyan>STATE CHANGED -> {gameState}</color>");
        }

        /*
        private GameStateModel GetGameStateModel()
        {
            GameStateModel gameStateModel = ModelStore.Get<GameStateModel>();

            if (gameStateModel == null)
            {
                gameStateModel = new GameStateModel();
                ModelStore.Register<GameStateModel>(gameStateModel);
            }

            return gameStateModel;
        }
        */

        // ---- FOR TESTING PURPOSE ONLY ----
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SharedEvents.OnGameStateChanged.Execute(GameState.Home);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SharedEvents.OnGameStateChanged.Execute(GameState.Gameplay);                
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"Current State -> {_currentState}");
            }
        }
    }
}