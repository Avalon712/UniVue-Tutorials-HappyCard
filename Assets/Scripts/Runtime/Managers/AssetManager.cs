using HappyCard.Utils;
using UnityEngine;
using UniVue.View.Config;
using HappyCard.Enums;
using YooAsset;
using System.Collections;
using HayypCard.Enums;
using System.Collections.Generic;
using HappyCard.Entities;
using UnityEngine.U2D;


namespace HappyCard.Managers
{
    public sealed class AssetManager : Singleton<AssetManager>
    {
        //-----------------------------------------常量字段--------------------------------------------------------

        private const string headIconPackageName = "HeadIcon";
        private const string propIconPackageName = "PropIcon";
        private const string productIconPackageName = "ProductIcon";
        private const string currencyIconPackageName = "CurrencyIcon";
        private const string pokerIconPackageName = "PokerIcon";
        private const string productDataPackageName = "Product";
        private const string propInfoDataPackageName = "PropInfo";

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取所有的道具信息
        /// </summary>
        public Dictionary<PropType,PropInfo> GetPropInfos()
        {
            ResourcePackage package = YooAssets.GetPackage(nameof(PropInfo));
            AssetInfo[] assetInfos = package.GetAssetInfos("PropInfo");
            Dictionary<PropType, PropInfo> propInfos = new(assetInfos.Length);
            for (int i = 0; i < assetInfos.Length; i++)
            {
                string location = assetInfos[i].AssetPath;
                PropInfo propInfo = package.LoadAssetSync<PropInfo>(location).AssetObject as PropInfo;
                propInfos.Add(propInfo.PropType, propInfo);
            }
            return propInfos;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取指定类型的游戏商品信息
        /// </summary>
        public List<Product> GetProducts(ProductType productType)
        {
            ResourcePackage package = YooAssets.GetPackage(productDataPackageName);
            AssetInfo[] assetInfos = package.GetAssetInfos(productType.ToString());
            List<Product> products = new List<Product>(assetInfos.Length);
            for (int i = 0; i < assetInfos.Length; i++)
            {
                string location = assetInfos[i].AssetPath;
                products.Add(package.LoadAssetSync<Product>(location).AssetObject as Product);
            }
            return products;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取游戏中所有的扑克牌图标
        /// </summary>
        /// <returns>SpriteAtlas</returns>
        public SpriteAtlas GetPokerIcons() 
        {
            string location = "Assets/Game Resources/UI/Cards/Cards.spriteatlas";
            ResourcePackage package = YooAssets.GetPackage(pokerIconPackageName);
            return package.LoadAssetSync<SpriteAtlas>(location).AssetObject as SpriteAtlas;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取游戏内置的所有头像的图标
        /// </summary>
        /// <returns>SpriteAtlas</returns>
        public SpriteAtlas GetHeadIcons() 
        {
            string location = "Assets/Game Resources/UI/Player HeadIcon/HeadIcon.spriteatlas";
            ResourcePackage package = YooAssets.GetPackage(headIconPackageName);
            return package.LoadAssetSync<SpriteAtlas>(location).AssetObject as SpriteAtlas;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取网络类型的图标
        /// </summary>
        /// <param name="mode">网络服务模式</param>
        /// <returns>Sprite</returns>
        public Sprite GetNetworkIcon(NetworkServiceMode mode)
        {
            return null;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取游戏的场景视图配置文件
        /// </summary>
        /// <param name="scene">游戏场景</param>
        /// <returns>SceneConfig</returns>
        public SceneConfig GetGameSceneViewConfig(GameScene scene)
        {
            string sceneName = scene.ToString();
            string location = $"Assets/Game Resources/Views/{sceneName}/{sceneName}Scene.asset";
            ResourcePackage package = YooAssets.GetPackage(sceneName);
            return package.LoadAssetSync<SceneConfig>(location).AssetObject as SceneConfig;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 异步加载场景的视图配置
        /// </summary>
        public InitializationOperation AsyncLoadScneViews(GameScene scene)
        {
            EPlayMode playMode = GameRunner.Instance.playMode;

            //每个场景都必须加载场景视图配置 --- 这里面包含了视图的图片等
            return InitResourcePackage(YooAssets.CreatePackage(scene.ToString()), playMode);
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 加载整个游戏中都需要使用到的资产，只能初始化一次
        /// </summary>
        public IEnumerable<InitializationOperation> AsyncLoadPublicAsset()
        {
            EPlayMode playMode = GameRunner.Instance.playMode;
            //加载头像信息
            yield return InitResourcePackage(YooAssets.CreatePackage(headIconPackageName), playMode);
            //加载道具信息
            yield return InitResourcePackage(YooAssets.CreatePackage(propInfoDataPackageName), playMode);
            //加载道具图标
            yield return InitResourcePackage(YooAssets.CreatePackage(propIconPackageName), playMode);
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 异步加载每个场景额外需要的资产
        /// </summary>
        public IEnumerable<InitializationOperation> AsyncLoadScenExtraAsset(GameScene scene)
        {
            EPlayMode playMode = GameRunner.Instance.playMode;
            switch (scene)
            {
                case GameScene.Login:
                    //None
                    break;

                case GameScene.Main:
                    //加载商店商品资源包
                    yield return InitResourcePackage(YooAssets.CreatePackage(productDataPackageName), playMode);
                    //加载产品图标
                    yield return InitResourcePackage(YooAssets.CreatePackage(productIconPackageName), playMode);
                    break;

                case GameScene.Room:
                    //None
                    break;

                case GameScene.Game:
                    yield return InitResourcePackage(YooAssets.CreatePackage(pokerIconPackageName), playMode);
                    break;
            }
        }


        //---------------------------------------卸载场景中的资源（不包含公共资源）-----------------------------------------

        /// <summary>
        /// 卸载游戏场景中的资源
        /// </summary>
        public void UnloadSceneAsset(GameScene scene)
        {
            //无论哪个场景都要卸载掉视图资产
            string sceneName = scene.ToString();
            YooAssets.DestroyPackage(sceneName);

            switch (scene)
            {
                case GameScene.Login:
                    break;

                case GameScene.Main:
                    //卸载商店商品资产包
                    YooAssets.DestroyPackage(productDataPackageName);
                    //卸载商店商品的图标
                    YooAssets.DestroyPackage(productIconPackageName);
                    break;
                case GameScene.Room:

                    break;
                case GameScene.Game:
                    //卸载扑克牌的资产
                    YooAssets.DestroyPackage(pokerIconPackageName);
                    break;
            }

        }


        //------------------------------------异步初始化资源包--------------------------------------------

        public InitializationOperation InitResourcePackage(ResourcePackage package,EPlayMode playMode)
        {
            switch (playMode)
            {
                case EPlayMode.EditorSimulateMode:
                    return InitResourcePackageInEditor(package);
                case EPlayMode.OfflinePlayMode:
                    return InitResourcePackageInOffline(package);
                case EPlayMode.HostPlayMode:
                    return InitResourcePackageInHostServer(package);
            }

            return null;
        }

        //----------------------------------在编辑器模式下加载资源包----------------------------------------------

        //编辑器下
        private InitializationOperation InitResourcePackageInEditor(ResourcePackage package)
        {
            var initParameters = new EditorSimulateModeParameters();
            var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, package.PackageName);
            initParameters.SimulateManifestFilePath = simulateManifestFilePath;
            return package.InitializeAsync(initParameters);
        }

        //--------------------------------------单机模式下----------------------------------------------------------

        /// <summary>
        /// 这种模式下要将ab包打在StreamingAssets目录下
        /// </summary>
        private InitializationOperation InitResourcePackageInOffline(ResourcePackage package)
        {
            var initParameters = new OfflinePlayModeParameters();
            return package.InitializeAsync(initParameters);
        }

        //---------------------------------------服务器主机----------------------------------------------------------

        private InitializationOperation InitResourcePackageInHostServer(ResourcePackage package)
        {
            //TODO
            return null;
        }

    }
}
