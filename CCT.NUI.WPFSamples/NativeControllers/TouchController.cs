using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CCT.NUI.WPFSamples
{
    public class TouchController
    {

        POINTER_TOUCH_INFO contact = new POINTER_TOUCH_INFO();

        public static void Touch(int x, int y)
        {
            bool flag = false;
            flag = InitializeTouchInjection(10, TOUCH_FEEDBACK_DEFAULT);

            POINTER_TOUCH_INFO contacts = new POINTER_TOUCH_INFO();


            contacts.pointerInfo.pointerType = POINTER_INPUT_TYPE.PT_TOUCH;
            contacts.touchFlags = TOUCH_FLAGS.TOUCH_FLAGS_NONE;

            contacts.orientation = 90;
            contacts.pressure = 32000;
            contacts.pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_DOWN | POINTER_FLAGS.POINTER_FLAG_INRANGE | POINTER_FLAGS.POINTER_FLAG_INCONTACT;
            contacts.touchMasks = TOUCH_MASK.TOUCH_MASK_CONTACTAREA | TOUCH_MASK.TOUCH_MASK_ORIENTATION | TOUCH_MASK.TOUCH_MASK_PRESSURE;
            contacts.pointerInfo.ptPixelLocation.x = x;
            contacts.pointerInfo.ptPixelLocation.y = y;
            contacts.pointerInfo.pointerId = 1;

            Rect touchArea = new Rect();
            touchArea.left = x - 2;
            touchArea.right = x + 2;
            touchArea.top = y - 2;
            touchArea.bottom = y + 2;
            contacts.rcContact = touchArea;

            flag = InjectTouchInput(1, ref contacts);

            contacts.pressure = 0;
            if (flag)
            {
                contacts.pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_UP;
                InjectTouchInput(1, ref contacts);
            }
        }

        public bool TouchDown(int x, int y)
        {
            bool flag = false;
            flag = InitializeTouchInjection(10, TOUCH_FEEDBACK_DEFAULT);

            contact.pointerInfo.pointerType = POINTER_INPUT_TYPE.PT_TOUCH;
            contact.touchFlags = TOUCH_FLAGS.TOUCH_FLAGS_NONE;

            contact.orientation = 90;
            contact.pressure = 32000;
            contact.pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_DOWN | POINTER_FLAGS.POINTER_FLAG_INRANGE | POINTER_FLAGS.POINTER_FLAG_INCONTACT;
            contact.touchMasks = TOUCH_MASK.TOUCH_MASK_CONTACTAREA | TOUCH_MASK.TOUCH_MASK_ORIENTATION | TOUCH_MASK.TOUCH_MASK_PRESSURE;
            contact.pointerInfo.ptPixelLocation.x = x;
            contact.pointerInfo.ptPixelLocation.y = y;
            contact.pointerInfo.pointerId = 1;

            Rect touchArea = new Rect();
            touchArea.left = x - 2;
            touchArea.right = x + 2;
            touchArea.top = y - 2;
            touchArea.bottom = y + 2;
            contact.rcContact = touchArea;

            flag = InjectTouchInput(1, ref contact);

            return flag;
        }

        public void TouchHold()
        {
            contact.pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_UPDATE | POINTER_FLAGS.POINTER_FLAG_INRANGE | POINTER_FLAGS.POINTER_FLAG_INCONTACT;
            for (int i = 0; i < 100000; i++)
            {        //loops for approx. 1 second causing 1 second HOLD effect
                InjectTouchInput(1, ref contact);
            }
        }

        public bool TouchDrag(int x, int y)
        {
            //Setting the Pointer Flag to Drag
            contact.pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_UPDATE | POINTER_FLAGS.POINTER_FLAG_INRANGE | POINTER_FLAGS.POINTER_FLAG_INCONTACT;

            contact.pointerInfo.ptPixelLocation.x = x; 
            contact.pointerInfo.ptPixelLocation.y = y;

            return InjectTouchInput(1, ref contact);
        }

        public bool TouchUp()
        {
            contact.pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_UP;
            return InjectTouchInput(1, ref contact);
        }

        #region Types

        [DllImport("User32.dll")]
        static extern bool InitializeTouchInjection(uint maxCount, int dwMode);

        [DllImport("User32.dll")]
        static extern bool InjectTouchInput(int count, ref POINTER_TOUCH_INFO contacts);

        const int MAX_TOUCH_COUNT = 256;

        //Specifies default touch visualizations.
        const int TOUCH_FEEDBACK_DEFAULT = 0x1;

        //Specifies indirect touch visualizations.
        const int TOUCH_FEEDBACK_INDIRECT = 0x2;

        //Specifies no touch visualizations.
        const int TOUCH_FEEDBACK_NONE = 0x3;

        [StructLayout(LayoutKind.Explicit)]
        public struct Rect
        {
            [FieldOffset(0)]
            public int left;
            [FieldOffset(4)]
            public int top;
            [FieldOffset(8)]
            public int right;
            [FieldOffset(12)]
            public int bottom;
        }


        public enum TOUCH_FLAGS { TOUCH_FLAGS_NONE = 0x00000000/*Indicates that no flags are set.*/ }

        public enum TOUCH_MASK
        {
            TOUCH_MASK_NONE = 0x00000000 // Default - none of the optional fields are valid
            ,
            TOUCH_MASK_CONTACTAREA = 0x00000001 // The rcContact field is valid
            ,
            TOUCH_MASK_ORIENTATION = 0x00000002 // The orientation field is valid
            ,
            TOUCH_MASK_PRESSURE = 0x00000004 // The pressure field is valid

        }

        public enum POINTER_FLAGS
        {
            POINTER_FLAG_NONE = 0x00000000,
            POINTER_FLAG_NEW = 0x00000001,
            POINTER_FLAG_INRANGE = 0x00000002,
            POINTER_FLAG_INCONTACT = 0x00000004,
            POINTER_FLAG_FIRSTBUTTON = 0x00000010,
            POINTER_FLAG_SECONDBUTTON = 0x00000020,
            POINTER_FLAG_THIRDBUTTON = 0x00000040,
            POINTER_FLAG_OTHERBUTTON = 0x00000080,
            POINTER_FLAG_PRIMARY = 0x00000100,
            POINTER_FLAG_CONFIDENCE = 0x00000200,
            POINTER_FLAG_CANCELLED = 0x00000400,
            POINTER_FLAG_DOWN = 0x00010000,
            POINTER_FLAG_UPDATE = 0x00020000,
            POINTER_FLAG_UP = 0x00040000,
            POINTER_FLAG_WHEEL = 0x00080000,
            POINTER_FLAG_HWHEEL = 0x00100000
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }


        public enum POINTER_INPUT_TYPE
        {
            PT_POINTER = 0x00000001,
            PT_TOUCH = 0x00000002,
            PT_PEN = 0x00000003,
            PT_MOUSE = 0x00000004
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_INFO
        {
            public POINTER_INPUT_TYPE pointerType;
            public uint pointerId;
            public uint frameId;
            public POINTER_FLAGS pointerFlags;

            public IntPtr sourceDevice;
            public IntPtr hwndTarget;

            public POINT ptPixelLocation;
            public POINT ptHimetricLocation;

            public POINT ptPixelLocationRaw;
            public POINT ptHimetricLocationRaw;

            public uint dwTime;
            public uint historyCount;

            public uint inputData;
            public uint dwKeyStates;
            public ulong PerformanceCount;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_TOUCH_INFO
        {
            /*
             * Contains basic pointer information common to all pointer types.
             */
            public POINTER_INFO pointerInfo;

            /*
             * Lists the touch flags.
             */
            public TOUCH_FLAGS touchFlags;

            public TOUCH_MASK touchMasks;

            /*
             * Pointer contact area in pixel screen coordinates. 
             * By default, if the device does not report a contact area, 
             * this field defaults to a 0-by-0 rectangle centered around the pointer location.
             */
            public Rect rcContact;

            public Rect rcContactRaw;

            /*
             * A pointer orientation, with a value between 0 and 359, where 0 indicates a touch pointer 
             * aligned with the x-axis and pointing from left to right; increasing values indicate degrees
             * of rotation in the clockwise direction.
             * This field defaults to 0 if the device does not report orientation.
             */
            public uint orientation;

            /*
             * Pointer pressure normalized in a range of 0 to 256.
             */
            public uint pressure;
        } 
        #endregion
    }
}
