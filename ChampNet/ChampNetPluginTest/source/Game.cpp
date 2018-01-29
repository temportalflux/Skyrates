#include "Game.h"
#include <cstdlib>

#include <iostream>
#include <cassert>

#include "StateApplication.h"
#include "StateData.h"

// initalize as DNE
Game *Game::gpGame = NULL;

/** Author: Dustin Yost
* A base class for all update loop based game information (runs the game)
*/
Game::Game(StateApplication *state)
{
	if (gpGame != NULL)
	{
		std::cout << "ERROR: Multiple game instances, deleting the old one.\n";
		delete gpGame;
		gpGame = NULL;
	}
	gpGame = this;

	mpState = NULL;
	mpNext = state;
}

Game::Game() : Game(NULL)
{
}

Game::~Game()
{
	if (mpState != NULL)
	{
		delete mpState;
		mpState = NULL;
	}

	// we are the instance, so mark as DNE
	gpGame = NULL;
}

const bool Game::isRunning() const {
	// return true if the state has not yet been activated or while the state is running
	return this->mpState == NULL || this->mpState->isRunning();
}

StateData* Game::getData() const
{
	return (this->mpState != NULL ? this->mpState : this->mpNext)->getData();
}

/** Author: Dustin Yost
* Handles all gameloop updates
*/
void Game::update()
{
	if (mpState != NULL) // DNE on first tick
	{
		this->mpState->updateInput();
		this->mpState->updateNetwork();
		this->mpState->updateGame();
		this->mpState->render();
	}

	if (mpNext != NULL) // not DNE on first tick
	{
		this->goToNextState();
	}
}

void Game::queueState(StateApplication *nextState)
{
	mpNext = nextState;
}

void Game::goToNextState()
{
	// Can only occur if the next state is not DNE
	assert(this->mpNext != NULL);

	// Notify the current state that it is exiting
	if (this->mpState != NULL)
	{
		this->mpState->onExitTo(this->mpNext);
	}
	
	// Notify the next state that it is entering
	// Transfer pertinent data
	this->mpNext->onEnterFrom(this->mpState);

	// If the current state was non-null
	if (this->mpState != NULL)
	{
		// delete the state from memory (all pertinent data has been transferred)
		delete mpState;
		mpState = NULL;
	}

	// next state is now the current state, change variables accordingly
	this->mpState = this->mpNext;
	// Mark next as DNE, the next is now the current
	this->mpNext = NULL;

}
