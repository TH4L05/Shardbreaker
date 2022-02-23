#include "stdafx.h"
#include "AkCallbackSerializer.h"
#include <AK/Tools/Common/AkLock.h>
#include <AK/Tools/Common/AkAutoLock.h>
#include <stdio.h>
#include "ExtraCallbacks.h"
#include <AK/Tools/Common/AkAssert.h>
#include <AK/SoundEngine/Common/AkMemoryMgr.h>


AkMIDIEventTypes AkSerializedMIDIEventCallbackInfo_byType_get(AkSerializedMIDIEventCallbackInfo *info) { return (AkMIDIEventTypes)info->byType; }
AkMidiNoteNo AkSerializedMIDIEventCallbackInfo_byOnOffNote_get(AkSerializedMIDIEventCallbackInfo *info) { return (AkMidiNoteNo)info->byParam1; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byVelocity_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam2; }
AkMIDICcTypes AkSerializedMIDIEventCallbackInfo_byCc_get(AkSerializedMIDIEventCallbackInfo *info) { return (AkMIDICcTypes)info->byParam1; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byCcValue_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam2; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byValueLsb_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam1; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byValueMsb_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam2; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byAftertouchNote_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam1; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byNoteAftertouchValue_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam2; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byChanAftertouchValue_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam1; }
AkUInt8 AkSerializedMIDIEventCallbackInfo_byProgramNum_get(AkSerializedMIDIEventCallbackInfo *info) { return info->byParam1; }

AkSerializedCallbackHeader* AkSerializedCallbackHeader_pNext_get(AkSerializedCallbackHeader *info)
{
	if (!info)
		return nullptr;

	auto retVal = info->pNext;
	AK::MemoryMgr::Free(AkMemID_Integration, info);
	return retVal;
}

template<typename InfoStruct>
struct AkSerializedCallbackData : AkSerializedCallbackHeader, InfoStruct {};

template<typename InfoStruct>
AkSerializedCallbackData<InfoStruct>* AllocCallbackStruct(void* pPackage, AkUInt32 eType, size_t uStringLength)
{
	auto uItemSize = sizeof(AkSerializedCallbackHeader) + sizeof(InfoStruct) + uStringLength;
	auto info = static_cast<AkSerializedCallbackData<InfoStruct>*>(AK::MemoryMgr::Malloc(AkMemID_Integration, uItemSize));
	if (!info)
		return nullptr;

	info->pPackage = pPackage;
	info->eType = eType;
	info->pNext = nullptr;
	return info;
}

namespace AkCallbackSerializerList
{
	CAkLock Lock;
	AkSerializedCallbackHeader* First{};
	AkSerializedCallbackHeader* Last{};
}

AkSerializedCallbackHeader* GetList()
{
	using namespace AkCallbackSerializerList;
	AkAutoLock<CAkLock> autoLock(Lock);
	auto retVal = First;
	First = Last = nullptr;
	return retVal;
}

void PushToList(AkSerializedCallbackHeader* newHeader)
{
	using namespace AkCallbackSerializerList;
	AkAutoLock<CAkLock> autoLock(Lock);
	if (First)
		Last = Last->pNext = newHeader;
	else
		First = Last = newHeader;
}

namespace AkCallbackSerializerHelper
{
	bool IsInitialized;
}

AKRESULT AkCallbackSerializer::Init()
{
	AkCallbackSerializerHelper::IsInitialized = true;
	return AK_Success;
}

void AkCallbackSerializer::Term()
{
	AK::Monitor::SetLocalOutput();
	for (auto iter = GetList(); iter; iter = AkSerializedCallbackHeader_pNext_get(iter)) {}
	AkCallbackSerializerHelper::IsInitialized = false;
}

void* AkCallbackSerializer::Lock()
{
	return GetList();
}

void AkCallbackSerializer::Unlock()
{
}

void AkCallbackSerializer::EventCallback(AkCallbackType in_eType, AkCallbackInfo* in_pCallbackInfo)
{
	if (!in_pCallbackInfo)
		return;

	switch (in_eType)
	{
	case AK_EndOfEvent:
	case AK_Starvation:
	case AK_MusicPlayStarted:
	{
		const auto copyFrom = static_cast<AkEventCallbackInfo*>(in_pCallbackInfo);
		auto info = AllocCallbackStruct<AkSerializedEventCallbackInfo>(in_pCallbackInfo->pCookie, in_eType, 0);
		if (!info)
			return;

		info->pCookie = copyFrom->pCookie;
		info->gameObjID = copyFrom->gameObjID;
		info->playingID = copyFrom->playingID;
		info->eventID = copyFrom->eventID;
		PushToList(info);
		break;
	}
	case AK_EndOfDynamicSequenceItem:
	{
		const auto copyFrom = static_cast<AkDynamicSequenceItemCallbackInfo*>(in_pCallbackInfo);
		auto info = AllocCallbackStruct<AkSerializedDynamicSequenceItemCallbackInfo>(in_pCallbackInfo->pCookie, in_eType, 0);
		if (!info)
			return;

		info->pCookie = copyFrom->pCookie;
		info->gameObjID = copyFrom->gameObjID;
		info->playingID = copyFrom->playingID;
		info->audioNodeID = copyFrom->audioNodeID;
		info->pCustomInfo = copyFrom->pCustomInfo;
		PushToList(info);
		break;
	}
	case AK_Marker:
	{
		const auto copyFrom = static_cast<AkMarkerCallbackInfo*>(in_pCallbackInfo);
		const auto uLength = copyFrom->strLabel ? strlen(copyFrom->strLabel) : 0;
		auto info = AllocCallbackStruct<AkSerializedMarkerCallbackInfo>(in_pCallbackInfo->pCookie, in_eType, uLength);
		if (!info)
			return;

		info->pCookie = copyFrom->pCookie;
		info->gameObjID = copyFrom->gameObjID;
		info->playingID = copyFrom->playingID;
		info->eventID = copyFrom->eventID;
		info->uIdentifier = copyFrom->uIdentifier;
		info->uPosition = copyFrom->uPosition;

		if (uLength)
			memcpy(info->strLabel, copyFrom->strLabel, uLength);
		info->strLabel[uLength] = 0;

		PushToList(info);
		break;
	}
	case AK_Duration:
	{
		const auto copyFrom = static_cast<AkDurationCallbackInfo*>(in_pCallbackInfo);
		auto info = AllocCallbackStruct<AkSerializedDurationCallbackInfo>(in_pCallbackInfo->pCookie, in_eType, 0);
		if (!info)
			return;

		info->pCookie = copyFrom->pCookie;
		info->gameObjID = copyFrom->gameObjID;
		info->playingID = copyFrom->playingID;
		info->eventID = copyFrom->eventID;
		info->fDuration = copyFrom->fDuration;
		info->fEstimatedDuration = copyFrom->fEstimatedDuration;
		info->audioNodeID = copyFrom->audioNodeID;
		info->mediaID = copyFrom->mediaID;
		info->bStreaming = copyFrom->bStreaming;
		PushToList(info);
		break;
	}
	case AK_MIDIEvent:
	{
		const auto copyFrom = static_cast<AkMIDIEventCallbackInfo*>(in_pCallbackInfo);
		auto info = AllocCallbackStruct<AkSerializedMIDIEventCallbackInfo>(in_pCallbackInfo->pCookie, in_eType, 0);
		if (!info)
			return;

		info->pCookie = copyFrom->pCookie;
		info->gameObjID = copyFrom->gameObjID;
		info->playingID = copyFrom->playingID;
		info->eventID = copyFrom->eventID;

		info->byType = copyFrom->midiEvent.byType;
		info->byChan = copyFrom->midiEvent.byChan;
		info->byParam1 = copyFrom->midiEvent.Gen.byParam1;
		info->byParam2 = copyFrom->midiEvent.Gen.byParam2;
		PushToList(info);
		break;
	}
	case AK_MusicSyncBeat:
	case AK_MusicSyncBar:
	case AK_MusicSyncEntry:
	case AK_MusicSyncExit:
	case AK_MusicSyncGrid:
	case AK_MusicSyncPoint:
	case AK_MusicSyncUserCue:
	{
		const auto copyFrom = static_cast<AkMusicSyncCallbackInfo*>(in_pCallbackInfo);
		const auto uLength = in_eType == AK_MusicSyncUserCue && copyFrom->pszUserCueName ? strlen(copyFrom->pszUserCueName) : 0;
		auto info = AllocCallbackStruct<AkSerializedMusicSyncCallbackInfo>(in_pCallbackInfo->pCookie, in_eType, uLength);
		if (!info)
			return;

		info->pCookie = copyFrom->pCookie;
		info->gameObjID = copyFrom->gameObjID;
		info->playingID = copyFrom->playingID;

		info->segmentInfo_iCurrentPosition = copyFrom->segmentInfo.iCurrentPosition;
		info->segmentInfo_iPreEntryDuration = copyFrom->segmentInfo.iPreEntryDuration;
		info->segmentInfo_iActiveDuration = copyFrom->segmentInfo.iActiveDuration;
		info->segmentInfo_iPostExitDuration = copyFrom->segmentInfo.iPostExitDuration;
		info->segmentInfo_iRemainingLookAheadTime = copyFrom->segmentInfo.iRemainingLookAheadTime;
		info->segmentInfo_fBeatDuration = copyFrom->segmentInfo.fBeatDuration;
		info->segmentInfo_fBarDuration = copyFrom->segmentInfo.fBarDuration;
		info->segmentInfo_fGridDuration = copyFrom->segmentInfo.fGridDuration;
		info->segmentInfo_fGridOffset = copyFrom->segmentInfo.fGridOffset;

		info->musicSyncType = copyFrom->musicSyncType;

		if (uLength)
			memcpy(info->userCueName, copyFrom->pszUserCueName, uLength);
		info->userCueName[uLength] = 0;

		PushToList(info);
		break;
	}
	case AK_MusicPlaylistSelect:
	{
		const auto copyFrom = static_cast<AkMusicPlaylistCallbackInfo*>(in_pCallbackInfo);
		auto info = AllocCallbackStruct<AkSerializedMusicPlaylistCallbackInfo>(in_pCallbackInfo->pCookie, in_eType, 0);
		if (!info)
			return;

		info->pCookie = copyFrom->pCookie;
		info->gameObjID = copyFrom->gameObjID;
		info->playingID = copyFrom->playingID;
		info->eventID = copyFrom->eventID;

		info->playlistID = copyFrom->playlistID;
		info->uNumPlaylistItems = copyFrom->uNumPlaylistItems;
		info->uPlaylistSelection = copyFrom->uPlaylistSelection;
		info->uPlaylistItemDone = copyFrom->uPlaylistItemDone;
		PushToList(info);
		break;
	}
	default:
		break;
	}
}

namespace AkCallbackSerializerHelper
{
	void LocalOutput(AK::Monitor::ErrorCode in_eErrorCode, const AkOSChar* in_pszError, AK::Monitor::ErrorLevel in_eErrorLevel, AkPlayingID in_playingID, AkGameObjectID in_gameObjID)
	{
		//Ak_Monitoring isn't defined on the regular SDK.  It's a modification that only the C# side sees.
		const auto uLength = AKPLATFORM::OsStrLen(in_pszError) + 1;
		auto info = AllocCallbackStruct<AkSerializedMonitoringCallbackInfo>(nullptr, AK_Monitoring_Val, uLength * sizeof(AkOSChar));
		if (!info)
			return;

		info->errorCode = in_eErrorCode;
		info->errorLevel = in_eErrorLevel;
		info->playingID = in_playingID;
		info->gameObjID = in_gameObjID;

		if (uLength)
			AKPLATFORM::SafeStrCpy(info->message, in_pszError, uLength);
		info->message[uLength] = 0;

		PushToList(info);
	}
}

void AkCallbackSerializer::SetLocalOutput(AkUInt32 in_uErrorLevel)
{
	AK::Monitor::LocalOutputFunc LocalOutputFunc = in_uErrorLevel ? AkCallbackSerializerHelper::LocalOutput : nullptr;
	AK::Monitor::SetLocalOutput(in_uErrorLevel, LocalOutputFunc);
}

void AkCallbackSerializer::BankCallback(AkUInt32 in_bankID, void* in_pInMemoryBankPtr, AKRESULT in_eLoadResult, void *in_pCookie)
{
	if (!in_pCookie)
		return;

	// Ak_Bank_Val is a customization only for C#.
	auto info = AllocCallbackStruct<AkSerializedBankCallbackInfo>(in_pCookie, AK_Bank_Val, 0);
	if (!info)
		return;

	info->bankID = in_bankID;
	info->inMemoryBankPtr = in_pInMemoryBankPtr;
	info->loadResult = in_eLoadResult;
	PushToList(info);
}

AKRESULT AkCallbackSerializer::AudioInterruptionCallbackFunc(bool in_bEnterInterruption, void* in_pCookie)
{
	// AK_AudioInterruption_Val is a customization only for C#.
	auto info = AllocCallbackStruct<AkSerializedAudioInterruptionCallbackInfo>(in_pCookie, AK_AudioInterruption_Val, 0);
	if (!info)
		return AK_Fail;

	info->bEnterInterruption = in_bEnterInterruption;
	PushToList(info);
	return AK_Success;
}

AKRESULT AkCallbackSerializer::AudioSourceChangeCallbackFunc(bool in_bOtherAudioPlaying, void* in_pCookie)
{
	// On iOS, this user callback is triggered by the initial WakeupFromSuspend() call
	// This is before the sound engine is initialized. 
	// Bypass this call.
	if (!AkCallbackSerializerHelper::IsInitialized)
		return AK_Cancelled;

	// AK_AudioSourceChange_Val is a customization only for C#.
	auto info = AllocCallbackStruct<AkSerializedAudioSourceChangeCallbackInfo>(in_pCookie, AK_AudioSourceChange_Val, 0);
	if (!info)
		return AK_Fail;

	info->bOtherAudioPlaying = in_bOtherAudioPlaying;
	PushToList(info);
	return AK_Success;
}
