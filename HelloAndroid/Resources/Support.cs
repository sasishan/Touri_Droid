using System;
using Android.Graphics;
using Android.Content;
using Java.IO;

namespace TouriDroid
{
	public static class Support
	{		
		public static String createImageFromdddBitmap(Bitmap bitmap, Context c) {
			String fileName = "myImage";//no .png or .jpg needed
			ContextWrapper cw = new ContextWrapper(c);
			Java.IO.File directory = cw.GetDir("imgDir", FileCreationMode.Private);
			Java.IO.File myPath = new Java.IO.File(directory, "test.png");

			try {
				
				FileOutputStream str = new FileOutputStream(myPath);
		
				using (var os = new System.IO.FileStream(myPath.AbsolutePath, System.IO.FileMode.Create))
				{
					bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
				}

			} catch (Exception e) {
				//e.PrintStackTrace();
				fileName = null;
			}
			return fileName;
		}

	}
}

