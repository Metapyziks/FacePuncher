using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
