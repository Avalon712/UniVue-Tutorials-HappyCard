using HappyCard.Entities;
using HappyCard.Entities.Configs;
using HappyCard.Enums;
using HappyCard.Network.Entities;
using HayypCard.Entities;
using HayypCard.Enums;
using HayypCard.Network.Entities;
using HayypCard.Network.Handlers;
using HayypCard.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HappyCard
{
    /// <summary>
    /// 这个类仅仅是为了方便做单元测试
    /// </summary>
    public sealed class GameTest : MonoBehaviour
    {
        [ContextMenu("TestShuffle")]
        private void TestShuffle()
        {
            //板子炮洗牌
            //List<PokerCard>[] pokerCards = PokerHelper.RandomShuffleForBanZiPao();

            //Debug.Log($"[{string.Join(", ", pokerCards[0])}]");
            //Debug.Log($"[{string.Join(", ", pokerCards[1])}]");
            //Debug.Log($"[{string.Join(", ", pokerCards[2])}]");
            //Debug.Log($"[{string.Join(", ", pokerCards[3])}]");

            //斗地主洗牌
            //PokerCard[] remaining;
            //List<PokerCard>[] pokerCards = PokerHelper.RandomShuffleForFightLandlord(out remaining);
            //Debug.Log($"剩余的三张牌[{string.Join(", ", remaining)}]");
            //Debug.Log($"{pokerCards[0].Count} [{string.Join(", ", pokerCards[0])}]");
            //Debug.Log($"{pokerCards[1].Count} [{string.Join(", ", pokerCards[1])}]");
            //Debug.Log($"{pokerCards[2].Count} [{string.Join(", ", pokerCards[2])}]");

            //炸金花洗牌
            //List<PokerCard>[] pokerCards = PokerHelper.RandomShuffleForZhaJinHua(5);
            //Debug.Log($"[{string.Join(", ", pokerCards[0])}]");
            //Debug.Log($"[{string.Join(", ", pokerCards[1])}]");
            //Debug.Log($"[{string.Join(", ", pokerCards[2])}]");
            //Debug.Log($"[{string.Join(", ", pokerCards[3])}]");
            //Debug.Log($"[{string.Join(", ", pokerCards[4])}]");
        }


        [ContextMenu("TestResort")]
        private void TestResort()
        {
            List<string> sequence = new List<string> { "P1","P3","P5","P2","P4"};
            Debug.Log($"目标的顺序: [{string.Join(", ", sequence)}]");
            List<Player> players = new List<Player> { 
                new Player() { ID = "P1" },
                new Player() { ID = "P4" },
                new Player() { ID = "P2" },
                new Player() { ID = "P5" },
                new Player() { ID = "P3" },
            };

            List<string> temp = new List<string>(players.Count);
            for (int i = 0; i < players.Count; i++)
            {
                temp.Add(players[i].ID);
            }

            Debug.Log($"未重新排序的顺序:[{string.Join(", ",temp)}]");

            temp.Clear();

            for (int i = 0; i < sequence.Count; i++)
            {
                string pID = sequence[i];
                for (int j = 0; j < players.Count; j++)
                {
                    if (players[j].ID == pID && i != j)
                    {
                        //位置互换
                        Player pj = players[j];
                        Player pi = players[i];
                        players[i] = pj;
                        players[j] = pi;
                    }
                }
            }

            for (int i = 0; i < players.Count; i++)
            {
                temp.Add(players[i].ID);
            }

            Debug.Log($"重新排序的顺序:[{string.Join(", ", temp)}]");
        }

        [ContextMenu("TestIPCodex")]
        private void TestIPCodex()
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Loopback, 65521);
            string str = IPEndPointHelper.EndPointToString(point);
            Debug.Log(str);
            EndPoint end = IPEndPointHelper.StringToEndPoint(str);
            Debug.Log(end);
        }

        [ContextMenu("TestProcotolCodex")]
        private void TestProcotolCodex()
        {
            SyncInfo syncInfo = new SyncInfo();
            syncInfo.Message = "测试协议编解码";
            syncInfo.GameEvent = GameEvent.Signup;
            syncInfo.SenderID = "test";
            syncInfo.ReceiverID = "test2";

            Debug.Log($"编码前: GameEvent={syncInfo.GameEvent}, SenderID={syncInfo.SenderID}, ReceiverID={syncInfo.ReceiverID}, Message={syncInfo.Message}");

            byte[] bytes = ProtocolCodecHelper.Encode(syncInfo);

            SyncInfo decode = ProtocolCodecHelper.Decode(bytes);

            Debug.Log($"解码后: GameEvent={decode.GameEvent}, SenderID={decode.SenderID}, ReceiverID={decode.ReceiverID}, Message={decode.Message}");
        }

        [ContextMenu("PrintNetworkInterfaceInfo")]
        private void PrintNetworkInterfaceInfo()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                NetworkInterface @interface = interfaces[i];
                IPInterfaceProperties properties = @interface.GetIPProperties();
                
                if(properties == null) { continue; }

                var unicastIPs = properties.UnicastAddresses;

                
                Debug.Log($"网络类型: {@interface.NetworkInterfaceType}");

                foreach (var ip in unicastIPs)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        Debug.Log("单播地址:" + ip.Address+" 子网掩码:"+ip.IPv4Mask + " DHCP :"+ (ip.PrefixOrigin==PrefixOrigin.Dhcp) + " PrefixOrigin:"+ip.PrefixOrigin+" SuffixOrigin:"+ip.SuffixOrigin);
                    }
                }
            }
        }


        [ContextMenu("BuildNetworkConfig")]
        private void BuildNetworkConfig()
        {
            NetworkInfo config = new NetworkInfo();
            config.HallServerIP = "127.0.0.1";
            config.HallServerPort = 9017;
            config.RoomServerIP = "127.0.0.1";
            config.RoomServerPort = 9795;
            Dictionary<GameEvent, URLInfo> urls = new()
            {
                { GameEvent.Login, new URLInfo("http://127.0.0.1:9017/login",HttpMethod.POST) },
                { GameEvent.Signup, new URLInfo("http://127.0.0.1:9017/signup",HttpMethod.POST)  },
                { GameEvent.Appeal, new URLInfo("http://127.0.0.1:9017/account_appeal",HttpMethod.POST)  },
                { GameEvent.CheckUpdate,new URLInfo("http://127.0.0.1:9017/check_update",HttpMethod.GET) },
            };
            config.Urls = urls;

            string json = JsonConvert.SerializeObject(config,Formatting.Indented);

            File.WriteAllText($"{Application.dataPath}/Game Config/{NetworkInfo.fileName}.json", json);

            AssetDatabase.Refresh();
        }

        [ContextMenu("BuildGameAppInfo")]
        private void BuildGameAppInfo()
        {
            GameAppInfo gameAppInfo = new();

            gameAppInfo.version = "1";

            string json = JsonConvert.SerializeObject(gameAppInfo, Formatting.Indented);

            File.WriteAllText($"{Application.dataPath}/Game Config/{GameAppInfo.fileName}.json", json);

            AssetDatabase.Refresh();
        }

        [ContextMenu("BuildBattleInfos")]
        private void BuildBattleInfos()
        {
            List<BattleInfo> battleInfos = new List<BattleInfo>(30);
            for (int i = 0; i < 30; i++)
            {
                battleInfos.Add(GetBattleInfo());
            }

            string json = JsonConvert.SerializeObject(battleInfos);
            File.WriteAllText($"{Application.dataPath}/Game Archive/battle.json", json);

            AssetDatabase.Refresh();
        }

        [ContextMenu("BuildDefaultGameSetting")]
        private void BuildDefaultGameSetting()
        {
            GameSetting gameSetting = new GameSetting();
            string json = JsonConvert.SerializeObject(gameSetting);
            File.WriteAllText($"{Application.dataPath}/Game Archive/setting.json", json);

            AssetDatabase.Refresh();
        }

        [ContextMenu("BuildArchiveData")]
        private void BuildArchiveData()
        {
            string userStr = File.ReadAllText($"{Application.dataPath}/Game Archive/user.json");
            string playerStr = File.ReadAllText($"{Application.dataPath}/Game Archive/player.json");
            string settingStr = File.ReadAllText($"{Application.dataPath}/Game Archive/setting.json");
            string battleRecord = File.ReadAllText($"{Application.dataPath}/Game Archive/battle.json");
            string bagStr = File.ReadAllText($"{Application.dataPath}/Game Archive/bag.json");

            ArchiveData archiveData = new ArchiveData();
            archiveData.User = JsonConvert.DeserializeObject<User>(userStr);
            archiveData.Player = JsonConvert.DeserializeObject<Player>(playerStr);
            archiveData.GameSetting = JsonConvert.DeserializeObject<GameSetting>(settingStr);
            archiveData.Bag = JsonConvert.DeserializeObject<List<BagItem>>(bagStr);
            archiveData.BattleRecord = JsonConvert.DeserializeObject<List<BattleInfo>>(battleRecord);

            string archiveStr = JsonConvert.SerializeObject(archiveData);
            File.WriteAllText($"{Application.dataPath}/Game Archive/archive.json", archiveStr);
            AssetDatabase.Refresh();
        }

        private BattleInfo GetBattleInfo()
        {
            List<string> strs =new()
            {
                "赵伟","钱芳","孙杰","李娟","周军","吴静","郑勇","王小小",
                "冯艳","陈平","赵霞","卫强","敏敏","沈涛","韩明","杨超",
                "朱丽","秦洋","校长","许静"
            };
            BattleInfo battleInfo = new BattleInfo();
            battleInfo.Gamemode = (GameMode)Random.Range(0,2);
            battleInfo.Gameplay = (Gameplay)Random.Range(0, 3);
            int l = 3;
            switch (battleInfo.Gameplay)
            {
                case Gameplay.FightLandord:
                    l = 3;
                    break;
                case Gameplay.BanZiPao:
                    l = 4;
                    break;
                case Gameplay.ZhaJinHua:
                    l = Random.Range(3, 5);
                    break;
            }
            List<string> temp = strs.GetRange(Random.Range(0, strs.Count-l), l);
            
            temp[0] = "Avalon712";
            battleInfo.Players = string.Join("、",temp);
            for (int i = 0; i < temp.Count; i++)
            {
                int m = Random.Range(-100, 101)*100*Random.Range(1,11);
                while(m == 0) { m = Random.Range(-100, 100) * 100 * Random.Range(1, 10); }
                string s = temp[i] + (m > 0 ? $"<color=yellow>↑{m}</color>" : $"<color=green>↓{-m}</color>");
                temp[i] = s;
            }

            battleInfo.Result = string.Join("、", temp);

            battleInfo.Date = GetRandomDate();

            battleInfo.RoomId = GetRandomStr();

            return battleInfo;
        }

        private string GetRandomDate()
        {
            // 生成随机的年、月、日、时、分、秒
            int year = Random.Range(2023, 2025); // 生成2023年到2024年之间的随机年份
            int month = Random.Range(1, 13); // 生成1月到12月之间的随机月份
            int day = Random.Range(1, DateTime.DaysInMonth(year, month) + 1); // 生成随机日期
            int hour = Random.Range(0, 24); // 生成0时到23时之间的随机小时
            int minute = Random.Range(0, 60); // 生成0分到59分之间的随机分钟
            int second = Random.Range(0, 60); // 生成0秒到59秒之间的随机秒

            // 创建随机时间
            DateTime randomDateTime = new DateTime(year, month, day, hour, minute, second);

            return randomDateTime.ToString("yyyy/MM/dd HH:mm:ss");
        }

        private string GetRandomStr()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder randomString = new StringBuilder();

            for (int i = 0; i < 12; i++)
            {
                randomString.Append(chars[Random.Range(0, chars.Length)]);
            }

            return randomString.ToString();
        }
    }
}
