using System.Collections.Generic;
using JetBrains.Annotations;
using Mage;
using Shared;
using Unity.VisualScripting;
using UnityEngine;

namespace initScene
{
    public class PlayerBearer : PlayerBearer<PlayerDealer>
    {
    }
    public abstract class PlayerBearer<TDealer> : MonoBehaviour, IPlayerUser<TDealer> where TDealer : Dealer
    {
        private readonly List<IPlayerUser<TDealer>> _playerUser = new();
        [SerializeField][CanBeNull] private TDealer mainPlayer;
        public List<PlayerBearer<TDealer>> childPlayerBearers = new();

        public TDealer MainPlayer { get=>mainPlayer; set=>SetMainPlayer(value); }
        public void Subscribe(IPlayerUser<TDealer> playerUser)
        {
            _playerUser.Add(playerUser);
            if (mainPlayer!=null) playerUser.MainPlayer = mainPlayer;
        }
        private void SetMainPlayer(TDealer player)
        {
            mainPlayer = player;
            foreach (IPlayerUser<TDealer> playerUser in _playerUser)
            {
                playerUser.MainPlayer = mainPlayer;
            }

            foreach (PlayerBearer<TDealer> pb in childPlayerBearers) if (pb != this) 
            {
                pb.MainPlayer = mainPlayer;
            } else throw new System.Exception("Parent cannot be the same as the child");
        }

    }

    public interface IPlayerUser<TDealer>  where TDealer : Dealer
    {
        [DoNotSerialize] public TDealer MainPlayer { get; set; }
    } 

}