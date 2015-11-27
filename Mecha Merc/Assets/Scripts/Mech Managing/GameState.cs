using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{

    public enum CurrentLocalGameState
    {
        Offline,
        Online,
        Editor,
    }
	
    public CurrentLocalGameState localGameState;

    /// <summary>
    /// Set the current local game state (Offline,Online,Editor)
    /// </summary>
    /// <param name="state"></param>
    public void SetLocalGameState(CurrentLocalGameState state)
    {
        localGameState = state;
    }

    /// <summary>
    /// Returns the local game state (Offline, Online, Editor)
    /// </summary>
    /// <returns></returns>
    public CurrentLocalGameState GetLocalGameState()
    {
        return localGameState;
    }
    
}
