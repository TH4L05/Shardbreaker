#include <AK/SoundEngine/Common/AkCallback.h>
#include <AK/Tools/Common/AkMonitorError.h>

#include "SwigExceptionSwitch.h"


PAUSE_SWIG_EXCEPTIONS
#ifdef SWIG
%feature("immutable");
#endif // SWIG

#if SWIG
%extend AkSerializedCallbackHeader
{
	AkSerializedCallbackHeader* pNext;
}
#else
struct AkSerializedCallbackHeader* AkSerializedCallbackHeader_pNext_get(struct AkSerializedCallbackHeader *info);
#endif // SWIG

struct AkSerializedCallbackHeader
{
	void* pPackage; //The C# CallbackPackage to return to C#
	AkSerializedCallbackHeader* pNext; //Pointer to the next callback
	AkUInt32 eType; //The type of structure following, generally an enumerate of AkCallbackType

	void* GetData() const { return (void*)(this + 1); }
};

struct AkSerializedCallbackInfo
{
	void* pCookie; ///< User data, passed to PostEvent()
	AkGameObjectID gameObjID; ///< Game object ID
};

struct AkSerializedEventCallbackInfo : AkSerializedCallbackInfo
{
	AkPlayingID		playingID;		///< Playing ID of Event, returned by PostEvent()
	AkUniqueID		eventID;		///< Unique ID of Event, passed to PostEvent()
};

enum AkMIDIEventTypes
{
	NOTE_OFF = 0x80,
	NOTE_ON = 0x90,
	NOTE_AFTERTOUCH = 0xa0,
	CONTROLLER = 0xb0,
	PROGRAM_CHANGE = 0xc0,
	CHANNEL_AFTERTOUCH = 0xd0,
	PITCH_BEND = 0xe0,
	SYSEX = 0xf0,
	ESCAPE = 0xf7,
	META = 0xff,
};

enum AkMIDICcTypes
{
	BANK_SELECT_COARSE = 0,
	MOD_WHEEL_COARSE = 1,
	BREATH_CTRL_COARSE = 2,
	CTRL_3_COARSE = 3,
	FOOT_PEDAL_COARSE = 4,
	PORTAMENTO_COARSE = 5,
	DATA_ENTRY_COARSE = 6,
	VOLUME_COARSE = 7,
	BALANCE_COARSE = 8,
	CTRL_9_COARSE = 9,
	PAN_POSITION_COARSE = 10,
	EXPRESSION_COARSE = 11,
	EFFECT_CTRL_1_COARSE = 12,
	EFFECT_CTRL_2_COARSE = 13,
	CTRL_14_COARSE = 14,
	CTRL_15_COARSE = 15,
	GEN_SLIDER_1 = 16,
	GEN_SLIDER_2 = 17,
	GEN_SLIDER_3 = 18,
	GEN_SLIDER_4 = 19,
	CTRL_20_COARSE = 20,
	CTRL_21_COARSE = 21,
	CTRL_22_COARSE = 22,
	CTRL_23_COARSE = 23,
	CTRL_24_COARSE = 24,
	CTRL_25_COARSE = 25,
	CTRL_26_COARSE = 26,
	CTRL_27_COARSE = 27,
	CTRL_28_COARSE = 28,
	CTRL_29_COARSE = 29,
	CTRL_30_COARSE = 30,
	CTRL_31_COARSE = 31,
	BANK_SELECT_FINE = 32,
	MOD_WHEEL_FINE = 33,
	BREATH_CTRL_FINE = 34,
	CTRL_3_FINE = 35,
	FOOT_PEDAL_FINE = 36,
	PORTAMENTO_FINE = 37,
	DATA_ENTRY_FINE = 38,
	VOLUME_FINE = 39,
	BALANCE_FINE = 40,
	CTRL_9_FINE = 41,
	PAN_POSITION_FINE = 42,
	EXPRESSION_FINE = 43,
	EFFECT_CTRL_1_FINE = 44,
	EFFECT_CTRL_2_FINE = 45,
	CTRL_14_FINE = 46,
	CTRL_15_FINE = 47,
	CTRL_20_FINE = 52,
	CTRL_21_FINE = 53,
	CTRL_22_FINE = 54,
	CTRL_23_FINE = 55,
	CTRL_24_FINE = 56,
	CTRL_25_FINE = 57,
	CTRL_26_FINE = 58,
	CTRL_27_FINE = 59,
	CTRL_28_FINE = 60,
	CTRL_29_FINE = 61,
	CTRL_30_FINE = 62,
	CTRL_31_FINE = 63,
	HOLD_PEDAL = 64,
	PORTAMENTO_ON_OFF = 65,
	SUSTENUTO_PEDAL = 66,
	SOFT_PEDAL = 67,
	LEGATO_PEDAL = 68,
	HOLD_PEDAL_2 = 69,
	SOUND_VARIATION = 70,
	SOUND_TIMBRE = 71,
	SOUND_RELEASE_TIME = 72,
	SOUND_ATTACK_TIME = 73,
	SOUND_BRIGHTNESS = 74,
	SOUND_CTRL_6 = 75,
	SOUND_CTRL_7 = 76,
	SOUND_CTRL_8 = 77,
	SOUND_CTRL_9 = 78,
	SOUND_CTRL_10 = 79,
	GENERAL_BUTTON_1 = 80,
	GENERAL_BUTTON_2 = 81,
	GENERAL_BUTTON_3 = 82,
	GENERAL_BUTTON_4 = 83,
	REVERB_LEVEL = 91,
	TREMOLO_LEVEL = 92,
	CHORUS_LEVEL = 93,
	CELESTE_LEVEL = 94,
	PHASER_LEVEL = 95,
	DATA_BUTTON_P1 = 96,
	DATA_BUTTON_M1 = 97,
	NON_REGISTER_COARSE = 98,
	NON_REGISTER_FINE = 99,
	ALL_SOUND_OFF = 120,
	ALL_CONTROLLERS_OFF = 121,
	LOCAL_KEYBOARD = 122,
	ALL_NOTES_OFF = 123,
	OMNI_MODE_OFF = 124,
	OMNI_MODE_ON = 125,
	OMNI_MONOPHONIC_ON = 126,
	OMNI_POLYPHONIC_ON = 127,
};

#if SWIG
%extend AkSerializedMIDIEventCallbackInfo
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
}
#else
AkMIDIEventTypes AkSerializedMIDIEventCallbackInfo_byType_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkMidiNoteNo AkSerializedMIDIEventCallbackInfo_byOnOffNote_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byVelocity_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkMIDICcTypes AkSerializedMIDIEventCallbackInfo_byCc_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byCcValue_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byValueLsb_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byValueMsb_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byAftertouchNote_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byNoteAftertouchValue_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byChanAftertouchValue_get(struct AkSerializedMIDIEventCallbackInfo *info);
AkUInt8 AkSerializedMIDIEventCallbackInfo_byProgramNum_get(struct AkSerializedMIDIEventCallbackInfo *info);
#endif // SWIG

struct AkSerializedMIDIEventCallbackInfo : AkSerializedEventCallbackInfo
{
	// AkMIDIEvent expanded to prevent packing issues
	// BEGIN_AKMIDIEVENT_EXPANSION

	AkUInt8 byType; // (Ak_MIDI_EVENT_TYPE_)
	AkMidiChannelNo byChan;

	AkUInt8		byParam1;
	AkUInt8		byParam2;

	// END_AKMIDIEVENT_EXPANSION
};

struct AkSerializedMarkerCallbackInfo : AkSerializedEventCallbackInfo
{
	AkUInt32	uIdentifier;		///< Cue point identifier
	AkUInt32	uPosition;			///< Position in the cue point (unit: sample frames)
	char		strLabel[1];		///< Label of the marker, read from the file
};

struct AkSerializedDurationCallbackInfo : AkSerializedEventCallbackInfo
{
	AkReal32	fDuration;				///< Duration of the sound (unit: milliseconds)
	AkReal32	fEstimatedDuration;		///< Estimated duration of the sound depending on source settings such as pitch. (unit: milliseconds)
	AkUniqueID	audioNodeID;			///< Audio Node ID of playing item
	AkUniqueID  mediaID;				///< Media ID of playing item. (corresponds to 'ID' attribute of 'File' element in SoundBank metadata file)
	bool bStreaming;				///< True if source is streaming, false otherwise.
};

struct AkSerializedDynamicSequenceItemCallbackInfo : AkSerializedCallbackInfo
{
	AkPlayingID playingID;			///< Playing ID of Dynamic Sequence, returned by AK::SoundEngine:DynamicSequence::Open()
	AkUniqueID	audioNodeID;		///< Audio Node ID of finished item
	void*		pCustomInfo;		///< Custom info passed to the DynamicSequence::Open function
};

struct AkSerializedMusicSyncCallbackInfo : AkSerializedCallbackInfo
{
	AkPlayingID playingID;			///< Playing ID of Event, returned by PostEvent()
	//AkSegmentInfo segmentInfo; ///< Segment information corresponding to the segment triggering this callback.

	// AkSegmentInfo expanded to prevent packing issues
	// BEGIN_AKSEGMENTINFO_EXPANSION
	AkTimeMs		segmentInfo_iCurrentPosition;		///< Current position of the segment, relative to the Entry Cue, in milliseconds. Range is [-iPreEntryDuration, iActiveDuration+iPostExitDuration].
	AkTimeMs		segmentInfo_iPreEntryDuration;		///< Duration of the pre-entry region of the segment, in milliseconds.
	AkTimeMs		segmentInfo_iActiveDuration;		///< Duration of the active region of the segment (between the Entry and Exit Cues), in milliseconds.
	AkTimeMs		segmentInfo_iPostExitDuration;		///< Duration of the post-exit region of the segment, in milliseconds.
	AkTimeMs		segmentInfo_iRemainingLookAheadTime;///< Number of milliseconds remaining in the "looking-ahead" state of the segment, when it is silent but streamed tracks are being prefetched.
	AkReal32		segmentInfo_fBeatDuration;			///< Beat Duration in seconds.
	AkReal32		segmentInfo_fBarDuration;			///< Bar Duration in seconds.
	AkReal32		segmentInfo_fGridDuration;			///< Grid duration in seconds.
	AkReal32		segmentInfo_fGridOffset;			///< Grid offset in seconds.
	// END_AKSEGMENTINFO_EXPANSION

	AkCallbackType musicSyncType;	///< Would be either AK_MusicSyncEntry, AK_MusicSyncBeat, AK_MusicSyncBar, AK_MusicSyncExit, AK_MusicSyncGrid, AK_MusicSyncPoint or AK_MusicSyncUserCue.
	char  userCueName[1];
};

struct AkSerializedMusicPlaylistCallbackInfo : public AkSerializedEventCallbackInfo
{
	AkUniqueID playlistID;			///< ID of playlist node
	AkUInt32 uNumPlaylistItems;		///< Number of items in playlist node (may be segments or other playlists)
	AkUInt32 uPlaylistSelection;	///< Selection: set by sound engine, modified by callback function (if not in range 0 <= uPlaylistSelection < uNumPlaylistItems then ignored).
	AkUInt32 uPlaylistItemDone;		///< Playlist node done: set by sound engine, modified by callback function (if set to anything but 0 then the current playlist item is done, and uPlaylistSelection is ignored)
};

struct AkSerializedBankCallbackInfo
{
	AkUInt32 bankID;
	void* inMemoryBankPtr; // changed from AkUIntPtr to 'void*'
	AKRESULT loadResult;
};

struct AkSerializedMonitoringCallbackInfo
{
	AK::Monitor::ErrorCode errorCode;
	AK::Monitor::ErrorLevel errorLevel;
	AkPlayingID playingID;
	AkGameObjectID gameObjID;
	AkOSChar message[1];
};

struct AkSerializedAudioInterruptionCallbackInfo
{
	bool bEnterInterruption;
};

struct AkSerializedAudioSourceChangeCallbackInfo
{
	bool bOtherAudioPlaying;
};

#ifdef SWIG
%feature("immutable", "");
#endif // SWIG
RESUME_SWIG_EXCEPTIONS

/// This class allows the Sound Engine callbacks to be processed in the user thread instead of the audio thread.
/// This is done by accumulating the callback generating events until the game calls CallbackSerializer::PostCallbacks().
/// All pending callbacks will be done at that point.  This removes the need for external synchronization for the callback
/// functions that the game would need otherwise.
class AkCallbackSerializer
{
public:
	static AKRESULT Init();

	PAUSE_SWIG_EXCEPTIONS
	static void Term();
	RESUME_SWIG_EXCEPTIONS

	static void* Lock();
	static void Unlock();

	static void SetLocalOutput(AkUInt32 in_uErrorLevel);

	static void EventCallback(AkCallbackType in_eType, AkCallbackInfo* in_pCallbackInfo);
	static void BankCallback(AkUInt32 in_bankID, void* in_pInMemoryBankPtr, AKRESULT in_eLoadResult, void *in_pCookie);

	static AKRESULT AudioInterruptionCallbackFunc(bool in_bEnterInterruption, void* in_pCookie);
	static AKRESULT AudioSourceChangeCallbackFunc(bool in_bOtherAudioPlaying, void* in_pCookie);
};
