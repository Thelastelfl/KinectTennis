using System.Runtime.InteropServices;
using UnityEngine;

/*
 * Author: YuYuYouEr.cn
 */
public class qfOpenCV {

    /*
	[DllImportAttribute(@"qfOpenCV.DLL", EntryPoint = "rgba_resize")]
	public static extern void rgba_resize(byte[] rgba_buf, int width, int height, int channels
											, byte[] rgba_new_buf, int new_width, int new_height
											, int flip);
	*/

    public static void rgbaToColor32(byte[] img_buf, int img_width, int img_height, int channels
                                , Color32[] color32)
    {
        for (int it = 0, ib = 0; ib < img_buf.Length; ++it, ib += channels)
        {
            color32[it].r = img_buf[ib + 0];
            color32[it].g = img_buf[ib + 1];
            color32[it].b = img_buf[ib + 2];
            color32[it].a = img_buf[ib + 3];
        }

    }

    public static void bgraToColor32(byte[] img_buf, int img_width, int img_height, int channels
                                , Color32[] color32)
    {
        for (int it = 0, ib = 0; ib < img_buf.Length; ++it, ib += channels)
        {
            color32[it].b = img_buf[ib + 0];
            color32[it].g = img_buf[ib + 1];
            color32[it].r = img_buf[ib + 2];
            color32[it].a = img_buf[ib + 3];
        }

    }
}
