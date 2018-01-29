#pragma once
#include <windows.h>

/// \addtogroup server
/// @{

/// A high accuracy timer - uses Large Integer to prevent rollover
/// \author Dean Lawson - Champlain College 2011
class Timer
{

private:
	/// The start time
	LARGE_INTEGER mStartTime;
	/// The end time
	LARGE_INTEGER mEndTime;

	LARGE_INTEGER mTimerFrequency;
	
	/// The time elapsed in ms since the timer was started
	double mElapsedTime;

	/// How fast time is expected to pass (1.0 = 1 ms per 1 ms). The actual time passed is multiplied by this.
	double mFactor;
	
	double mLastFactor;
	
	/// If the timer is paused
	bool mPaused;

	/// Calculates the difference between times in milliseconds
	/// Helper function - uses current frequency for the Timer
	double calcDifferenceInMS(LARGE_INTEGER from, LARGE_INTEGER to) const;


public:
	/// Constructor
	Timer();
	/// Destructor
	~Timer();

	/// Starts the timer
	void start();
	/// Stops the timer
	void stop();

	/// Returns how much time has elapsed since start in milliseconds
	/// @return double
	double getElapsedTime() const;
	/// Holds up the current thread until <code><see cref="mElapsedTime"/> == ms</code>.
	/// @param ms the elapsed time to wait for
	void sleepUntilElapsed( double ms );
	/// Pauses the timer.
	/// Sets <see cref="mPaused"/> to shouldPause.
	void pause( bool shouldPause );
	
	/// Returns the time scale
	/// @return double
	inline double getFactor() const { return mFactor; };
	/// Sets the time scale factor
	/// @param mult the multiplier to the current factor (<code><see cref="mFactor"/> = <see cref="mFactor"/> * mult</code>).
	inline void multFactor( double mult ) { mLastFactor = mFactor; mFactor *= mult; };
	/// Sets the time scale factor
	/// @param theFactor the new time scale factor
	inline void setFactor( double theFactor ) { mLastFactor = mFactor; mFactor = theFactor; };
	/// Sets the time scale factor to the most recent value prior to <see cref="multFactor"/> or <see cref="setFactor"/> being called.
	inline void restoreLastFactor() { mFactor = mLastFactor; };

};

/// @}
