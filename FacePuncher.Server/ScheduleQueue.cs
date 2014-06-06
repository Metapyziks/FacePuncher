/* Copyright (c) 2014 James King [metapyziks@gmail.com]
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
using System.Diagnostics;
using System.Linq;

using FacePuncher.Entities;

namespace FacePuncher
{
    public class ScheduleQueue
    {
        private class ScheduledAction
        {
            public double Time { get; set; }

            public Component Component { get; set; }

            public Action Action { get; set; }

            public ScheduledAction(double time, Component comp, Action action)
            {
                Time = time;
                Component = comp;
                Action = action;
            }

            public void Act()
            {
                if (!Component.Entity.IsActive) return;
                if (!Component.Entity.Contains(Component)) return;

                Action();
            }
        }

        private List<ScheduledAction> _heap;

        public double Time { get; private set; }

        public double NextTime { get { return _heap.Count > 0 ? _heap[0].Time : Time; } }

        public int Count { get { return _heap.Count; } }

        public ScheduleQueue()
        {
            _heap = new List<ScheduledAction>();
            Time = 0;
        }

        public void Add(double delay, Component comp, Action action)
        {
            var ent = comp.Entity;
            if (!ent.IsValid || !ent.IsActive) return;

            var time = Time + Math.Max(0.0, delay);

            var item = new ScheduledAction(time, comp, action);

            _heap.Add(item);

            for (int i = Count / 2 - 1; i >= 0; --i) {
                MinHeapify(i);
            }

            Debug.Assert(_heap.Min(x => x.Time) == _heap[0].Time);
        }

        private void MinHeapify(int index)
        {
            int left = index * 2 + 1;
            int right = left + 1;
            int smallest = index;

            if (left < Count && _heap[left].Time < _heap[smallest].Time) {
                smallest = left;
            }

            if (right < Count && _heap[right].Time < _heap[smallest].Time) {
                smallest = right;
            }

            if (smallest != index) {
                var temp = _heap[index];
                _heap[index] = _heap[smallest];
                _heap[smallest] = temp;

                MinHeapify(smallest);
            }
        }

        public void Act()
        {
            var first = _heap[0];
            _heap[0] = _heap[Count - 1];
            _heap.RemoveAt(Count - 1);

            if (_heap.Count > 1) {
                MinHeapify(0);
                Debug.Assert(_heap.Min(x => x.Time) == _heap[0].Time);
            }

            Time = first.Time;
            first.Act();
        }
    }
}
