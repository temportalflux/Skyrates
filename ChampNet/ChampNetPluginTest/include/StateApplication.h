#ifndef CHAMPNET_SERVER_STATE_APPLICATION_H
#define CHAMPNET_SERVER_STATE_APPLICATION_H

struct StateData;

/// The information state - handles all persistent data
class StateApplication
{

private:

	// Should the loop continue
	bool mIsRunning;

protected:

	StateData *mpState;

public:
	StateApplication();
	virtual ~StateApplication();

	inline const void stopRunning() { mIsRunning = false; }
	inline const bool isRunning() const { return mIsRunning; }

	StateData* getData() const;

	virtual void onEnterFrom(StateApplication *previous);

	virtual void onExitTo(StateApplication *next);

	/** Author: Dustin Yost
	* Collects all input changes
	*/
	virtual void updateInput();

	/** Updates the network | STUB */
	virtual void updateNetwork();

	/** Author: Dustin Yost
	* Handles all game updates and responses to input/network updates
	*/
	virtual void updateGame();

	/** Renders the game to the display | STUB */
	virtual void render();

	/** Author: Dustin Yost
	* Updates states according to input updates
	*/
	virtual void updateGameForInput(const char *current, const char *previous);

	/** Author: Dustin Yost
	* Called when a key is marked as down this update
	*/
	virtual void onKeyDown(int i);

};

#endif // CHAMPNET_SERVER_STATE_APPLICATION_H