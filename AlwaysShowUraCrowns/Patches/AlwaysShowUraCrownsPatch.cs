using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysShowUraCrowns.Patches
{
    internal class AlwaysShowUraCrownsPatch
    {
        #region SongSelectScoreDisplay
        [HarmonyPatch(typeof(SongSelect.SongSelectScoreDisplay))]
        [HarmonyPatch(nameof(SongSelect.SongSelectScoreDisplay.UpdateCrownNumDisplay))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectScoreDisplay_UpdateCrownNumDisplay_Prefix(SongSelect.SongSelectScoreDisplay __instance, int playerId)
        {
            PlayDataManager playData = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.PlayData;
            var musicInfoAccessers = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.musicInfoAccessers;
            int[,] array = new int[3, 5];
            foreach (MusicDataInterface.MusicInfoAccesser musicInfoAccesser in musicInfoAccessers)
            {
                int num = 5;
                try
                {
                    for (int i = 0; i < num; i++)
                    {
                        EnsoRecordInfo ensoRecordInfo;
                        playData.GetPlayerRecordInfo(playerId, musicInfoAccesser.UniqueId, (EnsoData.EnsoLevelType)i, out ensoRecordInfo);
                        switch (ensoRecordInfo.crown)
                        {
                            case DataConst.CrownType.Silver:
                                array[0, i]++;
                                break;
                            case DataConst.CrownType.Gold:
                                array[1, i]++;
                                break;
                            case DataConst.CrownType.Rainbow:
                                array[2, i]++;
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    ModLogger.Log(e.Message, LogType.Error);
                }
            }
            for (int j = 0; j < 5; j++)
            {
                __instance.crownNums[j].CrownNumbers[0].SetNum(array[0, j]);
                __instance.crownNums[j].CrownNumbers[1].SetNum(array[1, j]);
                __instance.crownNums[j].CrownNumbers[2].SetNum(array[2, j]);
            }
            return false;
        }

        [HarmonyPatch(typeof(SongSelect.SongSelectScoreDisplay))]
        [HarmonyPatch(nameof(SongSelect.SongSelectScoreDisplay.UpdateScoreDisplay))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        private static bool SongSelectScoreDisplay_UpdateScoreDisplay_Prefix(SongSelect.SongSelectScoreDisplay __instance, int playerId, int musicUniqueId, bool enableUra = false)
        {
            __instance.scoreCrownChangeAnim.Play("Loop", 0, 0f);
            int num;
            if (enableUra)
            {
                num = 5;
                __instance.rootAnim.Play("Ura");
            }
            else
            {
                num = 4;
                __instance.rootAnim.Play("Ura");
                __instance.bestScores[4].RootObject.SetValue(1);
            }
            try
            {
                for (int i = 0; i < num; i++)
                {
                    EnsoRecordInfo ensoRecordInfo;
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.PlayData.GetPlayerRecordInfo(playerId, musicUniqueId, (EnsoData.EnsoLevelType)i, out ensoRecordInfo);

                    __instance.bestScores[i].RootObject.SetValue(ensoRecordInfo.normalHiScore.score);
                }
            }
            catch (Exception e)
            {
                ModLogger.Log(e.Message, LogType.Error);
            }
            return false;
        }


        #endregion
    }
}
