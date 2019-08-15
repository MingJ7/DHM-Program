using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS.Imaging;
using Emgu.CV;
using System.Diagnostics;
using Emgu.CV.CvEnum;

namespace DHM_Main {
	class myFilter : TIS.Imaging.FrameFilterImpl {
		//private Image<Emgu.CV.Structure.Gray,Byte> result;
		public UMat raw;
		public event EventHandler<NewFrameEvent> NewFrameHandler;
		public string timeelapse;
		/*
         * This method fills the ArrayList arr with the frame types this filter
         * accepts as input.
         * 
         */
		public override void GetSupportedInputTypes(System.Collections.ArrayList frameTypes) {
			// This filter works for 8-bit-gray images only
			frameTypes.Add(new TIS.Imaging.FrameType(TIS.Imaging.MediaSubtypes.Y800));
			frameTypes.Add(new TIS.Imaging.FrameType(TIS.Imaging.MediaSubtypes.Y16));
		}

		/*
         *	This method returns the output frame type for a given input frame type.
         *
         *	The binarization filter does not change size or color format,
         *	so the only output frame type is the input frame type.
         */
		public override bool GetTransformOutputTypes(FrameType inType, System.Collections.ArrayList outTypes) {
			// We don't return the image
			outTypes.Add(inType);

			return true;
		}

		/*
         *	This method is called to copy image data from the src frame to the dest frame.
         *
         *	Depending on the value of m_bEnabled, this implementation applies a binarization or
         *	copies the image data without modifying it.
         */
		public override bool Transform(IFrame src, IFrame dest) {
			Stopwatch timer = Stopwatch.StartNew();
			unsafe {
				// Check whether the destination frame is available
				//if (dest.Ptr == null) return false;
				//dest.CopyFrom(src);

				IntPtr ptr = (IntPtr)src.Ptr;
				//if the IntPtr casting was successful, if both pointers have the same address
				if (ptr.ToPointer() == src.Ptr) {
					//if input is Y800 format, or 8-bit bit-depth,
					if (src.FrameType.Subtype.Equals(TIS.Imaging.MediaSubtypes.Y800)) {
						Mat temp = new Mat(src.FrameType.Size, DepthType.Cv8U, 1, ptr, src.FrameType.BufferSize / src.FrameType.Height);
						temp.CopyTo(raw);
						temp.Dispose();
						NewFrameHandler?.Invoke(this, new NewFrameEvent(raw));
					}
					//if input is Y16 format or 16-bit bit-depth,
					else if (src.FrameType.Subtype.Equals(TIS.Imaging.MediaSubtypes.Y16)) {
						Mat temp = new Mat(src.FrameType.Size, DepthType.Cv16U, 1, ptr, src.FrameType.BufferSize / src.FrameType.Height);
						temp.CopyTo(raw);
						temp.Dispose();
						NewFrameHandler?.Invoke(this, new NewFrameEvent(raw));
					}
				}
			}
			timer.Stop();
			TimeSpan timespan = timer.Elapsed;
			double fps = 1000 / timespan.TotalMilliseconds;
			timeelapse = fps.ToString();
			return true;
		}
		public void updateCamSize(System.Drawing.Size size, VideoFormat format) {
			if (format.FrameType.Subtype.Equals(MediaSubtypes.Y800)){
				raw.Create(size.Height, size.Width, DepthType.Cv8U, 1);
			} else if (format.FrameType.Subtype.Equals(MediaSubtypes.Y16)){
				raw.Create(size.Height, size.Width, DepthType.Cv16U, 1);
			}
		}
	}
	public class NewFrameEvent : EventArgs {
		public Emgu.CV.UMat transfer;
		public NewFrameEvent(Emgu.CV.UMat transfer) {
			this.transfer = transfer;
		}
	}
}
