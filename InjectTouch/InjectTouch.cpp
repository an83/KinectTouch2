// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "stdafx.h"

/******************************************************************
*                                                                 *
*  WinMain                                                        *
*                                                                 *
*  Application entrypoint                                         *
*                                                                 *
******************************************************************/


extern "C" __declspec(dllexport) 
	bool InjectTouch(int x, int y,POINTER_FLAGS pointerFlags){
	POINTER_TOUCH_INFO contact;
    BOOL bRet = TRUE;

    //
    // assume a maximum of 10 contacts and turn touch feedback off
    //
    InitializeTouchInjection(1, TOUCH_FEEDBACK_DEFAULT);

    //
    // initialize the touch info structure
    //
    memset(&contact, 0, sizeof(POINTER_TOUCH_INFO));

    contact.pointerInfo.pointerType = PT_TOUCH; //we're sending touch input
    contact.pointerInfo.pointerId = 0;          //contact 0
    contact.pointerInfo.ptPixelLocation.x = x;
    contact.pointerInfo.ptPixelLocation.y = y;
    contact.pointerInfo.pointerFlags = pointerFlags;
    contact.touchFlags = TOUCH_FLAG_NONE;
    contact.touchMask = TOUCH_MASK_CONTACTAREA | TOUCH_MASK_ORIENTATION | TOUCH_MASK_PRESSURE;
    contact.orientation = 90;
    contact.pressure = 32000;

    //
    // set the contact area depending on thickness
    //
    contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
    contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.x + 2;
    contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
    contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

    //
    // inject a touch down
    //
    bRet = InjectTouchInput(1, &contact);

	return bRet;
}

extern "C" __declspec(dllexport) 
	void Touch(int x, int y){
	POINTER_TOUCH_INFO contact;
    BOOL bRet = TRUE;

    //
    // assume a maximum of 10 contacts and turn touch feedback off
    //
    InitializeTouchInjection(1, TOUCH_FEEDBACK_DEFAULT);

    //
    // initialize the touch info structure
    //
    memset(&contact, 0, sizeof(POINTER_TOUCH_INFO));

    contact.pointerInfo.pointerType = PT_TOUCH; //we're sending touch input
    contact.pointerInfo.pointerId = 0;          //contact 0
    contact.pointerInfo.ptPixelLocation.x = x;
    contact.pointerInfo.ptPixelLocation.y = y;
    contact.pointerInfo.pointerFlags = POINTER_FLAG_DOWN | POINTER_FLAG_INRANGE | POINTER_FLAG_INCONTACT;
    contact.touchFlags = TOUCH_FLAG_NONE;
    contact.touchMask = TOUCH_MASK_CONTACTAREA | TOUCH_MASK_ORIENTATION | TOUCH_MASK_PRESSURE;
    contact.orientation = 90;
    contact.pressure = 32000;

    //
    // set the contact area depending on thickness
    //
    contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
    contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.x + 2;
    contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
    contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

    //
    // inject a touch down
    //
    bRet = InjectTouchInput(1, &contact);

    //
    // if touch down was succesfull, send a touch up
    //
    if (bRet) {
        contact.pointerInfo.pointerFlags = POINTER_FLAG_UP;

        //
        // inject a touch up
        //
        bRet = InjectTouchInput(1, &contact);
    }
}

extern "C" __declspec(dllexport) 
	void Hold(int x, int y){
	POINTER_TOUCH_INFO contact;
    BOOL bRet = TRUE;

    //
    // assume a maximum of 10 contacts and turn touch feedback off
    //
    InitializeTouchInjection(1, TOUCH_FEEDBACK_DEFAULT);

    //
    // initialize the touch info structure
    //
    memset(&contact, 0, sizeof(POINTER_TOUCH_INFO));

    contact.pointerInfo.pointerType = PT_TOUCH; //we're sending touch input
    contact.pointerInfo.pointerId = 0;          //contact 0
    contact.pointerInfo.ptPixelLocation.x = x;
    contact.pointerInfo.ptPixelLocation.y = y;
    contact.pointerInfo.pointerFlags = POINTER_FLAG_DOWN | POINTER_FLAG_INRANGE | POINTER_FLAG_INCONTACT;
    contact.touchFlags = TOUCH_FLAG_NONE;
    contact.touchMask = TOUCH_MASK_CONTACTAREA | TOUCH_MASK_ORIENTATION | TOUCH_MASK_PRESSURE;
    contact.orientation = 90;
    contact.pressure = 32000;

    //
    // set the contact area depending on thickness
    //
    contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
    contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.x + 2;
    contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
    contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

	//POINTER_FLAG_UPDATE when use with POINTER_FLAG_INRANGE and POINTER_FLAG_INCONTACT keeps the touch in drag mode with respect to last down Input.

	contact.pointerInfo.pointerFlags = POINTER_FLAG_UPDATE | POINTER_FLAG_INRANGE | POINTER_FLAG_INCONTACT;
	for(int i=0;i<100000;i++){        //loops for approx. 1 second causing 1 second HOLD effect
		InjectTouchInput(1, &contact);
	}

	contact.pointerInfo.pointerFlags = POINTER_FLAG_UP;       // Moving the touch Up after Hold Complete
	InjectTouchInput(1, &contact);
}

extern "C" __declspec(dllexport) 
	void Drag(int x, int y){
	POINTER_TOUCH_INFO contact;
    BOOL bRet = TRUE;

    //
    // assume a maximum of 10 contacts and turn touch feedback off
    //
    InitializeTouchInjection(1, TOUCH_FEEDBACK_DEFAULT);

    //
    // initialize the touch info structure
    //
    memset(&contact, 0, sizeof(POINTER_TOUCH_INFO));

    contact.pointerInfo.pointerType = PT_TOUCH; //we're sending touch input
    contact.pointerInfo.pointerId = 0;          //contact 0
    contact.pointerInfo.ptPixelLocation.x = x;
    contact.pointerInfo.ptPixelLocation.y = y;
    contact.pointerInfo.pointerFlags = POINTER_FLAG_DOWN | POINTER_FLAG_INRANGE | POINTER_FLAG_INCONTACT;
    contact.touchFlags = TOUCH_FLAG_NONE;
    contact.touchMask = TOUCH_MASK_CONTACTAREA | TOUCH_MASK_ORIENTATION | TOUCH_MASK_PRESSURE;
    contact.orientation = 90;
    contact.pressure = 32000;

    //
    // set the contact area depending on thickness
    //
    contact.rcContact.top = contact.pointerInfo.ptPixelLocation.y - 2;
    contact.rcContact.bottom = contact.pointerInfo.ptPixelLocation.x + 2;
    contact.rcContact.left = contact.pointerInfo.ptPixelLocation.x - 2;
    contact.rcContact.right = contact.pointerInfo.ptPixelLocation.x + 2;

	bool r1 = InjectTouchInput(1, &contact);

	//Setting the Pointer Flag to Drag
	contact.pointerInfo.pointerFlags = POINTER_FLAG_UPDATE | POINTER_FLAG_INRANGE | POINTER_FLAG_INCONTACT;
	
	for(int i=0;i<500;i++){
		contact.pointerInfo.ptPixelLocation.x--; // updating the X Co-ordinate to x-100 pixels
		InjectTouchInput(1, &contact);
	}
	
	// Lifts the touch input UP
	contact.pointerInfo.pointerFlags = POINTER_FLAG_UP;
	bool r2 = InjectTouchInput(1, &contact);

}


int
WINAPI WinMain(
    HINSTANCE hInstance,
    HINSTANCE hPrevInstance,
    LPSTR lpCmdLine,
    int nShowCmd)
{
    //Touch(10,10);'

	Drag(300,300);
}

