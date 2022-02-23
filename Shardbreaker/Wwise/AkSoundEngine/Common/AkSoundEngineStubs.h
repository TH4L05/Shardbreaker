//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

#ifndef AK_SOUNDENGINE_STUBS_H_
#define AK_SOUNDENGINE_STUBS_H_

#if defined AK_WIN || defined AK_XBOXONE
#define AK_STDCALL __stdcall
#else
#define AK_STDCALL
#endif

typedef void (AK_STDCALL * AkErrorLogger)(const char*);
void SetErrorLogger(AkErrorLogger logger = NULL);

typedef void (AK_STDCALL * AkGetAudioFormat)(AkPlayingID playingID, AkAudioFormat* format);
typedef bool (AK_STDCALL * AkGetAudioSamples)(AkPlayingID playingID, AkSampleType* samples, AkUInt32 channelIndex, AkUInt32 frames);
void SetAudioInputCallbacks(AkGetAudioSamples getAudioSamples, AkGetAudioFormat getAudioFormat);

struct AkUnityPlatformSpecificSettings
{
#if defined(AK_XBOX)
	AkUInt32 ApuHeapCachedSize;
	AkUInt32 ApuHeapNonCachedSize;
#endif
};

struct AkCommunicationSettings
{
	AkCommunicationSettings();

#ifdef AK_NX
	AkThreadProperties commsThreadProperties; ///< Communication & Connection threading properties (its default priority is AK_THREAD_PRIORITY_ABOVENORMAL)
#endif

	AkUInt32 uPoolSize;
	AkUInt16 uDiscoveryBroadcastPort;
	AkUInt16 uCommandPort;
	AkUInt16 uNotificationPort;

	/// Allows selecting the communication system used to connect remotely the Authoring tool on the device.
	enum AkCommSystem
	{
		AkCommSystem_Socket,	/// The recommended default communication system
		AkCommSystem_HTCS 		/// HTCS when available only, will default to AkCommSystem_Socket if the HTCS system is not available.
	};
	AkCommSystem commSystem;

	bool bInitSystemLib;

	char szAppNetworkName[64];
};

struct AkInitializationSettings
{
	AkInitializationSettings();
	~AkInitializationSettings();

	AkStreamMgrSettings streamMgrSettings;
	AkDeviceSettings deviceSettings;
	AkInitSettings initSettings;
	AkPlatformInitSettings platformSettings;
	AkMusicSettings musicSettings;

	AkUnityPlatformSpecificSettings unityPlatformSpecificSettings;

	bool useAsyncOpen = false;
};

#if SWIG
%extend AkSerializedExternalSourceInfo
{
	AkUInt32 iExternalSrcCookie;
	AkCodecID idCodec;
	AkOSChar* szFile;
	void* pInMemory;
	AkUInt32 uiMemorySize;
	AkFileID idFile;
}
#else
AkUInt32 AkSerializedExternalSourceInfo_iExternalSrcCookie_get(struct AkSerializedExternalSourceInfo *info);
void AkSerializedExternalSourceInfo_iExternalSrcCookie_set(struct AkSerializedExternalSourceInfo *info, AkUInt32 value);

AkCodecID AkSerializedExternalSourceInfo_idCodec_get(struct AkSerializedExternalSourceInfo *info);
void AkSerializedExternalSourceInfo_idCodec_set(struct AkSerializedExternalSourceInfo *info, AkCodecID value);

AkOSChar* AkSerializedExternalSourceInfo_szFile_get(struct AkSerializedExternalSourceInfo *info);
void AkSerializedExternalSourceInfo_szFile_set(struct AkSerializedExternalSourceInfo *info, AkOSChar* value);

void* AkSerializedExternalSourceInfo_pInMemory_get(struct AkSerializedExternalSourceInfo *info);
void AkSerializedExternalSourceInfo_pInMemory_set(struct AkSerializedExternalSourceInfo *info, void* value);

AkUInt32 AkSerializedExternalSourceInfo_uiMemorySize_get(struct AkSerializedExternalSourceInfo *info);
void AkSerializedExternalSourceInfo_uiMemorySize_set(struct AkSerializedExternalSourceInfo *info, AkUInt32 value);

AkFileID AkSerializedExternalSourceInfo_idFile_get(struct AkSerializedExternalSourceInfo *info);
void AkSerializedExternalSourceInfo_idFile_set(struct AkSerializedExternalSourceInfo *info, AkFileID value);
#endif // SWIG

struct AkSerializedExternalSourceInfo : AkExternalSourceInfo
{
	AkSerializedExternalSourceInfo() {}
	~AkSerializedExternalSourceInfo();

	AkSerializedExternalSourceInfo(void* in_pInMemory, AkUInt32 in_uiMemorySize, AkUInt32 in_iExternalSrcCookie, AkCodecID in_idCodec)
		: AkExternalSourceInfo(in_pInMemory, in_uiMemorySize, in_iExternalSrcCookie, in_idCodec) {}

	AkSerializedExternalSourceInfo(AkOSChar* in_pszFileName, AkUInt32 in_iExternalSrcCookie, AkCodecID in_idCodec);

	AkSerializedExternalSourceInfo(AkFileID in_idFile, AkUInt32 in_iExternalSrcCookie, AkCodecID in_idCodec)
		: AkExternalSourceInfo(in_idFile, in_iExternalSrcCookie, in_idCodec) {}

	void Clear(); //< Called at construction of the array class to wipe the memory as if this object were default constructed
	void Clone(const AkSerializedExternalSourceInfo& other);

	static int GetSizeOf() { return sizeof(AkSerializedExternalSourceInfo); }
};

	AKRESULT Init(AkInitializationSettings* settings);
	AKRESULT InitSpatialAudio(AkSpatialAudioInitSettings* settings);
	AKRESULT InitCommunication(AkCommunicationSettings* settings);

	void Term();

	AKRESULT RegisterGameObjInternal(AkGameObjectID in_GameObj);
	AKRESULT UnregisterGameObjInternal(AkGameObjectID in_GameObj);

#ifdef AK_SUPPORT_WCHAR
	AKRESULT RegisterGameObjInternal_WithName(AkGameObjectID in_GameObj, const wchar_t* in_pszObjName);
	AKRESULT SetBasePath(const wchar_t* in_pszBasePath);
	AKRESULT SetCurrentLanguage(const wchar_t* in_pszAudioSrcPath);
	AKRESULT LoadFilePackage(const wchar_t* in_pszFilePackageName, AkUInt32 & out_uPackageID);
	AKRESULT AddBasePath(const wchar_t* in_pszBasePath);
	AKRESULT SetGameName(const wchar_t* in_GameName);
	AKRESULT SetDecodedBankPath(const wchar_t* in_DecodedPath);
	AKRESULT LoadAndDecodeBank(const wchar_t* in_pszString, bool in_bSaveDecodedBank, AkBankID& out_bankID);
	AKRESULT LoadAndDecodeBankFromMemory(void* in_BankData, AkUInt32 in_BankDataSize, bool in_bSaveDecodedBank, const wchar_t* in_DecodedBankName, bool in_bIsLanguageSpecific, AkBankID& out_bankID);
	AkPlayingID PostEventOnRoom(
		const wchar_t* in_pszEventName,
		AkRoomID in_roomID,
		AkUInt32 in_uFlags = 0,
		AkCallbackFunc in_pfnCallback = NULL,
		void * in_pCookie = NULL,
		AkUInt32 in_cExternals = 0,
		AkExternalSourceInfo *in_pExternalSources = NULL,
		AkPlayingID	in_PlayingID = AK_INVALID_PLAYING_ID
	);
#else
	AKRESULT RegisterGameObjInternal_WithName(AkGameObjectID in_GameObj, const char* in_pszObjName);
	AKRESULT SetBasePath(const char* in_pszBasePath);
	AKRESULT SetCurrentLanguage(const char* in_pszAudioSrcPath);
	AKRESULT LoadFilePackage(const char* in_pszFilePackageName, AkUInt32 & out_uPackageID);
	AKRESULT AddBasePath(const char* in_pszBasePath);
	AKRESULT SetGameName(const char* in_GameName);
	AKRESULT SetDecodedBankPath(const char* in_DecodedPath);
	AKRESULT LoadAndDecodeBank(const char* in_pszString, bool in_bSaveDecodedBank, AkBankID& out_bankID);
	AKRESULT LoadAndDecodeBankFromMemory(void* in_BankData, AkUInt32 in_BankDataSize, bool in_bSaveDecodedBank, const char* in_DecodedBankName, bool in_bIsLanguageSpecific, AkBankID& out_bankID);
	AkPlayingID PostEventOnRoom(
		const char* in_pszEventName,
		AkRoomID in_roomID,
		AkUInt32 in_uFlags = 0,
		AkCallbackFunc in_pfnCallback = NULL,
		void * in_pCookie = NULL,
		AkUInt32 in_cExternals = 0,
		AkExternalSourceInfo *in_pExternalSources = NULL,
		AkPlayingID	in_PlayingID = AK_INVALID_PLAYING_ID);
#endif

	const AkOSChar* GetCurrentLanguage();
	AKRESULT UnloadFilePackage(AkUInt32 in_uPackageID);
	AKRESULT UnloadAllFilePackages();

	//Override for SetPosition to avoid filling a AkSoundPosition in C# and marshal that. 
	AKRESULT SetObjectPosition(AkGameObjectID in_GameObjectID, AkVector Pos, AkVector Front, AkVector Top);

	AKRESULT GetSourceMultiplePlayPositions(
		AkPlayingID		in_PlayingID,				///< Playing ID returned by AK::SoundEngine::PostEvent()
		AkUniqueID *	out_audioNodeID,			///< Audio Node IDs of sources associated with the specified playing ID. Indexes in this array match indexes in out_msTime.
		AkUniqueID *	out_mediaID,				///< Media ID of playing item. (corresponds to 'ID' attribute of 'File' element in SoundBank metadata file)
		AkTimeMs *		out_msTime,					///< Audio positions of sources associated with the specified playing ID. Indexes in this array match indexes in out_audioNodeID.
		AkUInt32 *		io_pcPositions,				///< Number of entries in out_puPositions. Needs to be set to the size of the array: it is adjusted to the actual number of returned entries
		bool			in_bExtrapolate = true		///< Position is extrapolated based on time elapsed since last sound engine update
		);

	AKRESULT SetListeners(AkGameObjectID in_emitterGameObj, AkGameObjectID* in_pListenerGameObjs, AkUInt32 in_uNumListeners);

	AKRESULT SetDefaultListeners(AkGameObjectID* in_pListenerObjs, AkUInt32 in_uNumListeners);
	
	AKRESULT AddOutput(const AkOutputSettings & in_Settings, AkOutputDeviceID *out_pDeviceID = NULL, AkGameObjectID* in_pListenerIDs = NULL, AkUInt32 in_uNumListeners = 0);

	void GetDefaultStreamSettings(AkStreamMgrSettings & out_settings);
	void GetDefaultDeviceSettings(AkDeviceSettings & out_settings);
	void GetDefaultMusicSettings(AkMusicSettings &out_settings);
	void GetDefaultInitSettings(AkInitSettings & out_settings);
	void GetDefaultPlatformInitSettings(AkPlatformInitSettings &out_settings);

	AkUInt32 GetMajorMinorVersion();
	AkUInt32 GetSubminorBuildVersion();

	void StartResourceMonitoring();
	void StopResourceMonitoring();
	void GetResourceMonitorDataSummary(AkResourceMonitorDataSummary& resourceMonitorDataSummary);

	AKRESULT SetRoomPortal(AkPortalID in_PortalID, const AkTransform& Transform, const AkExtent& Extent, bool bEnabled, AkRoomID FrontRoom, AkRoomID BackRoom);

	AKRESULT SetRoom(AkRoomID in_RoomID, AkRoomParams& in_roomParams, AkGeometrySetID GeometryID, const char* in_pName);

	AKRESULT RegisterSpatialAudioListener(AkGameObjectID in_gameObjectID);

	AKRESULT UnregisterSpatialAudioListener(AkGameObjectID in_gameObjectID);

	AKRESULT SetGeometry(AkGeometrySetID in_GeomSetID,
		AkTriangle* Triangles,
		AkUInt32 NumTriangles,
		AkVertex* Vertices,
		AkUInt32 NumVertices,
		AkAcousticSurface* Surfaces,
		AkUInt32 NumSurfaces,
		AkRoomID RoomID,
		bool EnableDiffraction,
		bool EnableDiffractionOnBoundaryEdges,
		bool EnableTriangles);

	AkPlayingID PostEventOnRoom(
		AkUniqueID in_eventID,
		AkRoomID in_roomID,
		AkUInt32 in_uFlags = 0,
		AkCallbackFunc in_pfnCallback = NULL,
		void * in_pCookie = NULL,
		AkUInt32 in_cExternals = 0,
		AkExternalSourceInfo *in_pExternalSources = NULL,
		AkPlayingID	in_PlayingID = AK_INVALID_PLAYING_ID);

#endif //AK_SOUNDENGINE_STUBS_H_
