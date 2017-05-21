﻿using CocosSharp;
using HexMex.Controls;
using HexMex.Game;

namespace HexMex.Scenes.Game
{
    public abstract class Menu
    {
        protected VisualSettings VisualSettings { get; }
        private MenuLayer host;
        protected ExtendedDrawNode DrawNode { get; private set; }
        protected CCSize ClientSize { get; private set; }

        public MenuLayer Host
        {
            get => host;
            set
            {
                host = value;
                ClientSize = host.ClientRectangle.Size;
                DrawNode = host.DrawNode;
            }
        }

        protected Menu(VisualSettings visualSettings)
        {
            VisualSettings = visualSettings;
        }

        public void AddedToScene()
        {
            OnAddedToScene();
        }

        public virtual void TouchCancel(CCPoint position, TouchCancelReason reason)
        {
        }

        public virtual void TouchDown(CCPoint position)
        {
        }

        public virtual void TouchUp(CCPoint position)
        {
        }

        protected virtual void OnAddedToScene()
        {
        }
    }
}