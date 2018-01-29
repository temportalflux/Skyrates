#ifndef _CHAMPNET_PLUGIN_LIB_H
#define _CHAMPNET_PLUGIN_LIB_H

// check if exporting
#ifdef CHAMPNET_PLUGIN_EXPORT
	// Define the symbol tag for export
	#define CHAMPNET_PLUGIN_SYMTAG __declspec(dllexport)
#else // !CHAMPNET_PLUGIN_EXPORT

	// check if importing
	#ifdef CHAMPNET_PLUGIN_IMPORT
		// Define the symbol tag for import
		#define CHAMPNET_PLUGIN_SYMTAG __declspec(dllimport)
	#else // !CHAMPNET_PLUGIN_IMPORT
		// Define the symbol tag for NULL (compiling)
		#define CHAMPNET_PLUGIN_SYMTAG
	#endif // CHAMPNET_PLUGIN_IMPORT

#endif // CHAMPNET_PLUGIN_EXPORT

#endif // _CHAMPNET_PLUGIN_LIB_H