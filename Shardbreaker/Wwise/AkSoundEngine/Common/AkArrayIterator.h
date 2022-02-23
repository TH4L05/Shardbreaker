//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

// NOTE: Need to maintain this file depending on SWIG parsing error on nested C++ class.
// AkArray::AkIterator proxy class, copied from <AK/Tool/Common/AkArray.h>

#include "SwigExceptionSwitch.h"

#ifdef SWIG
	
PAUSE_SWIG_EXCEPTIONS

	%rename(NextIter) AkIterator::operator++;
	%rename(PrevIter) AkIterator::operator--;
	%rename(GetItem) AkIterator::operator*;
	%rename(IsEqualTo) AkIterator::operator==;
	%rename(IsDifferentFrom) AkIterator::operator!=;

	struct AkIterator
	{
		 AK::SoundEngine::DynamicSequence::PlaylistItem* pItem;	///< Pointer to the item in the array.

		 /// ++ operator
		 AkIterator& operator++()
		 {
			 AKASSERT( pItem );
			 ++pItem;
			 return *this;
		 }

		 /// -- operator
		 AkIterator& operator--()
		 {
			 AKASSERT( pItem );
			 --pItem;
			 return *this;
		 }

		 /// * operator
		 AK::SoundEngine::DynamicSequence::PlaylistItem& operator*()
		 {
			 AKASSERT( pItem );
			 return *pItem;
		 }

		 /// == operator
		 bool operator ==( const AkIterator& in_rOp ) const
		 {
			return ( pItem == in_rOp.pItem );
		 }

		 /// != operator
		 bool operator !=( const AkIterator& in_rOp ) const
		 {
			return ( pItem != in_rOp.pItem );
		 }
	};

	%nestedworkaround AkArray<AK::SoundEngine::DynamicSequence::PlaylistItem, AK::SoundEngine::DynamicSequence::PlaylistItem const &,ArrayPoolDefault,AkGrowByPolicy_Proportional>::Iterator;

RESUME_SWIG_EXCEPTIONS

#endif // #ifdef SWIG