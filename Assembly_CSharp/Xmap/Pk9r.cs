using System;

namespace Assembly_CSharp.Xmap
{
    public class Pk9r
    {
        public static bool IsMapTransAsXmap;
        public static bool IsUseCapsual = false;
        public static int IdMapCapsualReturn = -1;

        public static bool Chat(string text)
        {
            try
            {
                if (text.Equals("xmp"))
                {
                    XmapController.ShowXmapMenu();
                }
                else if (text.StartsWith("xmp"))
                {
                    int idMap = int.Parse(text.Substring(3));
                    XmapController.StartRunToMapId(idMap);
                }
                else if (text.Equals("csb"))
                {
                    IsUseCapsual = !IsUseCapsual;
                    GameScr.info1.addInfo("Sử dụng capsual thường Xmap: " + (IsUseCapsual ? "Bật" : "Tắt"), 0);
                }
                else
                {
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (Exception e)
            {
                GameScr.info1.addInfo(e.Message, 0);
                return false;
            }

            return true;
        }

        public static bool XoaTauBay(Object obj)
        {
            Teleport teleport = (Teleport)obj;
            if (teleport.isMe)
            {
                Char.myCharz().isTeleport = false;
                if (teleport.type == 0)
                {
                    Controller.isStopReadMessage = false;
                    Char.ischangingMap = true;
                }
                Teleport.vTeleport.removeElement(teleport);
                return true;
            }
            return false;
        }

        public static void SelectMapTrans(int selected)
        {
            if (Pk9r.IsMapTransAsXmap)
            {
                XmapController.HideInfoDlg();
                string mapName = GameCanvas.panel.mapNames[selected];
                int idMap = Algorithm.GetIdMapFromPanelXmap(mapName);
                XmapController.StartRunToMapId(idMap);
                return;
            }
            SaveIdMapCapsualReturn();
            Service.gI().requestMapSelect(selected);
        }

        public static void FixBlackScreen()
        {
            Service.gI().requestChangeZone(-1, -1);
            GameScr.info1.addInfo("Fix Black Screen", 0);
        }

        public static void ResetMapTrans()
        {
            IsMapTransAsXmap = false;
        }

        public static void SaveIdMapCapsualReturn()
        {
            IdMapCapsualReturn = TileMap.mapID;
        }
    }
}
