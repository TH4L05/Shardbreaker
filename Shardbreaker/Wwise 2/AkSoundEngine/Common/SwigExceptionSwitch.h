//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////
#ifdef SWIG
// Temporarily disable global exception handling until re-enabled
#define PAUSE_SWIG_EXCEPTIONS %exception {$action}

#define CANCEL_SWIG_EXCEPTIONS(FUNCTION) %exception FUNCTION {$action}

// Enable SWIG exception handling
#define RESUME_SWIG_EXCEPTIONS %exception \
{\
	if (AK::SoundEngine::IsInitialized()) {\
		$action \
	} else {\
		AKPLATFORM::OutputDebugMsg("Wwise warning in $decl: AkInitializer.cs Awake() was not executed yet. Set the Script Execution Order properly so the current call is executed after.");\
		return $null;\
	}\
}

#define SWIG_EXCEPTION(FUNCTION) %exception FUNCTION \
{\
	if (AK::SoundEngine::IsInitialized()) {\
		$action \
	} else {\
		AKPLATFORM::OutputDebugMsg("Wwise warning in $decl: AkInitializer.cs Awake() was not executed yet. Set the Script Execution Order properly so the current call is executed after.");\
		return $null;\
	}\
}

#else
	#define PAUSE_SWIG_EXCEPTIONS
	#define RESUME_SWIG_EXCEPTIONS
#endif // #ifdef SWIG
