# 《HappyCard》学习项目说明
**注：游戏中使用到的素材来源于网上，此项目仅供学习娱乐使用，对于游戏中使用的素材部分来自本人从Unity商店中购买，此外如果此项目中所用资源若有侵权，联系我将进行删除。**

**此文档会随着游戏的开发进度而进行更新**

## 一、游戏中部分游戏界面截图
![登录界面截图](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/登录.png)

![注册页面](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/注册.png)

![大厅](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/大厅.png)

![背包](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/背包.png)

![设置](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/设置.png)

![房间](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/房间.png)

![消息列表](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/消息列表.png)

![游戏对局设置](https://github.com/Avalon712/UniVue-Tutorials-HappyCard/blob/master/imgs/游戏对局设置.png)

## 二、游戏玩法介绍
游戏中玩家可以在局域网下和好友一起玩，在没有服务器时，玩家记得及时存档以免数据丢失。在有服务器时会自带存档在服务器上，目前服务器还在开发中。
###斗地主
玩法和《欢乐斗地主一样》。
### 板子炮
   这是我家乡那里的一种玩法，四个人一起玩，然后手牌有黑桃7的玩家可以先进行叫牌（即在四个2中选一个2，注意这个2你手里不能有），然后拥有这种你叫的牌的人会是你的队友。场面会形成2对2的局面，你们需要根据大家的出牌情况判断出你的
队友。然后游戏胜利的条件是：①你最先出完牌你的队友次之，这种情况叫双赢，对面叫双输（emmmmm,我们方言虽然不这么叫，但是翻译过来就是这个意思）；②你最先出完牌，你的队友第三出完牌，这种情况叫单赢；③你最先出完牌，你队友最后出完牌,
这种情况则是平局。
   游戏中可以出牌的有对子、顺子，这些牌的大小比较和斗地主一致。在斗地主中的“连对”在这里叫做“板子炮”，这是一种炸弹牌，此外还有三个一样的牌、四个一样的牌都是炸弹。此外还有连炸，这些规则和斗地主差不多。
### 炸金花
   玩法百度吧，不想写字了.......

## 三、如何使用UniVue制作各种视图
### 实现玩家的背包视图
   游戏中玩家的背包使用了UniVue框架中的GridView进行创建的，可以很方便的实现无限滚动、循环滚动。此外要实现点击某个物品就在左边的PropInfoView展示此物品的信息，这是先将呈现每个物品的Item使用Toggle来封装，然后通过命名为
"Evt_ShowPropInfo_Toggle"来告诉UniVue这是一个需要绑定事件名为ShowPropInfo的UI，之后会创建一个UIEvent对象（实际是ToggleEvent），当点击这个Item后会触发这个事件的回调函数，在这个回调函数中，通过使用Vue对象访问ViewRouter
对象获取到这个PropInfoView视图对象，再调用其RebindModel()函数就能轻松实现了。此外还有当使用某个物品后这个物品的数量也会自动减少，这是通过点击名为"Evt_UseProp_Btn"的按钮触发"Use_Prop"事件，然后再事件的回调中将这个物品的
数量减一，就OK了。就这么简单。这儿讲述比较简略，如果你想知道UniVue具体在这个项目中如何使用的，建议看一下这个游戏的UI部分的逻辑，其实非常之简单，都是直接将要显示的数据都绑定到视图上就好了，当数据变化是，UI会自动更新，这就是
MVVM模式的既视感！！！

### 实现商店的Tab页（分页）效果
   商店视图是一个嵌套视图，它嵌套了四个视图，从前面的截图可以看见每个商品的分类都单独是一个视图，其中道具视图使用了UniVue的GridView，其它三个视图使用了ListView。 将这个四个视图设置为System基本的视图，这样这几个视图就是在
ShopView下自动实现互斥显示。

## 三、游戏中的网络接口
   由于前期的注意力没有太放在编码上，导致游戏中的逻辑感觉不是很清晰。网络部分在Network这个目录下面，实现了HTTP、TCP、UDP通信，还有一个Offline版本的模拟网络服务（其实就是利用了玩家的本地存档实现的）。

## 四、游戏中的对局回放
   目前还在制作中.......

## 五、商店系统
   这部分已经制作完毕，大家可以拉取代码后进行查看。

## 六、道具BUFF系统
   道具资源倒是制作完了，但是道具的作用效果还没做.......

## 七、玩家成长系统&任务系统
   制作中........

## 八、好友系统
   玩家可以和好友聊天、赠送资源等。（这部分功能需要服务器的支持）。

## 九、结算系统
   结算系统需要根据玩家当前使用的道具、对局表现、炸弹数量等来统一分析后得出，目前也是还在制作中.......
