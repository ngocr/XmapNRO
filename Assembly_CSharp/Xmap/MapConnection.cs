using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembly_CSharp.Xmap
{
    public class MapConnection
    {
        public static List<GroupMap> GroupMaps = new List<GroupMap>();

        public static LinkMaps GetLinkMaps()
        {
            LinkMaps linkMaps = new LinkMaps();

            LoadLinkMaps(linkMaps);

            return linkMaps;
        }

        private static void LoadLinkMaps(LinkMaps linkMaps)
        {
            LoadLinkMapsFromFile(linkMaps, "TextData\\LinkMapsXmap.txt");
            LoadLinkMapsAutoWaypointFromFile(linkMaps, "TextData\\AutoLinkMapsWaypoint.txt");

            LoadLinkMapsHome(linkMaps);
            LoadLinkMapSieuThi(linkMaps);

            LoadLinkMapCapsual(linkMaps);
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

        private static void LoadLinkMapsFromFile(LinkMaps linkMaps, string path)
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

                    LoadLinkMap(linkMaps, data[0], data[1], (TypeMapNext)data[2], info);
                }
            }
            catch (Exception e)
            {
                GameScr.info1.addInfo(e.Message, 0);
            }
        }

        private static void LoadLinkMapsAutoWaypointFromFile(LinkMaps linkMaps, string path)
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
                            LoadLinkMapAutoWaypoint(linkMaps, data[i], data[i - 1]);

                        if (i != data.Length - 1)
                            LoadLinkMapAutoWaypoint(linkMaps, data[i], data[i + 1]);
                    }
                }
            }
            catch (Exception e)
            {
                GameScr.info1.addInfo(e.Message, 0);
            }
        }

        private static void LoadLinkMapsHome(LinkMaps linkMaps)
        {
            const int ID_MAP_LANG_BASE = 7;
            int cgender = Char.myCharz().cgender;

            int idMapHome = Algorithm.GetIdMapFromName("Về nhà");
            int idMapLang = ID_MAP_LANG_BASE * cgender;

            LoadLinkMapAutoWaypoint(linkMaps, idMapLang, idMapHome);
            LoadLinkMapAutoWaypoint(linkMaps, idMapHome, idMapLang);
        }

        private static void LoadLinkMapSieuThi(LinkMaps linkMaps)
        {
            const int ID_MAP_SIEU_THI = 84;
            const int ID_NPC = 10;
            const int INDEX = 0;

            int idMapNext = 24 + 0;
            int[] info = new int[]
            {
                ID_NPC, INDEX
            };
            LoadLinkMap(linkMaps, ID_MAP_SIEU_THI, idMapNext, TypeMapNext.NpcMenu, info);
        }

        private static void LoadLinkMapCapsual(LinkMaps linkMaps)
        {
            if (!Algorithm.CanUseCapsual())
                return;

            XmapController.UseCapsual();
            XmapController.WaitInfoMapTrans();

            AddKeyLinkMaps(linkMaps, TileMap.mapID);
            string[] mapNames = GameCanvas.panel.mapNames;

            for (int select = 0; select < mapNames.Length; select++)
            {
                int mapID = Algorithm.GetIdMapFromName(mapNames[select]);

                if (mapID != -1)
                {
                    int[] info = GetInfoMapNextCapsual(select);
                    linkMaps[TileMap.mapID].Add(new MapNext(mapID, TypeMapNext.Capsual, info));
                }
            }
        }

        private static void LoadLinkMap(LinkMaps linkMaps, int idMapStart, int idMapNext, TypeMapNext type, int[] info)
        {
            AddKeyLinkMaps(linkMaps, idMapStart);

            MapNext mapNext = new MapNext(idMapNext, type, info);
            linkMaps[idMapStart].Add(mapNext);
        }

        private static void AddKeyLinkMaps(LinkMaps linkMaps, int idMap)
        {
            if (!linkMaps.ContainsKey(idMap))
                linkMaps.Add(idMap, new List<MapNext>());
        }

        private static void LoadLinkMapAutoWaypoint(LinkMaps linkMaps, int idMapStart, int idMapNext)
        {
            LoadLinkMap(linkMaps, idMapStart, idMapNext, TypeMapNext.AutoWaypoint, null);
        }

        private static int[] GetInfoMapNextCapsual(int select)
        {
            return new int[] { select };
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
