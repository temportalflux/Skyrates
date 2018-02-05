#pragma once

#include "Timer.h"

#include <map>
#include <string>

/// \addtogroup server
/// @{

/// A timer tracker - tracks concurrent execution times
/// \author Dean Lawson - Champlain College 2011
class PerformanceTracker
{

private:

	/// The map of times, from trackerName key to <see cref="Timer"\>
	std::map<std::string, Timer*> mTimers;

public:

	/// Constructor
	PerformanceTracker();
	/// Destructor
	~PerformanceTracker();

	/// Begins timer for some key
	/// @param trackerName the key of the timer
	void startTracking( const std::string& trackerName );
	/// Stops timer at some key
	/// @param trackerName the key of the timer
	void stopTracking( const std::string& trackerName );
	/// Returns the time in ms since the timer was started
	/// @param trackerName the key of the timer
	double getElapsedTime( const std::string& trackerName );
	/// Removes a timer for some key
	/// @param trackerName the key of the timer
	void removeTracker( const std::string& trackerName );
	/// Clears a timer (sets elapsed time to 0 and restarts)
	/// @param trackerName the key of the timer
	void clearTracker( const std::string& trackerName );

};

/// @}
