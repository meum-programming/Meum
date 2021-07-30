using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotManager : MonoBehaviour
{
	private static ScreenShotManager instance;
	public static ScreenShotManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<ScreenShotManager>();

				if (instance == null)
				{
					instance = new GameObject("ScreenShotManager").AddComponent<ScreenShotManager>();
					GameObject.DontDestroyOnLoad(instance.gameObject);
				}
			}
			return instance;
		}
	}


    public IEnumerator ScreenShowOn(bool isScreenShot)
    {
        if (isScreenShot)
        {
            yield return new WaitForSeconds(SoundManager.Instance.PlaySe("CameraReady").clip.length);

            yield return new WaitForSeconds(SoundManager.Instance.PlaySe("CameraShot").clip.length);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        Texture2D reSizeTexture2D = ScreenShotReSizing(ScreenCapture.CaptureScreenshotAsTexture(1));

        ScreenShotRequest.saveTypeEnum saveTypeEnum = isScreenShot ? ScreenShotRequest.saveTypeEnum.screenshot : ScreenShotRequest.saveTypeEnum.thumbnail;

        byte[] bytes = reSizeTexture2D.EncodeToPNG();

        string fileType = isScreenShot ? "screenshot_" : "thumbnail_";

        int roomType = MeumDB.Get().myRoomInfo.land_type;
        int uid = MeumDB.Get().myRoomInfo.owner.user_id;
        int middleId = isScreenShot ? uid : roomType;

        DateTime nowDT = DateTime.Now;
        string dateStr = isScreenShot ? string.Format("_{0}_{1:D2}_{2:D2}_{3:D2}_{4:D2}", nowDT.Year, nowDT.Month, nowDT.Day, nowDT.Hour, nowDT.Minute) : string.Empty;

        string fileName = string.Format("{0}{1}{2}.png", fileType, middleId, dateStr);

        bool nextOn = false;

        //스크린샷을 S3로 보내기
        ScreenShotRequest screenShotRequest = new ScreenShotRequest()
        {
            requestStatus = 0,
            id = MeumDB.Get().myRoomInfo.owner.user_id,
            saveType = saveTypeEnum,
            bytes = bytes,
            file_name = fileName,
            successOn = ResultData =>
            {
                ScreenShotInfoData data = (ScreenShotInfoData)ResultData;

                Debug.LogWarning(data.url);

                if (isScreenShot)
                {
                    Application.OpenURL(data.url);
                }

                nextOn = true;
            }
        };
        screenShotRequest.RequestOn();

        yield return new WaitUntil(() => nextOn);
    }

    Texture2D ScreenShotReSizing(Texture2D texture2D)
    {
        float width = texture2D.width;
        float height = texture2D.height;

        int newWidth = 1024;
        int newHeiht = 1024;

        if (width > height)
        {
            newHeiht = Mathf.RoundToInt((height * 1024) / width);
        }
        else
        {
            newWidth = Mathf.RoundToInt((width * 1024) / height);
        }

        Texture2D reSizeTexture2D = ScaleTexture(texture2D, newWidth, newHeiht);

        //카드 비율 420 : 240 의 이미지가 나올수 있도록 크롭 한다.
        int check_w_Rate = Mathf.RoundToInt(newWidth / 7);
        int check_h_Rate = Mathf.RoundToInt(newHeiht / 4);

        int cardNewWidth = 0;
        int cardNewHeiht = 0;

        if (check_w_Rate > check_h_Rate)
        {
            cardNewHeiht = newHeiht;
            cardNewWidth = Mathf.RoundToInt(newHeiht * 420 / 240);
        }
        else
        {
            cardNewWidth = newWidth;
            cardNewHeiht = Mathf.RoundToInt(newWidth * 240 / 420);
        }

#if dev
        Debug.LogWarning($"newHeiht = {newHeiht} , newWidth = {newWidth} , cardNewHeiht = {cardNewHeiht} , cardNewWidth = {cardNewWidth}");
#endif

        Texture2D cropTexture = CropTexture(reSizeTexture2D, cardNewWidth, cardNewHeiht);

        return cropTexture;
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    private Texture2D CropTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);

        int startX = 0;
        int startY = 0;

        if (source.width == targetWidth)
        {
            startY = Mathf.RoundToInt((source.height - targetHeight) / 2);
        }
        else
        {
            startX = Mathf.RoundToInt((source.width - targetWidth) / 2);
        }

        Color[] c = source.GetPixels(startX, startY, targetWidth, targetHeight);
        result.SetPixels(c);

        result.Apply();
        return result;
    }
}
