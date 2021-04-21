using System;

namespace Assembly_CSharp.Xmap
{
    public class Pk9rXmap
    {
        public static bool IsXmapRunning;
        public static bool IsMapTransAsXmap;
        public static bool IsUseCapsual = false;
        public static int IdMapCapsualReturn = -1;

        public static bool IsShowPanelMapTrans { get; set; } = true;

        public static bool Chat(string text)
        {
            if (text.Equals("xmp"))
            {
                if (IsXmapRunning)
                {
                    XmapController.FinishXmap();
                    GameScr.info1.addInfo("Đã huỷ Xmap", 0);
                }
                else
                {
                    XmapController.ShowXmapMenu();
                }
            }
            else if (text.StartsWith("xmp"))
            {
                if (IsXmapRunning)
                {
                    XmapController.FinishXmap();
                    GameScr.info1.addInfo("Đã huỷ Xmap", 0);
                }
                else
                {
                    int idMap = int.Parse(text.Substring(3));
                    XmapController.StartRunToMapId(idMap);
                }
            }
            else if (text.Equals("csb"))
            {
                IsUseCapsual = !IsUseCapsual;
                GameScr.info1.addInfo("Sử dụng capsual Xmap: " + (IsUseCapsual ? "Bật" : "Tắt"), 0);
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool HotKeys()
        {
            switch (GameCanvas.keyAsciiPress)
            {
                case 'x':
                    Chat("xmp");
                    break;
                case 'c':
                    Chat("csb");
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static void Update()
        {
            if (XmapData.Instance().IsLoading)
                XmapData.Instance().Update();
            if (IsXmapRunning)
                XmapController.Update();
        }

        public static void Info(string text)
        {
            if (text.Equals("Bạn chưa thể đến khu vực này"))
                XmapController.FinishXmap();
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
            if (IsMapTransAsXmap)
            {
                XmapController.HideInfoDlg();
                string mapName = GameCanvas.panel.mapNames[selected];
                int idMap = XmapData.GetIdMapFromPanelXmap(mapName);
                XmapController.StartRunToMapId(idMap);
                return;
            }
            XmapController.SaveIdMapCapsualReturn();
            Service.gI().requestMapSelect(selected);
        }

        public static void FixBlackScreen()
        {
            Controller.gI().loadCurrMap(0);
            Service.gI().finishLoadMap();
            Char.isLoadingMap = false;
        }

        public static void ResetMapTrans()
        {
            IsMapTransAsXmap = false;
        }
    }
}
