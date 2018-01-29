#ifndef CHAMPNET_SERVER_GAME_H
#define CHAMPNET_SERVER_GAME_H

class StateApplication;
struct StateData;

/// \addtogroup server
/// @{

/// A base class for all update loop based game information (runs the game)
/// \author Dustin Yost
class Game
{

protected:

	/// The current application state of the game
	StateApplication *mpState;
	/// The next application state, if applicable
	StateApplication *mpNext;

public:
	
	/// The static singleton instance of the game
	static Game *gpGame;

	/// Constructor
	/// @param state the beginning state
	Game(StateApplication *state);
	/// Constructor without state
	Game();
	/// Destructor
	virtual ~Game();

	/// Returns if the game is presently in a running state
	const bool isRunning() const;

	/// Returns the system data
	StateData* getData() const;

	/// Handles all gameloop updates
	void update();

	/// Sets a state to be transitioned to at the end of the current game loop
	void queueState(StateApplication *nextState);

	/// Switches states such that <code><see cref="mpState"/> = <see cref="mpNext"/></code>, and <code><see cref="mpNext"/> = null</code>
	void goToNextState();

};

/// @}

#endif // CHAMPNET_SERVER_GAME_H