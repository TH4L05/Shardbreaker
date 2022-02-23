//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

%module AkSoundEngine	//Names the C# namespace.  Must be the same thing as the name of the DLL.
%csconst(1);		//Generate static const variables in C# for all defines and const variables seen on the C++ side.

%include <wchar.i>				//Treat wchar* as strings in C#.
%include <arrays_csharp.i>
%include <typemaps.i>

//Remove warning about duplicate functions because of char and wchar use in the interface.
#pragma SWIG nowarn=516
#pragma SWIG nowarn=844
#define static_assert(...)

// Disable warnings that appear in the SWIG generated code
#if defined (AK_ANDROID)
%{
#pragma GCC diagnostic ignored "-Wuninitialized"
#pragma GCC diagnostic ignored "-Wmaybe-uninitialized"
%}
#elif defined(AK_XBOXONE)
%{
// Disable "unreferenced formal parameter" (4100) and "potentially uninitialized" (4701) warnings.
#pragma warning(disable: 4100)
#pragma warning(disable: 4701)
%}
#elif defined (AK_NX)
%{
#pragma GCC diagnostic ignored "-Wsometimes-uninitialized"
%}
#endif // #if defined (AK_NX)


%ignore AkEmitterListenerPair;
%ignore AkPolarCoord;
%ignore AkSphericalCoord;
%ignore AkListener;
%ignore AkSourcePosition;

/* Non primitive types */
%typemap(ctype) SWIGTYPE "void *"
%typemap(imtype, out="global::System.IntPtr") SWIGTYPE "global::System.IntPtr"
%typemap(cstype) SWIGTYPE "$&csclassname"

%typemap(ctype) SWIGTYPE [] "void *"
%typemap(imtype, out="global::System.IntPtr") SWIGTYPE [] "global::System.IntPtr"
%typemap(cstype) SWIGTYPE [] "$csclassname"

%typemap(ctype) SWIGTYPE * "void *"
%typemap(imtype, out="global::System.IntPtr") SWIGTYPE * "global::System.IntPtr"
%typemap(cstype) SWIGTYPE * "$csclassname"

%typemap(ctype) SWIGTYPE & "void *"
%typemap(imtype, out="global::System.IntPtr") SWIGTYPE & "global::System.IntPtr"
%typemap(cstype) SWIGTYPE & "$csclassname"

/* Default handling. Object passed by value. Convert to a pointer */
%typemap(in, canthrow=1) SWIGTYPE ($&1_type argp)
%{ argp = ($&1_ltype)$input; 
   if (!argp) {
     return $null;
   }
   $1 = *argp; %}

%typemap(directorout) SWIGTYPE
%{ if (!$input) {
     return $null;
   }
   $result = *($&1_ltype)$input; %}

%typemap(in, canthrow=1) SWIGTYPE & %{ $1 = ($1_ltype)$input;
  if (!$1) {
    return $null;
  } %}

// Proxy classes (base classes, ie, not derived classes)
%typemap(csbody) SWIGTYPE %{
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr($csclassname obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }
%}

// Derived proxy classes
%typemap(csbody_derived) SWIGTYPE %{
  private global::System.IntPtr swigCPtr;

  internal $csclassname(global::System.IntPtr cPtr, bool cMemoryOwn) : base($imclassname.$csclazznameSWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr($csclassname obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal override void setCPtr(global::System.IntPtr cPtr) {
    base.setCPtr($imclassname.$csclazznameSWIGUpcast(cPtr));
    swigCPtr = cPtr;
  }
%}

// Typewrapper classes
%typemap(csbody) SWIGTYPE *, SWIGTYPE &, SWIGTYPE [] %{
  private global::System.IntPtr swigCPtr;

  internal $csclassname(global::System.IntPtr cPtr, bool futureUse) {
    swigCPtr = cPtr;
  }

  protected $csclassname() {
    swigCPtr = global::System.IntPtr.Zero;
  }

  internal static global::System.IntPtr getCPtr($csclassname obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }
%}

%typemap(directorout, warning=SWIGWARN_TYPEMAP_DIRECTOROUT_PTR_MSG) SWIGTYPE &
%{ if (!$input) {
     return $null;
   }
   $result = ($1_ltype)$input; %}

%typemap(csconstruct,directorconnect="\n    SwigDirectorConnect();") SWIGTYPE %{: this($imcall, true) {$directorconnect
  }
%}

%typemap(csdestruct, methodname="Dispose", methodmodifiers="public") SWIGTYPE {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          $imcall;
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

%typemap(csdestruct_derived, methodname="Dispose", methodmodifiers="public") SWIGTYPE {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          $imcall;
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
      base.Dispose();
    }
  }

%typemap(csout) bool,               const bool &,
                char,               const char &,
				signed char,        const signed char &,
				unsigned char,      const unsigned char &,
				short,              const short &,
				unsigned short,     const unsigned short &,
				int,                const int &,
				unsigned int,       const unsigned int &,
				long,               const long &,
				unsigned long,      const unsigned long &,
				long long,          const long long &,
				unsigned long long, const unsigned long long &,
				float,              const float &,
				double,             const double &,
				char *, char *&, char[ANY], char[]
{ return $imcall; }

%typemap(csout) void { $imcall; }

%typemap(csout) enum SWIGTYPE { return ($csclassname)$imcall; }

%typemap(csout) const enum SWIGTYPE & { return ($*csclassname)$imcall; }

%typemap(csout) SWIGTYPE {
    $&csclassname ret = new $&csclassname($imcall, true);
    return ret;
  }
%typemap(csout) SWIGTYPE & {
    $csclassname ret = new $csclassname($imcall, $owner);
    return ret;
  }
%typemap(csout) SWIGTYPE *, SWIGTYPE [] {
    global::System.IntPtr cPtr = $imcall;
    $csclassname ret = (cPtr == global::System.IntPtr.Zero) ? null : new $csclassname(cPtr, $owner);
    return ret;
  }
%typemap(csout) SWIGTYPE (CLASS::*) {
    string cMemberPtr = $imcall;
    $csclassname ret = (cMemberPtr == null) ? null : new $csclassname(cMemberPtr, $owner);
    return ret;
  }


/* Properties */
%typemap(csvarin) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*),
                  char *, char *&, char[ANY], char[]
%{ set { $imcall; } %}

%typemap(csvarout) bool,               const bool &,
                   char,               const char &,
				   signed char,        const signed char &,
				   unsigned char,      const unsigned char &,
				   short,              const short &,
				   unsigned short,     const unsigned short &,
				   int,                const int &,
				   unsigned int,       const unsigned int &,
				   long,               const long &,
				   unsigned long,      const unsigned long &,
				   long long,          const long long &,
				   unsigned long long, const unsigned long long &,
				   float,              const float &,
				   double,             const double &,
				   char *, char *&, char[ANY], char[]
%{ get { return $imcall; } %}

%typemap(csvarout) void %{ get { $imcall; } %}

%typemap(csvarout) const enum SWIGTYPE & %{ get { return ($*csclassname)$imcall; } %}

%typemap(csvarout) enum SWIGTYPE %{ get { return ($csclassname)$imcall; } %}

%typemap(csvarout) SWIGTYPE %{
    get {
      $&csclassname ret = new $&csclassname($imcall, true);
      return ret;
    } %}
%typemap(csvarout) SWIGTYPE & %{
    get {
      $csclassname ret = new $csclassname($imcall, $owner);
      return ret;
    } %}
%typemap(csvarout) SWIGTYPE *, SWIGTYPE [] %{
    get {
      global::System.IntPtr cPtr = $imcall;
      $csclassname ret = (cPtr == global::System.IntPtr.Zero) ? null : new $csclassname(cPtr, $owner);
      return ret;
    } %}

%typemap(csvarout) SWIGTYPE (CLASS::*) %{
    get {
      string cMemberPtr = $imcall;
      $csclassname ret = (cMemberPtr == null) ? null : new $csclassname(cMemberPtr, $owner);
      return ret;
    } %}

/* Pointer reference typemaps */
%typemap(csout) SWIGTYPE *const& {
    global::System.IntPtr cPtr = $imcall;
    $*csclassname ret = (cPtr == global::System.IntPtr.Zero) ? null : new $*csclassname(cPtr, $owner);
    return ret;
  }


//This typemap will map the "void*" to a global::System.IntPtr on the C# side.
// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>
	%typemap(ctype) void * "void *"
	%typemap(imtype) void * "global::System.IntPtr"
	%typemap(cstype) void * "global::System.IntPtr"
	%typemap(csin) void * "$csinput"
	%typemap(in) void * %{ $1 = $input; %}
	%typemap(out) void * %{ $result = $1; %}
	%typemap(csout) void * { return $imcall; }
	%typemap(csvarin)  void* %{ set { $imcall; } %}
	%typemap(csvarout)  void* %{ get { return $imcall; }%}

	%typemap(ctype) void** "void**"
	%typemap(imtype) void** "global::System.IntPtr[]"
	%typemap(cstype) void** "global::System.IntPtr[]"
	%typemap(csin) void** "$csinput"
	%typemap(in) void** %{ $1 = $input; %}
	%typemap(out) void** %{ $result = $1; %}
	%typemap(csout) void** { return $imcall; }
	%typemap(csvarin)  void** %{ set { $imcall; } %}
	%typemap(csvarout)  void** %{ get { return $imcall; }%}

//This typemap will map the "AkUInt8*" to a global::System.IntPtr on the C# side.
// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>
	%typemap(ctype) AkUInt8 * "AkUInt8 *"
	%typemap(imtype) AkUInt8 * "global::System.IntPtr"
	%typemap(cstype) AkUInt8 * "global::System.IntPtr"
	%typemap(csin) AkUInt8 * "$csinput"
	%typemap(in) AkUInt8 * %{ $1 = $input; %}
	%typemap(out) AkUInt8 * %{ $result = $1; %}
	%typemap(csout) AkUInt8 * { return $imcall; }
	%typemap(csvarin)  AkUInt8* %{ set { $imcall; } %}
	%typemap(csvarout)  AkUInt8* %{ get { return $imcall; }%}
	
// Order: Must be before %include <AK/SoundEngine/Platforms/Windows/AkTypes.h>, for e.g., _w64
#if defined(AK_WIN) || defined(AK_XBOX)
	%include <windows.i>
#endif // #if defined(AK_WIN) || defined(AK_XBOX)

%ignore AkCodecDescriptor;
%ignore AkCreateFileSourceCallback;
%ignore AkCreateBankSourceCallback;
%ignore AkCreateFileCodecCallback;
%ignore AkCreateGrainCodecCallback;

// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>
#if defined( AK_XBOXONE )
	%include <AK/SoundEngine/Platforms/XBoxOne/AkTypes.h>
#elif defined( AK_WIN )
	%include <AK/SoundEngine/Platforms/Windows/AkTypes.h>
#if defined (AK_WIN_UNIVERSAL_APP)
	// Ripped from winnt.h in the Windows 10 SDK, in order to avoid SWIGTYPE when building AkThreadProperties struct.
	typedef struct _PROCESSOR_NUMBER {
		WORD   Group;
		BYTE  Number;
		BYTE  Reserved;
	} PROCESSOR_NUMBER;
#endif
#elif defined (AK_XBOXGC)
	%include <AK/SoundEngine/Platforms/XboxGC/AkTypes.h>
#elif defined( AK_APPLE )
	%import <sys/cdefs.h>
	%import <sys/_types/_int8_t.h>
	%import <sys/_types/_int16_t.h>
	%import <sys/_types/_int32_t.h>
	%import <sys/_types/_int64_t.h>
	%import <sys/_types/_intptr_t.h>
	%import <sys/_types/_uintptr_t.h>
	%import <_types/_uint8_t.h>
	%import <_types/_uint16_t.h>
	%import <_types/_uint32_t.h>
	%import <_types/_uint64_t.h>
	%import <sys/_types.h>
	%import <sys/types.h>
	%import <stdint.h>
	%import <machine/types.h>
	%include <AK/SoundEngine/Platforms/Mac/AkTypes.h>
	%include <AK/SoundEngine/Platforms/POSIX/AkTypes.h>
#elif defined ( AK_PS5 )
	%include <AK/SoundEngine/Platforms/PS5/AkTypes.h>
#elif defined ( AK_PS4 )
	%include <AK/SoundEngine/Platforms/PS4/AkTypes.h>
#elif defined( AK_ANDROID )
	#define	__signed signed
	%import <sys/types.h>
	%import <stdint.h>
	%include <AK/SoundEngine/Platforms/Android/AkTypes.h>
	%include <AK/SoundEngine/Platforms/POSIX/AkTypes.h>
#elif defined( AK_LINUX )
	%include <stdint.i>
	%include <AK/SoundEngine/Platforms/POSIX/AkTypes.h>
	%include <AK/SoundEngine/Platforms/Linux/AkTypes.h>
	#undef AK_SUPPORT_WCHAR
#elif defined( AK_NX )
	// The following defines are taken from stdint.h
	#define __NEED_int8_t
	#define __NEED_int16_t
	#define __NEED_int32_t
	#define __NEED_int64_t

	#define __NEED_uint8_t
	#define __NEED_uint16_t
	#define __NEED_uint32_t
	#define __NEED_uint64_t

	#define __NEED_intptr_t
	#define __NEED_uintptr_t

	#define __NEED_intmax_t
	#define __NEED_uintmax_t

	%include <bits/alltypes.h>

	// For Bit64
	%import <nn/nn_BitTypes.h>

	%ignore AkThread;
	%include <AK/SoundEngine/Platforms/NX/AkTypes.h>
	#undef AK_SUPPORT_WCHAR
#endif

#if defined ( AK_SONY ) || defined( AK_WIN )
typedef signed char int8_t;
typedef short int16_t;
typedef int int32_t;
typedef long long int64_t;
typedef unsigned char uint8_t;
typedef unsigned short uint16_t;
typedef unsigned int uint32_t;
typedef unsigned long long uint64_t;
#elif defined(AK_NX)
%typemap(ctype) AkUInt64 "AkUInt64"
%typemap(imtype) AkUInt64 "ulong"
%typemap(cstype) AkUInt64 "ulong"
%typemap(in) AkUInt64 %{ $1 = $input; %}
%typemap(out) AkUInt64 %{ $result = $1; %}
%typemap(csout) AkUInt64 { return $imcall; }
%typemap(csvarin)  AkUInt64 %{ set { $imcall; } %}
%typemap(csvarout)  AkUInt64 %{ get { return $imcall; } %}
#endif


// Char/Strings convertion methods
// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>

// char
%typemap(out) char*, char[ANY], char[] %{ $result = $1; %}
%typemap(imtype,
	out="global::System.IntPtr",
	inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]") char*, char[ANY], char[] "string"

%typemap(csin) char*, char[ANY], char[] "$csinput"
%typemap(csout) char*, char[ANY], char[] { return AkSoundEngine.StringFromIntPtrString($imcall); }

%typemap(csvarin) char*, char[ANY], char[] %{ set { $imcall; } %}
%typemap(csvarout) char*, char[ANY], char[] %{ get { return AkSoundEngine.StringFromIntPtrString($imcall); } %}

// wchar_t
%typemap(ctype) wchar_t*, wchar_t[ANY], wchar_t[] "wchar_t*"
%typemap(imtype,
	out="global::System.IntPtr",
	inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]") wchar_t*, wchar_t[ANY], wchar_t[] "string"

// Apple platform wchar_t is encoded with UTF-32, while Windows uses UTF-16.
#ifdef __APPLE__
	%typemap(in) wchar_t*, wchar_t[ANY], wchar_t[] %{ CONVERT_UTF16_TO_WCHAR($input, $1); %}
	%typemap(out) wchar_t*, wchar_t[ANY], wchar_t[] %{ CONVERT_WCHAR_TO_UTF16($1, $result); %}
#else
	%typemap(in) wchar_t*, wchar_t[ANY], wchar_t[] %{ $1 = ($1_ltype)$input; %}
	%typemap(out) wchar_t*, wchar_t[ANY], wchar_t[] %{ $result = $1; %}
#endif // #ifdef __APPLE__

%typemap(memberin) wchar_t * {
  delete [] $1;
  if ($input) {
     $1 = ($1_type) (new wchar_t[wcslen((const wchar_t *)$input)+1]);
     wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
     $1 = 0;
  }
}
%typemap(memberin,warning=SWIGWARN_TYPEMAP_CHARLEAK_MSG) const wchar_t * {
  if ($input) {
     $1 = ($1_type) (new wchar_t[wcslen((const wchar_t *)$input)+1]);
     wcscpy((wchar_t *)$1, (const wchar_t *)$input);
  } else {
     $1 = 0;
  }
}

%typemap(cstype) wchar_t*, wchar_t[ANY], wchar_t[] "string"
%typemap(csin) wchar_t*, wchar_t[ANY], wchar_t[] "$csinput"
%typemap(csout) wchar_t*, wchar_t[ANY], wchar_t[] { return AkSoundEngine.StringFromIntPtrWString($imcall); }

%typemap(csvarin) wchar_t*, wchar_t[ANY], wchar_t[] %{ set { $imcall; } %}
%typemap(csvarout) wchar_t*, wchar_t[ANY], wchar_t[] %{ get { return AkSoundEngine.StringFromIntPtrWString($imcall); } %}

// AkOSChar
%typemap(cstype) AkOSChar*, AkOSChar[ANY], AkOSChar[] "string"
%typemap(csin) AkOSChar*, AkOSChar[ANY], AkOSChar[] "$csinput"
%typemap(csout) AkOSChar*, AkOSChar[ANY], AkOSChar[] { return AkSoundEngine.StringFromIntPtrOSString($imcall); }
%typemap(csvarin) AkOSChar*, AkOSChar[ANY], AkOSChar[]  %{ set { $imcall; } %}
%typemap(csvarout) AkOSChar*, AkOSChar[ANY], AkOSChar[] %{ get { return AkSoundEngine.StringFromIntPtrOSString($imcall); } %}

#if defined(AK_WIN) || defined(AK_XBOX)
	%typemap(ctype) AkOSChar* "wchar_t*"
	%typemap(imtype, 
		out="global::System.IntPtr",
		inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPWStr)]") AkOSChar* "string"
#else // Current Non-Windows platforms all use char as native char. 
	%typemap(ctype) AkOSChar* "char*"
	%typemap(imtype,
		out="global::System.IntPtr",
		inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]") AkOSChar* "string"
#endif // #if defined(AK_WIN) || defined(AK_XBOX)

%typemap(ctype) AkGameObjectID "AkGameObjectID"
%typemap(imtype) AkGameObjectID "ulong"
%typemap(cstype) AkGameObjectID "ulong"
%typemap(in) AkGameObjectID %{ $1 = $input; %}
%typemap(out) AkGameObjectID %{ $result = $1; %}
%typemap(csout) AkGameObjectID { return $imcall; }
%typemap(csvarin)  AkGameObjectID %{ set { $imcall; } %}
%typemap(csvarout)  AkGameObjectID %{ get { return $imcall; } %}

%typemap(ctype) AkGameObjectID* "AkGameObjectID*"
%typemap(imtype) AkGameObjectID* "ulong[]"
%typemap(cstype) AkGameObjectID* "ulong[]"
%typemap(csin) AkGameObjectID* "$csinput"
%typemap(csout) AkGameObjectID* { return $imcall; }
%typemap(in) AkGameObjectID* %{ $1 = $input; %}
%typemap(out) AkGameObjectID* %{ $result = $1; %}

// Declaring our constants to avoid getter functions
// Order: Must be before %include <AK/SoundEngine/Common/AkTypes.h>
// %constant must occur before %ignore.

// Constants taken from <AK/SoundEngine/Common/AkTypes.h>
%constant AkPluginID				AK_INVALID_PLUGINID					= unchecked((uint)(-1));	///< Invalid FX ID
%constant AkGameObjectID			AK_INVALID_GAME_OBJECT				= unchecked((ulong)(-1));	///< Invalid game object (may also mean all game objects)
%constant AkUniqueID				AK_INVALID_UNIQUE_ID				= 0;						///< Invalid unique 32-bit ID
%constant AkRtpcID					AK_INVALID_RTPC_ID					= AK_INVALID_UNIQUE_ID;		///< Invalid RTPC ID
%constant AkUInt32					AK_INVALID_LISTENER_INDEX			= unchecked((uint)(-1));	///< Invalid listener index
%constant AkPlayingID				AK_INVALID_PLAYING_ID				= AK_INVALID_UNIQUE_ID;		///< Invalid playing ID
%constant AkUInt32					AK_DEFAULT_SWITCH_STATE				= 0;						///< Switch selected if no switch has been set yet
%constant AkMemPoolId				AK_INVALID_POOL_ID					= -1;						///< Invalid pool ID
%constant AkMemPoolId				AK_DEFAULT_POOL_ID					= -1;						///< Default pool ID, same as AK_INVALID_POOL_ID
%constant AkAuxBusID				AK_INVALID_AUX_ID					= AK_INVALID_UNIQUE_ID;		///< Invalid auxiliary bus ID (or no Aux bus ID)
%constant AkFileID					AK_INVALID_FILE_ID					= unchecked((uint)(-1));	///< Invalid file ID
%constant AkDeviceID				AK_INVALID_DEVICE_ID				= unchecked((uint)(-1));	///< Invalid streaming device ID
%constant AkBankID					AK_INVALID_BANK_ID					= AK_INVALID_UNIQUE_ID;		///< Invalid bank ID
%constant AkArgumentValueID			AK_FALLBACK_ARGUMENTVALUE_ID		= 0;						///< Fallback argument value ID
%constant AkChannelMask				AK_INVALID_CHANNELMASK				= 0;						///< Invalid channel mask
%constant AkUInt32					AK_INVALID_OUTPUT_DEVICE_ID			= AK_INVALID_UNIQUE_ID;		///< Invalid Device ID
%constant AkUInt32					AK_MIXER_FX_SLOT					= unchecked((uint)(-1));	///< Mixer slot
%constant AkGameObjectID			AK_DEFAULT_LISTENER_OBJ				= 0;						///< Default listener ID 0

// changed from AkPriority to unsigned int
%constant unsigned int				AK_DEFAULT_PRIORITY					=  50;						///< Default sound / I/O priority
%constant unsigned int				AK_MIN_PRIORITY						=  0;						///< Minimal priority value [0,100]
%constant unsigned int				AK_MAX_PRIORITY						=  100;						///< Maximal priority value [0,100]
%constant unsigned int				AK_DEFAULT_BANK_IO_PRIORITY			= AK_DEFAULT_PRIORITY;		///<  Default bank load I/O priority

// changed from AkReal32 to double
%constant double					AK_DEFAULT_BANK_THROUGHPUT			= 1*1024*1024/1000.0;		///<  Default bank load throughput (1 Mb/ms)

%constant unsigned int		AKCOMPANYID_AUDIOKINETIC			= 0;
%constant unsigned int		AK_LISTENERS_MASK_ALL 				= 0xFFFFFFFF;


%ignore AK_INVALID_PLUGINID;
%ignore AK_INVALID_GAME_OBJECT;
%ignore AK_INVALID_UNIQUE_ID;
%ignore AK_INVALID_RTPC_ID;
%ignore AK_INVALID_LISTENER_INDEX;
%ignore AK_INVALID_PLAYING_ID;
%ignore AK_DEFAULT_SWITCH_STATE;
%ignore AK_INVALID_POOL_ID;
%ignore AK_DEFAULT_POOL_ID;
%ignore AK_INVALID_AUX_ID;
%ignore AK_INVALID_FILE_ID;
%ignore AK_INVALID_DEVICE_ID;
%ignore AK_INVALID_BANK_ID;
%ignore AK_FALLBACK_ARGUMENTVALUE_ID;
%ignore AK_INVALID_CHANNELMASK;
%ignore AK_INVALID_OUTPUT_DEVICE_ID;
%ignore AK_MIXER_FX_SLOT;
%ignore AK_DEFAULT_LISTENER_OBJ;

%ignore AK_DEFAULT_PRIORITY;
%ignore AK_MIN_PRIORITY;
%ignore AK_MAX_PRIORITY;

%ignore AK_DEFAULT_BANK_IO_PRIORITY;
%ignore AK_DEFAULT_BANK_THROUGHPUT;

%ignore AKCOMPANYID_AUDIOKINETIC;
%ignore AKMOTIONDEVICEID_RUMBLE;
%ignore AK_LISTENERS_MASK_ALL;

%ignore AkNodeType;
%ignore WwiseObjectID;
%ignore WwiseObjectIDext;
%ignore EnvelopePoint;

%ignore AK::ATRAC9;

// Must be before %include <AK/SoundEngine/Common/AkTypes.h>
%rename(IsEqualTo) WwiseObjectIDext::operator==;
// WG-21816
%rename(GetNodeType) WwiseObjectIDext::GetType();

#define __cdecl

// Rules for input/output arguments
// Note: typedefs cannot be used in %apply rule name. Use define instead

#if defined(AK_WIN)
	#define ApplyAkUInt32 uint32_t
	#define ApplyAkInt32 int32_t
	#define ApplyAkUInt8 uint8_t
	#define ApplyAkUInt64 uint64_t
#else
	#if defined(AK_XBOX)
		#define ApplyAkUInt32 unsigned long
		#define ApplyAkInt32 long
	#else
		#define ApplyAkUInt32 unsigned int
		#define ApplyAkInt32 int
	#endif // defined(AK_XBOX)

	// AkUInt8 arguments
	#define ApplyAkUInt8 unsigned char

	// AkUInt64 arguments
	#define ApplyAkUInt64 unsigned long long
#endif // defined(AK_WIN)

// AkReal32 apply aliase
#define ApplyAkReal32 float

// bool and enum arguments
#define ApplyBoolEnum int

// enum arguments
#define ApplyOutputEnum int

// AkUInt32 arguments
// - as input array
%apply ApplyAkUInt32 INPUT[] { AkUInt32* in_paGameSyncID, AkUInt32*	in_paGameSyncID, AkUniqueID*, AkArgumentValueID* };
// - as output array
%apply ApplyAkUInt32 OUTPUT[] { AkPlayingID* out_aPlayingIDs, AkUniqueID* out_audioNodeID, AkUniqueID* out_mediaID };
// - as single output
%apply ApplyAkUInt32 &OUTPUT { AkStateID& out_rState, AkSwitchStateID& out_rSwitchState, AkBankID& out_bankID, AkUInt32& out_uChannelConfig};
// - as single input/output
%apply ApplyAkUInt32 &INOUT { AkUInt32&	io_ruNumEnvValues, AkUInt32& io_ruNumItems, AkUInt32& io_ruNumIDs, AkUInt32& io_ruNumSendValues, AkUInt32 &io_uChannelMask, AkUInt32& io_uNumAngles};
// - as pointer input/output
%apply ApplyAkUInt32 *INOUT {AkUInt32* io_pcPositions};

// AkInt32 arguments
// - as single output
%apply ApplyAkInt32 *OUTPUT { AkTimeMs*, AkMemPoolId* };
%apply ApplyAkInt32	OUTPUT[] { AkTimeMs* out_msTime };
%apply ApplyAkInt32 &OUTPUT { AkInt32& out_iValue, AkTimeMs& out_buffering };
%apply ApplyAkUInt32 &OUTPUT { AkUInt32& };

// AkReal32 arguments
%apply ApplyAkReal32 &OUTPUT { AkReal32&, AkRtpcValue& };
%apply ApplyAkReal32 INOUT[] { AkReal32* io_pfSpeakerAngles };
%apply ApplyAkReal32 INPUT[] { AkReal32* in_pfSpeakerAngles };
%apply ApplyAkReal32 INPUT[] { AK::SpeakerVolumes::VectorPtr in_pVolumeOffsets };

// Misc arguments
%apply ApplyBoolEnum &OUTPUT {bool&};
%apply ApplyBoolEnum &INOUT {AK::SoundEngine::Query::RTPCValue_type&};
%apply ApplyAkUInt8 &OUTPUT { AkUInt8& };
%apply ApplyOutputEnum &OUTPUT { AkPanningRule & }; // GetPanningRule()

%apply ApplyAkUInt64 { AkOutputDeviceID }
%apply ApplyAkUInt64 *OUTPUT { AkOutputDeviceID* }

#if defined(AK_SONY)
	%apply ApplyAkUInt64 { SceKernelCpumask };
	%apply ApplyAkUInt64 { SceAudioOut2PortHandle };
#endif // #if defined(AK_SONY)

%typemap(in) AkGroupType INPUT { $1 = (AkGroupType)$input; }

//Typemap that will route the AkCallbackFunc parameter through the AkCallbackSerializer
//The cookie must point to a CallbackPackage in C#.  The package will have the real C# call back plus the C# cookie.


	//This will force AkCallbackSerializer::EventCallback to be the callback on the C++ side.  Thus, it forces serialization through AkCallbackSerializer
	%typemap(in) AkCallbackFunc %{ $1 = $input != NULL ? AkCallbackSerializer::EventCallback : NULL; %}

	//This overrides the void* typemap for parameters named "in_pCookie" on the C# side.  It maps to "object" as input parameter.
	%typemap(cstype) void * in_pCookie "object"
	%typemap(csin) void * in_pCookie "$csinput != null ? (global::System.IntPtr)$csinput.GetHashCode() : global::System.IntPtr.Zero"

	//This maps AkCallbackFunc to AkCallbackManager.EventCallback delegate type on the C# side.
	%typemap(cstype) AkCallbackFunc "AkCallbackManager.EventCallback"
	%typemap(imtype) AkCallbackFunc "global::System.IntPtr"

	//This typemap triggers when receiving a AkCallbackFunc(AkCallbackManager.EventCallback) as input parameter on the C# side and injects the conversion code.
	//The callback pointer itself is treated like a boolean, telling the C++ side if the callback pointer is really needed.  It will be handled by a static function on the other side.
	%typemap(	csin,
				pre="	in_pCookie = AkCallbackManager.EventCallbackPackage.Create($csinput, in_pCookie, ref in_uFlags);"
				) AkCallbackFunc "in_uFlags != 0 ? (global::System.IntPtr)1 : global::System.IntPtr.Zero"

	%typemap(csout) AkPlayingID {
		uint ret = $imcall;
		AkCallbackManager.SetLastAddedPlayingID(ret);
		return ret;
	}

	%typemap(csout) void CancelEventCallback {
		AkCallbackManager.RemoveEventCallback(in_playingID);
	}

	%typemap(csout) void CancelEventCallbackCookie {
		AkCallbackManager.RemoveEventCallbackCookie(in_pCookie);
	}

//Typemap that will route the AkBankCallbackFunc parameter through the AkCallbackSerializer
//The cookie must point to a CallbackPackage in C#.  The package will have the real C# call back plus the C# cookie.

	//This will force AkCallbackSerializer::BankCallback to be the callback on the C++ side.  Thus, it forces serialization through AkCallbackSerializer
	%typemap(in) AkBankCallbackFunc %{ $1 = (AkBankCallbackFunc) AkCallbackSerializer::BankCallback; %}

	//This maps AkBankCallbackFunc to AkCallbackManager.EventCallback delegate type on the C# side.
	%typemap(cstype) AkBankCallbackFunc "AkCallbackManager.BankCallback"
	%typemap(imtype) AkBankCallbackFunc "global::System.IntPtr"

	//This typemap triggers when receiving a AkBankCallbackFunc(AkCallbackManager.BankCallback) as input parameter on the C# side and injects the conversion code.
	%typemap(csin, pre="		in_pCookie = new AkCallbackManager.BankCallbackPackage($csinput, in_pCookie);") AkBankCallbackFunc "global::System.IntPtr.Zero"

	%typemap(csout) void CancelBankCallbackCookie {
		AkCallbackManager.RemoveBankCallback(in_pCookie);
	}

%rename(AkMonitorErrorCode) AK::Monitor::ErrorCode;
%rename(AkMonitorErrorLevel) AK::Monitor::ErrorLevel;
%rename(AkDynamicSequenceType) AK::SoundEngine::DynamicSequence::DynamicSequenceType;

%rename(AkPreparationType) AK::SoundEngine::PreparationType;
%rename(AkMultiPositionType) AK::SoundEngine::MultiPositionType;
%rename(AkQueryRTPCValue) AK::SoundEngine::Query::RTPCValue_type;

%ignore AkExternalSourceInfo;
%rename(AkExternalSourceInfo) AkSerializedExternalSourceInfo;

//////////// AkVector
%typemap(ctype) AkVector "AkVector"
%typemap(ctype, out="AkVector*") AkVector*, const AkVector& "AkVector"
%typemap(ctype, out="AkVector*") AkVector& "AkVector*"
%typemap(imtype, out="UnityEngine.Vector3") AkVector, AkVector*, const AkVector& "UnityEngine.Vector3"
%typemap(imtype, out="UnityEngine.Vector3") AkVector& "ref UnityEngine.Vector3"
%typemap(cstype) AkVector, AkVector*, AkVector&, const AkVector& "UnityEngine.Vector3"
%typemap(cstype, out="UnityEngine.Vector3") AkVector& "ref UnityEngine.Vector3"
%typemap(csin) AkVector, AkVector*, const AkVector& "$csinput"
%typemap(csin) AkVector& "ref $csinput"
%typemap(csout) AkVector, AkVector*, AkVector& { return $imcall; }
%typemap(in) AkVector, AkVector& %{ $1 = $input; %}
%typemap(in) const AkVector&, AkVector* %{ $1 = &$input; %}
%typemap(out) AkVector, AkVector*, AkVector& %{ $result = $1; %}
%typemap(csvarin) AkVector, AkVector*, AkVector& %{ set { $imcall; } %}
%typemap(csvarout) AkVector, AkVector*, AkVector& %{ get { return $imcall; } %}

%ignore AkVector;

%typemap(cstype) AkVertex*, const AkVertex* "UnityEngine.Vector3[]"
%typemap(csin) AkVertex*, const AkVertex* "$csinput"
%typemap(imtype) AkVertex*, const AkVertex* "UnityEngine.Vector3[]"

%ignore AkVertex;

// Include all our public interfaces: Must be below WwiseObjectIDext operators
%{
#include <AK/SoundEngine/Common/AkTypes.h>
%}
%include <AK/SoundEngine/Common/AkTypes.h>

%pragma(csharp) moduleclassmodifiers="public partial class"


//This typemap replaces pointers to struct arrays with a hand-written object on the C# side
%define %cs_array_type(TYPE, CSTYPE, BUFFER)
	%typemap(cstype) TYPE*, const TYPE* "CSTYPE"
	%typemap(csin) TYPE*, const TYPE* "$csinput.BUFFER"
	%typemap(imtype) TYPE*, const TYPE* "global::System.IntPtr"
%enddef

%cs_array_type(AkExternalSourceInfo, AkExternalSourceInfoArray, GetBuffer())
%cs_array_type(AkObjectInfo, AkObjectInfoArray, GetBuffer())
%cs_array_type(AkObstructionOcclusionValues, AkObstructionOcclusionValuesArray, GetBuffer())
%cs_array_type(AkSourceSettings, AkSourceSettingsArray, GetBuffer())
%cs_array_type(AkAuxSendValue, AkAuxSendArray, GetBuffer())
%cs_array_type(AkSoundPosition, AkPositionArray, m_Buffer)
%cs_array_type(AkChannelEmitter, AkChannelEmitterArray, m_Buffer)
%cs_array_type(AkMIDIPost, AkMIDIPostArray, GetBuffer())
%cs_array_type(AkTriangle, AkTriangleArray, GetBuffer())
%cs_array_type(AkAcousticSurface, AkAcousticSurfaceArray, GetBuffer())
%cs_array_type(AkReflectionPathInfo, AkReflectionPathInfoArray, GetBuffer())
%cs_array_type(AkDiffractionPathInfo, AkDiffractionPathInfoArray, GetBuffer())
%cs_array_type(AkDeviceDescription, AkDeviceDescriptionArray, GetBuffer())

%ignore AkAuxSendValue::AkAuxSendValue();
%ignore AkSoundPosition::AkSoundPosition();
%ignore AkChannelEmitter::AkChannelEmitter();


//This typemap replaces the bitfield enum AkMemPoolAttributes with a simple int.  This avoids a "SWIGTYPE" file.
	%typemap(cstype) AkMemPoolAttributes "int"
	%typemap(csin) AkMemPoolAttributes "$csinput"
	%typemap(ctype) AkMemPoolAttributes "int"
	%typemap(imtype) AkMemPoolAttributes "int"
	%typemap(in) AkMemPoolAttributes %{ $1 = (AkMemPoolAttributes)$input; %}
	%typemap(out) AkMemPoolAttributes %{ $result = (AkMemPoolAttributes)$1; %}
	%typemap(csout) AkMemPoolAttributes { return $imcall; }
	%typemap(csvarin)  AkMemPoolAttributes %{ set { $imcall; } %}
	%typemap(csvarout)  AkMemPoolAttributes %{ get { return $imcall; } %}

//////////////////////////////////////////////////////////////////////////////////////////////////

%typemap(ctype) size_t "size_t"
%typemap(imtype) size_t "uint"
%typemap(cstype) size_t "uint"
%typemap(in) size_t %{ $1 = $input; %}
%typemap(out) size_t %{ $result = $1; %}
%typemap(csin) size_t "$csinput"
%typemap(csout) size_t { return $imcall; }
%typemap(csvarin, excode=SWIGEXCODE2)  size_t %{ set { $imcall; } %}
%typemap(csvarout, excode=SWIGEXCODE2)  size_t %{ get { return $imcall; } %}

%typemap(ctype) AkSpatialAudioID "AkSpatialAudioID"
%typemap(imtype) AkSpatialAudioID "ulong"
%typemap(cstype) AkSpatialAudioID "ulong"
%typemap(in) AkSpatialAudioID %{ $1 = $input; %}
%typemap(out) AkSpatialAudioID %{ $result = $1; %}
%typemap(csin) AkSpatialAudioID "$csinput"
%typemap(csout) AkSpatialAudioID { return $imcall; }
%typemap(csvarin, excode=SWIGEXCODE2)  AkSpatialAudioID %{ set { $imcall;$excode } %}
%typemap(csvarout, excode=SWIGEXCODE2)  AkSpatialAudioID %{ get { return $imcall;$excode } %}

%typemap(ctype) AkSpatialAudioID* "AkSpatialAudioID*"
%typemap(imtype) AkSpatialAudioID* "ulong[]"
%typemap(cstype) AkSpatialAudioID* "ulong[]"
%typemap(in) AkSpatialAudioID* %{ $1 = $input; %}
%typemap(out) AkSpatialAudioID* %{ $result = $1; %}
%typemap(csin) AkSpatialAudioID* "$csinput"
%typemap(csout) AkSpatialAudioID* { return $imcall; }
%typemap(csvarin, excode=SWIGEXCODE2)  AkSpatialAudioID* %{ set { $imcall;$excode } %}
%typemap(csvarout, excode=SWIGEXCODE2)  AkSpatialAudioID* %{ get { return $imcall;$excode } %}

%typemap(ctype) AkRoomID "AkRoomID"
%typemap(imtype) AkRoomID "ulong"
%typemap(cstype) AkRoomID "ulong"
%typemap(in) AkRoomID %{ $1 = $input; %}
%typemap(out) AkRoomID %{ $result = $1; %}
%typemap(csin) AkRoomID "$csinput"
%typemap(csout) AkRoomID { return $imcall; }
%typemap(csvarin, excode=SWIGEXCODE2)  AkRoomID %{ set { $imcall;$excode } %}
%typemap(csvarout, excode=SWIGEXCODE2)  AkRoomID %{ get { return $imcall;$excode } %}

//These typemaps make array of strings come accross properly.
#ifdef AK_SUPPORT_WCHAR
	// C# marshalling: platform independent
	%typemap(ctype) wchar_t const ** "wchar_t const*";
	%typemap(cstype) wchar_t const ** "string []"
	%typemap(imtype) wchar_t const ** "global::System.IntPtr"
	%typemap(csin, pre="			
		//Find the required size
		int size = 0;
		foreach(string s in $csinput)
			size += s.Length + 1;
				
		int sizeofChar = 2;	//Unicode
		global::System.IntPtr pMem = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeofChar + 2);
		
		//Write the length of array
		global::System.Runtime.InteropServices.Marshal.WriteInt16(pMem, (short)$csinput.Length);
		global::System.IntPtr pCurrent = (global::System.IntPtr)(pMem.ToInt64() + sizeofChar);
		
		//Copy the strings one after the other.
		foreach(string s in $csinput)
		{
			global::System.Runtime.InteropServices.Marshal.Copy(s.ToCharArray(), 0, pCurrent, s.Length);
			pCurrent = (global::System.IntPtr)(pCurrent.ToInt64() + sizeofChar * s.Length);
			global::System.Runtime.InteropServices.Marshal.WriteInt16(pCurrent, 0);	//Null-terminated string
			pCurrent = (global::System.IntPtr)(pCurrent.ToInt64() + sizeofChar);
		}		
		", post="	global::System.Runtime.InteropServices.Marshal.FreeHGlobal(pMem);") wchar_t const ** "pMem";
	
	#ifndef __APPLE__
		%typemap(in) wchar_t const ** %{
		   //Create a string array of the proper size
		   unsigned short size = *$input;
		   $input++; // skip size word
		   $1 = (wchar_t**)AkAlloca(size*sizeof(wchar_t*));
		   unsigned long len = 0;
		   //Make all pointers point to the right place.
		   for(unsigned long i = 0; i < size; i++)
		   {
				$1[i] = (wchar_t*)$input + len;
				len += (unsigned long)wcslen($1[i]) + 1;
		   }
		%}
	#else // #ifndef __APPLE__
		%typemap(in) wchar_t const ** %{
		   //Create a string array of the proper size
		   unsigned short size = *$input;
		   AkUtf16* csharpString = (AkUtf16*)$input; 
		   ++csharpString; // skip size word
		   $1 = (wchar_t**)AkAlloca(size*sizeof(wchar_t*));
		   unsigned long len = 0;
		   //Make all pointers point to the right place.
		   for(unsigned long i = 0; i < size; i++)
		   {
				AkUtf16* winWcharString = csharpString + len;
				size_t srcLen = AKPLATFORM::AkUtf16StrLen(winWcharString);
				wchar_t* appleWcharString = NULL;
				CONVERT_UTF16_TO_WCHAR(winWcharString, appleWcharString);
				$1[i] = appleWcharString;

				len += (unsigned long)srcLen + 1;
		   }
		%}
	#endif // #ifndef __APPLE__
	
#endif // #ifdef AK_SUPPORT_WCHAR
	
//Typemaps to make char string array come across PINVOKE properly
	%typemap(ctype) char const ** "wchar_t const*";
	%typemap(cstype) char const ** "string []"
	%typemap(imtype) char const ** "global::System.IntPtr"
	%typemap(csin, pre="
		//Find the required size
		int size = 0;
		foreach(string s in $csinput)
			size += s.Length + 1;

		int sizeofChar = 2;	// UNICODE
		global::System.IntPtr pMem = global::System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeofChar + 2);

		//Write the length of array
		global::System.Runtime.InteropServices.Marshal.WriteInt16(pMem, (short)$csinput.Length);
		global::System.IntPtr pCurrent = (global::System.IntPtr)(pMem.ToInt64() + sizeofChar);

		//Copy the strings one after the other.
		foreach(string s in $csinput)
		{
			global::System.Runtime.InteropServices.Marshal.Copy(s.ToCharArray(), 0, pCurrent, s.Length);
			pCurrent = (global::System.IntPtr)(pCurrent.ToInt64() + sizeofChar * s.Length);
			global::System.Runtime.InteropServices.Marshal.WriteInt16(pCurrent, 0);	//Null-terminated string
			pCurrent = (global::System.IntPtr)(pCurrent.ToInt64() + sizeofChar);
		}
		", post="	global::System.Runtime.InteropServices.Marshal.FreeHGlobal(pMem);") char const ** "pMem";

	// Only Android uses char-based APIs and Android uses UTF-32 for wchar_t.
	%typemap(in) char const ** %{
	   //Create a string array of the proper size
	   unsigned short size = *$input;
	   AkUtf16* csharpString = (AkUtf16*)$input; 
	   ++csharpString; // skip size word
	   $1 = (char**)AkAlloca(size*sizeof(wchar_t*));
	   unsigned long len = 0;
	   //Make all pointers point to the right place.
	   for(unsigned long i = 0; i < size; i++)
	   {
			AkUtf16* winWcharString = csharpString + len;
			size_t srcLen = AKPLATFORM::AkUtf16StrLen(winWcharString);
			char* charString = NULL;
			CONVERT_UTF16_TO_CHAR(winWcharString, charString);
			$1[i] = charString;

			len += (unsigned long)srcLen + 1;
	   }
	%}

%ignore RegisterBusVolumeCallback;
%ignore AkBusCallbackFunc;
%ignore RegisterBusMeteringCallback;
%ignore AkBusMeteringCallbackInfo;
%ignore RegisterOutputDeviceMeteringCallback;
%ignore AkOutputDeviceMeteringCallbackInfo;
%ignore AkSpeakerVolumeMatrixCallbackInfo;	//Not supported.
%ignore AkSpeakerVolumeMatrixBusCallbackInfo;	//Not supported.
%ignore AK::AkDeviceStatusCallbackFunc; //Not supported
%ignore AK::AkAudioDeviceEvent;
%ignore GetListenerSpatialization;

%ignore AkTaskSchedulerDesc; // Ignore parallelization APIs
%ignore taskSchedulerDesc;

%ignore SetPosition;	//SetPosition is replaced by SetObjectPosition below
%ignore GetSourcePlayPositions;
%ignore AK::SoundEngine::RegisterGameObj;
%ignore AK::SoundEngine::UnregisterGameObj;

%ignore AK::SoundEngine::PostEvent(AkUniqueID, AkGameObjectID, AkUInt32, AkCallbackFunc); //In C#, the Callback and the cookie must be defined.
%ignore AK::SoundEngine::PostEvent(char const *, AkGameObjectID, AkUInt32, AkCallbackFunc); //In C#, the Callback and the cookie must be defined.
%ignore AK::SoundEngine::PostEvent(wchar_t const *,AkGameObjectID, AkUInt32, AkCallbackFunc); //In C#, the Callback and the cookie must be defined.

%ignore AK::SoundEngine::PostMIDIOnEvent(AkUniqueID, AkGameObjectID, AkMIDIPost*, AkUInt16, bool, AkUInt32, AkCallbackFunc); //In C#, the Callback and the cookie must be defined.
%ignore AK::SoundEngine::PostMIDIOnEvent(AkUniqueID, AkGameObjectID, AkMIDIPost*, AkUInt16, bool, AkUInt32);  // ignore non-zero callback flags with callback set to null

%ignore AK::SoundEngine::PostEvent(AkUniqueID, AkGameObjectID, AkUInt32); // ignore non-zero callback flags with callback set to null
%ignore AK::SoundEngine::PostEvent(char const *, AkGameObjectID, AkUInt32); // ignore non-zero callback flags with callback set to null
%ignore AK::SoundEngine::PostEvent(wchar_t const *,AkGameObjectID, AkUInt32); // ignore non-zero callback flags with callback set to null

%ignore AK::SoundEngine::PostEvent(AkUniqueID, AkGameObjectID, AkUInt32, AkCallbackFunc, void *, AkUInt32); // ignore external count > 0, when buffer is null
%ignore AK::SoundEngine::PostEvent(char const *, AkGameObjectID, AkUInt32, AkCallbackFunc, void *, AkUInt32); // ignore external count > 0, when buffer is null
%ignore AK::SoundEngine::PostEvent(wchar_t const *,AkGameObjectID, AkUInt32, AkCallbackFunc, void *, AkUInt32); // ignore external count > 0, when buffer is null

%ignore PostEventOnRoom(AkUniqueID, AkRoomID, AkUInt32, AkCallbackFunc); //In C#, the Callback and the cookie must be defined.
%ignore PostEventOnRoom(char const *, AkRoomID, AkUInt32, AkCallbackFunc); //In C#, the Callback and the cookie must be defined.
%ignore PostEventOnRoom(wchar_t const *,AkRoomID, AkUInt32, AkCallbackFunc); //In C#, the Callback and the cookie must be defined.

%ignore PostEventOnRoom(AkUniqueID, AkRoomID, AkUInt32); // ignore non-zero callback flags with callback set to null
%ignore PostEventOnRoom(char const *, AkRoomID, AkUInt32); // ignore non-zero callback flags with callback set to null
%ignore PostEventOnRoom(wchar_t const *,AkRoomID, AkUInt32); // ignore non-zero callback flags with callback set to null

%ignore PostEventOnRoom(AkUniqueID, AkRoomID, AkUInt32, AkCallbackFunc, void *, AkUInt32); // ignore external count > 0, when buffer is null
%ignore PostEventOnRoom(char const *, AkRoomID, AkUInt32, AkCallbackFunc, void *, AkUInt32); // ignore external count > 0, when buffer is null
%ignore PostEventOnRoom(wchar_t const *,AkRoomID, AkUInt32, AkCallbackFunc, void *, AkUInt32); // ignore external count > 0, when buffer is null

%ignore AK::SoundEngine::DecodeBank;
%ignore AK::SoundEngine::GetContainerHistory;
%ignore AK::SoundEngine::SetContainerHistory;
%ignore AK::SoundEngine::DynamicSequence::GetPlayingItem;

%ignore AK::Monitor::SetLocalOutput;
%ignore AK::SoundEngine::AddSecondaryOutput(AkUInt32,AkAudioOutputType,const AkGameObjectID *,AkUInt32,AkUInt32,AkUniqueID);
%ignore AK::SoundEngine::AddSecondaryOutput(AkUInt32,AkAudioOutputType,const AkGameObjectID *,AkUInt32,AkUInt32);
%ignore AK::SoundEngine::AddSecondaryOutput(AkUInt32,AkAudioOutputType,const AkGameObjectID *,AkUInt32,AkUniqueID);
%ignore AK::SoundEngine::AddSecondaryOutput(AkUInt32,AkAudioOutputType,const AkGameObjectID *,AkUInt32);
%ignore LocalOutputFunc;
%ignore pad;

%ignore AK::SoundEngine::RegisterResourceMonitorCallback;
%ignore AK::SoundEngine::UnregisterResourceMonitorCallback;

%ignore AK::SoundEngine::RegisterAudioDeviceStatusCallback;
%ignore AK::SoundEngine::UnregisterAudioDeviceStatusCallback;

// Because SWIG doesn't understand pointers to AKRESULT
%ignore AK::SoundEngine::TryUnsetMedia;

%ignore s_aszErrorCodes;

%ignore AkAudioBuffer;
%ignore IAkMetering;
%ignore HasLFE;
%ignore HasCenter;
%ignore GetNumberOfAnglesForConfig;
%ignore AkAudioObject;
%ignore AkAudioObjects;


%ignore AkBitScanForward;
%ignore AkBitScanForward64;

// PS4 compiler don't like __attribute__ for inline
#undef AkForceInline
#define AkForceInline

%include <AK/SoundEngine/Common/AkCommonDefs.h>

// C# bindings allow to use only one of wchar_t*/char* API pairs (multiple-definition problem). 
// For platforms that support wchar_t*, in favor of wchar_t* APIs, ignore char* APIs; and vice versa.
#ifdef AK_SUPPORT_WCHAR
	#define IgnoredCharType char
#else
	#define IgnoredCharType wchar_t
#endif

%ignore AK::SoundEngine::PrepareEvent(PreparationType, const IgnoredCharType**, AkUInt32);
%ignore AK::SoundEngine::PrepareEvent(PreparationType, const IgnoredCharType**, AkUInt32, AkBankCallbackFunc, void*);
%ignore AK::SoundEngine::PrepareGameSyncs(PreparationType, AkGroupType, const IgnoredCharType*, const IgnoredCharType**, AkUInt32);
%ignore AK::SoundEngine::PrepareGameSyncs(PreparationType, AkGroupType, const IgnoredCharType*, const IgnoredCharType**, AkUInt32, AkBankCallbackFunc, void *);
// %ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const IgnoredCharType*, const IgnoredCharType**, AkUInt32);
// %ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const IgnoredCharType*, const IgnoredCharType**, AkUInt32, AkPlayingID);
%ignore AK::SoundEngine::LoadBank(const IgnoredCharType*, AkMemPoolId, AkBankID &);
%ignore AK::SoundEngine::LoadBank(const IgnoredCharType*, AkBankCallbackFunc, void *, AkMemPoolId, AkBankID &);
%ignore AK::SoundEngine::Query::GetRTPCValue(const IgnoredCharType*, AkGameObjectID, AkRtpcValue&, RTPCValue_type&);
%ignore AK::SoundEngine::Query::GetSwitch(const IgnoredCharType*, AkGameObjectID, AkSwitchStateID&);
%ignore AK::SoundEngine::Query::GetState(const IgnoredCharType*, AkStateID&);
%ignore AK::SoundEngine::Query::QueryAudioObjectIDs(const IgnoredCharType*, AkUInt32&, AkObjectInfo*);
%ignore AK::SoundEngine::Query::GetListeners(AkGameObjectID, AkGameObjectID*,	AkUInt32& );

%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const char*, const char**, AkUInt32);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const char*, const char**, AkUInt32, AkPlayingID);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const char*, const char**, AkUInt32, AkPlayingID, AkCandidateCallbackFunc);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const char*, const char**, AkUInt32, AkPlayingID, AkCandidateCallbackFunc, void*);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const char*, const char**, AkUInt32, AkPlayingID);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const wchar_t*, const wchar_t**, AkUInt32);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const wchar_t*, const wchar_t**, AkUInt32, AkPlayingID);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const wchar_t*, const wchar_t**, AkUInt32, AkPlayingID, AkCandidateCallbackFunc);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(const wchar_t*, const wchar_t**, AkUInt32, AkPlayingID, AkCandidateCallbackFunc, void*);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(AkUniqueID, AkArgumentValueID*, AkUInt32, AkPlayingID, AkCandidateCallbackFunc);
%ignore AK::SoundEngine::DynamicDialogue::ResolveDialogueEvent(AkUniqueID, AkArgumentValueID*, AkUInt32, AkPlayingID, AkCandidateCallbackFunc, void*);

%ignore AkCallbackSerializer::EventCallback;
%ignore AkCallbackSerializer::BankCallback;
%ignore AkCallbackSerializer::AudioInterruptionCallback;
%ignore AkCallbackSerializer::BGMCallback;
%ignore AK::SoundEngine::DynamicSequence::Open(AkGameObjectID, AkUInt32, AkCallbackFunc ); //In C#, the Callback and the cookie must be defined.
%ignore AK::SoundEngine::DynamicSequence::Open(AkGameObjectID, AkUInt32); // ignore non-zero callback flags with callback set to null

%ignore AK::SoundEngine::GetDefaultInitSettings;
%ignore AK::SoundEngine::GetDefaultPlatformInitSettings;
%ignore AK::MusicEngine::Init;
%ignore AK::MusicEngine::Term;
%ignore AK::MusicEngine::GetDefaultInitSettings;
%ignore AK::SpatialAudio::Init;

//These ignores hide functions with problematic parameters like arrays, in/out params,...
%ignore AK::SpatialAudio::SetPortal;
%ignore AK::SpatialAudio::SetRoom;
%ignore AK::SpatialAudio::SetPosition;
%ignore AK::SpatialAudio::SetGeometry;
%ignore AkGeometryParams;
%ignore AkRoomParams::GeometryID;
%ignore AkRayInfo;
%ignore AkSpatialAudioInitSettings::fcnDiffractionAccum;

//Hiding Register and Unregister Listener to change its name as to not confuse it with SoundEngine's Re/SetListeners
%ignore AK::SpatialAudio::RegisterListener;
%ignore AK::SpatialAudio::UnregisterListener;

//These ignores hide ID structs
%ignore AkSpatialAudioID;
%ignore AkRoomID;
%ignore AK::SpatialAudio::kOutdoorRoomID;
%ignore AkPortalParams;

//These ignores hide strings
%ignore AkImageSourceSettings::name;
%ignore AkEmitterSettings::name;
%ignore AkPortalParams::strName;
%ignore AkRoomParams::strName;

//These ignores hide unnecessary image source structs
%ignore AkImageSourceName;
%ignore AkImageSourceTexture;
%ignore AkImageSourceSettings::texture;
%ignore AkReflectImageSource;
%ignore AkReflectGameData;

//These ignores hide arrays
%ignore AkReflectionPathInfo::pathPoint;
%ignore AkReflectionPathInfo::surfaces;
%ignore AkReflectionPathInfo::diffraction;

%ignore AkDiffractionPathInfo::portals;
%ignore AkDiffractionPathInfo::rooms;
%ignore AkDiffractionPathInfo::nodes;
%ignore AkDiffractionPathInfo::angles;

%ignore AK::SoundEngine::RegisterPlugin;
%ignore AK::SoundEngine::RegisterCodec;
%ignore AK::SoundEngine::Init;
%ignore AK::SoundEngine::Term;
%ignore AK::SoundEngine::RegisterGlobalCallback;
%ignore AK::SoundEngine::UnregisterGlobalCallback;
%ignore AK::SoundEngine::GetDefaultDeviceSettings;
%ignore AK::StreamMgr::GetDefaultDeviceSettings;
%ignore AkCallbackInfo;
%ignore AkEventCallbackInfo;
%ignore AkDurationCallbackInfo;
%ignore AkDynamicSequenceItemCallbackInfo;
%ignore AkMarkerCallbackInfo;
%ignore AkMIDIEventCallbackInfo;
%ignore AkMusicPlaylistCallbackInfo;
%ignore AkMusicSyncCallbackInfo;
%ignore AkMidiNoteChannelPair;
// Explicit ignores from generated API.
%ignore AK::Monitor::s_aszErrorCodes;
%ignore AkAssertHook;
%ignore AkCallbackFunc;
%ignore AkGlobalCallbackFunc;
%ignore AK::MemoryMgr::Init;

// Windows, Metro:
// Until user has specific audio performance requirements, ignore low-level platformInitSettings members for now.
%ignore pfnAssertHook;
%ignore hWnd;
%ignore IDirectSound8;
%ignore pXAudio2;
%ignore IXAudio2;
%ignore GetWwiseXAudio2Interface;
%ignore GetDirectSoundInstance;
%ignore IMMDevice;
%ignore GetDeviceID;

// Motion: Registration already handled in handwritten Init.
%ignore AkCreatePluginCallback;
%ignore AkCreateSinkPluginCallback;
%ignore pfSinkPluginFactory;

// Query: Ignore AkArray-related features, to avoid too much SWIG hacks: SWIG need separate fully-qualified AkArray classes for each typedef/subclass.
%ignore AK::SoundEngine::Query::AkGameObjectsList;
%ignore AK::SoundEngine::Query::GetActiveGameObjects(AkGameObjectsList&);
%ignore AK::SoundEngine::Query::GameObjDst;
%ignore AK::SoundEngine::Query::AkRadiusList;
%ignore AK::SoundEngine::Query::GetMaxRadius(AkRadiusList &);

// DynaimcDialogue: Ignore unsupported AkArray operations (WG-23783)
%ignore AkArray::BinarySearch;

// iOS: Ignore audio input related features
%ignore kAudioSessionCategory_PlayAndRecord;
%ignore AudioInputCallbackFunc;
%ignore AudioInterruptionCallbackFunc;
%ignore BGMCallbackFunc;
%ignore inputCallback;
%ignore inputCallbackCookie;
%ignore interruptionCallback;
%ignore interruptionCallbackCookie;
%ignore BGMCallback;
%ignore BGMCallbackCookie;
%ignore AudioBufferList;
%ignore audioCallbacks;
%ignore AkAudioCallbacks;
%ignore AkResourceMonitorCallbackFunc;

// Android: Ignore low-level  API
%ignore GetWwiseOpenSLInterface;

// Android: Ignore low-level settings in Android AkPlatforms Settings
%ignore AkPlatformInitSettings::pSLEngine;
%ignore AkPlatformInitSettings::pJavaVM;
%ignore AkPlatformInitSettings::jActivity;
%ignore AkPlatformInitSettings::sharingMode;
%ignore AkPlatformInitSettings::usage;
%ignore AkPlatformInitSettings::contentType;

%extend AkAuxSendValue
{
	void Set(AkGameObjectID listener, AkAuxBusID id, AkReal32 value)
	{
		$self->listenerID = listener;
		$self->auxBusID = id;
		$self->fControlValue = value;
	}

	bool IsSame(AkGameObjectID listener, AkAuxBusID id)
	{
		return $self->listenerID == listener && $self->auxBusID == id;
	}

%proxycode %{
  public void Set(UnityEngine.GameObject listener, uint id, float value)
  {
    var listener_id = AkSoundEngine.GetAkGameObjectID(listener);
    AkSoundEngine.PreGameObjectAPICall(listener, listener_id);

    Set(listener_id, id, value);
  }

  public bool IsSame(UnityEngine.GameObject listener, uint id)
  {
    var listener_id = AkSoundEngine.GetAkGameObjectID(listener);
    AkSoundEngine.PreGameObjectAPICall(listener, listener_id);

    return IsSame(listener_id, id);
  }
%}

	static int GetSizeOf() { return sizeof(AkAuxSendValue); }
}

%extend AkMIDIEvent
{
	AkMIDIEventTypes byType;

	AkMidiNoteNo	byOnOffNote;
	AkUInt8			byVelocity;

	AkMIDICcTypes		byCc;
	AkUInt8		byCcValue;

	AkUInt8		byValueLsb;
	AkUInt8		byValueMsb;

	AkUInt8		byAftertouchNote;
	AkUInt8		byNoteAftertouchValue;

	AkUInt8		byChanAftertouchValue;

	AkUInt8		byProgramNum;
	
	AkUInt16	uCmd;
	AkUInt32	uArg;
}

%extend AkMIDIPost
{
	AkPlayingID PostOnEvent(
		  AkUniqueID in_eventID
		, AkGameObjectID in_gameObjectID
		, AkUInt32 in_uNumPosts
	)
	{
		if (!AK::SoundEngine::IsInitialized())
			return AK_Fail;

		return AK::SoundEngine::PostMIDIOnEvent(in_eventID, in_gameObjectID, $self, static_cast<AkUInt16>(in_uNumPosts), false, 0, NULL, NULL, AK_INVALID_PLAYING_ID);
	}

	AkPlayingID PostOnEvent(
		  AkUniqueID in_eventID
		, AkGameObjectID in_gameObjectID
		, AkUInt32 in_uNumPosts
		, bool in_bAbsoluteOffsets
	)
	{
		if (!AK::SoundEngine::IsInitialized())
			return AK_Fail;

		return AK::SoundEngine::PostMIDIOnEvent(in_eventID, in_gameObjectID, $self, static_cast<AkUInt16>(in_uNumPosts), in_bAbsoluteOffsets, 0, NULL, NULL, AK_INVALID_PLAYING_ID);
	}

	AkPlayingID PostOnEvent(
		  AkUniqueID in_eventID
		, AkGameObjectID in_gameObjectID
		, AkUInt32 in_uNumPosts
		, bool in_bAbsoluteOffsets
		, AkUInt32 in_uFlags
		, AkCallbackFunc in_pfnCallback
		, void * in_pCookie
	)
	{
		if (!AK::SoundEngine::IsInitialized())
			return AK_Fail;

		return AK::SoundEngine::PostMIDIOnEvent(in_eventID, in_gameObjectID, $self, static_cast<AkUInt16>(in_uNumPosts), in_bAbsoluteOffsets, in_uFlags, in_pfnCallback, in_pCookie, AK_INVALID_PLAYING_ID);
	}

	AkPlayingID PostOnEvent(
		  AkUniqueID in_eventID
		, AkGameObjectID in_gameObjectID
		, AkUInt32 in_uNumPosts
		, bool in_bAbsoluteOffsets
		, AkUInt32 in_uFlags
		, AkCallbackFunc in_pfnCallback
		, void * in_pCookie
		, AkPlayingID in_playingID
	)
	{
		if (!AK::SoundEngine::IsInitialized())
			return AK_Fail;

		return AK::SoundEngine::PostMIDIOnEvent(in_eventID, in_gameObjectID, $self, static_cast<AkUInt16>(in_uNumPosts), in_bAbsoluteOffsets, in_uFlags, in_pfnCallback, in_pCookie, in_playingID);
	}


%proxycode %{
  public uint PostOnEvent(uint in_eventID, UnityEngine.GameObject in_gameObjectID, uint in_uNumPosts)
  {
    var in_gameObjectID_id = AkSoundEngine.GetAkGameObjectID(in_gameObjectID);
    AkSoundEngine.PreGameObjectAPICall(in_gameObjectID, in_gameObjectID_id);

    uint ret = PostOnEvent(in_eventID, in_gameObjectID_id, in_uNumPosts);
	AkCallbackManager.SetLastAddedPlayingID(ret);
	return ret;
  }
  
  public uint PostOnEvent(uint in_eventID, UnityEngine.GameObject in_gameObjectID, uint in_uNumPosts, bool in_bAbsoluteOffsets)
  {
    var in_gameObjectID_id = AkSoundEngine.GetAkGameObjectID(in_gameObjectID);
    AkSoundEngine.PreGameObjectAPICall(in_gameObjectID, in_gameObjectID_id);

    uint ret = PostOnEvent(in_eventID, in_gameObjectID_id, in_uNumPosts, in_bAbsoluteOffsets);
	AkCallbackManager.SetLastAddedPlayingID(ret);
	return ret;
  }
  
  public uint PostOnEvent(uint in_eventID, UnityEngine.GameObject in_gameObjectID, uint in_uNumPosts, bool in_bAbsoluteOffsets, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
  {
    var in_gameObjectID_id = AkSoundEngine.GetAkGameObjectID(in_gameObjectID);
    AkSoundEngine.PreGameObjectAPICall(in_gameObjectID, in_gameObjectID_id);

    uint ret =  PostOnEvent(in_eventID, in_gameObjectID_id, in_uNumPosts, in_bAbsoluteOffsets, in_uFlags, in_pfnCallback, in_pCookie);
	AkCallbackManager.SetLastAddedPlayingID(ret);
	return ret;
  }
  
  public uint PostOnEvent(uint in_eventID, UnityEngine.GameObject in_gameObjectID, uint in_uNumPosts, bool in_bAbsoluteOffsets, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_playingID)
  {
    var in_gameObjectID_id = AkSoundEngine.GetAkGameObjectID(in_gameObjectID);
    AkSoundEngine.PreGameObjectAPICall(in_gameObjectID, in_gameObjectID_id);
	in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);

    uint ret =  PostOnEvent(in_eventID, in_gameObjectID_id, in_uNumPosts, in_bAbsoluteOffsets, in_uFlags, in_pfnCallback, in_pCookie, in_playingID);
	AkCallbackManager.SetLastAddedPlayingID(ret);
	return ret;
  }
  
%}

	void Clone(const AkMIDIPost& other)
	{
		*(AkMIDIPost*)($self) = other;
	}

	static int GetSizeOf() { return sizeof(AkMIDIPost); }
}



// Spatial Audio extends

%extend AkTriangle
{
	void Clear() { *(AkTriangle*)$self = AkTriangle(); }
	static int GetSizeOf() { return sizeof(AkTriangle); }
	void Clone(const AkTriangle& other){ *(AkTriangle*)($self) = other; }
}

%extend AkVertex
{
	void Clear() { *(AkVertex*)$self = AkVertex(); }
	static int GetSizeOf() { return sizeof(AkVertex); }
	void Clone(const AkVertex& other){ *(AkVertex*)($self) = other; }
}

%extend AkAcousticSurface
{
	void Clear() { *(AkAcousticSurface*)$self = AkAcousticSurface(); }

	void DeleteName()
	{
		if ($self->strName)
		{
			delete $self->strName;
			$self->strName = NULL;
		}
	}

	static int GetSizeOf() { return sizeof(AkAcousticSurface); }
	
	void Clone(const AkAcousticSurface& other){ *(AkAcousticSurface*)($self) = other; }
}

%extend AkDeviceDescription
{
	void Clear() { *(AkDeviceDescription*)$self = AkDeviceDescription(); }

	static int GetSizeOf() { return sizeof(AkDeviceDescription); }
	
	void Clone(const AkDeviceDescription& other){ *(AkDeviceDescription*)($self) = other; }
}

%extend AkReflectionPathInfo
{
	static int GetSizeOf() { return sizeof(AkReflectionPathInfo); }

	AkVector GetPathPoint(AkUInt32 idx) const
	{
		return $self->pathPoint[idx];
	}

	const AkAcousticSurface& GetAcousticSurface(AkUInt32 idx) const
	{
		return $self->surfaces[idx];
	}

	const AkReal32 GetDiffraction(AkUInt32 idx) const
	{
		return $self->diffraction[idx];
	}
	
	void Clone(const AkReflectionPathInfo& other){ *(AkReflectionPathInfo*)($self) = other; }
}

%extend AkDiffractionPathInfo
{
	static int GetSizeOf() { return sizeof(AkDiffractionPathInfo); }

	AkVector GetNodes(AkUInt32 idx) const
	{
		return $self->nodes[idx];
	}

	const AkReal32 GetAngles(AkUInt32 idx) const
	{
		return $self->angles[idx];
	}
	
	const AkUInt64 GetPortals(AkUInt32 idx) const
	{
		return $self->portals[idx].id;
	}

	const AkUInt64 GetRooms(AkUInt32 idx) const
	{
		return $self->rooms[idx].id;
	}

	void Clone(const AkDiffractionPathInfo& other){ *(AkDiffractionPathInfo*)($self) = other; }
};

%extend AkSourceSettings
{
	void Clear()
	{
		$self->sourceID = AK_INVALID_UNIQUE_ID;
		$self->pMediaMemory = (AkUInt8*)0;
		$self->uMediaSize = 0;
	}
	static int GetSizeOf() { return sizeof(AkSourceSettings); }
	void Clone(const AkSourceSettings& other){ *(AkSourceSettings*)($self) = other; }
}

%extend AkObjectInfo
{
	void Clear() { *(AkObjectInfo*)$self = AkObjectInfo(); }
	static int GetSizeOf() { return sizeof(AkObjectInfo); }
	void Clone(const AkObjectInfo& other){ *(AkObjectInfo*)($self) = other; }
}

%extend AkObstructionOcclusionValues
{
	void Clear() { *(AkObstructionOcclusionValues*)$self = AkObstructionOcclusionValues(); }
	static int GetSizeOf() { return sizeof(AkObstructionOcclusionValues); }
	void Clone(const AkObstructionOcclusionValues& other){ *(AkObstructionOcclusionValues*)($self) = other; }
}

%define %cs_callback(TYPE, CSTYPE)
	%typemap(ctype) TYPE, TYPE& "void*"
	%typemap(in) TYPE  %{ $1 = (TYPE)$input; %}
	%typemap(in) TYPE& %{ $1 = (TYPE*)&$input; %}
	%typemap(imtype) TYPE, TYPE& "CSTYPE"
	%typemap(cstype) TYPE, TYPE& "CSTYPE"
	%typemap(csin) TYPE, TYPE& "$csinput"
%enddef

%cs_callback(AkErrorLogger, AkLogger.ErrorLoggerInteropDelegate)
%cs_callback(AkGetAudioFormat, AkAudioInputManager.AudioFormatInteropDelegate)
%cs_callback(AkGetAudioSamples, AkAudioInputManager.AudioSamplesInteropDelegate)

%{
#if defined(AK_SONY)
#undef SWIGEXPORT
#define SWIGEXPORT __declspec(dllexport)
#endif

#include <AK/SoundEngine/Common/AkSoundEngine.h>
#include <AK/SoundEngine/Common/AkModule.h>
#include <AK/SoundEngine/Common/AkStreamMgrModule.h>
#include <AK/MusicEngine/Common/AkMusicEngine.h>
#include "../Common/AkCallbackSerializer.h"
#include <AK/Tools/Common/AkMonitorError.h>
#include <AK/Tools/Common/AkPlatformFuncs.h>
#include <AK/SoundEngine/Common/AkDynamicDialogue.h>
#include "../Common/AkDynamicSequence_PlaylistItem.h"
#include "../Common/AkDynamicSequence_Playlist.h"
#include <AK/SoundEngine/Common/AkQueryParameters.h>
#include <AK/Plugin/AkReflectGameData.h>
#include <AK/SpatialAudio/Common/AkSpatialAudioTypes.h>
#include <AK/SpatialAudio/Common/AkSpatialAudio.h>

#if defined(AK_LINUX) || defined(AK_NX) || defined(AK_GGP)
#undef AK_SUPPORT_WCHAR
#endif
%}

%ignore AkExternalSourceArray;
%ignore GetExternalSources();
%ignore AK::GetWindowsDevice;

%rename(AkPlaylistItem) AK::SoundEngine::DynamicSequence::PlaylistItem;
%rename(AkPlaylist) AK::SoundEngine::DynamicSequence::Playlist;

// Translate C++ operators into function calls
%rename(Assign) AK::SoundEngine::DynamicSequence::PlaylistItem::operator=;
%rename(IsEqualTo) AK::SoundEngine::DynamicSequence::PlaylistItem::operator==;
%rename(ItemAtIndex) AkArray::operator[];

// Rename DynamicSequence APIs for clarity after SWIG removes namespaces for clarity.
%rename(DynamicSequenceOpen) AK::SoundEngine::DynamicSequence::Open;
%rename(DynamicSequenceClose) AK::SoundEngine::DynamicSequence::Close;
%rename(DynamicSequencePlay) AK::SoundEngine::DynamicSequence::Play;
%rename(DynamicSequencePause) AK::SoundEngine::DynamicSequence::Pause;
%rename(DynamicSequenceResume) AK::SoundEngine::DynamicSequence::Resume;
%rename(DynamicSequenceStop) AK::SoundEngine::DynamicSequence::Stop;
%rename(DynamicSequenceBreak) AK::SoundEngine::DynamicSequence::Break;
%rename(DynamicSequenceLockPlaylist) AK::SoundEngine::DynamicSequence::LockPlaylist;
%rename(DynamicSequenceUnlockPlaylist) AK::SoundEngine::DynamicSequence::UnlockPlaylist;
%rename(DynamicSequenceGetPauseTimes) AK::SoundEngine::DynamicSequence::GetPauseTimes;

// Renamed so that the types have names that users expect within C#
%rename(AkCallbackInfo) AkSerializedCallbackInfo;
%rename(AkEventCallbackInfo) AkSerializedEventCallbackInfo;
%rename(AkMIDIEventCallbackInfo) AkSerializedMIDIEventCallbackInfo;
%rename(AkMarkerCallbackInfo) AkSerializedMarkerCallbackInfo;
%rename(AkDurationCallbackInfo) AkSerializedDurationCallbackInfo;
%rename(AkDynamicSequenceItemCallbackInfo) AkSerializedDynamicSequenceItemCallbackInfo;
%rename(AkMusicSyncCallbackInfo) AkSerializedMusicSyncCallbackInfo;
%rename(AkMusicPlaylistCallbackInfo) AkSerializedMusicPlaylistCallbackInfo;
%rename(AkBankCallbackInfo) AkSerializedBankCallbackInfo;
%rename(AkMonitoringCallbackInfo) AkSerializedMonitoringCallbackInfo;
%rename(AkAudioInterruptionCallbackInfo) AkSerializedAudioInterruptionCallbackInfo;
%rename(AkAudioSourceChangeCallbackInfo) AkSerializedAudioSourceChangeCallbackInfo;

// Provide return values when the sound engine is not initialized
%typemap(out, null="AK_Fail") AKRESULT %{ $result = $1; %}
%typemap(out, null="AK_INVALID_PLAYING_ID") AkPlayingID %{ $result = $1; %}
%typemap(out, null="AK_INVALID_OUTPUT_DEVICE_ID") AkOutputDeviceID %{ $result = $1; %}
%typemap(out, null="AK_INVALID_UNIQUE_ID") AkUniqueID %{ $result = $1; %}
%typemap(out, null="AK_INVALID_POOL_ID") AkMemPoolId %{ $result = $1; %}
%typemap(out, null="AK_INVALID_CHANNELMASK") AkChannelMask %{ $result = $1; %}

// NOTE: From Unity side, script execution order is crucial: API calls must be made after sound engine is initialized and before it is terminated.
// This code will check that the sound engine is initialized for every function call.  Must be done AFTER "Init" is declared (in the inline section above). It's inserted to all APIs.
// Order: Must be before %include <AkUnityApiHeader.h>
%include "SwigExceptionSwitch.h"

// Expose AkArray Iterator: Step 1: Tell SWIG to use a global proxy class for the nested AkArray::Iterator class.
%ignore AkGrowByPolicy_Legacy;
%ignore AkGrowByPolicy_NoGrow;
%ignore AkGrowByPolicy_Proportional;
%include "AkArrayIterator.h";
%{
#include "../Common/AkArrayProxy.h"
%}
%include "AkArrayProxy.h"

%{
SWIGINTERN AkMIDIEventTypes AkMIDIEvent_byType_get(AkMIDIEvent *info) { return (AkMIDIEventTypes)info->byType; }
SWIGINTERN void AkMIDIEvent_byType_set(AkMIDIEvent *info, AkMIDIEventTypes value) { info->byType = (AkUInt8)value; }

SWIGINTERN AkMidiNoteNo AkMIDIEvent_byOnOffNote_get(AkMIDIEvent *info) { return (AkMidiNoteNo)info->NoteOnOff.byNote; }
SWIGINTERN void AkMIDIEvent_byOnOffNote_set(AkMIDIEvent *info, AkMidiNoteNo value) { info->NoteOnOff.byNote = value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byVelocity_get(AkMIDIEvent *info) { return info->NoteOnOff.byVelocity; }
SWIGINTERN void AkMIDIEvent_byVelocity_set(AkMIDIEvent *info, AkUInt8 value) { info->NoteOnOff.byVelocity = value; }

SWIGINTERN AkMIDICcTypes AkMIDIEvent_byCc_get(AkMIDIEvent *info) { return (AkMIDICcTypes)info->Cc.byCc; }
SWIGINTERN void AkMIDIEvent_byCc_set(AkMIDIEvent *info, AkMIDICcTypes value) { info->Cc.byCc = (AkUInt8)value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byCcValue_get(AkMIDIEvent *info) { return info->Cc.byValue; }
SWIGINTERN void AkMIDIEvent_byCcValue_set(AkMIDIEvent *info, AkUInt8 value) { info->Cc.byValue = value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byValueLsb_get(AkMIDIEvent *info) { return info->PitchBend.byValueLsb; }
SWIGINTERN void AkMIDIEvent_byValueLsb_set(AkMIDIEvent *info, AkUInt8 value) { info->PitchBend.byValueLsb = value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byValueMsb_get(AkMIDIEvent *info) { return info->PitchBend.byValueMsb; }
SWIGINTERN void AkMIDIEvent_byValueMsb_set(AkMIDIEvent *info, AkUInt8 value) { info->PitchBend.byValueMsb = value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byAftertouchNote_get(AkMIDIEvent *info) { return info->NoteAftertouch.byNote; }
SWIGINTERN void AkMIDIEvent_byAftertouchNote_set(AkMIDIEvent *info, AkUInt8 value) { info->NoteAftertouch.byNote = value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byNoteAftertouchValue_get(AkMIDIEvent *info) { return info->NoteAftertouch.byValue; }
SWIGINTERN void AkMIDIEvent_byNoteAftertouchValue_set(AkMIDIEvent *info, AkUInt8 value) { info->NoteAftertouch.byValue = value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byChanAftertouchValue_get(AkMIDIEvent *info) { return info->ChanAftertouch.byValue; }
SWIGINTERN void AkMIDIEvent_byChanAftertouchValue_set(AkMIDIEvent *info, AkUInt8 value) { info->ChanAftertouch.byValue = value; }

SWIGINTERN AkUInt8 AkMIDIEvent_byProgramNum_get(AkMIDIEvent *info) { return info->ProgramChange.byProgramNum; }
SWIGINTERN void AkMIDIEvent_byProgramNum_set(AkMIDIEvent *info, AkUInt8 value) { info->ProgramChange.byProgramNum = value; }

SWIGINTERN AkUInt16 AkMIDIEvent_uCmd_get(AkMIDIEvent *info) { return info->WwiseCmd.uCmd; }
SWIGINTERN void AkMIDIEvent_uCmd_set(AkMIDIEvent *info, AkUInt16 value) { info->WwiseCmd.uCmd = value; }

SWIGINTERN AkUInt32 AkMIDIEvent_uArg_get(AkMIDIEvent *info) { return info->WwiseCmd.uArg; }
SWIGINTERN void AkMIDIEvent_uArg_set(AkMIDIEvent *info, AkUInt32 value) { info->WwiseCmd.uArg = value; }
%}

RESUME_SWIG_EXCEPTIONS

%include "../Common/AkDynamicSequence_PlaylistItem.h"
%template(AkPlaylistArray) AkArray<AK::SoundEngine::DynamicSequence::PlaylistItem, AK::SoundEngine::DynamicSequence::PlaylistItem const &, ArrayPoolDefault,AkGrowByPolicy_Proportional>;
// Expose AkArray Iterator: Step 2: Tell C++ compiler to use proxy class.
%{
typedef AkArray<AK::SoundEngine::DynamicSequence::PlaylistItem, AK::SoundEngine::DynamicSequence::PlaylistItem const &,ArrayPoolDefault,AkGrowByPolicy_Proportional>::Iterator AkIterator;
%}
%include "../Common/AkDynamicSequence_Playlist.h"

%ignore AkStreamMgrSettings::AkStreamMgrSettings();
%ignore AkDeviceSettings::AkDeviceSettings();
%ignore AkInitSettings::AkInitSettings();
%ignore AkPlatformInitSettings::AkPlatformInitSettings();
%ignore AkMusicSettings::AkMusicSettings();
%ignore AkUnityPlatformSpecificSettings::AkUnityPlatformSpecificSettings();

CANCEL_SWIG_EXCEPTIONS(AkSpatialAudioInitSettings::AkSpatialAudioInitSettings());

CANCEL_SWIG_EXCEPTIONS(AK::SoundEngine::IsInitialized);
CANCEL_SWIG_EXCEPTIONS(AK::GetDeviceIDFromName);
CANCEL_SWIG_EXCEPTIONS(AK::GetWindowsDeviceName);
CANCEL_SWIG_EXCEPTIONS(AK::GetWindowsDeviceCount);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::AkChannelConfig);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::Clear);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::SetStandard);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::SetStandardOrAnonymous);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::SetAnonymous);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::SetAmbisonic);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::IsValid);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::Serialize);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::Deserialize);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::RemoveLFE);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::RemoveCenter);
CANCEL_SWIG_EXCEPTIONS(AkChannelConfig::IsChannelConfigSupported);
CANCEL_SWIG_EXCEPTIONS(AkCallbackSerializer::Lock);
CANCEL_SWIG_EXCEPTIONS(AkCallbackSerializer::Unlock);

// ignored methods with "const AkGameObjectID*" arguments to fix WG-31581
%ignore AK::SoundEngine::SetListeners;
%ignore AK::SoundEngine::SetDefaultListeners;
%ignore AK::SoundEngine::AddOutput;
%ignore AK::SoundEngine::GetGameInputDeviceID;

SWIG_EXCEPTION(SetListeners);
SWIG_EXCEPTION(SetDefaultListeners);
SWIG_EXCEPTION(AddOutput);

SWIG_EXCEPTION(AkCallbackSerializer::Init);
SWIG_EXCEPTION(AkCallbackSerializer::AudioSourceChangeCallbackFunc);

SWIG_EXCEPTION(RegisterSpatialAudioListener);
SWIG_EXCEPTION(UnregisterSpatialAudioListener);
SWIG_EXCEPTION(SetRoom);
SWIG_EXCEPTION(SetRoomPortal);

// API declarations blob.
%include <AkUnityApiHeader.h>

//Reset the exception to nothing.  This is IMPORTANT, the constructors/destructors are generated after all the interface 
//so to avoid the IsInitialized check in the AkInitSettings structure constructor, the exception must be reset.
PAUSE_SWIG_EXCEPTIONS

%include "AkSoundEngineStubs.h"
%{
#include "../Common/AkSoundEngineStubs.h"
%}