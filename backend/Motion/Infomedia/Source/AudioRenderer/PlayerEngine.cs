// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlayerEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Motion.Infomedia.AudioRenderer.Playback;

    using NLog;

    /// <summary>
    /// The player engine that manages audio players with their priority.
    /// If a higher priority player is enqueued, the currently playing one is stopped
    /// and discarded and the new player is played immediately.
    /// If an equal or lower priority player is enqueued, it is put in the priority queue
    /// and will be played once all higher priority and previously added equal priority
    /// players have finished playing.
    /// </summary>
    public class PlayerEngine
    {
      
        private readonly object locker = new object();

        private readonly SortedDictionary<int, Queue<Dictionary<AudioIOHandler, AudioPlayer>>> queue =
            new SortedDictionary<int, Queue<Dictionary<AudioIOHandler, AudioPlayer>>>(new InvertedComparer());

        private AudioPlayer currentPlayer;
        private int currentPriority;

        private AudioIOHandler previousAudioIOHandler;

        /// <summary>
        /// Enqueue a new player to be played at the given priority.
        /// For the algorithm, see the class description.
        /// </summary>
        /// <param name="priority">
        ///     The priority.
        ///     The lower the number, the lower the priority.
        /// </param>
        /// <param name="player">
        ///     The player.
        /// </param>
        /// <param name="audioIOHandler">The audio IO handler</param>
        public void Enqueue(int priority, AudioPlayer player, AudioIOHandler audioIOHandler)
        {
            IDisposable dispose = null;
            lock (this.locker)
            {
                if (this.currentPlayer != null && this.currentPriority < priority)
                {
                    this.currentPlayer.Completed -= this.CurrentPlayerOnCompleted;
                    this.currentPlayer.Stop();
                    dispose = this.currentPlayer;
                    this.currentPlayer = null;
                }

                if (this.currentPlayer == null)
                {
                    this.currentPlayer = player;
                    this.currentPriority = priority;
                }
                else
                {
                    this.EnqueueForLater(priority, player, audioIOHandler);
                    return;
                }
            }

            this.StartCurrentPlayer(audioIOHandler);

            if (dispose != null)
            {
                dispose.Dispose();
            }
        }

        private void StartCurrentPlayer(AudioIOHandler audioIOHandler)
        {
            AudioPlayer player;
            lock (this.locker)
            {
                player = this.currentPlayer;
            }

            if (player == null)
            {
                if (this.previousAudioIOHandler != null)
                {
                    this.previousAudioIOHandler.SpeakerEnabled = false;
                }

                return;
            }

            if (this.previousAudioIOHandler != null && this.previousAudioIOHandler != audioIOHandler)
            {
                this.previousAudioIOHandler.SpeakerEnabled = false;
            }

            audioIOHandler.SpeakerEnabled = true;
            this.previousAudioIOHandler = audioIOHandler;
            player.Completed += this.CurrentPlayerOnCompleted;
            player.Start(audioIOHandler);
        }

        private void CurrentPlayerOnCompleted(object sender, EventArgs eventArgs)
        {
            AudioIOHandler audioIOHandler = null;

            IDisposable dispose;
            lock (this.locker)
            {
                dispose = this.currentPlayer;
                if (this.currentPlayer != null)
                {
                    this.currentPlayer.Completed -= this.CurrentPlayerOnCompleted;
                    this.currentPlayer = null;
                }

                foreach (var playerQueue in this.queue.Values.Where(playerQueue => playerQueue.Count > 0))
                {
                    var player = playerQueue.Dequeue();
                    this.currentPlayer = player.First().Value;
                    audioIOHandler = player.First().Key;
                    break;
                }
            }

            this.StartCurrentPlayer(audioIOHandler);

            if (dispose != null)
            {
                dispose.Dispose();
            }
        }

        private void EnqueueForLater(int priority, AudioPlayer player, AudioIOHandler audioIOHandler)
        {
            lock (this.locker)
            {
                Queue<Dictionary<AudioIOHandler, AudioPlayer>> players;
                if (!this.queue.TryGetValue(priority, out players))
                {
                    players = new Queue<Dictionary<AudioIOHandler, AudioPlayer>>();
                    this.queue[priority] = players;
                }

                var item = new Dictionary<AudioIOHandler, AudioPlayer>();
                item.Add(audioIOHandler, player);
                players.Enqueue(item);
            }
        }

        private class InvertedComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return y.CompareTo(x);
            }
        }
    }
}