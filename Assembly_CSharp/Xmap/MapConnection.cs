using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembly_CSharp.Xmap
{
    public class MapConnection
    {
        public static List<GroupMap> GroupMaps = new List<GroupMap>();
        public static LinkMaps MyLinkMaps;
        public static bool IsLoading;

        private static bool IsLoadingCapsual;

        public static void LoadLinkMaps()
        {
            IsLoading = true;
        }

        public static void LoadGroupMapsFromFile(string path)
        {
            GroupMaps.Clear();
            try
            {
                StreamReader sr = new StreamReader(path);
                string textLine;
                string textLine2;
                while ((textLine = sr.ReadLine()) != null)
                {
                    textLine = textLine.Trim();
                    if (textLine.StartsWith("#") || textLine.Equals(""))
                        continue;

                    textLine2 = sr.ReadLine().Trim();

                    string[] textData = textLine2.Split(' ');
                    List<int> data = Array.ConvertAll(textData, s => int.Parse(s)).ToList();

                    GroupMaps.Add(new GroupMap(textLine, data));
                }
            }
            catch (Exception e)
            {
                GameScr.info1.addInfo(e.Message, 0);
            }

            RemoveMapsHomeInGroupMaps();
        }

        public static void Update()
        {
            if (!IsLoadingCapsual)
            {
                LoadLinkMapBase();

                if (Algorithm.CanUseCapsual())
                {
                    XmapController.UseCapsual();
                    IsLoadingCapsual = true;
                    return;
                }

                IsLoading = false;
                return;
            }
            if (IsWaitInfoMapTrans())
                return;

            LoadLinkMapCapsual();
            IsLoadingCapsual = false;
            IsLoading = false;
        }

        private static void LoadLinkMapBase()
        {
            MyLinkMaps = new LinkMaps();
            LoadLinkMapsFromFile("TextData\\LinkMapsXmap.txt");
            LoadLinkMapsAutoWaypointFromFile("TextData\\AutoLinkMapsWaypoint.txt");

            LoadLinkMapsHome();
            LoadLinkMapSieuThi();
        }

        private static void LoadLinkMapsFromFile(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path);
                string textLine;
                while ((textLine = sr.ReadLine()) != null)
                {
                    textLine = textLine.Trim();

                    if (textLine.StartsWith("#") || textLine.Equals(""))
                        continue;

                    string[] textData = textLine.Split(' ');
                    int[] data = Array.ConvertAll(textData, s => int.Parse(s));

                    int lenInfo = data.Length - 3;
                    int[] info = new int[lenInfo];
                    Array.Copy(data, 3, info, 0, lenInfo);

                    LoadLinkMap(data[0], data[1], (TypeMapNext)data[2], info);
                }
            }
            catch (Exception e)
            {
                GameScr.info1.addInfo(e.Message, 0);
            }
        }

        private static void LoadLinkMapsAutoWaypointFromFile(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path);
                string textLine;
                while ((textLine = sr.ReadLine()) != null)
                {
                    textLine = textLine.Trim();

                    if (textLine.StartsWith("#") || textLine.Equals(""))
                        continue;

                    string[] textData = textLine.Split(' ');
                    int[] data = Array.ConvertAll(textData, s => int.Parse(s));

                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i != 0)
                            LoadLinkMap(data[i], data[i - 1], TypeMapNext.AutoWaypoint, null);

                        if (i != data.Length - 1)
                            LoadLinkMap(data[i], data[i + 1], TypeMapNext.AutoWaypoint, null);
                    }
                }
            }
            catch (Exception e)
            {
                GameScr.info1.addInfo(e.Message, 0);
            }
        }

        private static void LoadLinkMapsHome()
        {
            const int ID_MAP_LANG_BASE = 7;
            int cgender = Char.myCharz().cgender;

            int idMapHome = Algorithm.GetIdMapFromName("Về nhà");
            int idMapLang = ID_MAP_LANG_BASE * cgender;

            LoadLinkMap(idMapLang, idMapHome, TypeMapNext.AutoWaypoint, null);
            LoadLinkMap(idMapHome, idMapLang, TypeMapNext.AutoWaypoint, null);
        }

        private static void LoadLinkMapSieuThi()
        {
            const int ID_MAP_TTVT_BASE = 24;
            const int ID_MAP_SIEU_THI = 84;
            const int ID_NPC = 10;
            const int INDEX = 0;

            int offset = Char.myCharz().cgender;
            int idMapNext = ID_MAP_TTVT_BASE + offset;
            int[] info = new int[]
            {
                ID_NPC, INDEX
            };
            LoadLinkMap(ID_MAP_SIEU_THI, idMapNext, TypeMapNext.NpcMenu, info);
        }

        private static void LoadLinkMapCapsual()
        {
            AddKeyLinkMaps(TileMap.mapID);
            string[] mapNames = GameCanvas.panel.mapNames;
            for (int select = 0; select < mapNames.Length; select++)
            {
                int mapID = Algorithm.GetIdMapFromName(mapNames[select]);

                if (mapID != -1)
                {
                    int[] info = new int[] { select };
                    MyLinkMaps[TileMap.mapID].Add(new MapNext(mapID, TypeMapNext.Capsual, info));
                }
            }
        }

        private static void LoadLinkMap(int idMapStart, int idMapNext, TypeMapNext type, int[] info)
        {
            AddKeyLinkMaps(idMapStart);

            MapNext mapNext = new MapNext(idMapNext, type, info);
            MyLinkMaps[idMapStart].Add(mapNext);
        }

        private static void AddKeyLinkMaps(int idMap)
        {
            if (!MyLinkMaps.ContainsKey(idMap))
                MyLinkMaps.Add(idMap, new List<MapNext>());
        }

        private static bool IsWaitInfoMapTrans()
        {
            return !Pk9r.IsShowPanelMapTrans;
        }

        private static void RemoveMapsHomeInGroupMaps()
        {
            int cgender = Char.myCharz().cgender;
            foreach (var groupMap in GroupMaps)
            {
                switch (cgender)
                {
                    case 0:
                        groupMap.IdMaps.Remove(22);
                        groupMap.IdMaps.Remove(23);
                        break;
                    case 1:
                        groupMap.IdMaps.Remove(21);
                        groupMap.IdMaps.Remove(23);
                        break;
                    default:
                        groupMap.IdMaps.Remove(21);
                        groupMap.IdMaps.Remove(22);
                        break;
                }
            }
        }
    }
}
