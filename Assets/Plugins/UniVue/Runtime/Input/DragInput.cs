using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniVue.Input
{
    public sealed class DragInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform _mover; //移动者
        private Vector3 _offset; //开始拖拽是鼠标与UI位置的偏移量
        //屏幕拖拽区域
        private float _minWidth, _maxWidth, _minHeight, _maxHeight;

        /// <summary>
        /// 对外提供可注册事件
        /// </summary>
        public event Action<PointerEventData> onBeginDrag, onDrag, onEndDrag;

        /// <summary>
        /// 是否能够使得UI进行拖拽，默认为false
        /// </summary>
        [HideInInspector] public bool Draggable { get; set; } = true;

        /// <summary>
        /// 设置接受移动输入
        /// </summary>
        /// <param name="receiver">接受拖拽信号</param>
        /// <param name="mover">移动者</param>
        /// <returns>DragInput</returns>
        public static DragInput ReceiveInput(GameObject receiver,GameObject mover)
        {
            if (receiver != null)
            {
                DragInput dragInput = receiver.GetComponent<DragInput>();
                if(dragInput != null) { return dragInput; }

                RectTransform rectTransform = mover.GetComponent<RectTransform>();
                if (rectTransform == null) { return null; }

                dragInput = receiver.AddComponent<DragInput>();
                dragInput._mover = rectTransform;

                //计算拖拽区域
                dragInput.CalculateMovementArea();

                return dragInput;
            }
            return null;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDrag != null) { onBeginDrag(eventData); }

            if (Draggable)
            {
                Vector3 mousePos;
                //将鼠标的屏幕坐标转换成_rectTransform平面下的世界坐标
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_mover, eventData.position, eventData.enterEventCamera, out mousePos))
                {
                    //计算UI和指针之间的位置偏移量
                    _offset = _mover.position - mousePos;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) { onDrag(eventData); }

            if (Draggable)
            {
                Vector3 mousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_mover, eventData.position, eventData.enterEventCamera, out mousePos))
                {
                    Vector3 currPos = _offset + mousePos;

                    //限制水平/垂直拖拽范围在最小/最大值内
                    currPos.x = Mathf.Clamp(currPos.x, _minWidth, _maxWidth);
                    currPos.y = Mathf.Clamp(currPos.y, _minHeight, _maxHeight);
                    currPos.z = 0;

                    //更新位置
                    //bug: RectTransform的局部坐标发生严重的变化......
                    _mover.position = currPos;
                }

            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDrag != null) { onEndDrag(eventData); }
        }

        /// <summary>
        /// 计算可移动区域
        /// </summary>
        private void CalculateMovementArea()
        {
            Rect rect = _mover.rect;
            _minWidth = rect.width / 2;
            _maxWidth = Screen.width - _minWidth;
            _minHeight = rect.height / 2;
            _maxHeight = Screen.height - _minHeight;
        }
    }
}

