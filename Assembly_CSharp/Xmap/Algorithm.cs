using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembly_CSharp.Xmap
{
    public class Algorithm
    {
        private const int ID_ITEM_CAPSUAL_VIP = 194;
        private const int ID_ITEM_CAPSUAL = 193;
        private const int ID_MAP_HOME_BASE = 21;
        private const int ID_MAP_TTVT_BASE = 24;

        public static List<int> FindWay(int idMapStart, int idMapEnd)
        {
            List<int> wayPassed = GetWayPassedStart(idMapStart);

            List<int> way = FindWay(idMapEnd, wayPassed);
            return way;
        }

        public static Waypoint FindWaypoint(int idMap)
        {
            for (int i = 0; i < TileMap.vGo.size(); i++)
            {
                Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);

                string nameMap = GetNameMap(idMap);
                string textPopup = GetTextPopup(waypoint.popup);

                if (nameMap.Equals(textPopup))
                    return waypoint;
            }
            return null;
        }

        public static int GetIdMapFromName(string mapName)
        {
            int offset = Char.myCharz().cgender;

            if (mapName.Equals("Về nhà"))
                return ID_MAP_HOME_BASE + offset;

            if (mapName.Equals("Trạm tàu vũ trụ"))
                return ID_MAP_TTVT_BASE + offset;

            if (mapName.Contains("Về chỗ cũ: "))
            {
                mapName = mapName.Replace("Về chỗ cũ: ", "");
                if (TileMap.mapNames[Pk9r.IdMapCapsualReturn].Equals(mapName))
                    return Pk9r.IdMapCapsualReturn;
                if (mapName.Equals("Rừng đá"))
                    return -1;
            }

            for (int i = 0; i < TileMap.mapNames.Length; i++)
                if (mapName.Equals(TileMap.mapNames[i]))
                    return i;

            return -1;
        }

        public static int GetIdMapFromPanelXmap(string mapName)
        {
            int idMap = int.Parse(mapName.Split(':')[0]);
            return idMap;
        }

        public static int GetPosWaypointX(Waypoint waypoint)
        {
            if (waypoint.maxX < 60)
                return 15;

            if (waypoint.minX > TileMap.pxw - 60)
                return TileMap.pxw - 15;

            return (waypoint.minX + waypoint.maxX) / 2;
        }

        public static int GetPosWaypointY(Waypoint waypoint)
        {
            return waypoint.maxY;
        }

        public static bool CanUseCapsual()
        {
            return !IsMyCharDie() && Pk9r.IsUseCapsual && HasCapsual();
        }

        public static bool CanNextMap()
        {
            return !Char.isLoadingMap && !Char.ischangingMap && !Controller.isStopReadMessage;
        }

        public static bool IsMyCharDie()
        {
            return Char.myCharz().statusMe == 14 || Char.myCharz().cHP <= 0;
        }

        private static List<int> FindWay(int idMapEnd, List<int> wayPassed)
        {
            int idMapLast = wayPassed.Last();

            if (IsWayPassedFinished(idMapEnd, idMapLast))
                return wayPassed;

            if (!CanGetMapNexts(idMapLast))
                return null;

            List<List<int>> ways = GetWays(idMapEnd, wayPassed);

            List<int> bestWay = GetBestWay(ways);
            return bestWay;
        }
        private static List<List<int>> GetWays(int idMapEnd, List<int> wayPassed)
        {
            List<List<int>> ways = new List<List<int>>();

            int mapLast = wayPassed.Last();
            List<MapNext> mapNexts = GetMapNexts(mapLast);

            foreach (MapNext mapNext in mapNexts)
            {
                int idMap = mapNext.MapID;
                List<int> wayContinue = GetWayContinue(idMapEnd, wayPassed, idMap);

                if (wayContinue != null)
                    ways.Add(wayContinue);
            }

            return ways;
        }

        private static List<int> GetWayContinue(int idMapEnd, List<int> wayPassed, int idMapNext)
        {
            if (IsPassed(idMapNext, wayPassed))
                return null;

            List<int> wayPassedNext = GetWayPassedNext(wayPassed, idMapNext);

            List<int> wayContinue = FindWay(idMapEnd, wayPassedNext);
            return wayContinue;
        }

        private static List<int> GetBestWay(List<List<int>> ways)
        {
            int minStep = 99;
            List<int> bestWay = null;

            foreach (List<int> way in ways)
            {
                if (way.Count < minStep && !IsBadWay(way))
                {
                    bestWay = way;
                    minStep = way.Count;
                }
            }
            return bestWay;
        }

        public static List<MapNext> GetMapNexts(int idMap)
        {
            if (CanGetMapNexts(idMap))
                return MapConnection.MyLinkMaps[idMap];

            return null;
        }

        private static List<int> GetWayPassedStart(int idMapStart)
        {
            List<int> wayPassed = new List<int>()
            {
                idMapStart
            };
            return wayPassed;
        }

        private static List<int> GetWayPassedNext(List<int> wayPassed, int idMapNext)
        {
            List<int> wayNext = new List<int>(wayPassed)
            {
                idMapNext
            };
            return wayNext;
        }

        private static string GetTextPopup(PopUp popUp)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < popUp.says.Length; i++)
            {
                stringBuilder.Append(popUp.says[i]);
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString().Trim();
        }

        private static string GetNameMap(int idMap)
        {
            return TileMap.mapNames[idMap];
        }

        private static bool IsWayPassedFinished(int idMapEnd, int idMapLastWayPassed)
        {
            return idMapLastWayPassed == idMapEnd;
        }

        private static bool CanGetMapNexts(int idMap)
        {
            return MapConnection.MyLinkMaps.ContainsKey(idMap);
        }

        private static bool IsPassed(int idMap, List<int> wayPassed)
        {
            return wayPassed.Contains(idMap);
        }

        private static bool IsBadWay(List<int> way)
        {
            return IsWayGoFutureAndBack(way)
                    || (Char.myCharz().taskMaint.taskId <= 30 && IsWayGoCold(way));
        }

        private static bool IsWayGoFutureAndBack(List<int> way)
        {
            int[] mapsGoFuture = new int[] { 27, 28, 29 };
            for (int i = 1; i < way.Count - 1; i++)
                if (way[i] == 102 && way[i + 1] == 24 && mapsGoFuture.Contains(way[i - 1]))
                    return true;
            return false;
        }

        private static bool IsWayGoCold(List<int> way)
        {
            for (int i = 0; i < way.Count; i++)
                if (way[i] >= 105 && way[i] <= 110)
                    return true;
            return false;
        }

        private static bool HasCapsual()
        {
            Item[] items = Char.myCharz().arrItemBag;

            for (int i = 0; i < items.Length; i++)
                if (IsCapsual(items[i]))
                    return true;

            return false;
        }

        private static bool IsCapsual(Item item)
        {
            return item != null && (item.template.id == ID_ITEM_CAPSUAL || item.template.id == ID_ITEM_CAPSUAL_VIP);
        }
    }
}
