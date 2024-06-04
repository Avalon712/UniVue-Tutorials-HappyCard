using HappyCard.Entities;
using HappyCard.Entities.Configs;
using HappyCard.Utils;
using HayypCard.Entities;
using HayypCard.Enums;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UniVue.Utils;
using static HayypCard.Entities.TaskInfo;

namespace HappyCard.Managers
{
    /// <summary>
    /// 管理游戏运行时数据，以及保存游戏数据、加载游戏数据
    /// </summary>
    public sealed class GameDataManager : Singleton<GameDataManager>
    {
        #region 私有字段
        //本地存档数据
        private ArchiveData _archiveData;

        //资产数据
        private GameAppInfo _gameAppInfo;
        private List<Product> _coinProducts;
        private List<Product> _hpProducts;
        private List<Product> _diamondProducts;
        private List<Product> _propProducts;
        private Dictionary<PropType, PropInfo> _propInfos;
        private NetworkInfo _networkInfo;
        private List<HeadIcon> _headIcons;
        private Dictionary<PokerCard, CardInfo> _pokerIcons;
        private List<TaskInfo> _taskInfos;
        #endregion

        #region 本地静态数据
        /// <summary>
        /// 本地存档所在路径
        /// </summary>
        public string localArchiveFilePath=> $"{Application.dataPath}/Game Static Resources/Game Archive/archive.json";
        /// <summary>
        /// 本地游戏程序信息所在路径
        /// </summary>
        public string localGameAppInfoPath => $"{Application.dataPath}/Game Static Resources/Game Config/application.json";
        /// <summary>
        /// 本地网络配置信息所在路径
        /// </summary>
        public string localNetworkInfoPath => $"{Application.dataPath}/Game Static Resources/Game Config/network.json";

        #endregion

        /// <summary>
        /// 游戏存档数据
        /// </summary>
        public ArchiveData ArchiveData => _archiveData;

        /// <summary>
        /// 获取游戏程序信息
        /// </summary>
        public GameAppInfo GameAppInfo 
        {
            get
            {
                if(_gameAppInfo == null)
                {
                    string json = File.ReadAllText(localGameAppInfoPath);
                    _gameAppInfo = JsonConvert.DeserializeObject<GameAppInfo>(json);
                }
                return _gameAppInfo;
            }
        }

        /// <summary>
        /// 玩家登录等信息（与服务器一致）
        /// </summary>
        public User User { get => _archiveData.User; set => _archiveData.User = value; }

        /// <summary>
        /// 当前玩家的数据
        /// </summary>
        public Player Player { get => _archiveData.Player; set => _archiveData.Player=value; }

        /// <summary>
        /// 网络配置信息
        /// </summary>
        public NetworkInfo NetworkInfo
        {
            get
            {
                if(_networkInfo == null)
                {
                    string json = File.ReadAllText(localNetworkInfoPath);
                    _networkInfo = JsonConvert.DeserializeObject<NetworkInfo>(json);
                }
                return _networkInfo;
            }
        }

        /// <summary>
        /// 游戏设置数据
        /// </summary>
        public GameSetting GameSetting => _archiveData.GameSetting;

        /// <summary>
        /// 金币商品
        /// </summary>
        public List<Product> CoinProducts  
        {
            get
            {
                if(_coinProducts == null)
                {
                    _coinProducts = AssetManager.Instance.GetProducts(ProductType.Coin);
                }
                return _coinProducts;
            }
        }

        /// <summary>
        /// 钻石商品
        /// </summary>
        public List<Product> DiamondProducts
        {
            get
            {
                if(_diamondProducts == null)
                {
                    _diamondProducts = AssetManager.Instance.GetProducts(ProductType.Diamond);
                }
                return _diamondProducts;
            }
        }

        /// <summary>
        /// 体力商品
        /// </summary>
        public List<Product> HPProducts 
        { 
            get
            {
                if(_hpProducts == null)
                {
                    _hpProducts = AssetManager.Instance.GetProducts(ProductType.HP);
                }
                return _hpProducts;
            }
        }

        /// <summary>
        /// 道具商品
        /// </summary>
        public List<Product> PropProducts 
        {
            get
            {
                if(_propProducts == null)
                {
                    _propProducts = AssetManager.Instance.GetProducts(ProductType.Prop);
                }
                return _propProducts;
            }
        }

        /// <summary>
        /// 所有的道具信息
        /// </summary>
        public Dictionary<PropType,PropInfo> PropInfos 
        {
            get
            {
                if(_propInfos == null)
                {
                    _propInfos  = AssetManager.Instance.GetPropInfos();
                }
                return _propInfos;
            }
        }

        /// <summary>
        /// 玩家的背包信息
        /// </summary>
        public List<BagItem> Bag => _archiveData.Bag;

        /// <summary>
        /// 玩家对局记录
        /// </summary>
        public List<BattleInfo> BattleRecord => _archiveData.BattleRecord;

        /// <summary>
        /// 玩家所在的房间的信息
        /// </summary>
        public RoomInfo RoomInfo { get; set; }

        /// <summary>
        /// 游戏中所有的头像
        /// </summary>
        public List<HeadIcon> HeadIcons
        {
            get
            {
                if(_headIcons == null)
                {
                    SpriteAtlas atlas = AssetManager.Instance.GetHeadIcons();
                    Sprite[] sprites = new Sprite[atlas.spriteCount];
                    int count = atlas.GetSprites(sprites);
                    _headIcons = new (count);

                    for (int i = 0; i < sprites.Length; i++)
                    {
                        sprites[i].name = sprites[i].name.Replace("(Clone)", string.Empty);
                        _headIcons.Add(new HeadIcon(sprites[i]));
                        sprites[i] = null;
                    }
                }
                return _headIcons;
            }
        }

        /// <summary>
        /// 所有扑克牌
        /// </summary>
        public Dictionary<PokerCard,CardInfo> Pokers
        {
            get
            {
                if(_pokerIcons == null)
                {
                    SpriteAtlas atlas = AssetManager.Instance.GetPokerIcons();
                    _pokerIcons = new(atlas.spriteCount);
                    Type type = typeof(PokerCard);
                    string[] cardNames = Enum.GetNames(type);
                    for (int i = 0; i < cardNames.Length; i++)
                    {
                        PokerCard pokerCard = Enum.Parse<PokerCard>(cardNames[i]);
                        CardInfo cardInfo = new()
                        {
                            PokerCard = pokerCard,
                            Icon = atlas.GetSprite(cardNames[i])
                        };
                        _pokerIcons.Add(pokerCard, cardInfo);
                    }

                }
                return _pokerIcons;
            }
        }

        /// <summary>
        /// 玩家的所有任务进度信息
        /// </summary>
        public List<TaskProcess> TaskProcesses => _archiveData.TaskProcesses;

        /// <summary>
        /// 所有的任务信息
        /// </summary>
        public List<TaskInfo> TaskInfos
        {
            get
            {
                if(_taskInfos == null)
                {
                    _taskInfos = AssetManager.Instance.GetTaskInfos();
                    for (int i = 0; i < _taskInfos.Count; i++)
                    {
                        TaskProcess process = TaskProcesses.Find((p) => p.id == _taskInfos[i].ID);
                        _taskInfos[i].Task = process;
                    }
                }
                return _taskInfos;
            }
        }

        /// <summary>
        /// 加载本地存档
        /// </summary>
        public IEnumerator AsyncLoadLocalArchiveData()
        {
            string filePath = localArchiveFilePath;
            if (File.Exists(filePath))
            {
                Task<string> task = File.ReadAllTextAsync(filePath);
                while (!task.IsCompleted)
                {
                    yield return null;
                }
                _archiveData = JsonConvert.DeserializeObject<ArchiveData>(task.Result);
            }
            else
            {
                _archiveData = new ArchiveData(); //防止空指针
                //_archiveData.User = new User(); //会在注册时进行赋值
                //_archiveData.Player = Player.Default; //会在登录时进行赋值
                _archiveData.Bag = new();
                _archiveData.BattleRecord = new();
                _archiveData.GameSetting = new();
            }
        }

        /// <summary>
        /// 根据商品编号以及商品类型获得指定的商品信息
        /// </summary>
        public Product GetProduct(string id,ProductType productType)
        {
            switch (productType)
            {
                case ProductType.Coin:
                    return CoinProducts.Find((p) => p.ID == id);
                case ProductType.Diamond:
                    return DiamondProducts.Find((p) => p.ID == id);
                case ProductType.HP:
                    return HPProducts.Find((p) => p.ID == id);
                case ProductType.Prop: 
                    return PropProducts.Find((p) => p.ID == id); 
            }

            return null;
        }

        /// <summary>
        /// 添加对局记录==>最大记录数量为100
        /// </summary>
        public void AddBattleInfo(BattleInfo battleInfo)
        {
            if(BattleRecord!=null)
            { 
                BattleRecord.Add(battleInfo);
                if (BattleRecord.Count > 100)
                {
                    ListUtil.TrailDelete(BattleRecord, 0); //移除首元素
                }
            }
        }

        /// <summary>
        /// 添加背包数据
        /// </summary>
        /// <returns>如果玩家当前没有此种类的道具则不为null；如果当前玩家已经拥有同类物品则返回null</returns>
        public BagItem AddBagItem(PropType propType)
        {
            //判断当前玩家是否已经有此类物品
            BagItem bagItem = Bag.Find((item) => item.PropType == propType);

            if(bagItem == null)
            {
                bagItem = new BagItem();
                bagItem.PropType = propType;
                bagItem.Num = 1; //数量为1
                Bag.Add(bagItem); //添加到玩家背包中
                return bagItem;
            }
            else
            {
                bagItem.Num += 1;
                return null;
            }
        }

        /// <summary>
        /// 卸载任务数据
        /// </summary>
        public void UnloadAllTaskInfos()
        {
            _taskInfos?.Clear();
            _taskInfos = null;
        }

        /// <summary>
        /// 卸载商品信息
        /// </summary>
        public void UnloadAllProducts()
        {
            if (_coinProducts != null) { _coinProducts.Clear(); _coinProducts = null; }
            if (_hpProducts != null) { _hpProducts.Clear(); _hpProducts = null; }
            if (_diamondProducts != null) { _diamondProducts.Clear(); _diamondProducts = null; }
            if (_propProducts != null) { _propProducts.Clear(); _propProducts = null; }
        }

        /// <summary>
        /// 卸载扑克牌资源
        /// </summary>
        public void UnloadAllPokerIcons()
        {
            if (_pokerIcons != null)
            {
                _pokerIcons.Clear();
                _pokerIcons = null;
            }
        }
    }
}
