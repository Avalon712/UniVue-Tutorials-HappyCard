using HappyCard.Entities;
using HappyCard.Entities.Configs;
using HappyCard.Enums;
using HappyCard.Managers;
using HappyCard.Network;
using HappyCard.Network.Entities;
using HappyCard.Utils;
using HayypCard;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Utils;
using UniVue.View.Views.Flexible;
using YooAsset;

namespace HappyCard
{
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public sealed class GameRunner : UnitySingleton<GameRunner>
    {
        [Header("游戏运行模式")]
        public EPlayMode playMode = EPlayMode.EditorSimulateMode;


        //--------------------------------------------------------------------------------------
        private void Awake()
        {
            DontDestroyOnLoad(this);

            //添加一个初始的Http网络服务组件
            gameObject.AddComponent<GameHttpService>();

            //系统设置
#if !UNITY_EDITOR
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
#endif
            //初始化Vue
            Vue.Initialize(new VueConfig() { MaxHistoryRecord=15});

            //初始化YooAsset
            YooAssets.Initialize();

            //配置自动装配EventCall
#if !ENABLE_IL2CPP
            //支持反射
            Vue.AutowireEventCalls();
#else       //il2cpp的AOT编译
            //不支持反射
            Vue.Instance.BuildAutowireInfos(typeof(LoginHandler), typeof(GameUpdateHandler));
#endif

            StartCoroutine(OnEnterGameApp());
        }

        //------------------------------------刚进入游戏进行的初始化---------------------------------------

        #region 进入游戏
        private IEnumerator OnEnterGameApp()
        {
            //创建视图
            using(var it = GameObjectFindUtil.BreadthFind(GameObject.Find("Canvas"), nameof(GameUI.LoadView), nameof(GameUI.EnsureTipView)).GetEnumerator())
            {
                while (it.MoveNext())
                {
                    if(it.Current.name == nameof(GameUI.LoadView)) { new FlexibleView(it.Current); }
                    else if(it.Current.name == nameof(GameUI.EnsureTipView)) { new FEnsureTipView(it.Current); }
                }
            }

            //绑定数据显示场景信息
            LoadInfo loadInfo = new LoadInfo();
            loadInfo.Bind(nameof(GameUI.LoadView));

            //检查游戏更新
            loadInfo.message = "检查游戏更新中";
            GameHttpService httpService = GetComponent<GameHttpService>();
            yield return httpService.AsyncSendGetRequest(CheckGameUpdate(loadInfo));

            //加载场景资源
            loadInfo.message = "场景资源加载中";
            //先加载公共资源
            using(var it = AssetManager.Instance.AsyncLoadPublicAsset().GetEnumerator())
            {
                while (it.MoveNext())
                {
                    var op = it.Current;
                    while (!op.IsDone) { yield return null; }
                }
            }

            loadInfo.message = "本地存档数据加载中";
            yield return GameDataManager.Instance.AsyncLoadLocalArchiveData();

            loadInfo.message = "本地存档数据加载完毕";
            loadInfo.Unbind(); //解除数据绑定

            //初始化广域网服务必须在最后面
            NetworkManager.Instance.SetGameNetworkServiceMode(GameDataManager.Instance.NetworkInfo.mode);

            //前期初始化完成再注册事件 
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            //主动触发事件
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        #endregion

        #region 检查游戏更新
        /// <summary>
        /// 检查游戏更新
        /// </summary>
        /// <returns>游戏是否需要进行更新</returns>
        public HttpInfo<GameAppInfo, GameAppInfo> CheckGameUpdate(LoadInfo loadInfo)
        {
            HttpInfo<GameAppInfo, GameAppInfo> httpInfo = new(GameEvent.CheckUpdate);

            //处理进度
            httpInfo.onProcess = (process) => loadInfo.process = process * 100;

            //处理成功
            httpInfo.onSuccessed = () =>
            {
                //版本比对
                if (httpInfo.responseData.version != GameDataManager.Instance.GameAppInfo.version)
                {
                    string message = "检查到游戏更新,需要进行更新才能进入游戏,点击取消将退出游戏";
                    Vue.Router.GetView<FEnsureTipView>(nameof(GameUI.EnsureTipView)).Open("游戏更新",message, GameUpdate, QuitGameApp);
                }

                GameDataManager.Instance.NetworkInfo.mode = NetworkServiceMode.WAN;
            };

            httpInfo.onFailed = () => GameDataManager.Instance.NetworkInfo.mode = NetworkServiceMode.LAN;

            return httpInfo;
        }
#endregion

        #region 游戏更新
        public void GameUpdate()
        {
            Vue.Router.Close(nameof(GameUI.EnsureTipView)); //关闭提示更新的视图
        }
        #endregion

        #region 退出游戏
        public void QuitGameApp()
        {
            Application.Quit();
        }
        #endregion

        //------------------------------------场景加载完成--------------------------------------------------
        private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
        {
            //GameScene gameScene = (GameScene)scene.buildIndex; //emmmm，构建顺序不一致会导致bug....
            GameScene gameScene = Enum.Parse<GameScene>(scene.name); 
            StartCoroutine(AsyncInitializeScene(gameScene));
        }

        private IEnumerator AsyncInitializeScene(GameScene gameScene)
        {
            if(gameScene != GameScene.Login)
            {
                //获得当前场景下的LoadView
                GameObject viewObject = GameObjectFindUtil.BreadthFind(nameof(GameUI.LoadView), GameObject.Find("Canvas"));
                new FlexibleView(viewObject);
            }
            
            LoadInfo loadInfo = new LoadInfo();
            loadInfo.Bind(nameof(GameUI.LoadView));

            loadInfo.message = "场景加载中"; 
            //加载场景视图
            var operation = AssetManager.Instance.AsyncLoadScneViews(gameScene);

            while (!operation.IsDone)
            {
                yield return null;
            }

            //加载场景视图
            Vue.LoadViews(AssetManager.Instance.GetGameSceneViewConfig(gameScene));

            //加载每个场景额外的资产
            using(var it = AssetManager.Instance.AsyncLoadScenExtraAsset(gameScene).GetEnumerator())
            {
                while (it.MoveNext())
                {
                    var op = it.Current;
                    while (!op.IsDone)
                    {
                        yield return null;
                    }
                }
            }

            //装配场景下的EventCall
            Vue.AutowireAndUnloadEventCalls(gameScene.ToString());

            loadInfo.message = "场景完成";

            //调试
            //OutputAllLoadedViews();

            //准备游戏
            GameDataBind.Instance.PrepareGameDataAfterSceneLoaded(gameScene);
        }


        //--------------------------------------场景卸载完成------------------------------------------------
        private void OnSceneUnloaded(Scene scene)
        {
            GameScene gameScene = (GameScene)scene.buildIndex;

            //清空上一个场景注册的网络事件
            NetworkManager.Instance.ClearHandles();

            //释放视图资源
            Vue.UnloadCurrentSceneResources();

            //卸载场景资源
            AssetManager.Instance.UnloadSceneAsset(gameScene);

            //释放游戏场景资源
            GameDataBind.Instance.DisposeGameDataAfterSceneUnloaded(gameScene);
        }

        //---------------------------------------------------------------------------------------------------

        private void OutputAllLoadedViews()
        {
            using(var it = Vue.Router.GetAllView().GetEnumerator())
            {
                while (it.MoveNext())
                {
                    Debug.Log(it.Current.name);
                }
            }
        }
    }
}
