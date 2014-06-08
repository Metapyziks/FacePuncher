/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using FacePuncher.Entities;

namespace FacePuncher
{
    /// <remarks>
    /// Adapted from Tamme Schichler's AttachedAnimations.Delay implementation.
    /// Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
    /// <https://bitbucket.org/Tamschi/attachedanimations>
    /// </remarks>
    public class Delay : INotifyCompletion
    {
        public DelayQueue Queue { get; private set; }

        public double Delta { get; internal set; }

        public Component Component { get; set; }

        public bool IsCompleted { get; private set; }

        public bool IsValid
        {
            get
            {
                return Component.IsActive;
            }
        }

        private event Action _continuations;

        public Delay(DelayQueue queue, double delta, Component comp, bool forceYeild = false)
        {
            Queue = queue;
            Delta = delta;
            Component = comp;

            if (forceYeild || delta > 0) {
                Queue.Enqueue(this);
            } else {
                IsCompleted = true;
            }
        }

        public Delay GetAwaiter()
        {
            return this;
        }

        public double GetResult()
        {
            return -Delta;
        }

        public void OnCompleted(Action continuation)
        {
            if (!IsValid) return;

            if (IsCompleted) continuation();
            else _continuations += continuation;
        }

        public void NotifyCompletion()
        {
            IsCompleted = true;

            if (IsValid && _continuations != null) {
                _continuations();
            }
        }
    }

    /// <remarks>
    /// Adapted from Tamme Schichler's AttachedAnimations.DelayDispatcher implementation.
    /// Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
    /// <https://bitbucket.org/Tamschi/attachedanimations>
    /// </remarks>
    public class DelayQueue
    {
        private LinkedList<Delay> _queue;

        public bool IsEmpty
        {
            get { return _queue.First != null; }
        }

        public DelayQueue()
        {
            _queue = new LinkedList<Delay>();
        }

        public void Enqueue(Delay delay)
        {
            if (!delay.IsValid) return;

            var current = _queue.First;
            while (current != null) {
                if (delay.Delta < current.Value.Delta) {
                    _queue.AddBefore(current, delay);
                    current.Value.Delta -= delay.Delta;
                    return;
                }

                delay.Delta -= current.Value.Delta;
                current = current.Next;
            }

            _queue.AddLast(delay);
        }

        public double AdvanceOnce(ref double delta)
        {
            var current = _queue.First;
            if (current == null) {
                delta = 0;
                return 0.0;
            }

            var delay = current.Value;
            var temp = delay.Delta;

            delay.Delta -= delta;

            if (delay.Delta > 0) {
                temp = delta;
                delta = 0;
                return temp;
            }

            delta = -delay.Delta;
            delay.NotifyCompletion();

            _queue.RemoveFirst();

            return temp;
        }
    }
}
