﻿using CocosSharp;

namespace HexMex.Scenes
{
    public abstract class TouchLayer : CCLayer
    {
        protected TouchLayer(HexMexCamera hexMexCamera) : base(hexMexCamera)
        {

        }

        protected TouchLayer()
        {

        }

        public virtual void OnTouchDown(TouchEventArgs e) { }
        public virtual void OnTouchUp(TouchEventArgs e) { }
        public virtual void OnTouchCancelled(TouchEventArgs e, TouchCancelReason cancelReason) { }

        public virtual bool BlockDragOrPintch(CCTouch touch) => false;
    }
}